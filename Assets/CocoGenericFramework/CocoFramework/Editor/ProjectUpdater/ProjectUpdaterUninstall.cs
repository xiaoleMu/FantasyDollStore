using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TabTale.ProjectUpdater {

	public class ProjectUpdaterUninstall 
	{
		public static void UninstallPackage(string package) 
		{
			string[] pkgs = GetPackagesFileSetsFiles(package);
			List<string> pkgList = pkgs.ToList();
			foreach (string fileSetPath in pkgList) 
			{
				UninstallFileSet(fileSetPath);
			}
		}

		static void UninstallFileSet(string pathToFileSet) 
		{
			Debug.Log ("deleting " + pathToFileSet + " files !");
			try {
				string[] filesToDelete = File.ReadAllLines(pathToFileSet);
				foreach(var fileToDelete in filesToDelete) {
					
					if (InIgnoreList(fileToDelete)) 
						continue;
					
					string fullPathToDelete = Path.Combine(Application.dataPath.Substring(0,Application.dataPath.LastIndexOf("Assets")),fileToDelete);
					if (File.Exists(fullPathToDelete)) {
						Debug.Log(fullPathToDelete + " will be deleted !");

						Debug.Log ("Simulating delete of " + fileToDelete);
						//AssetDatabase.DeleteAsset(fileToDelete);

						//File.Delete(fullPathToDelete);
					}
				}
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
		}

		static string[] GetPackagesFileSetsFiles(string packageName) {
			string path = Path.Combine(Application.dataPath,"Tabtale/UnityPackages");
			path = Path.Combine(path,"FileSet");
			if (! Directory.Exists(path))
				Directory.CreateDirectory(path);

			string fileSets = packageName + ".*.files.txt";
			return System.IO.Directory.GetFiles( path, fileSets, SearchOption.AllDirectories );
		}

		static bool InIgnoreList(string filePath) {
			if (filePath.StartsWith(Path.Combine("Assets","StreamingAssets")))  {
				if (filePath.Contains("versions")) // Delete the psdk versions files
					return false;
				return true;
			}

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
	}
}