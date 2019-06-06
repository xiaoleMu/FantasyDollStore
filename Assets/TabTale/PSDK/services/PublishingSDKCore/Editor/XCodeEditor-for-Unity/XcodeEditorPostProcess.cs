#if UNITY_IPHONE
using System.IO;
using UnityEngine;
using UnityEditor;
using PSDK.UnityEditor.XCodeEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;

public static class XCodeEditorPostProcess
{

	[PostProcessBuild(40000)]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{

		string subfoldersUnder = Path.Combine(Application.dataPath, TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath);

		// untaring atred frameworks, only on OSX (not working for windows platform)
		string thirdPartyProjModFilePath = Path.Combine(path,"psdkThirdParty.projmods");
		#if UNITY_EDITOR_OSX
		string frameworks = "";
		string embedded_frameworks = "";
		var tars = System.IO.Directory.GetFiles( subfoldersUnder , "*.forXcode.tar", SearchOption.AllDirectories );

		foreach( var file in tars ) {

			Untar (file,path);

			string filename = Path.GetFileName(file);
			filename = filename.Substring(0, filename.LastIndexOf(".forXcode.tar"));
			if(filename.Contains(".embeddedframework")){
				if (embedded_frameworks.Length > 0 && embedded_frameworks[embedded_frameworks.Length-1] != ',') 
					embedded_frameworks += ",";
				embedded_frameworks += "\"" + filename + "\"";
			}
			else {
				if (frameworks.Length > 0 && frameworks[frameworks.Length-1] != ',') 
					frameworks += ",";
				frameworks += "\"" + filename + "\"";
			}

		}
		if (frameworks.Length > 0 || embedded_frameworks.Length > 0) {
			string thirdPartyProjMod = 
			"{\n" + 
				"\"group\": \"psdk\",\n" + 
					"\"libs\": [],\n" + 
					"\"frameworks\": [],\n" + 
					"\"headerpaths\": [],\n" + 
					"\"files\": ["+frameworks+"],\n" + 
					"\"folders\": [" +embedded_frameworks+ "],\n" + 
					"\"excludes\": [\"^.*.DS_Store$\",\"^.*.meta$\", \"^.*.mdown^\", \"^.*.pdf$\", \"^.*.svn$\"],\n" + 
					"\"buildSettings\": {\"GCC_ENABLE_CPP_EXCEPTIONS\": \"YES\",\"GCC_ENABLE_OBJC_EXCEPTIONS\":\"YES\",\"OTHER_LDFLAGS\" : [\"-ObjC\"]}\n" + 
			"}\n";
			UnityEngine.Debug.Log(thirdPartyProjMod);
			File.WriteAllText(thirdPartyProjModFilePath,thirdPartyProjMod);
		}
		#endif
		

		// Create a new project object from build target
		XCProject project = new XCProject( path );

		if (File.Exists(thirdPartyProjModFilePath)) {
			UnityEngine.Debug.Log ("invoking " + thirdPartyProjModFilePath);
			project.ApplyMod( thirdPartyProjModFilePath );
		}

		// Find and run through all projmods files to patch the project
		var files = System.IO.Directory.GetFiles( subfoldersUnder , "*.projmods", SearchOption.AllDirectories );
		foreach( var file in files ) {
			UnityEngine.Debug.Log ("invoking " + file);
			try {
				project.ApplyMod( file );
			}
			catch (System.Exception e) {
				UnityEngine.Debug.LogException(e);
				UnityEngine.Debug.LogError("Failed for mod:" + file);
				if (UnityEditorInternal.InternalEditorUtility.inBatchMode) {
					EditorApplication.Exit(-1);
				}
			}
		}
		
		// Finally save the xcode project
		project.Save();
	}


	static void Untar(string file, string destPath = null) {
			// check if on mac
		//string file_basename = Path.GetFileName(file);
		ProcessStartInfo proc = new ProcessStartInfo();
		string command = "tar";
		proc.FileName = command;
		proc.WorkingDirectory = Path.GetDirectoryName(file);
		string arguments = "xvf \"" + file + "\""; 
		if (destPath != null) {
			arguments += " -C \"" + destPath + "\"";
		}
		proc.Arguments = arguments;
		proc.WindowStyle = ProcessWindowStyle.Minimized;
		proc.CreateNoWindow = true;
		UnityEngine.Debug.Log (command + " " + arguments);
		proc.UseShellExecute = false; 
		proc.CreateNoWindow = true;
		proc.WindowStyle = ProcessWindowStyle.Hidden;
		proc.RedirectStandardOutput = true;
		Process p = Process.Start(proc);
		string strOutput = p.StandardOutput.ReadToEnd(); 
		p.WaitForExit(); 
		UnityEngine.Debug.Log(strOutput);
	}
}
#endif

