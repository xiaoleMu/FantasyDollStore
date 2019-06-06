using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class RankEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	GameDB gameDB = new GameDB();
	
	// Config Data Vars
	RankConfigData rankData = null;
	List<RankConfigData> allRanks = null;
	string [] rankNames = null;
	int selectedRankIndex = -1;
	int newRankNameSufix = 0;
	
	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Rank Editor")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(RankEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Rank Editor";
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
		rankData = null;
		selectedRankIndex = -1;
		
		gameDB.InitLocalDB();
		allRanks = gameDB.LoadConfig<RankConfigData>() as List<RankConfigData>;
		InitIAPNames();
	}
	
	void InitIAPNames()
	{
		if (allRanks == null)
			return;
		rankNames = new string[allRanks.Count];
		int s = 0;
		foreach(var p in allRanks)
		{
			rankNames[s] = p.id;
			++s;
		}
	}
	
	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in allRanks)
		{
			if (d.id.Contains("New Rank") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Rank (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (allRanks.Count > 0)
				gameDB.DeleteAllConfigs(allRanks[0].GetTableName());
			foreach(var p in allRanks)
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
			RankConfigData newData = new RankConfigData();
			newData.id ="New Rank " + newRankNameSufix.ToString();
			++newRankNameSufix;
			allRanks.Insert(0, newData);
			InitIAPNames();
			selectedRankIndex = 0;
			rankData = newData;
			ShowNotification(new GUIContent("New Rank added."));
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
		if (allRanks == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (allRanks.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("iAPs:", GUILayout.Width(100));
			int oldIntValue = selectedRankIndex;
			selectedRankIndex = EditorGUILayout.Popup(oldIntValue, rankNames, GUILayout.Width(250));
			if (oldIntValue != selectedRankIndex)
			{
				rankData = allRanks[selectedRankIndex];
			}
			if (rankData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Rank!", "Are you sure you want to delete Rank'" + rankData.id + "'?", "Yes, Delete it.", "No!"))
					{
						gameDB.DeleteConfig(rankData.GetTableName(), rankData.id);
						allRanks.Remove(rankData);
						InitIAPNames();
						selectedRankIndex = -1;
						rankData = null;
						ShowNotification(new GUIContent("Rank deleted."));
						return false;
					}
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (rankData != null);
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
		oldStringValue = rankData.id;
		rankData.id = EditorGUILayout.TextField(rankData.id, GUILayout.Width(150));
		if (rankData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = rankData.name;
		rankData.name = EditorGUILayout.TextField(rankData.name, GUILayout.Width(150));
		if (rankData.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Rank Up Text ID:", GUILayout.Width(100));
		oldStringValue = rankData.rankUpTextId;
		rankData.rankUpTextId = EditorGUILayout.TextField(rankData.rankUpTextId, GUILayout.Width(150));
		if (rankData.rankUpTextId != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Index:", GUILayout.Width(100));
		oldIntValue = rankData.index;
		rankData.index = EditorGUILayout.IntField(rankData.index, GUILayout.Width(150));
		if (rankData.index != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("XP:", GUILayout.Width(100));
		oldIntValue = rankData.xp;
		rankData.xp = EditorGUILayout.IntField(rankData.xp, GUILayout.Width(150));
		if (rankData.xp != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Restrictions:", rankData.restrictions, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGenericPropertyData("Properties:", rankData.properties, ref dirty);

		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
