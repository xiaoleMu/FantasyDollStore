using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class RewardItemEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	// Config Data Vars
	RewardItemConfigData rewardItemData = null;
	int selectedRewardItemIndex = -1;
	int newRewardItemNameSufix = 0;

	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Reward Item Editor &#5")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(RewardItemEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Reward Item Editor";
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
		rewardItemData = null;
		selectedRewardItemIndex = -1;
	
		EditorHelpers.LoadRewardItemConfigs();
		EditorHelpers.LoadRewardConfigs();
		EditorHelpers.LoadItemConfigs();
		EditorHelpers.LoadCollectibleConfigs();
	}
	
	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in EditorHelpers.allRewardItems)
		{
			if (d.id.Contains("New Reward Item") || d.id.Contains("Clone") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Reward Item (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (EditorHelpers.allRewardItems.Count > 0)
				EditorHelpers.gameDB.DeleteAllConfigs(EditorHelpers.allRewardItems[0].GetTableName());
			foreach(var p in EditorHelpers.allRewardItems)
			{
				EditorHelpers.gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion();
			EditorHelpers.InitRewardItemNames();
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
			RewardItemConfigData newData = new RewardItemConfigData();
			newData.id ="New Reward Item " + newRewardItemNameSufix.ToString();
			++newRewardItemNameSufix;
			EditorHelpers.allRewardItems.Insert(0, newData);
			EditorHelpers.InitRewardItemNames();
			selectedRewardItemIndex = 0;
			rewardItemData = newData;
			ShowNotification(new GUIContent("New Reward Item added."));
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
		if (EditorHelpers.allRewardItems == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (EditorHelpers.allRewardItems.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Rewards:", GUILayout.Width(100));
			int oldIntValue = selectedRewardItemIndex;
			selectedRewardItemIndex = EditorGUILayout.Popup(oldIntValue, EditorHelpers.rewardItemNames, GUILayout.Width(250));
			if (oldIntValue != selectedRewardItemIndex)
			{
				rewardItemData = EditorHelpers.allRewardItems[selectedRewardItemIndex];
			}
			if (rewardItemData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Reward Item!", "Are you sure you want to delete reward item'" + rewardItemData.id + "'?", "Yes, Delete it.", "No!"))
					{
						EditorHelpers.gameDB.DeleteConfig(rewardItemData.GetTableName(), rewardItemData.id);
						EditorHelpers.allRewardItems.Remove(rewardItemData);
						EditorHelpers.InitRewardItemNames();
						selectedRewardItemIndex = -1;
						rewardItemData = null;
						ShowNotification(new GUIContent("Reward Item deleted."));
						return false;
					}
				}
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
				{
					RewardItemConfigData newData = rewardItemData.Clone() as RewardItemConfigData;
					newData.id = rewardItemData.id + "(Clone)";
					EditorHelpers.allRewardItems.Insert(0, newData);
					EditorHelpers.InitRewardItemNames();
					selectedRewardItemIndex = 0;
					rewardItemData = newData;
					ShowNotification(new GUIContent("Reward Item duplicated."));
					dirty = true;
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (rewardItemData != null);
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
		oldStringValue = rewardItemData.id;
		rewardItemData.id = EditorGUILayout.TextField(rewardItemData.id, GUILayout.Width(150));
		if (rewardItemData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Weight:", GUILayout.Width(100));
		oldFloatValue = rewardItemData.weight;
		rewardItemData.weight = EditorGUILayout.FloatField(rewardItemData.weight, GUILayout.Width(150));
		if (rewardItemData.weight != oldFloatValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Allow Social Factor:", GUILayout.Width(100));
		oldBoolValue = rewardItemData.allowSocialFactor;
		rewardItemData.allowSocialFactor = EditorGUILayout.Toggle(rewardItemData.allowSocialFactor, GUILayout.Width(150));
		if (rewardItemData.allowSocialFactor != oldBoolValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Allow RV Factor:", GUILayout.Width(100));
		oldBoolValue = rewardItemData.allowRVFactor;
		rewardItemData.allowRVFactor = EditorGUILayout.Toggle(rewardItemData.allowRVFactor, GUILayout.Width(150));
		if (rewardItemData.allowRVFactor != oldBoolValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Reward:", GUILayout.Width(100));
		oldIntValue = EditorHelpers.IndexFromName(rewardItemData.rewardConfigId, EditorHelpers.rewardNames);
		int newIndex = EditorGUILayout.Popup(oldIntValue, EditorHelpers.rewardNames, GUILayout.Width(150));
		if (newIndex != oldIntValue)
		{
			rewardItemData.rewardConfigId = EditorHelpers.rewardNames[newIndex];
			dirty = true;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Index:", GUILayout.Width(100));
		oldIntValue = rewardItemData.index;
		rewardItemData.index = EditorGUILayout.IntField(rewardItemData.index, GUILayout.Width(150));
		if (rewardItemData.index != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Rewards:", rewardItemData.rewards, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Restrictions:", rewardItemData.restrictions, ref dirty);
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
