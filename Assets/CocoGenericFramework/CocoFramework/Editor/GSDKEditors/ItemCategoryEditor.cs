using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class ItemCategoryEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	// CategoryConfigData
	CategoryConfigData category = null;
	int selectedCategoryIndex = -1;
	int newCategoryNameSufix = 0;

	List<ItemConfigData> allItems = null;
	string [] itemConfigNames = null;
	List<CollectibleConfigData> allCollectibles = null;
	string [] collectibleConfigNames = null;

	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Category Editor &#3")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(ItemCategoryEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Category Editor";
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
		EditorHelpers.ClearData();
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
			DrawCategory();
		}
		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	#region Utility Methods
	void LoadData()
	{
		category = null;
		selectedCategoryIndex = -1;

		EditorHelpers.LoadCategoryConfigs();
		EditorHelpers.LoadItemConfigs();
		EditorHelpers.LoadCollectibleConfigs();
	}
	
	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in EditorHelpers.allCategories)
		{
			if (d.id.Contains("New Category") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Category (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (EditorHelpers.allCategories.Count > 0)
				EditorHelpers.gameDB.DeleteAllConfigs(EditorHelpers.allCategories[0].GetTableName());
			foreach(var p in EditorHelpers.allCategories)
			{
				EditorHelpers.gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion();
			EditorHelpers.InitCategoryNames();
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
			CategoryConfigData newData = new CategoryConfigData();
			newData.id = "New Category " + newCategoryNameSufix.ToString();
			++newCategoryNameSufix;
			EditorHelpers.allCategories.Insert(0, newData);
			EditorHelpers.InitCategoryNames();
			selectedCategoryIndex = 0;
			category = newData;
			ShowNotification(new GUIContent("New Category added."));
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
		if (EditorHelpers.allCategories == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (EditorHelpers.allCategories.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Categories:", GUILayout.Width(100));
			int oldIntValue = selectedCategoryIndex;
			selectedCategoryIndex = EditorGUILayout.Popup(oldIntValue, EditorHelpers.categoryNames, GUILayout.Width(250));
			if (oldIntValue != selectedCategoryIndex)
			{
				category = EditorHelpers.allCategories[selectedCategoryIndex];
			}
			if (category != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Category!", "Are you sure you want to delete parameter '" + category.id + "'?", "Yes, Delete it.", "No!"))
					{
						EditorHelpers.gameDB.DeleteConfig(category.GetTableName(), category.id);
						EditorHelpers.allCategories.Remove(category);
						EditorHelpers.InitCategoryNames();
						selectedCategoryIndex = -1;
						category = null;
						ShowNotification(new GUIContent("Category deleted."));
						return false;
					}
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (category != null);
	}
	#endregion

	#region Draw Category
	void DrawCategory()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		string oldStringValue = string.Empty;
		int oldIntValue = 0;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID:", GUILayout.Width(100));
		oldStringValue = category.id;
		category.id = EditorGUILayout.TextField(category.id, GUILayout.Width(150));
		if (category.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = category.name;
		category.name = EditorGUILayout.TextField(category.name, GUILayout.Width(150));
		if (category.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Index:", GUILayout.Width(100));
		oldIntValue = category.index;
		category.index = EditorGUILayout.IntField(category.index, GUILayout.Width(150));
		if (category.index != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Restrictions:", category.restrictions, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfAssetData("Assets:", category.assets, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGenericPropertyData("Properties:", category.properties, ref dirty);
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
