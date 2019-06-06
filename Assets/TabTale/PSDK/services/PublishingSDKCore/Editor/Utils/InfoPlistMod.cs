using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Callbacks;

namespace TabTale.PSDK.UnityEditor
{
	[InitializeOnLoad]
    public static class InfoPlistMod
    {

		static InfoPlistMod() {
		}

		[PostProcessBuild(90)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
				#if UNITY_IOS
				UpdateInfoPlist (pathToBuiltProject);
				#endif
		}

//		[MenuItem ("PSDK/Update Info.plist")]
//		static void UpdateInfoPlistCommand () {
//			UpdateInfoPlist (Application.dataPath);
//		}

		static void UpdateInfoPlist(string pathToBuiltProject) {
			UpdateInfoPlistFromManifestModsUnderPath (Path.Combine(Application.dataPath,TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath),pathToBuiltProject);
		}

		public static void UpdateInfoPlistFromManifestModsUnderPath(string path, string pathToBuiltProject) {
			var files = System.IO.Directory.GetFiles( path , "*.infoPlistMod.xml", SearchOption.AllDirectories );
			foreach( var file in files ) {
				UpdateInfoPlistFromMod(file,  pathToBuiltProject);
			}
		}

		private static bool UpdateInfoPlistFromMod(string modPath, string pathToBuiltProject) {
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
			
			
			XmlNode mainManNode = FindChildNodeWithChildrens(main, "plist");
			XmlNode modManNode = FindChildNodeWithChildrens(mod, "plist");
			
			if (mainManNode == null)
			{
				Debug.LogError("No plist tag at  " + outputFile);
				return false;
			}
			
			if (modManNode == null)
			{
				Debug.LogError("No plist tag at " + modPath);
				return false;
			}
			
			

			UpdateInfoPlistRecursivly (mainManNode, modManNode);

			//UpdateAndroidManifestNodesOrder(mainManNode);

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

		private static XmlNode FindChildNodeWithChildrens(XmlNode parent, string name)
		{
			XmlNode curr = parent.FirstChild;
			while (curr != null)
			{
				if (curr.Name.Equals(name) || (curr.InnerText != null && curr.Name.Equals("key") && curr.InnerText.Equals(name) ))
				{
					if (curr.ChildNodes.Count > 0) {
						return curr;
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

		private static bool UpdateNodeAttributes(XmlNode main, XmlNode mod) {
			if (mod == null || mod.Attributes == null) return true;
			foreach (XmlAttribute modAttribute in  mod.Attributes) {
				XmlNode mainAttribute = main.Attributes.GetNamedItem(modAttribute.Name);
				if (null == mainAttribute ) { // missing attribute, copy it
					if (mod is XmlElement) {
						((XmlElement)mod).SetAttribute(modAttribute.Name,modAttribute.Value);
					}
				}
				else { // existing attribute, leave it as is
					if (modAttribute.Value != mainAttribute.Value) {
						Debug.LogWarning("Info.plist attribute " + main.Name + ":" + mainAttribute.Name + " has different values: main:" + mainAttribute.Value + ", mod:" + modAttribute.Value);
					}
				}
			}
			return true;
		}
		
		private static bool UpdateNodeBody(XmlNode main, XmlNode mod) {
			bool rc = true;
			rc = UpdateInfoPlistRecursivly (main, mod);
			return rc;
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

		private static bool UpdateInfoPlistRecursivly(XmlNode mainManNode, XmlNode modManNode) {
			var curr = modManNode.FirstChild;
			while (curr != null)
			{
				XmlNode mainNode = null;
				if (XmlNodeHasKey(curr)) {
					mainNode = FindElementWithKey(XmlNodeKey(curr),mainManNode);
					Log ("Main FindElementWithKey of");
					Log (XmlNodeKey(curr));
					Log ("returned ");
					Log (mainNode);
				}
				else {
//					if (mainNode == null)
//						mainNode = FindChildNode(mainManNode,XmlNodeKey(curr));
				}

				if (mainNode == null) { // node not exist in main, copy from mod
					if (curr.NextSibling != null) {
						XmlNode newImportedKeyNode = mainManNode.OwnerDocument.ImportNode(curr, true);
						mainManNode.AppendChild(newImportedKeyNode); 
						XmlNode newImportedValueNode = mainManNode.OwnerDocument.ImportNode(curr.NextSibling, true);
						mainManNode.AppendChild(newImportedValueNode); 
					}
				}
				else { // node exist at main, travcerse its attributes and body.
					UpdateNodeAttributes(mainNode,curr);
					UpdateNodeBody(mainNode,curr);
				}

				curr = curr.NextSibling; // going to the value
				if (curr != null)
					curr = curr.NextSibling; // going to next key
			}
			bruteForceUpdateMainNode(mainManNode);
			return true;
		}

		private static void bruteForceUpdateMainNode(XmlNode mainManNode) {
			if (mainManNode == null) return;
			if (mainManNode.Attributes == null) return;
//			XmlNode node = mainManNode.Attributes.GetNamedItem("android:name");
//			if (node == null) return;
//			string androidName = node.Value;
//			if (androidName == null) return;
//			node = mainManNode.Attributes.GetNamedItem("android:value");
//			if (node == null) return;
//			string androidValue = node.Value;
//			if (androidValue == null) return;
//
//			if ( androidName == "unityplayer.ForwardNativeEventsToDalvik" && node.Value !="true") {
//				Debug.Log ("MAnifestMod: Changing unityplayer.ForwardNativeEventsToDalvik to true");
//				node.Value ="true";
//			}

		}

//		private static void UpdateAndroidManifestNodesOrder(XmlNode mainManNode) {
//
//			if (mainManNode == null) return;
//
//			XmlNode applicationNode =  FindChildNode(mainManNode,"application");
//					
//			// always put application node at the end.
//			if (applicationNode != null) {
//				mainManNode.RemoveChild(applicationNode);
//				mainManNode.AppendChild(applicationNode);
//			}
//			return;
//		}

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
	}
}
