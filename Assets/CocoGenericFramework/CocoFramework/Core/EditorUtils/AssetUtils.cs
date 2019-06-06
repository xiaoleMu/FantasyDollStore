

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;



namespace TabTale
{	
	public static class AssetUtils
	{

		#if UNITY_EDITOR

		public static void CreateAsset<T>() where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path == "")
			{
				path = "Assets";
			}
			else if (File.Exists(path)) // modified this line, folders can have extensions.
			{
				path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}
			
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).Name + ".asset");
			
			AssetDatabase.CreateAsset(asset, assetPathAndName);
			
			AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

		#endif

		public static void UnzipAndroidStreamingAsset()
		{
		}
		public static bool CopyStreamingAssetsFile(string streamingAssetsfileRelativePath, string destinationFilePath) {
			
			string assetsFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, streamingAssetsfileRelativePath);
			
			
			try {
				if (assetsFilePath.Contains("jar:file") || assetsFilePath.Contains("://")) 
				{
					// Android
					int timeout = 7; // seconds
					System.DateTime startTime = System.DateTime.Now;
					
					WWW www = new WWW(assetsFilePath);
					Debug.Log ("trying to read file: " + assetsFilePath);
					while (!www.isDone) {
						TimeSpan interval = DateTime.Now - startTime;
						if (interval.Seconds > timeout) 
							return false;
					}
					if (! String.IsNullOrEmpty(www.error)) {
						return false;
					}
					
					System.IO.File.WriteAllBytes(destinationFilePath, www.bytes);
					return true;
				}
				else {
					if (! System.IO.File.Exists(assetsFilePath))
						return false;
					
					System.IO.File.WriteAllBytes(destinationFilePath, System.IO.File.ReadAllBytes(assetsFilePath));
					return true;
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
			
			return false;
		}
	};
}

