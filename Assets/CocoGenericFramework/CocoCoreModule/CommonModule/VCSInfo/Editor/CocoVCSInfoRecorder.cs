using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CocoPlay.VCSInfo
{
	[InitializeOnLoad]
	public class CocoVCSInfoRecorder
	{
		private const int COMMIT_ID_LENGTH = 5;

		static CocoVCSInfoRecorder ()
		{
			RecordVCSInfo ();
		}

		private static void RecordVCSInfo ()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode) {
				return;
			}

			string output = ObtainVCSInfo ();

			string path = CocoVCSInfoDisplayer.InfoFilePath;
			CocoVCSInfo vcsInfo = LoadVCSInfo (path);

			bool changed = UpdateVCSInfo (vcsInfo, output);
			if (!changed) {
				return;
			}

			SaveVCSInfo (vcsInfo, path);
			AssetDatabase.Refresh ();
		}

		private static string ObtainVCSInfo ()
		{
			var p = new Process {
				StartInfo = {
					FileName = "git",
					Arguments = "rev-parse HEAD",
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			};

			p.Start ();
			var output = p.StandardOutput.ReadToEnd ();
			p.Close ();

			return output;
		}

		private static bool UpdateVCSInfo (CocoVCSInfo vcsInfo, string output)
		{
			bool changed = false;

			// commit id
			string commitId = output.Substring (0, COMMIT_ID_LENGTH);
			if (!string.Equals (commitId, vcsInfo.CommitId)) {
				vcsInfo.CommitId = commitId;
				changed = true;
			}

			// load time
			try {
				string loadTime = DateTime.UtcNow.ToString (CocoVCSInfoDisplayer.INFO_TIME_FORMAT, CultureInfo.InvariantCulture);
				if (!string.Equals (loadTime, vcsInfo.LoadTime)) {
					vcsInfo.LoadTime = loadTime;
					changed = true;
				}
			}
			catch (Exception e) {
				Debug.LogErrorFormat ("CocoVCSInfoRecorder->UpdateVCSInfo: load time read error! [{0}]", e.Message);
			}

			return changed;
		}

		private static void SaveVCSInfo (CocoVCSInfo vcsInfo, string path)
		{
			string json = JsonUtility.ToJson (vcsInfo);

			string directory = Path.GetDirectoryName (path);
			if (string.IsNullOrEmpty (directory)) {
				return;
			}
			if (!Directory.Exists (directory)) {
				Directory.CreateDirectory (directory);
			}

			if (File.Exists (path)) {
				File.Delete (path);
			}
			File.WriteAllText (path, json);
			Debug.Log ("vcs info saved: " + json);
		}

		private static CocoVCSInfo LoadVCSInfo (string path)
		{
			CocoVCSInfo vcsInfo = new CocoVCSInfo ();

			if (File.Exists (path)) {
				string json = File.ReadAllText (path);
				JsonUtility.FromJsonOverwrite (json, vcsInfo);
			}

			return vcsInfo;
		}
	}
}