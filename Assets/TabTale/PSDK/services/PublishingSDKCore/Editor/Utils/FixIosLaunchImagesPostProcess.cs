#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using PSDK.UnityEditor.XCodeEditor;

namespace TabTale.PSDK.UnityEditor
{
	public static class FixIosLaunchImagesPostProcess
	{
		
		static FixIosLaunchImagesPostProcess() {
		}

		[PostProcessBuild(2147483647)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			bool compatibilityMode = true;
			string jsonPath = System.IO.Path.Combine(Application.streamingAssetsPath, "psdk_ios.json");
			string jsonStr = System.IO.File.ReadAllText(jsonPath);
			Dictionary<string,object> json = (Dictionary<string,object>)Plugins.PSDK.Json.Deserialize(jsonStr);
			if(json.ContainsKey("global")){
				Dictionary<string,object> globalDict = (Dictionary<string,object>)json["global"];
				if(globalDict.ContainsKey("compatibilityMode")){
					compatibilityMode = (bool)globalDict["compatibilityMode"];
				}
			}
			if (target == BuildTarget.iOS) {
				if(compatibilityMode)
					FixLaunchImages (pathToBuiltProject);
				else
					FixXibLaunchImages(pathToBuiltProject);
			}
		}

		private static void FixXibLaunchImages(string pathToBuiltProject)
		{
			string[] filePaths = Directory.GetFiles(Path.Combine(pathToBuiltProject, "Data/Raw"), "*.xib");
			if(filePaths != null && filePaths.Length > 0){
				XCProject project = new XCProject(pathToBuiltProject);

				foreach(string file in filePaths){
					project.AddFile(file);
				}

				project.AddFile(Path.Combine(pathToBuiltProject, "Data/Raw/LaunchScreenAssets.xcassets"));

				var outputFile = Path.Combine(pathToBuiltProject, "Info.plist");

				if (!File.Exists(outputFile))
				{
					Debug.LogError(outputFile + " must exists before hand !");
					return;
				}

				XmlDocument main = new XmlDocument();
				main.Load(outputFile);

				if (main == null)
				{
					Debug.LogError("Couldn't load " + outputFile);
					return;
				}

				XmlNode dictMainNode = FindLastChildNodeInHirarchy(main,new List<string>() {"plist","dict"});

				if (dictMainNode == null)
				{
					Debug.LogError("No dict tag at  " + outputFile);
					return;
				}

				XmlElement elem = dictMainNode.OwnerDocument.CreateElement("key");
				elem.InnerText="UILaunchStoryboardName";
				dictMainNode.AppendChild(elem);
				elem = dictMainNode.OwnerDocument.CreateElement("string");
				elem.InnerText="LaunchScreen-iPhone";
				dictMainNode.AppendChild(elem);

				elem = dictMainNode.OwnerDocument.CreateElement("key");
				elem.InnerText="UILaunchStoryboardName~ipad";
				dictMainNode.AppendChild(elem);
				elem = dictMainNode.OwnerDocument.CreateElement("string");
				elem.InnerText="LaunchScreen-iPad";
				dictMainNode.AppendChild(elem);

				main.Save(outputFile);

				project.Save();
			}

		}

//		[MenuItem ("PSDK/Update launch images Info.plist")]
//		static void TestMenu () {
//			string pathToBuiltProject = "/Users/israelp/gitrep/games/CocoBallerinaEmptyPsdk210/ios210Unity523PsdkCore";
//			//FixLaunchImagesInInfoPlist (pathToBuiltProject);
//			//FixLaunchImagesPbxProject (pathToBuiltProject);
//			//MoveLaunchImagesFiles (pathToBuiltProject);
//			DeleteLaunchScreenXibFiles (pathToBuiltProject);
//		}
		
		
		private static void FixLaunchImages(string pathToBuiltProject) {

			string dataRawDir = Path.Combine(pathToBuiltProject, "Data/Raw");
			string splashXibFp = Path.Combine(dataRawDir,"splash.xib");
			string xcAssetsFp = Path.Combine(dataRawDir,"LaunchScreenAssets.xcassets");

			if(File.Exists(splashXibFp)){
				File.Delete(splashXibFp);
			}
			if(Directory.Exists(xcAssetsFp)){
				Directory.Delete(xcAssetsFp,true);
			}

			FixLaunchImagesInInfoPlist (pathToBuiltProject);
			FixLaunchImagesPbxProject (pathToBuiltProject);
			MoveLaunchImagesFiles (pathToBuiltProject);
			DeleteLaunchScreenXibFiles (pathToBuiltProject);
		}
		
