using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class RewardEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	// Config Data Vars
	RewardConfigData rewardData = null;
	List<RewardConfigData> allRewards = null;
	string [] rewardNames = null;
	int selectedRewardIndex = -1;
	int newRewardNameSufix = 0;
	
	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Reward Editor &#6")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(RewardEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Reward Editor";
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
		rewardData = null;
		selectedRewardIndex = -1;

		EditorHelpers.LoadItemConfigs();
		EditorHelpers.LoadCollectibleConfigs();
		allRewards = EditorHelpers.gameDB.LoadConfig<RewardConfigData>() as List<RewardConfigData>;
		InitDataConfigNames();
	}
	
	void InitDataConfigNames()
	{
		if (allRewards == null)
			return;
		rewardNames = new string[allRewards.Count];
		int s = 0;
		foreach(var p in allRewards)
		{
			rewardNames[s] = p.id;
			++s;
		}
	}

	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in allRewards)
		{
			if (d.id.Contains("New Reward") || d.id.Contains("Clone") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Reward (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (allRewards.Count > 0)
				EditorHelpers.gameDB.DeleteAllConfigs(allRewards[0].GetTableName());
			foreach(var p in allRewards)
			{
				EditorHelpers.gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion();
			InitDataConfigNames();
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
			RewardConfigData newData = new RewardConfigData();
			newData.id ="New Reward " + newRewardNameSufix.ToString();
			++newRewardNameSufix;
			allRewards.Insert(0, newData);
			InitDataConfigNames();
			selectedRewardIndex = 0;
			rewardData = newData;
			ShowNotification(new GUIContent("New Reward added."));
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
		if (allRewards == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (allRewards.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Rewards:", GUILayout.Width(100));
			int oldIntValue = selectedRewardIndex;
			selectedRewardIndex = EditorGUILayout.Popup(oldIntValue, rewardNames, GUILayout.Width(250));
			if (oldIntValue != selectedRewardIndex)
			{
				rewardData = allRewards[selectedRewardIndex];
			}
			if (rewardData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Reward!", "Are you sure you want to delete reward '" + rewardData.id + "'?", "Yes, Delete it.", "No!"))
					{
						EditorHelpers.gameDB.DeleteConfig(rewardData.GetTableName(), rewardData.id);
						allRewards.Remove(rewardData);
						InitDataConfigNames();
						selectedRewardIndex = -1;
						rewardData = null;
						ShowNotification(new GUIContent("Reward deleted."));
						return false;
					}
				}
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
				{
					RewardConfigData newData = rewardData.Clone() as RewardConfigData;
					newData.id = rewardData.id + "(Clone)";
					allRewards.Insert(0, newData);
					InitDataConfigNames();
					selectedRewardIndex = 0;
					rewardData = newData;
					ShowNotification(new GUIContent("Reward duplicated."));
					dirty = true;
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (rewardData != null);
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
		oldStringValue = rewardData.id;
		rewardData.id = EditorGUILayout.TextField(rewardData.id, GUILayout.Width(150));
		if (rewardData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Reward Type:", GUILayout.Width(100));
		RewardType oldRewardType = rewardData.rewardType;
		rewardData.rewardType = (RewardType)EditorGUILayout.EnumPopup(rewardData.rewardType, GUILayout.Width(150));
		if (rewardData.rewardType != oldRewardType)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		if (rewardData.rewardType != RewardType.MysteryBox)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Delay Type:", GUILayout.Width(100));
			DelayType oldDelayType = rewardData.delayType;
			rewardData.delayType = (DelayType)EditorGUILayout.EnumPopup(rewardData.delayType, GUILayout.Width(150));
			if (rewardData.delayType != oldDelayType)
				dirty = true;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();
			EditorHelpers.DrawListOfInts("Delays:", rewardData.delays, ref dirty);
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Social Factor:", GUILayout.Width(100));
		oldFloatValue = rewardData.socialFactor;
		rewardData.socialFactor = EditorGUILayout.FloatField(rewardData.socialFactor, GUILayout.Width(150));
		if (rewardData.socialFactor != oldFloatValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("RV Factor:", GUILayout.Width(100));
		oldFloatValue = rewardData.rvFactor;
		rewardData.rvFactor = EditorGUILayout.FloatField(rewardData.rvFactor, GUILayout.Width(150));
		if (rewardData.rvFactor != oldFloatValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Double Factor:", GUILayout.Width(100));
		oldBoolValue = rewardData.doubleFactor;
		rewardData.doubleFactor = EditorGUILayout.Toggle(rewardData.doubleFactor, GUILayout.Width(150));
		if (rewardData.doubleFactor != oldBoolValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Permanent Social Factor:", GUILayout.Width(100));
		oldBoolValue = rewardData.permanentSocialFactor;
		rewardData.permanentSocialFactor = EditorGUILayout.Toggle(rewardData.permanentSocialFactor, GUILayout.Width(150));
		if (rewardData.permanentSocialFactor != oldBoolValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Restrictions:", rewardData.restrictions, ref dirty);
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
