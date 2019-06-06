using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CocoPlay.PSDKPackage
{
	public class PackageImportWindow : EditorWindow
	{
		#region Window

		private static PackageImportWindow _instance;

		[MenuItem ("CocoPlay/Framework/Run PSDK Package Importer...", false, 100)]
		private static void Init ()
		{
			if (EditorApplication.isPlaying) {
				EditorUtility.DisplayDialog ("Play Mode", "The game is running -_-", "OK");
				return;
			}

			if (_instance != null) {
				EditorUtility.DisplayDialog ("Another Opened", "Another importer aleardy is opened, please close it at first.", "OK");
				return;
			}

			_instance = GetWindow<PackageImportWindow> ("PSDK Importer", true);
			if (_instance != null) {
				_instance.minSize = new Vector2 (600, 400);
				_instance.Show ();
			}
		}

		private void OnGUI ()
		{
			BeginWindows ();

			OnGUIPackageLoad ();
			OnGUIPackageExtract ();

			EndWindows ();
		}

		#endregion


		#region Load

		private class PackageInfo
		{
			private readonly string _name;
			private readonly string _fullPath;

			public string Name {
				get { return _name; }
			}

			public string FullPath {
				get { return _fullPath; }
			}

			public bool IsSelected { get; set; }

			public PackageInfo (string path)
			{
				_name = Path.GetFileNameWithoutExtension (path);
				_fullPath = path;
				IsSelected = true;
			}
		}

		private string _packageRootDirectory = string.Empty;
		private readonly List<PackageInfo> _packageInfos = new List<PackageInfo> ();

		private void OnGUIPackageLoad ()
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			// load button
			Color originalColor = GUI.contentColor;
			GUI.contentColor = Color.yellow;
			if (GUILayout.Button ("Load Packages")) {
				_packageRootDirectory = EditorUtility.OpenFolderPanel ("Load Packages", _packageRootDirectory, "");
				LoadPackages (_packageRootDirectory);
				return;
			}
			GUI.contentColor = originalColor;

			// package infos
			EditorGUILayout.LabelField ("Package Path:", _packageRootDirectory);
			EditorGUILayout.BeginVertical (GUI.skin.box);
			if (_packageInfos.Count > 0) {
				foreach (var packageInfo in _packageInfos) {
					packageInfo.IsSelected = EditorGUILayout.ToggleLeft (packageInfo.Name, packageInfo.IsSelected);
				}
			} else {
				EditorGUILayout.LabelField ("No Packages was found.");
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.EndVertical ();
		}

		private void LoadPackages (string rootDirectory)
		{
			if (!Directory.Exists (rootDirectory)) {
				return;
			}

			var paths = Directory.GetFiles (rootDirectory, "*.unitypackage");
			_packageInfos.Clear ();

			foreach (var path in paths) {
				_packageInfos.Add (new PackageInfo (path));
			}
		}

		#endregion


		#region Extract

		private PackageExtractor _extractor;

		private PackageExtractor Extractor {
			get { return _extractor ?? (_extractor = new PackageExtractor ()); }
		}

		private void OnGUIPackageExtract ()
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			// load button
			Color originalColor = GUI.contentColor;
			GUI.contentColor = Color.green;
			if (GUILayout.Button ("Import Packages")) {
				ExtractPackages ();
			}
			GUI.contentColor = originalColor;

			EditorGUILayout.EndVertical ();
		}

		private void ExtractPackages ()
		{
			if (_packageInfos == null || _packageInfos.Count <= 0) {
				return;
			}

			try {
				// clear old assets
				string oldDirectory = Path.Combine (Application.dataPath, "TabTale/PSDK");
				if (Directory.Exists (oldDirectory)) {
					Directory.Delete (oldDirectory, true);
				}

				// create temp extract directory
				string extractDirectory = Path.Combine (_packageRootDirectory, "psdk_extract");
				Extractor.CreateDirectory (extractDirectory, true);

				// extact package assets
				float startProgress = 0f;
				float stepProgress = 0.8f / _packageInfos.Count;
				foreach (var packageInfo in _packageInfos) {
					if (packageInfo.IsSelected) {
						Extractor.ExtractPackage (packageInfo.FullPath, extractDirectory, startProgress, stepProgress);
					}

					startProgress += stepProgress;
				}

				// move all assets in project path
				EditorUtility.DisplayProgressBar ("Move Assets", "move all assets in project ...", 0.9f);
				string projectPath = Path.GetDirectoryName (Application.dataPath);
				Extractor.MoveAllFileSystemEntries (extractDirectory, projectPath, true);

				// clear
				Directory.Delete (extractDirectory, true);
			}
			catch (Exception e) {
				Debug.LogErrorFormat ("{0}->ExtractPackages: {1}", GetType ().Name, e.Message);
			}
			finally {
				EditorUtility.ClearProgressBar ();
			}

			// refresh assets
			AssetDatabase.Refresh ();
			AssetDatabase.SaveAssets ();
		}

		#endregion
	}
}