		private static bool FixLaunchImagesInInfoPlist(string pathToBuiltProject) {
			//Debug.Log ("Updating manifest from mod: " + modPath);
			bool result = true;
			
			var outputFile = Path.Combine(pathToBuiltProject, "Info.plist");
			
			if (!File.Exists(outputFile))
			{
				Debug.LogError(outputFile + " must exists before hand !");
				return false;
			}
			
			XmlDocument main = new XmlDocument();
			main.Load(outputFile);
			
			if (main == null)
			{
				Debug.LogError("Couldn't load " + outputFile);
				return false;
			}
			
			XmlNode dictMainNode = FindLastChildNodeInHirarchy(main,new List<string>() {"plist","dict"});
			
			if (dictMainNode == null)
			{
				Debug.LogError("No dict tag at  " + outputFile);
				return false;
			}

			
			UpdateInfoPlistRecursivly (dictMainNode);

			// Fixing the empty array at the end of the.
			// <!DOCTYPE plist PUBLIC "-//Apple Computer//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd"[]>
			// Courtesy of Jared J.
			
			if (main.DocumentType != null)
			{
				var name = main.DocumentType.Name;
				var publicId = main.DocumentType.PublicId;
				var systemId = main.DocumentType.SystemId;
				var parent = main.DocumentType.ParentNode;
				var documentTypeWithNullInternalSubset = main.CreateDocumentType(name, publicId, systemId, null);
				parent.ReplaceChild(documentTypeWithNullInternalSubset, main.DocumentType);
			}

			main.Save(outputFile);
			return result;
		}
		
		
		
		private static XmlNode FindChildNode(XmlNode parent, string name)
		{
			XmlNode curr = parent.FirstChild;
			while (curr != null)
			{
				if (curr.Name.Equals(name) || (curr.InnerText != null && curr.Name.Equals("key") && curr.InnerText.Equals(name) ))
				{
					return curr;
				}
				curr = curr.NextSibling;
			}
			return null;
		}
		
		private static XmlNode FindLastChildNodeInHirarchy(XmlNode parent, IList<string> hirarchy, int depth=0)
		{
			if (hirarchy.Count <= depth)
				return null;

			XmlNode curr = parent.FirstChild;
			while (curr != null)
			{
				if (curr.Name.Equals(hirarchy[depth]))
				{
					if (hirarchy.Count -1 == depth) {
						return curr;
					}
					else {
						XmlNode tmp = FindLastChildNodeInHirarchy(curr, hirarchy, depth +1);
						if (tmp != null) 
							return tmp;
					}
				}
				curr = curr.NextSibling;
			}

			return null;
		}

		private static XmlElement FindElementWithKey(string key, XmlNode parent)
		{
			
			var curr = parent.FirstChild;
			while (curr != null)
			{
				
				if (( curr.Name.Equals(key) || (curr.InnerText != null && curr.Name.Equals("key") && curr.InnerText.Equals(key))) && curr is XmlElement)
				{
					return curr as XmlElement;
				}
				curr = curr.NextSibling;
			}
			return null;
		}
		
		private static bool XmlNodeHasKey(XmlNode node) {
			return (XmlNodeKey (node) != null);
		}
		
		private static string XmlNodeKey(XmlNode node) {
			if (node == null)
				return null;
			
			if (node.Name == "key")
				return node.InnerText;
			
			return node.Name;
		}
		
