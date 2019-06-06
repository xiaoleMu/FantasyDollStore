#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using System.Text;
using System.IO;
using System.IO;
using System.Diagnostics;
using TabTale.Plugins.PSDK;
using PSDK.UnityEditor.XCodeEditor;


namespace Tabtale.PSDK.Editor {

	public static class PsdkiOSPostProcessUtils {


	static string filePath;
	static string projectRootPath;




	public static string GetLatestModifiedFileInDirectory(DirectoryInfo directoryInfo, string filesEndWith = "")
	{
		if (directoryInfo == null || !directoryInfo.Exists)
			return null;
			
			FileInfo[] files = directoryInfo.GetFiles();
			System.DateTime lastWrite = System.DateTime.MinValue;
			string latestFilePath = null;
			
			foreach (FileInfo file in files)
			{
				if (file.LastWriteTime > lastWrite && file.Name.EndsWith(filesEndWith))
				{
					lastWrite = file.LastWriteTime;
					latestFilePath = file.FullName;
				}
			}
			
			return latestFilePath;
		}


		public static string GetTeamIdFromMobileProvision() {
			bool svBundle = PsdkUtils.BundleIdentifier.StartsWith ("com.tabtaleint.");
			string baseDirectory = Path.Combine (Application.dataPath,"..");
			baseDirectory = Path.Combine (baseDirectory,"BuildResources");
			baseDirectory = Path.Combine (baseDirectory,"provisions");
			string directory = Path.Combine (baseDirectory,"regular");
			if (svBundle)
				directory = Path.Combine (baseDirectory,"sv");
			string latestFilePath = GetLatestModifiedFileInDirectory (new DirectoryInfo (directory),".mobileprovision");
			if (latestFilePath == null) {
				UnityEngine.Debug.LogWarning("no provision profile found at path:" + directory + " not setting iOS TeamId !");
				if (svBundle) // now trying the otherway around
					directory = Path.Combine (baseDirectory,"regular");
				else
					directory = Path.Combine (baseDirectory,"sv");
				latestFilePath = GetLatestModifiedFileInDirectory (new DirectoryInfo (directory),".mobileprovision");
			}
			if (latestFilePath == null) {
				UnityEngine.Debug.LogError("no provision profile found at path:" + directory + " not setting iOS TeamId !");
				return null;
			}
			UnityEngine.Debug.Log ("security cms -D -i " + latestFilePath + " | plutil -extract \"TeamIdentifier\" xml1   -o - - ");
			string plistSecurityOutput = RunShellCommand ("security","cms -D -i \"" + latestFilePath +"\"");
			IDictionary<string,object> secPlistDict = Plist.readPlistSource(plistSecurityOutput) as IDictionary<string, object>; 
			if (! secPlistDict.ContainsKey ("TeamIdentifier")) {
				UnityEngine.Debug.LogError("no team Identifier found at path:" + directory + " not setting iOS TeamId !");
				return null;
			}
			string teamId = (secPlistDict ["TeamIdentifier"] as IList<object>) [0].ToString();
			UnityEngine.Debug.Log ("+++" + teamId + "+++");
			return teamId;
		}

		public static string RunShellCommand(string command, string arguments) {
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
			return strOutput;
		}

		// security cms -D -i BuildResources/provisions/regular/Jumpy_Soccer__AdHoc.mobileprovision | plutil -extract "TeamIdentifier" xml1   -o - - 

}
}
#endif
