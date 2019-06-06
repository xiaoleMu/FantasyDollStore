using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace CocoPlay.PSDKPackage
{
	public class PackageExtractor
	{
		public void ExtractPackage (string packagePath, string targetDirectory, float startProgress, float stepProgress)
		{
			if (string.IsNullOrEmpty (packagePath)) {
				return;
			}

			// create temp extract directory
			string tempDirectory = Path.GetDirectoryName (packagePath);
			if (tempDirectory == null) {
				return;
			}
			tempDirectory = Path.Combine (tempDirectory, "extract_temp");
			CreateDirectory (tempDirectory, true);

			// extract files to temp directory
			if (ExtracNativeAssets (packagePath, tempDirectory)) {
				InflateAssets (tempDirectory, targetDirectory, startProgress, stepProgress);
			}

			// clear temp directory
			Directory.Delete (tempDirectory, true);
		}

		private void ShowProgressBar (string msg, float startProgress, float stepProgress, float currProgress)
		{
			EditorUtility.DisplayProgressBar ("Extract Packages", msg, startProgress + stepProgress * currProgress);
		}

		private bool ExtracNativeAssets (string packagePath, string targetDirectory)
		{
			var process = new Process {
				StartInfo = {
					FileName = "tar",
					Arguments = string.Format ("xzf \"{0}\" -C \"{1}\"", packagePath, targetDirectory),
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			};

			try {
				process.Start ();
				process.WaitForExit ();
				process.Close ();
			}
			catch (Exception e) {
				Debug.LogErrorFormat ("{0}->ExtractPackage: extract package {1} failed: {2}", GetType ().Name, packagePath, e.Message);
				return false;
			}

			return true;
		}

		private void InflateAssets (string nativeDirectory, string targetDirectory, float startProgress, float stepProgress)
		{
			var assetDirectories = Directory.GetDirectories (nativeDirectory);

			float progress = 0;
			string msg = string.Empty;
			foreach (var directory in assetDirectories) {
				string targetPath = InflateAsset (directory, targetDirectory);
				progress += 1;
				if (string.IsNullOrEmpty (targetPath)) {
					return;
				}

				msg = string.Format ("Inflating Assets: {0}", targetPath);
				ShowProgressBar (msg, startProgress, stepProgress, progress / assetDirectories.Length);
			}
		}

		private string InflateAsset (string assetDirectory, string targetDirectory)
		{
			// path name
			string filePath = Path.Combine (assetDirectory, "pathname");
			if (!File.Exists (filePath)) {
				return string.Empty;
			}

			string pathName = File.ReadAllText (filePath);

			// asset
			filePath = Path.Combine (assetDirectory, "asset");
			if (!File.Exists (filePath)) {
				filePath = string.Empty;
			}

			// meta
			string metaFilePath = Path.Combine (assetDirectory, "asset.meta");
			if (!File.Exists (metaFilePath)) {
				metaFilePath = string.Empty;
			}

			if (string.IsNullOrEmpty (filePath) && string.IsNullOrEmpty (metaFilePath)) {
				return string.Empty;
			}

			Debug.LogFormat ("path - {0}\nasset - {1}\nmeta - {2}", pathName, filePath, metaFilePath);
			string fullPathName = Path.Combine (targetDirectory, pathName);

			if (string.IsNullOrEmpty (filePath)) {
				// folder asset
				CreateDirectory (fullPathName);
			} else {
				// file asset
				MoveFile (filePath, fullPathName);
			}

			if (!string.IsNullOrEmpty (metaFilePath)) {
				string targetMetaPath = fullPathName + ".meta";
				MoveFile (metaFilePath, targetMetaPath);
			}

			return pathName;
		}

		private void MoveFile (string sourcePath, string targetPath, bool replace = false)
		{
			//Debug.LogError (sourcePath + " -> " + targetPath + ", " + replace);
			if (File.Exists (targetPath)) {
				if (!replace) {
					return;
				}

				File.Delete (targetPath);
			}

			string directory = Path.GetDirectoryName (targetPath);
			CreateDirectory (directory);

			File.Move (sourcePath, targetPath);
		}

		public void CreateDirectory (string directoryPath, bool replace = false)
		{
			if (Directory.Exists (directoryPath)) {
				if (!replace) {
					return;
				}

				Directory.Delete (directoryPath, true);
			}

			Directory.CreateDirectory (directoryPath);
		}

		public void MoveAllFileSystemEntries (string rootDirectory, string targetDirectory, bool replace = false)
		{
			string targetPath;

			// move files
			var filePaths = Directory.GetFiles (rootDirectory);
			foreach (var filePath in filePaths) {
				targetPath = Path.GetFileName (filePath);
				targetPath = Path.Combine (targetDirectory, targetPath);
				MoveFile (filePath, targetPath, replace);
			}

			var directoryPaths = Directory.GetDirectories (rootDirectory);
			foreach (var directoryPath in directoryPaths) {
				targetPath = Path.GetFileName (directoryPath);
				if (string.IsNullOrEmpty (targetPath)) {
					continue;
				}

				targetPath = Path.Combine (targetDirectory, targetPath);
				CreateDirectory (targetPath);
				MoveAllFileSystemEntries (directoryPath, targetPath, replace);
			}
		}
	}
}