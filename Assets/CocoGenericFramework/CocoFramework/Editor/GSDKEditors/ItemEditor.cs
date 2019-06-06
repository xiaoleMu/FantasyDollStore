using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TabTale;

public class ItemEditor : EditorWindow
{
	Color contentColor;
	Color backgroundColor;
	bool dirty = false;
	
	// Config Data Vars
	ItemConfigData itemData = null;
	int selectedItemIndex = -1;
	int newItemNameSufix = 0;

	#region Unity Editor Methods
	[MenuItem("TabTale/DB Editors/Item Editor &#1")]
	public static void ShowWindow()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null)
		{
			EditorUtility.DisplayDialog("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		}
		else
			EditorWindow.GetWindow(typeof(ItemEditor));
	}
	
	bool inited = false;
	void Init(bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!inited || forceLoad)
		{
			titleContent.text = "Item Editor";
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
		EditorHelpers.LoadCollectibleConfigs();
		EditorHelpers.LoadItemGroupConfigs();
		EditorHelpers.LoadIAPConfigs(true);
	}

	void Save()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;
		
		// check if names are valid
		foreach(var d in EditorHelpers._allItems)
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
			if (EditorHelpers.allItems.Count > 0)
				EditorHelpers.gameDB.DeleteAllConfigs(EditorHelpers._allItems[0].GetTableName());
			foreach(var p in EditorHelpers._allItems)
			{
				EditorHelpers.gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion();
			EditorHelpers.InitItemConfigNames();
			dirty = false;
		}
	}
	#endregion

	#region Services
	bool iapCreatorSectionOpened = true;
	string iapName = "";
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
			ItemConfigData newData = new ItemConfigData();
			newData.id ="New Item " + newItemNameSufix.ToString();
			++newItemNameSufix;
			EditorHelpers.allItems.Insert(0, newData);
			EditorHelpers.InitItemConfigNames();
			selectedItemIndex = 0;
			itemData = newData;
			ShowNotification(new GUIContent("New Item added."));
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
		if (EditorHelpers.allItems == null)
		{
			EditorGUILayout.HelpBox("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}

		iapCreatorSectionOpened = EditorGUILayout.Foldout(iapCreatorSectionOpened, "iAP Creator");

		if (iapCreatorSectionOpened)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("iAP Name:", GUILayout.Width(100));
			iapName = EditorGUILayout.TextField(iapName, GUILayout.Width(250));
			if (GUILayout.Button("Create", GUILayout.Width(70)))
			{
				if (iapName.Length > 0)
				{
					bool exists = false;
					foreach(var id in EditorHelpers.allIAPs)
					{
						if (id.id == iapName)
						{
							ShowNotification(new GUIContent("iAP with id (" + iapName + ") already exists."));
							exists = true;
							break;
						}
					}
					if (!exists)
					{
						IAPConfigData newiAP = new IAPConfigData();
						newiAP.id = iapName;
						newiAP.name = iapName;
						var newIAPData = new IAPData(StoreType.All, iapName);
						newiAP.iapData.Add(newIAPData);
						EditorHelpers.allIAPs.Add(newiAP);
						EditorHelpers.InitIAPNames(true);
						ShowNotification(new GUIContent("New iAP created."));
						if (EditorHelpers.allIAPs.Count > 0)
							EditorHelpers.gameDB.DeleteAllConfigs(EditorHelpers.allIAPs[0].GetTableName());
						foreach(var p in EditorHelpers.allIAPs)
						{
							EditorHelpers.gameDB.SaveConfig(p.GetTableName(), p.id, LitJson.JsonMapper.ToJson(p));
						}
						EditorHelpers.gameDB.IncrementLocalDBVersion();
					}
				}
				else
					ShowNotification(new GUIContent("iAP must have a name."));
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();EditorGUILayout.Separator();
		}
		
		if (EditorHelpers.DrawItemsFilter("Item Filter:"))
		{
			selectedItemIndex = -1;
			itemData = null;
		}
		if (EditorHelpers.allItems.Count > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Items:", GUILayout.Width(100));
			int oldIntValue = selectedItemIndex;
			selectedItemIndex = EditorGUILayout.Popup(oldIntValue, EditorHelpers.itemConfigNames, GUILayout.Width(250));
			if (oldIntValue != selectedItemIndex)
			{
				itemData = EditorHelpers.allItems[selectedItemIndex];
			}
			if (itemData != null)
			{
				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button("Delete", GUILayout.Width(70)))
				{
					if (EditorUtility.DisplayDialog("Deleting Item!", "Are you sure you want to delete parameter '" + itemData.id + "'?", "Yes, Delete it.", "No!"))
					{
						EditorHelpers.gameDB.DeleteConfig(itemData.GetTableName(), itemData.id);
						EditorHelpers.allItems.Remove(itemData);
						EditorHelpers.InitItemConfigNames();
						selectedItemIndex = -1;
						itemData = null;
						ShowNotification(new GUIContent("Item deleted."));
						return false;
					}
				}
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
				{
					ItemConfigData newData = itemData.Clone() as ItemConfigData;
					newData.id = itemData.id + "(Clone)";
					EditorHelpers.allItems.Insert(0, newData);
					EditorHelpers.InitItemConfigNames();
					selectedItemIndex = 0;
					itemData = newData;
					ShowNotification(new GUIContent("Item duplicated."));
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

//		EditorGUILayout.BeginHorizontal();
//		EditorGUILayout.LabelField("iAP Config ID:", GUILayout.Width(100));
//		oldStringValue = itemData.iapConfigId;
//		itemData.iapConfigId = EditorGUILayout.TextField(itemData.iapConfigId, GUILayout.Width(150));
//		if (itemData.iapConfigId != oldStringValue)
//			dirty = true;
//		EditorGUILayout.EndHorizontal();

		EditorHelpers.DrawStringAsPopup("iAP Config ID:", ref itemData.iapConfigId, EditorHelpers.iAPNames, ref dirty);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Quantity:", GUILayout.Width(100));
		oldIntValue = itemData.quantity;
		itemData.quantity = EditorGUILayout.IntField(itemData.quantity, GUILayout.Width(150));
		if (itemData.quantity != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Category:", GUILayout.Width(100));
		oldStringValue = itemData.category;
		itemData.category = EditorGUILayout.TextField(itemData.category, GUILayout.Width(150));
		if (itemData.category != oldStringValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Index:", GUILayout.Width(100));
		oldIntValue = itemData.index;
		itemData.index = EditorGUILayout.IntField(itemData.index, GUILayout.Width(150));
		if (itemData.index != oldIntValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Item Type:", GUILayout.Width(100));
		ItemType oldItemType = itemData.itemType;
		itemData.itemType = (ItemType)EditorGUILayout.EnumPopup(itemData.itemType, GUILayout.Width(150));
		if (itemData.itemType != oldItemType)
			dirty = true;
		EditorGUILayout.EndHorizontal();

		if (itemData.itemType == ItemType.Currency || itemData.itemType == ItemType.Reward)
		{
			EditorGUILayout.BeginHorizontal();
			string keyType = string.Empty;
			if (itemData.itemType == ItemType.Currency)
				keyType = "Currency Key:";
			else if (itemData.itemType == ItemType.Reward)
				keyType = "Reward Id:";
			EditorGUILayout.LabelField(keyType, GUILayout.Width(100));
			oldStringValue = itemData.itemTypeKey;
			itemData.itemTypeKey = EditorGUILayout.TextField(itemData.itemTypeKey, GUILayout.Width(150));
			if (itemData.itemTypeKey != oldStringValue)
				dirty = true;
			EditorGUILayout.EndHorizontal();
		}
		else if (itemData.itemType == ItemType.PackCost)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Item Group:", GUILayout.Width(100));
			int oldPackCostKey = EditorHelpers.IndexFromName(itemData.itemTypeKey, EditorHelpers.itemGroupNames);
			int newPackCostKey = EditorGUILayout.Popup(oldPackCostKey, EditorHelpers.itemGroupNames, GUILayout.Width(150));
			if (newPackCostKey != oldPackCostKey)
			{
				itemData.itemTypeKey = EditorHelpers.itemGroupNames[newPackCostKey];
				dirty = true;
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Costs:", itemData.costs, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Alternative Costs:", itemData.alternativeCosts, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGameElemetData("Restrictions:", itemData.restrictions, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfAssetData("Assets:", itemData.assets, ref dirty);
		EditorGUILayout.Separator();
		EditorHelpers.DrawListOfGenericPropertyData("Properties:", itemData.properties, ref dirty);
		EditorGUILayout.Separator();EditorGUILayout.Separator();EditorGUILayout.Separator();
	}
	#endregion
}