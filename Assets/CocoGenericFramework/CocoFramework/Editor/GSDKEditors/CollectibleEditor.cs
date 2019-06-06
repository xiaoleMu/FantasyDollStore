using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class CollectibleEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;

	GameDB gameDB = new GameDB();

	// Config Data Vars
	CollectibleConfigData collectibleData = null;
	List<CollectibleConfigData> allCollectibles = null;
	string [] collectibleConfigNames = null;
	int selectedCollectibleIndex = -1;
	int newCollectibleNameSufix = 0;

	List<ItemGroupConfigData> allItemGroups = null;
	string [] itemGroupNames = null;

	// CategoryConfigData
	//	List<CategoryConfigData> allCategories = null;
	//	string [] categoryNames = null;

	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Collectible Editor &#4")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(CollectibleEditor));
	}

	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Collectible Editor";
			inited = true;
			LoadData();
		}
	}

	void Update()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode)
		{
			if (dirty)
			{
				EditorApplication.isPlaying = false;
				Focus();
				EditorWindow.focusedWindow.ShowNotification(new GUIContent("Game stopped because there is an open editor (" + titleContent.text + ")" + "with unsaved changes!"));
			}
			else
				Close();
		}
	}

	void OnDestroy()
	{
		inited = false;
		if (dirty && EditorUtility.DisplayDialog("There are unsaved changes that will be lost!", "Are you sure you want to close the window and lose usaved changes?", "Save it!", "Forget it..."))
		{
			Save();
		}
		EditorInstances.liveInstance = null;
	}

	Vector2 scrollPos;
	public void OnGUI()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		Init();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		if (DrawLoadService())
		{
			GUILayout.Space(30);
			DrawCollectible();
		}
		EditorGUILayout.EndScrollView();
	}
	#endregion

	#region Utility Methods
	void LoadData()
	{
		collectibleData = null;
		selectedCollectibleIndex = -1;

		gameDB.InitLocalDB();
		allCollectibles = gameDB.LoadConfig<CollectibleConfigData>() as List<CollectibleConfigData>;
		//		allCategories = gameDB.LoadConfig<CategoryConfigData>() as List<CategoryConfigData>;
		InitCollectibleConfigNames();
		//		InitCategoryNames();
		allItemGroups = gameDB.LoadConfig<ItemGroupConfigData>() as List<ItemGroupConfigData>;
		InitItemGroupConfigNames();
	}

	void InitCollectibleConfigNames()
	{
		if (allCollectibles == null)
			return;
		collectibleConfigNames = new string[allCollectibles.Count];
		int s = 0;
		foreach(var p in allCollectibles)
		{
			collectibleConfigNames[s] = p.id;
			++s;
		}
	}

	void InitItemGroupConfigNames()
	{
		if (allItemGroups == null)
			return;
		itemGroupNames = new string[allItemGroups.Count];
		int s = 0;
		foreach(var p in allItemGroups)
		{
			itemGroupNames[s] = p.id;
			++s;
		}
	}

	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;

		// check if names are valid
		foreach(var d in allCollectibles)
		{
			if (d.id.Contains("New Collectible") || d.id.Contains("Clone") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Collectible (" + d.id + ") has invalid name!\n";
				break;
			}
		}

		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (allCollectibles.Count > 0)
				gameDB.DeleteAllConfigs(allCollectibles[0].GetTableName());
			foreach(var p in allCollectibles)
			{
				gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			gameDB.IncrementLocalDBVersion();
			InitCollectibleConfigNames();
			dirty = false;
		}
	}
	#endregion

	#region Services
	bool DrawLoadService()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		EditorGUILayout.BeginHorizontal();
		GUI.backgroundColor = EditorHelpers.orangeColor;
		if (GUILayout.Button("Load", GUILayout.Width(60)))
		{
			LoadData();
		}

		GUI.backgroundColor = EditorHelpers.yellowColor;
		if (GUILayout.Button("New", GUILayout.Width(60)))
		{
			CollectibleConfigData newData = new CollectibleConfigData();
			newData.id ="New Collectible " + newCollectibleNameSufix.ToString();
			newData.name = newData.id;
			++newCollectibleNameSufix;
			allCollectibles.Insert(0, newData);
			InitCollectibleConfigNames();
			selectedCollectibleIndex = 0;
			collectibleData = newData;
			ShowNotification(new GUIContent("New Collectible added."));
			dirty = true;
		}
		GUI.backgroundColor = EditorHelpers.greenColor;
		if (GUILayout.Button("Save", GUILayout.Width(60)))
		{
			Save();
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		if (allCollectibles == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (allCollectibles.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Collectibles:", GUILayout.Width(100));
			int oldIntValue = selectedCollectibleIndex;
			selectedCollectibleIndex = EditorGUILayout.Popup(oldIntValue, collectibleConfigNames, GUILayout.Width(250));
			if (oldIntValue != selectedCollectibleIndex)
			{
				collectibleData = allCollectibles[selectedCollectibleIndex];
			}
			if (collectibleData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Collectible!", "Are you sure you want to delete collectible '" + collectibleData.id + "'?", "Yes, Delete it.", "No!"))
					{
						gameDB.DeleteConfig(collectibleData.GetTableName(), collectibleData.id);
						allCollectibles.Remove(collectibleData);
						InitCollectibleConfigNames();
						selectedCollectibleIndex = -1;
						collectibleData = null;
						ShowNotification(new GUIContent("Collectible deleted."));
						return false;
					}
				}
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
				{
					CollectibleConfigData newData = collectibleData.Clone() as CollectibleConfigData;
					newData.id = collectibleData.id + "(Clone)";
					++newCollectibleNameSufix;
					allCollectibles.Insert(0, newData);
					InitCollectibleConfigNames();
					selectedCollectibleIndex = 0;
					collectibleData = newData;
					ShowNotification(new GUIContent("Collectible duplicated."));
					dirty = true;
				}				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}

		return (collectibleData != null);
	}
	#endregion

	#region Draw Item
	void DrawCollectible()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		string oldStringValue = string.Empty;
		int oldIntValue = 0;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID:", GUILayout.Width(100));
		oldStringValue = collectibleData.id;
		collectibleData.id = EditorGUILayout.TextField(collectibleData.id, GUILayout.Width(150));
		if (collectibleData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = collectibleData.name;
		collectibleData.name = EditorGUILayout.TextField(collectibleData.name, GUILayout.Width(150));
		if (collectibleData.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfAssetData("Assets:", collectibleData.assets, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGenericPropertyData("Properties:", collectibleData.properties, ref dirty);
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
