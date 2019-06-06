using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class GeneralParamsEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	GameDB gameDB = new GameDB();
	
	// Config Data Vars
	GeneralParameterConfigData data = null;
	List<GeneralParameterConfigData> allData = null;
	string [] dataConfigNames = null;
	string [] typeNames = 
	{
		"int",
		"float",
		"string",
		"bool"
	};
	int selectedDataIndex = -1;
	int newDataConfigNameSufix = 0;

	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/General Editor")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(GeneralParamsEditor));
	}

	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "General Editor";
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
			DrawGeneralParameters();
		}
		EditorGUILayout.EndScrollView();
	}
	#endregion

	#region Utility Methods
	void LoadData()
	{
		data = null;
		selectedDataIndex = -1;
		
		gameDB.InitLocalDB();
		if (allData != null)
			allData.Clear();
		allData = gameDB.LoadConfig<GeneralParameterConfigData>() as List<GeneralParameterConfigData>;
		InitDataConfigNames();
	}

	void InitDataConfigNames()
	{
		dataConfigNames = new string[allData.Count];
		int s = 0;
		foreach(var p in allData)
		{
			dataConfigNames[s] = p.id;
			++s;
		}
	}

	int IndexFromTypeName(string typeName)
	{
		for (int i=0; i<typeNames.Length; ++i)
			if (typeNames[i] == typeName)
				return i;
		return -1;
	}

	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in allData)
		{
			if (d.id.Contains("New Parameter") || d.id.Length == 0)
			{
				canSave = false;
				notificationMsg = "Parameter (" + d.id + ") has invalid name!\n";
				break;
			}
		}
		
		if (!canSave)
		{
			ShowNotification(new GUIContent(notificationMsg + "\nConfig is not saved!"));
		}
		else
		{
			if (allData.Count > 0)
				gameDB.DeleteAllConfigs(allData[0].GetTableName());
			foreach(var p in allData)
			{
				gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			gameDB.IncrementLocalDBVersion();
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
			GeneralParameterConfigData newData = new GeneralParameterConfigData();
			newData.id ="New Parameter " + newDataConfigNameSufix.ToString();
			++newDataConfigNameSufix;
			allData.Insert(0, newData);
			InitDataConfigNames();
			selectedDataIndex = 0;
			data = newData;
			ShowNotification(new GUIContent("New Parameter added."));
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

		if (allData == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}

		if (allData.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Parameters:", GUILayout.Width(70));
			int oldIntValue = selectedDataIndex;
			selectedDataIndex = EditorGUILayout.Popup(oldIntValue, dataConfigNames, GUILayout.Width(250));
			if (oldIntValue != selectedDataIndex)
			{
				data = allData[selectedDataIndex];
			}
			if (data != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Parameter!", "Are you sure you want to delete parameter '" + data.id + "'?", "Yes, Delete it.", "No! I was wrong and I am sorry."))
					{
						gameDB.DeleteConfig(data.GetTableName(), data.id);
						allData.Remove(data);
						InitDataConfigNames();
						selectedDataIndex = -1;
						data = null;
						ShowNotification(new GUIContent("Parameter deleted."));
						return false;
					}
				}
				GUI.backgroundColor = backgroundColor;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
		}
		
		return (data != null);
	}
	#endregion

	#region General Params
	void DrawGeneralParameters()
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		string oldStringValue = string.Empty;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Id:", GUILayout.Width(70));
		oldStringValue = data.id;
		data.id = EditorGUILayout.TextField(data.id, GUILayout.Width(150));
		if (data.id != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Type:", GUILayout.Width(70));
		int oldIndex = IndexFromTypeName(data.type);
		int newIndex = EditorGUILayout.Popup(oldIndex, typeNames, GUILayout.Width(150));
		if (oldIndex != newIndex)
		{
			data.type = typeNames[newIndex];
			dirty = true;
		}
		EditorGUILayout.EndHorizontal();

//		public string value;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("value:", GUILayout.Width(70));
		if (data.type == "int")
		{
			int value = 0;
			int.TryParse(data.value, out value);
			int newValue = EditorGUILayout.IntField(value, GUILayout.Width(150));
			if (newValue != value)
			{
				data.value = newValue.ToString();
				dirty = true;
			}
		}
		else if (data.type == "float")
		{
			float value = 0;
			float.TryParse(data.value, out value);
			float newValue = EditorGUILayout.FloatField(value, GUILayout.Width(150));
			if (newValue != value)
			{
				data.value = newValue.ToString();
				dirty = true;
			}
		}
		else if (data.type == "string")
		{
			string value = data.value;
			string newValue = EditorGUILayout.TextField(value, GUILayout.Width(150));
			if (newValue != value)
			{
				data.value = newValue;
				dirty = true;
			}
		}
		else if (data.type == "bool")
		{
			bool value = false;
			bool.TryParse(data.value, out value);
			bool newValue = EditorGUILayout.Toggle(value, GUILayout.Width(150));
			if (newValue != value)
			{
				data.value = newValue.ToString();
				dirty = true;
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Description:", GUILayout.Width(70));
		oldStringValue = data.description;
		data.description = EditorGUILayout.TextField(data.description, GUILayout.Width(150));
		if (data.description != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

	}
	#endregion
}
