using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using NUnit.Framework;

namespace TabTale.PSDK.UnityEditor
{
	[InitializeOnLoad]
    public static class ManifestMod
    {
		private const string ANDROID_NAME			=	"android:name";
		private const string ACTIVITY				=	"activity";
		private const string RECEIVER				=	"receiver";
		private const string APPLICATION			=	"application";
		private const string ANDROID_MANIFEST_XML 	= 	"AndroidManifest.xml";
		private const string PLUGINS_RELPATH 		= 	"Plugins";
		private const string ANDROID_RELPATH		= 	"Android";


		static string outputFile;
		static ManifestMod() {
			#if UNITY_ANDROID
			string outputFile = Path.Combine(Application.dataPath, PLUGINS_RELPATH);
			outputFile = Path.Combine(outputFile, ANDROID_RELPATH);
			outputFile = Path.Combine(outputFile, ANDROID_MANIFEST_XML);
			UpdateManifest (outputFile);
			#endif
		}

		[PostProcessBuild(90)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			#if UNITY_ANDROID
			string outputFile = Path.Combine(pathToBuiltProject, PlayerSettings.productName);
			outputFile = Path.Combine(outputFile, "src/main/" + ANDROID_MANIFEST_XML);
			if (File.Exists(outputFile)) {
				UpdateManifest (outputFile, true);
			}
			#endif
		}

		[MenuItem ("TabTale/PSDK/Update Manifest")]
		static void UpdateManifestMenuCommand () {
			string outputFile = Path.Combine(Application.dataPath, PLUGINS_RELPATH);
			outputFile = Path.Combine(outputFile, ANDROID_RELPATH);
			outputFile = Path.Combine(outputFile, ANDROID_MANIFEST_XML);
			UpdateManifest (outputFile);
		}

		static void UpdateManifest(string androidManifestFile, bool withGradle=false) {
			if (Application.dataPath.Contains("/PublishingSDK/")) {
				Debug.LogWarning ("Contains PublishingSDK, not merging manifests !!!");
				return;
			}
			UpdateManifestFromManifestModsUnderPath (androidManifestFile,Path.Combine(Application.dataPath,TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath),withGradle);
		}

		public static void UpdateManifestFromManifestModsUnderPath(string androidManifestFile, string path, bool withGradle) {
			GenerateInitialManifest ();
			DeleteXpathsFromManifest (androidManifestFile);
			var files = System.IO.Directory.GetFiles( path , "*.manifestmod", SearchOption.AllDirectories );
			foreach( var file in files ) {
				UpdateAndroidManifestFromMod(androidManifestFile,file);
			}
			if (withGradle) {
				files = System.IO.Directory.GetFiles (path, "*.gradle_manifestmod", SearchOption.AllDirectories);
				foreach (var file in files) {
					UpdateAndroidManifestFromMod (androidManifestFile,file);
				}
			}

			FixUnityPlayerNativeActivity (androidManifestFile);
		}

		private static bool UpdateAndroidManifestFromMod(string androidManifestFile,string modPath) {
			//Debug.Log ("Updating manifest from mod: " + modPath);
			bool result = true;

			outputFile = androidManifestFile;
			if (!File.Exists(outputFile))
			{
				Debug.LogError("Manifest must be generated before hand !");
				return false;
			}

			XmlDocument main = new XmlDocument ();
			main.Load(outputFile);

			XmlDocument mod = new XmlDocument();
			mod.Load(modPath);

			if (main == null)
			{
				Debug.LogError("Couldn't load " + outputFile);
				return false;
			}
			
			if (mod == null)
			{
				Debug.LogError("Couldn't load " + modPath);
				return false;
			}
			
			
			XmlNode mainManNode = FindChildNode(main, "manifest");
			XmlNode modManNode = FindChildNode(mod, "manifest");
			
			if (mainManNode == null)
			{
				Debug.LogError("No manifest tag at  " + outputFile);
				return false;
			}
			
			if (modManNode == null)
			{
				Debug.LogError("No manifest tag at " + modPath);
				return false;
			}
			
			UpdateAndroidManifestRecursivly (mainManNode, modManNode);
			UpdateAndroidManifestNodesOrder(mainManNode);

			main.Save(outputFile);
			return result;
		}

