#if UNITY_IPHONE
using System.IO;
using UnityEngine;
using UnityEditor;
using PSDK.UnityEditor.XCodeEditor;
using UnityEditor.Callbacks;

public static class RateUsPostProcess
{
	// A helper class the current script's path
	private class RateUsPathHelper : EditorWindow { }

	[PostProcessBuild(40001)]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		// Create a new project object from build target
		XCProject project = new XCProject( path );

		/*
		RateUsPathHelper helper = new RateUsPathHelper();
		var ms = MonoScript.FromScriptableObject(helper);
		var currentScriptPath = AssetDatabase.GetAssetPath (ms);
		Debug.Log ("**** " + currentScriptPath);
		*/

		string currentScriptPath = TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath +  "/services/RateUs";

		string subfoldersUnder = System.IO.Path.Combine(Application.dataPath, currentScriptPath);
		var files = System.IO.Directory.GetFiles( subfoldersUnder , "*.projmods", SearchOption.AllDirectories );
		foreach( var file in files ) 
		{
			Debug.Log ("invoking " + file);
			project.ApplyMod( file );
		}
		
		// Finally save the xcode project
		project.Save();
	}
}
#endif
