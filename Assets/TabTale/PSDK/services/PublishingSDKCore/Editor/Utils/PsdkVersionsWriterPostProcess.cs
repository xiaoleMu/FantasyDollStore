using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public static class PsdkVersionsWriterPostProcess
{
	static PsdkVersionsWriterPostProcess() {
		writeVersionsToFile();
	}

	[PostProcessBuild(98)]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		DidReloadScripts ();
	}

	[DidReloadScripts]
	private static void DidReloadScripts() {
		writeVersionsToFile();
	}


	private static void writeVersionsToFile() {
		string playerSettingsPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.streamingAssetsPath,"psdk"),"versions");

		if (! System.IO.Directory.Exists(playerSettingsPath))
			System.IO.Directory.CreateDirectory(playerSettingsPath);

		string[] files = System.IO.Directory.GetFiles(playerSettingsPath, "*.unitypackage.version.txt", SearchOption.TopDirectoryOnly);
		string lastVersion = null;
		string versionWarning = "";
		string versions = "";
		foreach(string file in files) {
			System.IO.StreamReader myFile =
				new System.IO.StreamReader(file);
			string version = myFile.ReadToEnd();
			string filename = System.IO.Path.GetFileName(file);
			string pkg = filename.Substring(0,filename.IndexOf("."));
			if (versions.Length > 0)
				versions += ",";
			versions += pkg + ":" + version;
			myFile.Close();		
			if (lastVersion == null)
				lastVersion = version;
			else
			{
				if ( lastVersion != version) {
					if (versionWarning == "")
						versionWarning += "WARNING: ";
					versionWarning += pkg + " has different version, ";
				}
			}
		}
		versionWarning = versionWarning.Trim ().Replace(System.Environment.NewLine,"");
		versions = versions.Trim ().Replace(System.Environment.NewLine,"");
		if (! System.String.IsNullOrEmpty(versionWarning))
			versions += "," + versionWarning;
		
		File.WriteAllText(System.IO.Path.Combine(playerSettingsPath,"versions.txt"),versions );
	}

}
