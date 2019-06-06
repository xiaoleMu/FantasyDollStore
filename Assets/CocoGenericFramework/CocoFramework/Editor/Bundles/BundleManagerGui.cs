using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TabTale.AssetManagement {
#if SIMULATION
	public class BundleManagerGui : EditorWindow
	{

		[MenuItem("TabTale/Framework/Bundles/Build Asset Bundles")]
		static void BuildAssetBundles () 
		{
		}

		[MenuItem("TabTale/Framework/Bundles/Build Bundle From Selected")]
		static void OnButton()
		{
			// Get existing open window or if none, make a new one:
			//BundleManagerGui window = EditorWindow.GetWindow<BundleManagerGui>() as BundleManagerGui;
		}

		[MenuItem("TabTale/Framework/Bundles/Test Load Catalog")]
		static void OnTestLoadCatalog()
		{
			//GameApplication.Instance.AssetManager.LoadValue("http://localhost:8000/api/assetcatalogs", (data) => Debug.Log(string.Format("received data!")));
		}

		/*static void BuildAssetBundlesFromSelected()
		{
			// Bring up save panel
			string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
			if (path.Length != 0) 
			{
				// Build the resource file from the active selection.
				Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
				BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, 
				                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
				Selection.objects = selection;
			}
		}*/

		string myString = "Hello World";
		bool groupEnabled;
		bool myBool = true;
		float myFloat = 1.23f;

		void OnEnable()
		{
			this.title = "Bundle Creator";
		}

		void OnGUI () 
		{
			GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
			myString = EditorGUILayout.TextField ("Text Field", myString);
			
			groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
				myBool = EditorGUILayout.Toggle ("Toggle", myBool);
				myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
			EditorGUILayout.EndToggleGroup ();

			//EditorGUILayout.
		}

		[MenuItem("TabTale/Framework/Bundles/Build AssetBundle From Selection - Track dependencies")]
		static void BuildBundle () 
		{
			// Bring up save panel
			string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
			if (path.Length != 0) 
			{
				// Build the resource file from the active selection.
				Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
				BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, 
				                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.iPhone);
				Selection.objects = selection;
			}
		}
	}
#endif
}
