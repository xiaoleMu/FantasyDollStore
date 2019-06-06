using UnityEngine;
using UnityEditor;
using System.Collections;

public class PsdkRenameFailedImportedFiles :  AssetPostprocessor  
{
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
	{
		bool changed = false;
		string appDataPathUp = System.IO.Path.Combine (Application.dataPath, "..");
		foreach (string str in importedAssets)
		{
			if (! str.ToUpper().Contains("PSDK")) continue;
			if (! str.Contains(" 1.")) continue;
			string old_file = str.Replace(" 1","");
			Debug.LogWarning("PsdkRenameFailedImportedFiles: moving " + str + " to " + old_file);
			changed = true;
			if (! AssetDatabase.DeleteAsset(old_file)) {
				Debug.LogError("PsdkRenameFailedImportedFiles: Failed to delete asset " + old_file);
				string file_to_delete = System.IO.Path.Combine(appDataPathUp,old_file);
				if (System.IO.File.Exists(file_to_delete)) {
					System.IO.File.Delete(file_to_delete);
					System.IO.File.Delete(file_to_delete + ".meta");
					AssetDatabase.Refresh();
				}
			}
			string moveerr = AssetDatabase.MoveAsset(str,old_file);
			if (! string.IsNullOrEmpty(moveerr)) {
				Debug.LogError("PsdkRenameFailedImportedFiles: Failed to move asset " + moveerr);
			}
		}
		if (changed) {
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}