		private static bool UpdateInfoPlistRecursivly(XmlNode mainManNode) {
			
			
			string[] nodesToRemove = new string[] {"UILaunchImages","UILaunchStoryboardName~iphone","UILaunchStoryboardName~ipad"};
			
			XmlNode node = null;
			
			for(int i=0; i < nodesToRemove.Length; ++i) {
				node = FindChildNode (mainManNode, nodesToRemove[i]);
				if (node != null) {
					XmlNode siblingToDelete = node.NextSibling;
					mainManNode.RemoveChild(node);
					mainManNode.RemoveChild(siblingToDelete);
				}
			}
			
			// Adding UILaunchImageFile
			node = FindChildNode (mainManNode, "UILaunchImageFile");
			if (node == null) { // Add the node
				XmlElement elem = mainManNode.OwnerDocument.CreateElement("key");
				elem.InnerText="UILaunchImageFile";
				mainManNode.AppendChild(elem);
				elem = mainManNode.OwnerDocument.CreateElement("string");
				elem.InnerText="LaunchImage.png";
				mainManNode.AppendChild(elem);
			}
			return true;
		}
		
		
		static void Log(string str) {
			Debug.LogWarning (str);
		}
		
		static void Log(XmlNode node) {
			if (node == null) {
				Log ("node is null !");
				return;
			}
			string str = "node:" + node.Name;
			if (node.InnerText != null) {
				str += "," + node.InnerText;
			}
			Log (str);
		}
		
		
		// fix ios project file
		private static bool FixLaunchImagesPbxProject(string pathToBuiltProject) {
			var outputFile = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj");
			outputFile = Path.Combine(outputFile, "project.pbxproj");
			
			if (!File.Exists(outputFile))
			{
				Debug.LogError(outputFile + " must exists before hand !");
				return false;
			}
			
			string content = File.ReadAllText (outputFile);
			if (content == null) {
				Debug.LogError("failed to read " + outputFile + " for dropping LaunchImage !");
				return false;
			}
			for (int i=0; i < 3; i++) {
				content = content.Replace ("ASSETCATALOG_COMPILER_LAUNCHIMAGE_NAME = LaunchImage;", "");
				content = content.Replace ("ASSETCATALOG_COMPILER_LAUNCHIMAGE_NAME=LaunchImage;", "");
			}
			
			File.WriteAllText (outputFile, content);
			// ASSETCATALOG_COMPILER_LAUNCHIMAGE_NAME = LaunchImage;
			return true;
		}


		
		private static void MoveLaunchImagesFiles(string pathToBuiltProject) {
			// Create a new project object from build target
			XCProject project = new XCProject( pathToBuiltProject );
			
			try {
				// Find and run through all default pngs
				string subfoldersUnder = System.IO.Path.Combine(pathToBuiltProject, "Unity-iPhone");
				subfoldersUnder = System.IO.Path.Combine(subfoldersUnder, "Images.xcassets");
				subfoldersUnder = System.IO.Path.Combine(subfoldersUnder, "LaunchImage.launchimage");
				var files = System.IO.Directory.GetFiles( subfoldersUnder , "*.png");
				foreach( var file in files ) {
					//Debug.Log ("Moving  " + file + " to " + pathToBuiltProject);
					if (! File.Exists(file)) continue;
					string dstFile = Path.Combine(pathToBuiltProject, Path.GetFileName(file));
					File.Move(file,dstFile);
				}

				// Adding support for unity ios8 splash extention.

				Dictionary<string, string> dict = new Dictionary<string,string>();
				dict.Add("LaunchImage.png", "Default.png");
				dict.Add("LaunchImage@2x.png", "Default@2x.png");
				dict.Add("LaunchImage-700-568h@3x.png","Default-Portrait@3x.png");
				dict.Add("LaunchImage-568h@2x.png","Default-568h@2x.png");
				dict.Add("LaunchImage-700@2x.png","Default@2x.png");
				dict.Add("LaunchImage-700-568h@2x.png","Default-568h@2x.png");
				dict.Add("LaunchImage-700-Landscape@2x~ipad.png","Default-Landscape@2x.png");
				dict.Add("LaunchImage-700-Landscape~ipad.png","Default-Landscape.png");
				dict.Add("LaunchImage-700-Portrait@2x~ipad.png","Default-Portrait@2x.png");
				dict.Add("LaunchImage-700-Portrait~ipad.png","Default-Portrait.png");
				dict.Add("LaunchImage-800-667h@2x.png","Default-667h@2x.png");
				dict.Add("LaunchImage-800-Landscape-736h@3x.png","Default-Landscape@3x.png");
				dict.Add("LaunchImage-800-Portrait-736h@3x.png","Default-Portrait@3x.png");
				dict.Add("LaunchImage-Landscape@2x~ipad.png","Default-Landscape@2x.png");
				dict.Add("LaunchImage-Landscape~ipad.png","Default-Landscape.png");
				dict.Add("LaunchImage-Portrait@2x~ipad.png","Default-Portrait@2x.png");
				dict.Add("LaunchImage-Portrait~ipad.png","Default-Portrait.png");
                dict.Add("LaunchImage-700-Landscape-568h@2x.png", "Default-Landscape@3x.png");

                foreach (KeyValuePair<string,string> item in dict) {
					string srcFile = Path.Combine(pathToBuiltProject, Path.GetFileName(item.Value));
					string dstFile = Path.Combine(pathToBuiltProject, Path.GetFileName(item.Key));
					File.Copy(srcFile,dstFile);
					project.AddFile (dstFile);
				}
			}
			catch(System.Exception e) {
				Debug.LogException(e);
			}
			
			// Finally save the xcode project
			project.Save();
			
			//			try {
			//				string[] filesToDelete = new string[]  {"LaunchScreenImage-Landscape.png","LaunchScreenImage-Portrait.png","LaunchScreen.xib"};
			//				for (int i=0; i < filesToDelete.Length; i++) {
			//					string fileToDel = Path.Combine(pathToBuiltProject,filesToDelete[i]);
			//					if (! File.Exists(fileToDel)) continue;
			//					Debug.Log ("Deleting " + fileToDel);
			//					//File.Delete(fileToDel);
			//				}
			//			}
			//			catch(System.Exception e) {
			//				Debug.LogException(e);
			//			}
		}

