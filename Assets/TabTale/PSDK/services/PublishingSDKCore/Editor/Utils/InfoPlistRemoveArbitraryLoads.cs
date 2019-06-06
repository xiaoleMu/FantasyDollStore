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
    public static class InfoPlistRemoveArbitraryLoads
    {

		static InfoPlistRemoveArbitraryLoads() {
		}

		[PostProcessBuild(90)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
				#if UNITY_IOS
				UpdateInfoPlist (pathToBuiltProject);
				#endif
		}

//		[MenuItem ("PSDK/Remove Arbitrary Loads")]
//		static void UpdateInfoPlistCommand2 () {
//			UpdateInfoPlist (Application.dataPath);
//		}

		static void UpdateInfoPlist(string pathToBuiltProject) {
			UpdateInfoPlist(Path.Combine(Application.dataPath,TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath),pathToBuiltProject);
		}

		private static void UpdateInfoPlist(string path, string pathToBuiltProject) {
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
			
		changeArbitraryLoads(main);	

			main.Save(outputFile);
			return;
		}

		private static bool changeArbitraryLoads(XmlNode parent) {
			while (parent != null) {
                if (parent.Name.Equals("key") && parent.InnerText.Equals("NSAllowsArbitraryLoads")) {
					parent.ParentNode.RemoveChild (parent.NextSibling);
					parent.ParentNode.RemoveChild (parent);
					return true;
                 }
				if (parent.FirstChild != null) {
					if (changeArbitraryLoads (parent.FirstChild))
						return true;
				}
				parent = parent.NextSibling;
			}
			return false;
		}

	}
}
