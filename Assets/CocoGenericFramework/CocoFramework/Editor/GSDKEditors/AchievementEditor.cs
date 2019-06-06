using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class AchievementEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	GameDB gameDB = new GameDB();
	
	// Config Data Vars
	AchievementsConfigData achievementData = null;
	List<AchievementsConfigData> allAchievements = null;
	string [] achievementNames = null;
	int selectedAchievementIndex = -1;
	int newAchievementNameSufix = 0;
	
	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Achievement Editor")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(AchievementEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Achievement Editor";
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
			DrawItem();
		}
		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	#region Utility Methods
	void LoadData()
	{
		achievementData = null;
		selectedAchievementIndex = -1;
		
		gameDB.InitLocalDB();
		allAchievements = gameDB.LoadConfig<AchievementsConfigData>() as List<AchievementsConfigData>;
		InitIAPNames();
	}
	
	void InitIAPNames()
	{
		if (allAchievements == null)
			return;
		achievementNames = new string[allAchievements.Count];
		int s = 0;
		foreach(var p in allAchievements)
		{
			achievementNames[s] = p.id;
			++s;
		}
	}
	
	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in allAchievements)
		{
			if (d.id.Contains("New Achievement") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Achievement (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (allAchievements.Count > 0)
				gameDB.DeleteAllConfigs(allAchievements[0].GetTableName());
			foreach(var p in allAchievements)
			{
				gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			gameDB.IncrementLocalDBVersion();
			InitIAPNames();
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
			AchievementsConfigData newData = new AchievementsConfigData();
			newData.id ="New Achievement " + newAchievementNameSufix.ToString();
			++newAchievementNameSufix;
			allAchievements.Insert(0, newData);
			InitIAPNames();
			selectedAchievementIndex = 0;
			achievementData = newData;
			ShowNotification(new GUIContent("New Achievement added."));
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
		if (allAchievements == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (allAchievements.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("iAPs:", GUILayout.Width(100));
			int oldIntValue = selectedAchievementIndex;
			selectedAchievementIndex = EditorGUILayout.Popup(oldIntValue, achievementNames, GUILayout.Width(250));
			if (oldIntValue != selectedAchievementIndex)
			{
				achievementData = allAchievements[selectedAchievementIndex];
			}
			if (achievementData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Achievement!", "Are you sure you want to delete Achievement'" + achievementData.id + "'?", "Yes, Delete it.", "No!"))
					{
						gameDB.DeleteConfig(achievementData.GetTableName(), achievementData.id);
						allAchievements.Remove(achievementData);
						InitIAPNames();
						selectedAchievementIndex = -1;
						achievementData = null;
						ShowNotification(new GUIContent("Achievement deleted."));
						return false;
					}
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (achievementData != null);
	}
	#endregion
	
	#region Draw Item
	void DrawItem()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		string oldStringValue = string.Empty;
		int oldIntValue = 0;
		float oldFloatValue = 0;
		bool oldBoolValue = true;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("ID:", GUILayout.Width(100));
		oldStringValue = achievementData.id;
		achievementData.id = EditorGUILayout.TextField(achievementData.id, GUILayout.Width(150));
		if (achievementData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = achievementData.name;
		achievementData.name = EditorGUILayout.TextField(achievementData.name, GUILayout.Width(150));
		if (achievementData.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Achievement ID:", GUILayout.Width(100));
		oldStringValue = achievementData.achievementId;
		achievementData.achievementId = EditorGUILayout.TextField(achievementData.achievementId, GUILayout.Width(150));
		if (achievementData.achievementId != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Progress:", GUILayout.Width(100));
		oldIntValue = achievementData.progress;
		achievementData.progress = EditorGUILayout.IntField(achievementData.progress, GUILayout.Width(150));
		if (achievementData.progress != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorHelpers.DrawStringAsPopup("Store:", ref achievementData.store, EditorHelpers.storeNames, ref dirty);
		
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
