using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class iAPEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	// Config Data Vars
	IAPConfigData iAPData = null;
	int selectedIAPIndex = -1;
	int newIAPNameSufix = 0;
	
	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/iAP Editor &#q")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(iAPEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "iAP Editor";
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
		iAPData = null;
		selectedIAPIndex = -1;

		EditorHelpers.LoadIAPConfigs();
	}
	
	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in EditorHelpers.allIAPs)
		{
			if (d.id.Contains("New iAP") || d.id.Contains("Clone") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "iAP (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (EditorHelpers.allIAPs.Count > 0)
				EditorHelpers.gameDB.DeleteAllConfigs(EditorHelpers.allIAPs[0].GetTableName());
			foreach(var p in EditorHelpers.allIAPs)
			{
				EditorHelpers.gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion();
			EditorHelpers.InitIAPNames();
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
			IAPConfigData newData = new IAPConfigData();
			newData.id ="New iAP " + newIAPNameSufix.ToString();
			++newIAPNameSufix;
			EditorHelpers.allIAPs.Insert(0, newData);
			EditorHelpers.InitIAPNames();
			selectedIAPIndex = 0;
			iAPData = newData;
			ShowNotification(new GUIContent("New iAP added."));
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
		if (EditorHelpers.allIAPs == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (EditorHelpers.allIAPs.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("iAPs:", GUILayout.Width(100));
			int oldIntValue = selectedIAPIndex;
			selectedIAPIndex = EditorGUILayout.Popup(oldIntValue, EditorHelpers.iAPNames, GUILayout.Width(250));
			if (oldIntValue != selectedIAPIndex)
			{
				iAPData = EditorHelpers.allIAPs[selectedIAPIndex];
			}
			if (iAPData != null)
			{
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button("Duplicate", GUILayout.Width(70)))
				{
					IAPConfigData newData = iAPData.Clone() as IAPConfigData;
					newData.id = iAPData.id + "(Clone)";
					EditorHelpers.allIAPs.Insert(0, newData);
					EditorHelpers.InitIAPNames();
					selectedIAPIndex = 0;
					iAPData = newData;
					ShowNotification(new GUIContent("iAP duplicated."));
					dirty = true;
				}

				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting iAP!", "Are you sure you want to delete iAP'" + iAPData.id + "'?", "Yes, Delete it.", "No!"))
					{
						EditorHelpers.gameDB.DeleteConfig(iAPData.GetTableName(), iAPData.id);
						EditorHelpers.allIAPs.Remove(iAPData);
						EditorHelpers.InitIAPNames();
						selectedIAPIndex = -1;
						iAPData = null;
						ShowNotification(new GUIContent("iAP deleted."));
						return false;
					}
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (iAPData != null);
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
		oldStringValue = iAPData.id;
		iAPData.id = EditorGUILayout.TextField(iAPData.id, GUILayout.Width(250));
		if (iAPData.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name:", GUILayout.Width(100));
		oldStringValue = iAPData.name;
		iAPData.name = EditorGUILayout.TextField(iAPData.name, GUILayout.Width(250));
		if (iAPData.name != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Description:", GUILayout.Width(100));
		oldStringValue = iAPData.description;
		iAPData.description = EditorGUILayout.TextField(iAPData.description, GUILayout.Width(250));
		if (iAPData.description != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorHelpers.DrawListOfIAPData("iAP Data", iAPData.iapData, ref dirty);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Consumable:", GUILayout.Width(100));
		oldBoolValue = iAPData.consumable;
		iAPData.consumable = EditorGUILayout.Toggle(iAPData.consumable, GUILayout.Width(20));
		if (iAPData.consumable != oldBoolValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("No Ads:", GUILayout.Width(100));
		oldBoolValue = iAPData.noAdsIap;
		iAPData.noAdsIap = EditorGUILayout.Toggle(iAPData.noAdsIap, GUILayout.Width(20));
		if (iAPData.noAdsIap != oldBoolValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}
