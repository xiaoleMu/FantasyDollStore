#if UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;
using PSDK.UnityEditor.XCodeEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;

public static class CopyAndRunPSDKXcodeScriptsHandler
{

	[PostProcessBuild(50001)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		string sourcePSDKXcodeScriptsHandlerFilePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets/TabTale/PSDK/services/PublishingSDKCore/Editor/Utils/psdk_xcode_scripts_handler.py");
		string destPSDKXcodeScriptsHandlerFilePath = Path.Combine(path, "psdk_xcode_scripts_handler.py");

		File.Copy(sourcePSDKXcodeScriptsHandlerFilePath, destPSDKXcodeScriptsHandlerFilePath, true);

		if (RunShellCommand ("python", AddQuotesIfRequired(destPSDKXcodeScriptsHandlerFilePath)) != 0) {
			throw new System.Exception ("Failed to run psdk_xcode_scripts_handler.py");
		}
	}

	public static string AddQuotesIfRequired(string path)
	{
	    return !string.IsNullOrEmpty(path) ? 
		path.Contains(" ") ? "\"" + path + "\"" : path
		: string.Empty;                                
	}
	
	public static int RunShellCommand(string command, string arguments)
	{
		// check if on mac
		//string file_basename = Path.GetFileName(file);
		ProcessStartInfo proc = new ProcessStartInfo();
		proc.FileName = command;
		//proc.WorkingDirectory = Path.GetDirectoryName(file);
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
		return p.ExitCode;
	}

}
#endif
