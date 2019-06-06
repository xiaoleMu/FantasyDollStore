using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;

namespace TabTale.Database
{
	public class DbUpdate
	{
		static Process process = null;

		[MenuItem("TabTale/GSDK/Update Database From Server")]
		static void UpdateDb () 
		{
			try
			{
				AssetDatabase.StartAssetEditing();
				process = new Process();
				process.EnableRaisingEvents = false;
				process.StartInfo.FileName = "python";
				process.StartInfo.Arguments = "Assets/TabTale/GameFramework/Build/pullDatabase.py";
				process.StartInfo.WorkingDirectory = Application.dataPath + "/../";
				process.OutputDataReceived += new DataReceivedEventHandler( OnDataReceived );
				process.ErrorDataReceived += new DataReceivedEventHandler( OnErrorReceived );
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardError = true;
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				
				UnityEngine.Debug.Log( "DbUpdate: Successfully launched app" );
			}
			catch( Exception e )
			{
				UnityEngine.Debug.LogError( "DbUpdate: Unable to launch app. " + e.Message );
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
				UnityEngine.Debug.Log ("DbUpdate: Done");
			}
			else
			{
				UnityEngine.Debug.Log ("DbUpdate: " + eventArgs.Data);
			}
		}
		
		
		static void OnErrorReceived( object sender, DataReceivedEventArgs eventArgs )
		{
			if(eventArgs.Data != null)
				UnityEngine.Debug.LogError( eventArgs.Data );
		}
	}
}