		public static void GenerateInitialManifest()
        {
			string pluginsAndroidFolder = Path.Combine (Application.dataPath, PLUGINS_RELPATH);
			if (! System.IO.Directory.Exists (pluginsAndroidFolder))
						System.IO.Directory.CreateDirectory (pluginsAndroidFolder);

			pluginsAndroidFolder = Path.Combine (pluginsAndroidFolder, ANDROID_RELPATH);
			if (! System.IO.Directory.Exists (pluginsAndroidFolder))
				System.IO.Directory.CreateDirectory (pluginsAndroidFolder);

			var outputFile = Path.Combine (pluginsAndroidFolder,ANDROID_MANIFEST_XML);

            // only copy over a fresh copy of the AndroidManifest if one does not exist
            if (!File.Exists(outputFile))
            {
		var inputFile = Path.Combine(EditorApplication.applicationPath,"..");
		inputFile = Path.Combine(inputFile,"PlaybackEngines");
		inputFile = Path.Combine(inputFile,"AndroidPlayer");
		inputFile = Path.Combine(inputFile,"Apk");
		inputFile = Path.Combine(inputFile,"AndroidManifest.xml");
                File.Copy(inputFile, outputFile);
            }
        }


        private static XmlNode FindChildNode(XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

		private static XmlElement FindElementWithAndroidName(string name, string androidName,string ns, string value, XmlNode parent)
        {

            var curr = parent.FirstChild;
            while (curr != null)
            {

                if (curr.Name.Equals(name) && curr is XmlElement)
                {
					string attrValue = ((XmlElement)curr).GetAttribute(androidName);
					if (attrValue == value)
                    	return curr as XmlElement;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

		private static bool UpdateNodeAttributes(XmlNode main, XmlNode mod) {
			if (mod == null || mod.Attributes == null) return true;
			foreach (XmlAttribute modAttribute in  mod.Attributes) {
				XmlNode mainAttribute = main.Attributes.GetNamedItem(modAttribute.Name);
				if (null == mainAttribute ) { // missing attribute, copy it
					if (mod is XmlElement) {
						((XmlElement)main).SetAttribute(modAttribute.Name, modAttribute.NamespaceURI, modAttribute.Value);
					}
				}
				else { // existing attribute, leave it as is
					if (modAttribute.Value != mainAttribute.Value) {
						string androidName = XmlNodeAndroidName(mod);
						Debug.LogWarning("Manifest attribute " + main.Name + ":" + androidName + ":" + mainAttribute.Name + " has different values: main:" + mainAttribute.Value + ", mod:" + modAttribute.Value);
					}
				}
			}
			return true;
		}
		
		private static bool UpdateNodeBody(XmlNode main, XmlNode mod) {
			bool rc = true;
			rc = UpdateAndroidManifestRecursivly (main, mod);
			return rc;
		}

		private static bool XmlNodeHasAndroidName(XmlNode node) {
			return (XmlNodeAndroidName (node) != null);
		}

		private static string XmlNodeAndroidName(XmlNode node) {
			if (node == null)
				return null;
			
			if (node.Attributes == null || node.Attributes.Count < 1)
				return null;
			XmlNode androidNameNode = node.Attributes.GetNamedItem (ANDROID_NAME);

			if (androidNameNode == null)
								return null;
			return androidNameNode.Value;
		}

		private static bool UpdateAndroidManifestRecursivly(XmlNode mainManNode, XmlNode modManNode) {
			var curr = modManNode.FirstChild;
			while (curr != null)
			{
				XmlNode mainNode = null;
				if (XmlNodeHasAndroidName(curr) && curr.Name != APPLICATION) {
					string androidName = curr.Attributes.GetNamedItem(ANDROID_NAME).Name;
					string androidNameValue = curr.Attributes.GetNamedItem(ANDROID_NAME).Value;
					string ns = mainManNode.OwnerDocument.GetNamespaceOfPrefix("android");
					mainNode = FindElementWithAndroidName(curr.Name,androidName, ns, androidNameValue,mainManNode);
				}
				else {
					if (mainNode == null) {
						mainNode = FindChildNode(mainManNode,curr.Name);
					}
				}

				if (mainNode == null) { // node not exist in main, copy from mod
					XmlNode newImportedNode = mainManNode.OwnerDocument.ImportNode(curr, true);
					mainManNode.AppendChild(newImportedNode); 
				}
				else { // node exist at main, travcerse its attributes and body.
					UpdateNodeAttributes(mainNode,curr);
					UpdateNodeBody(mainNode,curr);
				}

				curr = curr.NextSibling;
			}
			bruteForceUpdateMainNode(mainManNode);
			return true;
		}

		private static void bruteForceUpdateMainNode(XmlNode mainManNode) {
			if (mainManNode == null) return;

			if (mainManNode.Attributes == null) return;
			XmlNode node = mainManNode.Attributes.GetNamedItem(ANDROID_NAME);
			if (node == null) return;


			string androidName = node.Value;
			if (androidName == null) return;

			node = mainManNode.Attributes.GetNamedItem("android:value");
			if (node == null) return;
			string androidValue = node.Value;
			if (androidValue == null) return;


			if ( androidName == "unityplayer.ForwardNativeEventsToDalvik" && node.Value !="true") {
				Debug.Log ("ManifestMod: Changing unityplayer.ForwardNativeEventsToDalvik to true");
				node.Value ="true";
			}

		}

		private static void UpdateAndroidManifestNodesOrder(XmlNode mainManNode) {

			if (mainManNode == null) return;

			XmlNode applicationNode =  FindChildNode(mainManNode,APPLICATION);
					
			// always put application node at the end.
			if (applicationNode != null) {
				mainManNode.RemoveChild(applicationNode);
				mainManNode.AppendChild(applicationNode);
			}

			// making singular multiple receiver first 
			// they will make sure to call all the rest

			XmlNode singularMultipleReceiverNode = null;
			List<XmlNode> otherReceivers = new List<XmlNode> (); 
			var curr = applicationNode.FirstChild;
			while (curr != null) {
				XmlNode recievrNode = curr;
				curr = curr.NextSibling;
				if (recievrNode.Name != RECEIVER)
					continue;
				string androidNameValue = recievrNode.Attributes.GetNamedItem (ANDROID_NAME).Value;
				if (androidNameValue == "com.appsflyer.MultipleInstallBroadcastReceiver" || androidNameValue == "com.tabtale.publishingsdk.psdksingular.SingularMultipleInstallBroadcastReceiver") {
					if (otherReceivers.Count == 0) { 
						// MultipleInstallBroadcastReceiver is already the first receiver, do nothing
						break;
					} else {
						// MultipleInstallBroadcastReceiver is not the first one, need change
						singularMultipleReceiverNode = recievrNode;
					}
				} else {
					otherReceivers.Add (recievrNode);
				}
			}
			// If no singular MultipleInstallBroadcastReceiver, nothing to do
			if (singularMultipleReceiverNode != null) {

				// Make MultipleInstallBroadcastReceiver first receiver
				applicationNode.RemoveChild (singularMultipleReceiverNode);
				foreach (XmlNode n in otherReceivers) {
					applicationNode.RemoveChild (n);
				}
				applicationNode.AppendChild (singularMultipleReceiverNode);
				foreach (XmlNode n in otherReceivers) {
					applicationNode.AppendChild (n);
				}
			}
		}

		private static void FixUnityPlayerNativeActivity(string androidManifestFile) {
			if (!File.Exists(androidManifestFile))
			{
				Debug.LogError("Manifest must be generated before hand !");
				return;
			}

			XmlDocument main = new XmlDocument();
			main.Load(androidManifestFile);
			XmlNode mainManNode = FindChildNode(main, "manifest");

			if (mainManNode == null) return;
			XmlNode applicationNode =  FindChildNode(mainManNode,APPLICATION);

			if (applicationNode == null) {
				Debug.LogError("no aplication tag found in AndroidManifest,xml");
				return;
			}

			bool ttunityActivityExist = false;

			var curr = applicationNode.FirstChild;
			while (curr != null)
			{
				XmlNode activityNode = curr;
				curr = curr.NextSibling;
				if (activityNode.Name != ACTIVITY) continue;

				// check if MAIN activity
				XmlNode intentFilterNode = activityNode.FirstChild;
				if (intentFilterNode == null) continue;
				
				while (intentFilterNode != null && intentFilterNode.Name != "intent-filter") {
 					Debug.Log ("intentFilterNode.Name:" + intentFilterNode.Name);
 					intentFilterNode = intentFilterNode.NextSibling;
 				}
 
 				if (intentFilterNode == null || intentFilterNode.Name != "intent-filter") {
 					continue;
 				}

 				while (intentFilterNode != null && intentFilterNode.Name != "intent-filter") {
 					Debug.Log ("intentFilterNode.Name:" + intentFilterNode.Name);
 					intentFilterNode = intentFilterNode.NextSibling;
 				}
 
 				if (intentFilterNode == null || intentFilterNode.Name != "intent-filter") {
 					continue;
 				}


				XmlNode actionNode = intentFilterNode.FirstChild;
				while (actionNode !=null) {
					XmlNode workingOn = actionNode;
					actionNode = actionNode.NextSibling;

					if (workingOn.Name != "action") continue;
					if (workingOn.Attributes == null) continue;
					string androidNameValue = workingOn.Attributes.GetNamedItem(ANDROID_NAME).Value;
					if (androidNameValue == null) continue;
					if ( androidNameValue == "android.intent.action.MAIN") { // Update acitivity name to TTUnityPlayerNativeActivity
						string androidActivityNameValue = activityNode.Attributes.GetNamedItem(ANDROID_NAME).Value;
						if (androidActivityNameValue == "com.tabtale.publishing.ttunity.TTUnityPlayerNativeActivity") { 
							ttunityActivityExist = true;
							((XmlElement)activityNode).SetAttribute("android:launchMode","singleTop");
							((XmlElement)activityNode).SetAttribute("android:hardwareAccelerated","true");
							putForwardNativeEventsToDalvikOnMainActivity(activityNode);
							main.Save(androidManifestFile);
							return;
						}

						if (outputFile != null) {
							activityNode.OwnerDocument.Save(outputFile + ".psdk_backup");
						}

						((XmlElement)activityNode).SetAttribute(ANDROID_NAME,"com.tabtale.publishing.ttunity.TTUnityPlayerNativeActivity");
						((XmlElement)activityNode).SetAttribute("android:launchMode","singleTop");
						((XmlElement)activityNode).SetAttribute("android:hardwareAccelerated","true");
                        			putForwardNativeEventsToDalvikOnMainActivity(activityNode);
						ttunityActivityExist = true;
					}
				}
			} // end while

			if (! ttunityActivityExist) {
				string txt  = 
									"<activity android:name=\"com.tabtale.publishing.ttunity.TTUnityPlayerNativeActivity\" android:label=\"@string/app_name\" android:launchMode=\"singleTop\" android:hardwareAccelerated=\"true\">"
									+"	<intent-filter>"
									+"		<action android:name=\"android.intent.action.MAIN\" />"
									+"		<category android:name=\"android.intent.category.LAUNCHER\" />"
									+"		<category android:name=\"android.intent.category.LEANBACK_LAUNCHER\" />"
									+"		</intent-filter>"
									+"		<meta-data android:name=\"unityplayer.UnityActivity\" android:value=\"true\" />"
									+"		<meta-data android:name=\"unityplayer.ForwardNativeEventsToDalvik\" android:value=\"true\" />"
									+"		</activity>";
				XmlText xfrag = applicationNode.OwnerDocument.CreateTextNode(txt);
				applicationNode.AppendChild(xfrag);
			}

			main.Save(androidManifestFile);
		}
        
        private static void putForwardNativeEventsToDalvikOnMainActivity(XmlNode mainActivity) {
            if (mainActivity == null) 
                return;
        
            XmlNode metaDataNode = mainActivity.FirstChild;
            XmlText xfrag = null;
            XmlElement elem = mainActivity.OwnerDocument.CreateElement("meta-data");
            elem.SetAttribute("name","http://schemas.android.com/apk/res/android","unityplayer.ForwardNativeEventsToDalvik");
            elem.SetAttribute("value","http://schemas.android.com/apk/res/android","true");
 
            if (metaDataNode == null) {
                mainActivity.AppendChild(elem);
                return;
            }
            do  {
                if (metaDataNode.Name == "meta-data") {
                    if (metaDataNode.Attributes == null)
                        continue;
                        
                    string androidNameValue = null;
                    if ( metaDataNode.Attributes.GetNamedItem(ANDROID_NAME) != null)
                        androidNameValue = metaDataNode.Attributes.GetNamedItem(ANDROID_NAME).Value;
                     
                     if (androidNameValue == null || androidNameValue != "unityplayer.ForwardNativeEventsToDalvik") 
                        continue;
                     // make it true and return;
                    ((XmlElement)metaDataNode).SetAttribute("android:value","true");
                    return;                    
                }
            } while ((metaDataNode = metaDataNode.NextSibling) != null );
  
            mainActivity.AppendChild(elem);
            return;               
        }

			public static string xmlNodeToString(System.Xml.XmlNode node, int indentation)
			{
				using (var sw = new System.IO.StringWriter())
				{
					using (var xw = new System.Xml.XmlTextWriter(sw))
					{
						xw.Formatting = System.Xml.Formatting.Indented;
						xw.Indentation = indentation;
					if (node.ParentNode != null){
						node.ParentNode.WriteContentTo(xw);
					}
					else {
						node.WriteContentTo(xw);
					}
					}
					return sw.ToString();
				}
			}

		static void DeleteXpathsFromManifest(string androidManifestPath) {

			string[] files = System.IO.Directory.GetFiles( Application.dataPath , "*.xpathDelFromiAndroidManifest.txt", SearchOption.AllDirectories );
			if (files.Length == 0) 
				return;

			// Load the document and set the root element.
			XmlDocument doc = new XmlDocument();
			doc.Load(androidManifestPath);
			XmlNode root = doc.DocumentElement;
			
			// Add the namespace.
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("android", "http://schemas.android.com/apk/res/android");
			
			foreach (var file in files) {
				UnityEngine.Debug.Log("AndroidManifest.xml remove file: " + file);
				foreach (string xpathLineToDel in File.ReadAllLines(file)) {
					UnityEngine.Debug.Log("AndroidManifest.xml remove line: " + xpathLineToDel);
					XmlNodeList nodesList = root.SelectNodes (xpathLineToDel, nsmgr);
					foreach (XmlNode node in nodesList) {
						node.ParentNode.RemoveChild(node);
					}
				}
			}
			
			// Fixing ForwardNativeEventToDalvik to true
			XmlNodeList nodes = root.SelectNodes ("//meta-data[@android:name=\"unityplayer.ForwardNativeEventsToDalvik\" and @android:value=\"false\"]", nsmgr);
			foreach (XmlNode node in nodes) {
				bool update = false;
				string attrNSURI = "";
				foreach( XmlAttribute attr in node.Attributes) {
					if (attr.Value == "unityplayer.ForwardNativeEventsToDalvik") {
						attrNSURI = attr.NamespaceURI;
						update = true;
					}
				}
				if (update)
					((XmlElement)node).SetAttribute("android:value", attrNSURI, "true");
			}
			
			doc.Save (androidManifestPath);
		}

	}
}

