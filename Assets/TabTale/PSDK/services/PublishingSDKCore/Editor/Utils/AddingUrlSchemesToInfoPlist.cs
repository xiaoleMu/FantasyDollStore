using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;

namespace TabTale.PSDK.UnityEditor
{
	[InitializeOnLoad]
    public static class AddingUrlSchemesToInfoPlist
    {

		static AddingUrlSchemesToInfoPlist() {
			#if UNITY_IOS
//			CreatingUrlSchemesInfoPlistMod ();
			#endif
		}

		[PostProcessBuild(1)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			#if UNITY_IOS
			CreatingUrlSchemesInfoPlistMod ();
			#endif
		}

		//[MenuItem ("PSDK/Create urlSchemes.infoPlistMod.xml")]
		static void CreatingUrlSchemesInfoPlistMod () {
			string bundleIdentifier = TabTale.Plugins.PSDK.PsdkUtils.BundleIdentifier;
			string str = "<plist>\n";
			str += "\t<dict>\n";
			str += "\t\t<key>CFBundleURLTypes</key>\n";
			str += "\t\t<array>\n";
			str += "\t\t\t<dict>\n";
			str += "\t\t\t\t<key>CFBundleTypeRole</key>\n";
			str += "\t\t\t\t<string>None</string>\n";
			str += "\t\t\t\t<key>CFBundleURLName</key>\n";
			str += "\t\t\t\t<string>"+bundleIdentifier+"</string>\n";
			str += "\t\t\t\t<key>CFBundleURLSchemes</key>\n";
			str += "\t\t\t\t<array>\n";
			str += "\t\t\t\t\t<string>"+bundleIdentifier+"</string>\n";
			str += "\t\t\t\t</array>\n";
			str += "\t\t\t</dict>\n";
			str += "\t\t</array>\n";
			str += "\t</dict>\n";
			str += "</plist>\n";

			string fileDir = Path.Combine (Application.dataPath,  TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath);
			if (! Directory.Exists (fileDir))
				Directory.CreateDirectory (fileDir);
                
                //services/PublishingSDKCore/Editor/InfoPlist

			fileDir = Path.Combine (fileDir, "services");
			if (! Directory.Exists (fileDir))
				Directory.CreateDirectory (fileDir);

			fileDir = Path.Combine (fileDir, "PublishingSDKCore");
			if (! Directory.Exists (fileDir))
				Directory.CreateDirectory (fileDir);
			
			fileDir = Path.Combine (fileDir, "Editor");
			if (! Directory.Exists (fileDir))
				Directory.CreateDirectory (fileDir);
			
			fileDir = Path.Combine (fileDir, "InfoPlist");
			if (! Directory.Exists (fileDir))
				Directory.CreateDirectory (fileDir);
			

			string filePath = Path.Combine (fileDir, "urlSchemes.infoPlistMod.xml");
			File.WriteAllText (filePath,str);
		}
	}
}
