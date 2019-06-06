using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class ItemGroupEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	

	// Config Data Vars
	ItemGroupConfigData itemGroupData = null;
	int selectedItemGroupIndex = -1;
	int newItemGroupNameSufix = 0;

	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Item Group Editor &#2")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(ItemGroupEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Item Group Editor";
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
			DrawItemGroup();
		}
		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	#region Utility Methods
	void LoadData()
	{
		itemGroupData = null;
		selectedItemGroupIndex = -1;

		EditorHelpers.LoadItemGroupConfigs();
		EditorHelpers.LoadItemConfigs();
	}

	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in EditorHelpers.allItemGroups)
		{
			if (d.id.Contains("New Item") || d.id.Contains("Clone") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Item (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (EditorHelpers.allItemGroups.Count > 0)
				EditorHelpers.gameDB.DeleteAllConfigs(EditorHelpers.allItemGroups[0].GetTableName());
			foreach(var p in EditorHelpers.allItemGroups)
			{
				EditorHelpers.gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion();
			EditorHelpers.InitItemGroupNames();
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
			ItemGroupConfigData newData = new ItemGroupConfigData();
			newData.id ="New Item Group " + newItemGroupNameSufix.ToString();
			++newItemGroupNameSufix;
			EditorHelpers.allItemGroups.Insert(0, newData);
			EditorHelpers.InitItemGroupNames();
			selectedItemGroupIndex = 0;
			itemGroupData = newData;
			ShowNotification(new GUIContent("New Item Group added."));
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
		if (EditorHelpers.allItemGroups == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (EditorHelpers.allItemGroups.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Items Groups:", GUILayout.Width(100));
			int oldIntValue = selectedItemGroupIndex;
			selectedItemGroupIndex = EditorGUILayout.Popup(oldIntValue, EditorHelpers.itemGroupNames, GUILayout.Width(250));
			if (oldIntValue != selectedItemGroupIndex)
			{
				itemGroupData = EditorHelpers.allItemGroups[selectedItemGroupIndex];
			}
			if (itemGroupData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Item Group!", "Are you sure you want to delete parameter '" + itemGroupData.id + "'?", "Yes, Delete it.", "No!"))
					{
						EditorHelpers.gameDB.DeleteConfig(itemGroupData.GetTableName(), itemGroupData.id);
						EditorHelpers.allItemGroups.Remove(itemGroupData);
						EditorHelpers.InitItemGroupNames();
						selectedItemGroupIndex = -1;
						itemGroupData = null;
						ShowNotification(new GUIContent("Item deleted."));
						return false;
					}
				}
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
				{
					ItemGroupConfigData newData = itemGroupData.Clone() as ItemGroupConfigData;
					newData.id = itemGroupData.id + "(Clone)";
					EditorHelpers.allItemGroups.Insert(0, newData);
					EditorHelpers.InitItemGroupNames();
					selectedItemGroupIndex = 0;
					itemGroupData = newData;
					ShowNotification(new GUIContent("Item Group duplicated."));
					dirty = true;
				}				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (itemGroupData != null);
	}
	#endregion

	#region Draw Item
	void DrawItemGroup()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		string oldStringValue = string.Empty;
		int oldIntValue = 0;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID:", GUILayout.Width(100));
		oldStringValue = itemGroupData.id;
		itemGroupData.id = EditorGUILayout.TextField(itemGroupData.id, GUILayout.Width(150));
		if (itemGroupData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = itemGroupData.name;
		itemGroupData.name = EditorGUILayout.TextField(itemGroupData.name, GUILayout.Width(150));
		if (itemGroupData.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Type:", GUILayout.Width(100));
		ItemGroupConfigType oldType = itemGroupData.type;
		itemGroupData.type = (ItemGroupConfigType)EditorGUILayout.EnumPopup(itemGroupData.type, GUILayout.Width(150));
		if (itemGroupData.type != oldType)
			dirty = true;
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Category:", GUILayout.Width(100));
		oldStringValue = itemGroupData.category;
		itemGroupData.category = EditorGUILayout.TextField(itemGroupData.category, GUILayout.Width(150));
		if (itemGroupData.category != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Index:", GUILayout.Width(100));
		oldIntValue = itemGroupData.index;
		itemGroupData.index = EditorGUILayout.IntField(itemGroupData.index, GUILayout.Width(150));
		if (itemGroupData.index != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfNamesAsPopups("Items:", itemGroupData.items, EditorHelpers.itemConfigNames, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Restrictions:", itemGroupData.restrictions, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfAssetData("Assets:", itemGroupData.assets, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGenericPropertyData("Properties:", itemGroupData.properties, ref dirty);
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
