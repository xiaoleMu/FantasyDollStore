using UnityEngine;
using UnityEditor;
using System.IO;


public static class CocoCleanDatasMenu
{
	[MenuItem ("Coco Common/Misc Utility/Clean All", false, 140)]
	private static void CleanAll ()
	{
		Debug.LogWarning ("Clean All !");
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();

		CleanCache ();
		CleanDirectory (Application.persistentDataPath);
	}

	[MenuItem ("Coco Common/Misc Utility/Clean PlayerPrefs", false, 141)]
	private static void CleanPlayerPrefs ()
	{
		Debug.LogWarning ("Clean PlayerPrefs !");
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();
	}

	[MenuItem ("Coco Common/Misc Utility/Clean DB", false, 142)]
	private static void CleanDB ()
	{
		string pPath = Application.persistentDataPath + "/DB/game.db";

		if (!File.Exists (pPath))
			return;

		Debug.LogWarning ("game.db has been removed !");
		File.Delete (pPath);
	}

	[MenuItem ("Coco Common/Misc Utility/Clean Cache", false, 143)]
	private static void CleanCache ()
	{
		Debug.LogWarning ("Cache has been cleaned !");
#if UNITY_2017_1_OR_NEWER
		Caching.ClearCache ();
#else
		Caching.CleanCache ();
#endif
	}

	[MenuItem ("Coco Common/Misc Utility/Clean DIY Content", false, 144)]
	private static void CleanDIY ()
	{
		string pPath = Application.persistentDataPath + "/diy/";

		if (!Directory.Exists (pPath))
			return;

		Debug.LogWarning ("DIY content has been removed !");
		Directory.Delete (pPath, true);
	}

	private static void CleanDirectory (string path)
	{
		var files = Directory.GetFiles (path);
		foreach (var file in files) {
			File.Delete (file);
		}

		var directories = Directory.GetDirectories (path);
		foreach (var directory in directories) {
			Directory.Delete (directory, true);
		}
	}
}