using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class FindProject {

	#if UNITY_EDITOR_OSX

	[MenuItem("Assets/Find References In Project", false, 2000)]
	private static void FindProjectReferences()
	{
		string appDataPath = Application.dataPath;
		string output = "";
		string selectedAssetPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		List<string> references = new List<string>();

		string guid = AssetDatabase.AssetPathToGUID (selectedAssetPath);

		var psi = new System.Diagnostics.ProcessStartInfo();
		psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
		psi.FileName = "/usr/bin/mdfind";
		psi.Arguments = "-onlyin " + Application.dataPath + " " + guid;
		psi.UseShellExecute = false;
		psi.RedirectStandardOutput = true;
		psi.RedirectStandardError = true;

		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo = psi;

		process.OutputDataReceived += (sender, e) => {
			if(string.IsNullOrEmpty(e.Data))
				return;

			string relativePath = "Assets" + e.Data.Replace(appDataPath, "");

			// skip the meta file of whatever we have selected
			if(relativePath == selectedAssetPath + ".meta")
				return;

			references.Add(relativePath);

		};
		process.ErrorDataReceived += (sender, e) => {
			if(string.IsNullOrEmpty(e.Data))
				return;

			output += "Error: " + e.Data + "\n";
		};
		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();

		process.WaitForExit(2000);

		foreach(var file in references){
			output += file + "\n";
			Debug.Log(file, AssetDatabase.LoadMainAssetAtPath(file));
		}

		Debug.LogWarning(references.Count + " references found for object " + Selection.activeObject.name + "\n\n" + output);
	}

	#endif
}
