using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceEditorHelper
	{
		#region GUI

		public static void WideLabelField (string label, GUIStyle style, int additionalWideCount = 1)
		{
			if (additionalWideCount <= 0) {
				EditorGUILayout.LabelField (label, style);
				return;
			}

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (label, style);
			for (var i = 0; i < additionalWideCount; i++) {
				EditorGUILayout.LabelField ("");
			}
			EditorGUILayout.EndHorizontal ();
		}

		#endregion


		#region File / Directory

		public static void ClearDirectory (string path)
		{
			if (!Directory.Exists (path)) {
				return;
			}

			var directories = Directory.GetDirectories (path);
			foreach (var directory in directories) {
				Directory.Delete (directory, true);
			}

			var files = Directory.GetFiles (path);
			foreach (var file in files) {
				File.Delete (file);
			}
		}

		public static void DeleteDirectory (string path)
		{
			if (!Directory.Exists (path)) {
				return;
			}

			Directory.Delete (path, true);

			var metaPath = path + ".meta";
			if (File.Exists (metaPath)) {
				File.Delete (metaPath);
			}
		}

		public static void DeleteFile (string path)
		{
			if (!File.Exists (path)) {
				return;
			}

			File.Delete (path);

			var metaPath = path + ".meta";
			if (File.Exists (metaPath)) {
				File.Delete (metaPath);
			}
		}

		public static void MoveFile (string sourcePath, string targetPath)
		{
			if (!File.Exists (sourcePath)) {
				return;
			}

			DeleteFile (targetPath);
			File.Move (sourcePath, targetPath);

			// meta
			var sourceMetaPath = sourcePath + ".meta";
			if (!File.Exists (sourceMetaPath)) {
				return;
			}

			var targetMetaPath = sourcePath + ".meta";
			File.Move (sourceMetaPath, targetMetaPath);
		}

		public static IEnumerable<string> CollectBrotherDirectoryNames (string rootPath, string directoryName)
		{
			if (!Directory.Exists (rootPath)) {
				return null;
			}

			var currDirectory = Path.Combine (rootPath, directoryName);
			if (!Directory.Exists (rootPath)) {
				return null;
			}

			var directories = Directory.GetDirectories (rootPath);
			var brothers = from directory in directories where directory != currDirectory select Path.GetFileName (directory);
			return brothers;
		}

		#endregion
	}
}