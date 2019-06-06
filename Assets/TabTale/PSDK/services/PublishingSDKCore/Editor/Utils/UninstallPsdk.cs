using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TabTale.PSDK.Editor {

	public class UninstallPsdk {

		[MenuItem("TabTale/PSDK/Uninstall Psdk")]
        public static void UninstallPsdkMenuFunc() {
                if (EditorUtility.DisplayDialog("Uninstall PSDK", "Are you sure you want to uninstall PSDK", "Uninstall", "Cancel")) {
		UninstallPsdkFunc();
                }
        }


		public static void UninstallPsdkFunc() {
			UninstallPsdkFunc(false);
		}

		public static void UninstallPsdkFunc(bool full) {
			string[] pkgs = getPsdkPackagesFileSetsFiles();
			List<string> sortedPkgs = sortpsdkPackagesFileSetsFiles(pkgs);
			foreach (string fileSetPath in sortedPkgs) {
				UninstallFileSet(fileSetPath, full);
			}
		}

		static string[] getPsdkPackagesFileSetsFiles() {
			List<string> list = new List<string>();
			if (Directory.Exists (Path.Combine(Application.dataPath,TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath))) {
				foreach(var item in System.IO.Directory.GetFiles(Path.Combine(Application.dataPath,TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath) , "*.files.txt", SearchOption.AllDirectories )) {
					if (! item.Contains("FileSet")) continue;
					list.Add (item);
				}
			}
			return list.ToArray();
		}

		static List<string> sortpsdkPackagesFileSetsFiles(string[] list) {
			List<string> output = new List<string>();
			List<string> corePkgs = new List<string>();
			foreach(string item in list) {
				if (item.Contains("PSDKCore"))
					corePkgs.Add(item);
				else
					output.Add(item);
			}
			output.Sort();
			output.AddRange(corePkgs);
			return output;
		}

		static void UninstallFileSet(string pathToFileSet, bool full = false) {
			try {
				string[] filesToDelete = File.ReadAllLines(pathToFileSet);
				foreach(var fileToDelete in filesToDelete) {
					DeleteAFile(fileToDelete, ! full);
				}
				if (File.Exists(pathToFileSet)) {
					AssetDatabase.DeleteAsset(pathToFileSet);
				}
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
		}

		static bool InIgnoreList(string filePath) {
			if (filePath.StartsWith(Path.Combine("Assets","StreamingAssets")))  {
				if (filePath.Contains("versions")) // Delete the psdk versions files
					return false;
				return true;
			}
			if (filePath.Contains("UninstallPsdk.cs")) return true;
			if (filePath.Contains ("Plugins")) {
				if (filePath.Contains ("PSDK")) {
					if (filePath.Contains ("Interfaces"))
						return true;
					if (filePath.Contains ("EventsListeners"))
						return true;
					if (filePath.Contains ("Utils"))
						return true;
					if (filePath.Contains ("ServiceMgr"))
						return true;
				}
			}

			return false;
		}
		public static void UninstallPsdkUnityPackage(string pkgName, bool fullyUninstall = false) {
			if (pkgName == null) {
				Debug.LogError("null psdk unity package name !!!");
				return;
			}

			if (! pkgName.EndsWith (".unitypackage")) {
				pkgName += ".unitypackage";
			}

			if (!fullyUninstall && pkgName == "PSDKCore.unitypackage") {
				Debug.LogError("PSDKCore.unitypackage cannot be uninstalled individually, please use PSDK/Uninstall PSDK ...");
				return;
			}

			string unity5pkgname =  pkgName.Replace (".unitypackage","").Replace ("PSDK","");
			string unity5psdkServicesPath = System.IO.Path.Combine ("Assets",System.IO.Path.Combine ("PSDK", "services"));

			Debug.Log ("pkg for delete " + pkgName);
			IList<string> filesToDeleteList = new List<string>();
			IList<string> filesToPreserveList = new List<string>();
			string[] fileSetsFiles = getPsdkPackagesFileSetsFiles ();
			foreach (string fileSetPath in fileSetsFiles) {
				if (fileSetPath.Contains(pkgName) || (fileSetPath.Contains(unity5pkgname) && fileSetPath.Contains(unity5psdkServicesPath))) {
					// FileSets to delete
					Debug.Log ("filepath to read for delete " + fileSetPath);
					string[] filesToDeleteArray = File.ReadAllLines(fileSetPath);
					foreach(var fileToDelete in filesToDeleteArray) {
						if (!fullyUninstall && InIgnoreList(fileToDelete)) 
							continue;
						filesToDeleteList.Add(fileToDelete);
					}
					filesToDeleteList.Add(fileSetPath);
				}
				else {
					// FileSets to preserve
					string[] filesToPreserveArray = File.ReadAllLines(fileSetPath);
					foreach(var fileToPreserve in filesToPreserveArray) {
						filesToPreserveList.Add(fileToPreserve);
							filesToDeleteList.Remove(fileToPreserve);
					}

				}
			}
			foreach(var fileToPreserve in filesToPreserveList) {
				filesToDeleteList.Remove(fileToPreserve);
			}
			foreach(var fileToDelete in filesToDeleteList) {
				DeleteAFile(fileToDelete);
			}


		}


		static void DeleteAFile(string fileToDelete, bool checkIgnoreList = true) {

			if (checkIgnoreList) {
				if (InIgnoreList (fileToDelete)) 
					return;
			}

			string fullPathToDelete = Path.Combine(Application.dataPath.Substring(0,Application.dataPath.LastIndexOf("Assets")),fileToDelete);
			if (File.Exists(fullPathToDelete)) {
				Debug.Log(fullPathToDelete + " will be deleted !");
				AssetDatabase.DeleteAsset(fileToDelete);
			}
		}
	}


}
