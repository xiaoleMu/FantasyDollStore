using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;

namespace TabTale.ProjectUpdater
{
	public static class ProjectUpdater
	{
		static Process process = null;
		static StreamWriter messageStream;

		private const string c_logColor = "#0092ff";
		private const string c_errorColor = "#cc0000";

		public static void StartUpdateProcess()
		{
			try
			{
				AssetDatabase.StartAssetEditing();
				process = new Process();
				process.EnableRaisingEvents = false;
				process.StartInfo.FileName = "python";
				process.StartInfo.Arguments = "updateProject.py";
				process.StartInfo.WorkingDirectory = Application.dataPath + "/../";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardError = true;
				process.OutputDataReceived += new DataReceivedEventHandler( OnDataReceived );
				process.ErrorDataReceived += new DataReceivedEventHandler( OnErrorReceived );
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				messageStream = process.StandardInput;

				Log( "ProjectUpdater: Successfully launched app" );
			}
			catch( Exception e )
			{
				LogError( "ProjectUpdater: Unable to launch app. " + e.Message );
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
			}
		}

		static void OnDataReceived( object sender, DataReceivedEventArgs eventArgs )
		{
			if(eventArgs.Data == null)
			{
				ProjectUpdaterDependenciesGUI.isDone = true;
				Log ("ProjectUpdater: Done");
            }
			else
			{
				Log ("ProjectUpdater: " + eventArgs.Data);
			}
        }
		
		
		static void OnErrorReceived( object sender, DataReceivedEventArgs eventArgs )
		{
			LogError( eventArgs.Data );
		}
		
		
		static void OnApplicationQuit()
		{
			if( process != null && ! process.HasExited )
			{
				process.Kill();
			}
		}

		static void Log(string msg, bool isError = false)
		{
			ProjectUpdaterDependenciesGUI.PrintLog(msg);
		}

		static void LogError(string msg)
		{
			Log (msg, true);
		}
	}

}