		private static void DeleteLaunchScreenXibFiles(string pathToBuiltProject) {

			try {
				XCProject project = new XCProject( pathToBuiltProject );

				IDictionary<string,string> backRefMapping = new Dictionary<string,string>();
				foreach(KeyValuePair<string,PBXFileReference> item in  project.fileReferences) {
					if (item.Value.data.ContainsKey("path")) {
						UnityEngine.Debug.Log("backmap adding " + item.Value.data["path"].ToString() + "-->" + item.Key.ToString());
						backRefMapping[item.Value.data["path"].ToString()]=item.Key.ToString();
					}
				}
				foreach(KeyValuePair<string,PBXBuildFile> item in  project.buildFiles) {
					if (item.Value.data.ContainsKey("fileRef")) {
						UnityEngine.Debug.Log("backmap adding " + item.Value.data["fileRef"].ToString() + "-->" + item.Key.ToString());
						backRefMapping[item.Value.data["fileRef"].ToString()]=item.Key.ToString();
					}
				}

				var files = System.IO.Directory.GetFiles( pathToBuiltProject , "LaunchScreen-*.xib");
				foreach( var file in files ) {
					if (! File.Exists(file)) continue;
					string baseFileName = Path.GetFileName(file);
					UnityEngine.Debug.Log("handling file: " + baseFileName);
					if (backRefMapping.ContainsKey(baseFileName)) {
						string fileGUID = backRefMapping[baseFileName];
						string RscRefGUID = null;
						if (backRefMapping.ContainsKey(fileGUID)) {
							RscRefGUID = backRefMapping[fileGUID];
						}
						if (! project._objects.Remove(fileGUID)) {
							UnityEngine.Debug.LogError("Failed removing file ref " + fileGUID);
						}
						if (! project._objects.Remove(RscRefGUID)) {
							UnityEngine.Debug.LogError("Failed removing file ref " + RscRefGUID);
						}
						foreach(string grpKey in project.groups.Keys) {
							project.groups[grpKey].children.Remove(RscRefGUID);
							project.groups[grpKey].children.Remove(fileGUID);
						}
						foreach(string grpKey in project.resourcesBuildPhases.Keys) {
							project.resourcesBuildPhases[grpKey].RemoveBuildFile(RscRefGUID);
							project.resourcesBuildPhases[grpKey].RemoveBuildFile(fileGUID);
						}
					}
					File.Delete(file);
				}
				project.Save();

			}
			catch(System.Exception e) {
				Debug.LogException(e);
			}
		}

	}
}
#endif
