using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class EventEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;

	// Config Data Vars
	EventConfigData itemData = null;
	int selectedItemIndex = -1;
	int newItemNameSufix = 0;

	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Event Editor")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(EventEditor));
	}

	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Event Editor";
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
			DrawItem();
		}
		EditorGUILayout.EndScrollView();
	}
	#endregion

	#region Utility Methods
	void LoadData()
	{
		itemData = null;
		selectedItemIndex = -1;

		EditorHelpers.LoadItemConfigs();
		EditorHelpers.LoadEventConfigs();
		EditorHelpers.LoadCollectibleConfigs();
		EditorHelpers.LoadItemGroupConfigs();
		EditorHelpers.LoadIAPConfigs(true);
	}

	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;

		// check if names are valid
		foreach(var d in EditorHelpers.allEvents)
		{
			if (d.id.Contains("New Event") || d.id.Contains("Clone") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Event (" + d.id + ") has invalid name!\n";
				break;
			}
			else if (d.startTime.Year < 2016 || d.endTime.Year < 2016)
			{
				canSave = false;
				notificationMsg = "Event (" + d.id + ") has invalid start/end times!\n";
				break;
			}
		}

		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (EditorHelpers.allEvents.Count > 0)
				EditorHelpers.gameDB.DeleteAllConfigs(EditorHelpers.allEvents[0].GetTableName());
			foreach(var e in EditorHelpers.allEvents)
			{
				EditorHelpers.gameDB.SaveConfig(e.GetTableName(), e.id, LitJson.JsonMapper.ToJson(e));
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion();
			EditorHelpers.InitEventNames();
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
			EventConfigData newData = new EventConfigData();
			newData.id ="New Event " + newItemNameSufix.ToString();
			++newItemNameSufix;
			EditorHelpers.allEvents.Insert(0, newData);
			EditorHelpers.InitEventNames();
			selectedItemIndex = 0;
			itemData = newData;
			ShowNotification(new GUIContent("New Event added."));
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
		if (EditorHelpers.allEvents == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (EditorHelpers.allEvents.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Events:", GUILayout.Width(100));
			int oldIntValue = selectedItemIndex;
			selectedItemIndex = EditorGUILayout.Popup(oldIntValue, EditorHelpers.eventNames, GUILayout.Width(250));
			if (oldIntValue != selectedItemIndex)
			{
				itemData = EditorHelpers.allEvents[selectedItemIndex];
			}
			if (itemData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Event!", "Are you sure you want to delete event '" + itemData.id + "'?", "Yes, Delete it.", "No!"))
					{
						EditorHelpers.gameDB.DeleteConfig(itemData.GetTableName(), itemData.id);
						EditorHelpers.allEvents.Remove(itemData);
						EditorHelpers.InitEventNames();
						selectedItemIndex = -1;
						itemData = null;
						ShowNotification(new GUIContent("Event deleted."));
						return false;
					}
				}
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
				{
					EventConfigData newData = itemData.Clone() as EventConfigData;
					newData.id = itemData.id + "(Clone)";
					EditorHelpers.allEvents.Insert(0, newData);
					EditorHelpers.InitEventNames();
					selectedItemIndex = 0;
					itemData = newData;
					ShowNotification(new GUIContent("Event duplicated."));
					dirty = true;
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}

		return (itemData != null);
	}
	#endregion

	#region Draw Item
	void DrawItem()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		string oldStringValue = string.Empty;
		int oldIntValue = 0;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID:", GUILayout.Width(100));
		oldStringValue = itemData.id;
		itemData.id = EditorGUILayout.TextField(itemData.id, GUILayout.Width(150));
		if (itemData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = itemData.name;
		itemData.name = EditorGUILayout.TextField(itemData.name, GUILayout.Width(150));
		if (itemData.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Description:", GUILayout.Width(100));
		oldStringValue = itemData.description;
		itemData.description = EditorGUILayout.TextField(itemData.description, GUILayout.Width(150));
		if (itemData.description != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Objective ID:", GUILayout.Width(100));
		oldStringValue = itemData.objectiveId;
		itemData.objectiveId = EditorGUILayout.TextField(itemData.objectiveId, GUILayout.Width(150));
		if (itemData.objectiveId != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorHelpers.DrawDateTime("Start Time:", ref itemData.startTime, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawDateTime("End Time:", ref itemData.endTime, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfStrings("Levels:", itemData.levels, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfStrings("Afinity Items:", itemData.affinityItems, ref dirty, 100f, 200f);

		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfEventPrizeData("Rank Prizes:", itemData.rankPrizes, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfEventPrizeData("Progression Prizes:", itemData.progressionPrizes, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfAssetData("Assets:", itemData.assets, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGenericPropertyData("Properties:", itemData.properties, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Restrictions:", itemData.restrictions, ref dirty);

		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}