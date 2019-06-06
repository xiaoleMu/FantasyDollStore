using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class LeaderboardEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	GameDB gameDB = new GameDB();
	
	// Config Data Vars
	LeaderboardConfigData leaderboardData = null;
	List<LeaderboardConfigData> allLeaderboards = null;
	string [] leaderboardNames = null;
	int selectedLeaderboardIndex = -1;
	int newLeaderboardNameSufix = 0;
	
	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Leaderboard Editor")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(LeaderboardEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Leaderboard Editor";
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
		leaderboardData = null;
		selectedLeaderboardIndex = -1;
		
		gameDB.InitLocalDB();
		allLeaderboards = gameDB.LoadConfig<LeaderboardConfigData>() as List<LeaderboardConfigData>;
		InitLeaderboardNames();
	}
	
	void InitLeaderboardNames()
	{
		if (allLeaderboards == null)
			return;
		leaderboardNames = new string[allLeaderboards.Count];
		int s = 0;
		foreach(var p in allLeaderboards)
		{
			leaderboardNames[s] = p.id;
			++s;
		}
	}
	
	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in allLeaderboards)
		{
			if (d.id.Contains("New ID") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Leaderboard (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (allLeaderboards.Count > 0)
				gameDB.DeleteAllConfigs(allLeaderboards[0].GetTableName());
			foreach(var p in allLeaderboards)
			{
				gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			gameDB.IncrementLocalDBVersion();
			InitLeaderboardNames();
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
			LeaderboardConfigData newData = new LeaderboardConfigData();
			newData.id ="New ID " + newLeaderboardNameSufix.ToString();
			++newLeaderboardNameSufix;
			allLeaderboards.Insert(0, newData);
			InitLeaderboardNames();
			selectedLeaderboardIndex = 0;
			leaderboardData = newData;
			ShowNotification(new GUIContent("New Leaderboard added."));
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
		if (allLeaderboards == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (allLeaderboards.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("iAPs:", GUILayout.Width(100));
			int oldIntValue = selectedLeaderboardIndex;
			selectedLeaderboardIndex = EditorGUILayout.Popup(oldIntValue, leaderboardNames, GUILayout.Width(250));
			if (oldIntValue != selectedLeaderboardIndex)
			{
				leaderboardData = allLeaderboards[selectedLeaderboardIndex];
			}
			if (leaderboardData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Leaderboard!", "Are you sure you want to delete Leaderboard'" + leaderboardData.id + "'?", "Yes, Delete it.", "No!"))
					{
						gameDB.DeleteConfig(leaderboardData.GetTableName(), leaderboardData.id);
						allLeaderboards.Remove(leaderboardData);
						InitLeaderboardNames();
						selectedLeaderboardIndex = -1;
						leaderboardData = null;
						ShowNotification(new GUIContent("Leaderboard deleted."));
						return false;
					}
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (leaderboardData != null);
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
		oldStringValue = leaderboardData.id;
		leaderboardData.id = EditorGUILayout.TextField(leaderboardData.id, GUILayout.Width(150));
		if (leaderboardData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = leaderboardData.name;
		leaderboardData.name = EditorGUILayout.TextField(leaderboardData.name, GUILayout.Width(150));
		if (leaderboardData.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Leaderboard ID:", GUILayout.Width(100));
		oldStringValue = leaderboardData.leaderboardId;
		leaderboardData.leaderboardId = EditorGUILayout.TextField(leaderboardData.leaderboardId, GUILayout.Width(150));
		if (leaderboardData.leaderboardId != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorHelpers.DrawStringAsPopup("Store:", ref leaderboardData.store, EditorHelpers.storeNames, ref dirty);
		
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
