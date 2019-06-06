using UnityEngine;
using UnityEditor;
using TabTale;
using System.Collections.Generic;
using System;

public class EditorHelpers
{
	static Color contentColor;
	static Color backgroundColor;
	public static Color orangeColor = new Color(255.0f/255.0f, 165.0f/255.0f, 0.0f/255.0f);
	public static Color yellowColor = Color.yellow;
	public static Color greenColor = Color.green;
	public static Color redColor = Color.red;
	public static Color blueColor = Color.blue;

	public static GameDB gameDB = null;

	#region Data
	public static string EMPTY = "EMPTY";
	public static List<ItemConfigData> allItems
	{
		get 
		{
			if (_itemConfigNamesFilter.Length == 0)
				return _allItems;
			return _allItemsFiltered;
		}
		set
		{ 
			allItems = value;
			_itemConfigNamesFilter = "";
		}
	}
	public static string [] itemConfigNames
	{
		get
		{ 
			if (_itemConfigNamesFilter.Length == 0)
				return _itemConfigNames;
			return _itemConfigNamesFiltered;
		}
		set 
		{
			_itemConfigNames = value;
			_itemConfigNamesFilter = "";
		}
	}
	public static List<ItemConfigData> _allItems = null;
	static string [] _itemConfigNames = null;
	static string _itemConfigNamesFilter = "";
	static List<ItemConfigData> _allItemsFiltered = new List<ItemConfigData>();
	static string [] _itemConfigNamesFiltered = null;
	public static List<CollectibleConfigData> allCollectibles = null;
	public static string [] collectibleConfigNames = null;
	public static List<ItemGroupConfigData> allItemGroups = null;
	public static string [] itemGroupNames = null;
	public static List<CategoryConfigData> allCategories = null;
	public static string [] categoryNames = null;
	public static List<RewardItemConfigData> allRewardItems = null;
	public static string [] rewardItemNames = null;
	public static List<RewardConfigData> allRewards = null;
	public static string [] rewardNames = null;
	public static List<IAPConfigData> allIAPs = null;
	public static string [] iAPNames = null;
	public static List<EventConfigData> allEvents = null;
	public static string [] eventNames = null;

	public static void ClearData()
	{
		if (_allItems != null)
			_allItems.Clear();
		_allItems = null;
		_itemConfigNames = null;

		if (allCollectibles != null)
			allCollectibles.Clear();
		allCollectibles = null;
		collectibleConfigNames = null;

		if (allItemGroups != null)
			allItemGroups.Clear();
		allItemGroups = null;
		itemGroupNames = null;

		if (allCategories != null)
			allCategories.Clear();
		allCategories = null;
		categoryNames = null;

		if (allRewardItems != null)
			allRewardItems.Clear();
		rewardItemNames = null;

		if (allRewards != null)
			allRewards.Clear();
		allRewards = null;
		rewardNames = null;

		if (allIAPs != null)
			allIAPs.Clear();
		allIAPs = null;
		iAPNames = null;

		if (allEvents != null)
			allEvents.Clear();
		allEvents = null;
		eventNames = null;

		ClearStateRawData ();

		gameDB = null;
	}

	public static void LoadItemConfigs()
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		_allItems = gameDB.LoadConfig<ItemConfigData>() as List<ItemConfigData>;
		InitNamesFromListOfConfigs(_allItems, ref _itemConfigNames);
	}

	public static void InitItemConfigNames()
	{
		InitNamesFromListOfConfigs(_allItems, ref _itemConfigNames);
	}

	public static void LoadCollectibleConfigs()
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		allCollectibles = gameDB.LoadConfig<CollectibleConfigData>() as List<CollectibleConfigData>;
		InitNamesFromListOfConfigs(allCollectibles, ref collectibleConfigNames);
	}

	public static void InitCollectibleConfigNames()
	{
		InitNamesFromListOfConfigs(allCollectibles, ref collectibleConfigNames);
	}

	public static void LoadItemGroupConfigs()
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		allItemGroups = gameDB.LoadConfig<ItemGroupConfigData>() as List<ItemGroupConfigData>;
		InitNamesFromListOfConfigs(allItemGroups, ref itemGroupNames);
	}

	public static void InitItemGroupNames()
	{
		InitNamesFromListOfConfigs(allItemGroups, ref itemGroupNames);
	}

	public static void LoadCategoryConfigs()
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		allCategories = gameDB.LoadConfig<CategoryConfigData>() as List<CategoryConfigData>;
		InitNamesFromListOfConfigs(allCategories, ref categoryNames);
	}

	public static void InitCategoryNames()
	{
		InitNamesFromListOfConfigs(allCategories, ref categoryNames);
	}

	public static void LoadRewardItemConfigs()
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		allRewardItems = gameDB.LoadConfig<RewardItemConfigData>() as List<RewardItemConfigData>;
		InitNamesFromListOfConfigs(allRewardItems, ref rewardItemNames);
	}

	public static void InitRewardItemNames()
	{
		InitNamesFromListOfConfigs(allRewardItems, ref rewardItemNames);
	}

	public static void LoadRewardConfigs()
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		allRewards = gameDB.LoadConfig<RewardConfigData>() as List<RewardConfigData>;
		InitNamesFromListOfConfigs(allRewards, ref rewardNames);
	}

	public static void InitRewardNames()
	{
		InitNamesFromListOfConfigs(allRewards, ref rewardNames);
	}

	public static void LoadIAPConfigs(bool allowEmptyEntry = false)
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		allIAPs = gameDB.LoadConfig<IAPConfigData>() as List<IAPConfigData>;
		InitIAPNames(allowEmptyEntry);
	}

	public static void InitIAPNames(bool allowEmptyEntry = false)
	{
		InitNamesFromListOfConfigs(allIAPs, ref iAPNames, allowEmptyEntry);
	}

	public static void LoadEventConfigs(bool allowEmptyEntry = false)
	{
		if (gameDB == null)
		{
			gameDB =  new GameDB();
			gameDB.InitLocalDB();
		}
		allEvents = gameDB.LoadConfig<EventConfigData>() as List<EventConfigData>;
		InitEventNames(allowEmptyEntry);
	}

	public static void InitEventNames(bool allowEmptyEntry = false)
	{
		InitNamesFromListOfConfigs(allEvents, ref eventNames, allowEmptyEntry);
	}
	#endregion

	public static void DrawListOfGameElemetData(string caption, List<GameElementData> list, ref bool dirty)
	{
		if (list == null)
			return;
		
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			GameElementData newData = new GameElementData(GameElementType.Item, "Key " + list.Count, 1);
			list.Add(newData);
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		GameElementData removedElement = null;
		foreach(var element in list)
		{
			if (DrawSingleGameElementData(element, ref dirty))
				removedElement = element;
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (null != removedElement)
		{
			list.Remove(removedElement);
			dirty = true;
		}
	}

	/// <summary>
	/// /Draws the single game element data.*/
	/// </summary>
	/// <returns><c>true</c>, if single game element data was marked for removal, <c>false</c> otherwise.</returns>
	/// <param name="element">Element.</param>
	/// <param name="dirty">Dirty.</param>
	public static bool DrawSingleGameElementData(GameElementData element, ref bool dirty, bool enableDelete = true)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Type:", GUILayout.Width(100));
		GameElementType oldType = element.type;
		element.type = (GameElementType)EditorGUILayout.EnumPopup(element.type, GUILayout.Width(150));
		if (element.type != oldType)
			dirty = true;
		bool removed = false;
		if (enableDelete)
		{
			GUI.backgroundColor = redColor;
			if (GUILayout.Button("Delete this entry", GUILayout.Width(100)))
			{
				removed = true;
			}
			GUI.backgroundColor = backgroundColor;
		}
		EditorGUILayout.EndHorizontal();

		if (element.type == GameElementType.Item)
		{
			if (allItems == null)
			{
				EditorGUILayout.HelpBox("Items are not loaded properly.", MessageType.Error);
			}
			else if (allItems.Count == 0)
			{
				EditorGUILayout.HelpBox("There are no Items. Use Item Editor to add some.", MessageType.Info);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Item:", GUILayout.Width(100));
				int oldIntValue = IndexFromName(element.key, itemConfigNames);
				int newIntValue = EditorGUILayout.Popup(oldIntValue, itemConfigNames, GUILayout.Width(150));
				if (oldIntValue != newIntValue)
				{
					element.key = itemConfigNames[newIntValue];
					dirty = true;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		else if (element.type == GameElementType.State)
		{
			if (allCollectibles == null)
			{
				EditorGUILayout.HelpBox("Collectibles are not loaded properly.", MessageType.Error);
			}
			else if (allCollectibles.Count == 0)
			{
				EditorGUILayout.HelpBox("There are no Collectibles. Use Collectibles Editor to add some.", MessageType.Info);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Collectible:", GUILayout.Width(100));
				int oldIntValue = IndexFromName(element.key, collectibleConfigNames);
				int newIntValue = EditorGUILayout.Popup(oldIntValue, collectibleConfigNames, GUILayout.Width(150));
				if (oldIntValue != newIntValue)
				{
					element.key = collectibleConfigNames[newIntValue];
					dirty = true;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		else if (element.type == GameElementType.Currency)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Key:", GUILayout.Width(100));
			string oldStringValue = element.key;
			element.key = EditorGUILayout.TextField(element.key, GUILayout.Width(150));
			if (element.key != oldStringValue)
				dirty = true;
			EditorGUILayout.EndHorizontal();
		}

		//		public string value;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Value:", GUILayout.Width(100));
		int oldValue = element.value;
		element.value = EditorGUILayout.IntField(element.value, GUILayout.Width(150));
		if (element.value != oldValue)
			dirty = true;
		EditorGUILayout.EndHorizontal();
		return removed;
	}

	public static void DrawListOfAssetData(string caption, List<AssetData> list, ref bool dirty)
	{
		if (list == null)
			return;
		
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			AssetData newData = new AssetData("ID " + list.Count, AssetType.String, string.Empty);
			list.Add(newData);
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		AssetData removedElement = null;
		foreach(var element in list)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Type:", GUILayout.Width(100));
			AssetType oldType = element.assetType;
			element.assetType = (AssetType)EditorGUILayout.EnumPopup(element.assetType, GUILayout.Width(150));
			if (element.assetType != oldType)
				dirty = true;
			GUI.backgroundColor = redColor;
			if (GUILayout.Button("Delete this entry", GUILayout.Width(100)))
			{
				removedElement = element;
			}
			GUI.backgroundColor = backgroundColor;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("ID:", GUILayout.Width(100));
			string oldStringValue = element.id;
			element.id = EditorGUILayout.TextField(element.id, GUILayout.Width(150));
			if (element.id != oldStringValue)
				dirty = true;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Value:", GUILayout.Width(100));
			oldStringValue = element.value;
			element.value = EditorGUILayout.TextField(element.value, GUILayout.Width(150));
			if (element.value != oldStringValue)
				dirty = true;
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (null != removedElement)
		{
			list.Remove(removedElement);
			dirty = true;
		}
	}
	
	public static void DrawListOfGenericPropertyData(string caption, List<GenericPropertyData> list, ref bool dirty)
	{
		if (list == null)
			return;
		
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			GenericPropertyData newData = new GenericPropertyData();
			newData.id = "ID " + list.Count;
			newData.type = PropertyType.String;
			newData.value = string.Empty;
			list.Add(newData);
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		GenericPropertyData removedElement = null;
		foreach(var element in list)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Type:", GUILayout.Width(100));
			PropertyType oldType = element.type;
			element.type = (PropertyType)EditorGUILayout.EnumPopup(element.type, GUILayout.Width(150));
			if (element.type != oldType)
				dirty = true;
			GUI.backgroundColor = redColor;
			if (GUILayout.Button("Delete this entry", GUILayout.Width(100)))
			{
				removedElement = element;
			}
			GUI.backgroundColor = backgroundColor;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("ID:", GUILayout.Width(100));
			string oldStringValue = element.id;
			element.id = EditorGUILayout.TextField(element.id, GUILayout.Width(150));
			if (element.id != oldStringValue)
				dirty = true;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Value:", GUILayout.Width(100));
			oldStringValue = element.value;
			element.value = EditorGUILayout.TextField(element.value, GUILayout.Width(150));
			if (element.value != oldStringValue)
				dirty = true;
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (null != removedElement)
		{
			list.Remove(removedElement);
			dirty = true;
		}
	}

	public static void DrawListOfEventPrizeData(string caption, List<EventPrizeData> list, ref bool dirty)
	{
		if (list == null)
			return;

		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			EventPrizeData newData = new EventPrizeData();
			list.Add(newData);
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		EventPrizeData removedElement = null;
		foreach(var element in list)
		{
			if (allItems == null)
			{
				EditorGUILayout.HelpBox("Items are not loaded properly.", MessageType.Error);
			}
			else if (allItems.Count == 0)
			{
				EditorGUILayout.HelpBox("There are no Items. Use Item Editor to add some.", MessageType.Info);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Item:", GUILayout.Width(100));
				EditorGUILayout.BeginVertical();
				if (EditorHelpers.DrawSingleGameElementData(element.prize, ref dirty))
					removedElement = element;
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}

			//		public string value;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Value:", GUILayout.Width(100));
			int oldValue = element.value;
			element.value = EditorGUILayout.IntField(element.value, GUILayout.Width(150));
			if (element.value != oldValue)
				dirty = true;
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (null != removedElement)
		{
			list.Remove(removedElement);
			dirty = true;
		}
	}

	public static void DrawListOfNamesAsPopups(string caption, List<string> list, string[] sourceNames, ref bool dirty)
	{
		if (list == null || sourceNames == null || sourceNames.Length == 0)
			return;
		
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			list.Add(sourceNames[0]);
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		string removedElement = null;
		for(int i=0; i<list.Count; ++i)
		{
			string element = list[i];
			EditorGUILayout.BeginHorizontal();
			int oldIndex = IndexFromName(element, sourceNames);
			int newIndex = EditorGUILayout.Popup(oldIndex, sourceNames, GUILayout.Width(150));
			if (newIndex != oldIndex)
			{
				list[i] = sourceNames[newIndex];
				dirty = true;
			}
			GUI.backgroundColor = redColor;
			if (GUILayout.Button("Delete this entry", GUILayout.Width(100)))
			{
				removedElement = element;
			}
			GUI.backgroundColor = backgroundColor;
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (null != removedElement)
		{
			list.Remove(removedElement);
			dirty = true;
		}
	}

	public static int IndexFromName(string name, string[] names)
	{
		for (int i=0; i<names.Length; ++i)
			if (names[i] == name)
				return i;
		return -1;
	}

	public static void DrawListOfInts(string caption, List<int> list, ref bool dirty)
	{
		if (list == null)
			return;
		
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			list.Add(0);
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		int removedIndex = -1;
		for(int i=0; i<list.Count; ++i)
		{
			EditorGUILayout.BeginHorizontal();
			int oldValue = list[i];
			list[i] = EditorGUILayout.IntField(list[i], GUILayout.Width(150));
			if (list[i] != oldValue)
				dirty = true;
			GUI.backgroundColor = redColor;
			if (GUILayout.Button("Delete this entry", GUILayout.Width(100)))
			{
				removedIndex = i;
			}
			GUI.backgroundColor = backgroundColor;
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if (removedIndex != -1)
		{
			list.RemoveAt(removedIndex);
			dirty = true;
		}
	}

	public static void DrawListOfStrings(string caption, List<string> list, ref bool dirty, float captionWidth = 100f, float stringWidth = 150f)
	{
		if (list == null)
			return;

		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(captionWidth));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			list.Add(string.Empty);
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		int removedIndex = -1;
		for(int i=0; i<list.Count; ++i)
		{
			EditorGUILayout.BeginHorizontal();
			string oldValue = list[i];
			list[i] = EditorGUILayout.TextField(list[i], GUILayout.Width(stringWidth));
			if (list[i] != oldValue)
				dirty = true;
			GUI.backgroundColor = redColor;
			if (GUILayout.Button("Delete this entry", GUILayout.Width(100)))
			{
				removedIndex = i;
			}
			GUI.backgroundColor = backgroundColor;
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (removedIndex != -1)
		{
			list.RemoveAt(removedIndex);
			dirty = true;
		}
	}

	public static void DrawStringAsPopup(string caption, ref string name, string[] sourceNames, ref bool dirty, float captionWidth = 100f, float popupWidth = 150f, bool useAsIndividualGroup = true)
	{
		if (sourceNames == null)
			return;
		
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		if (useAsIndividualGroup)
			EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(captionWidth));
		int oldIndex = IndexFromName(name, sourceNames);
		int newIndex = EditorGUILayout.Popup(oldIndex, sourceNames, GUILayout.Width(popupWidth));
		if (oldIndex != newIndex)
		{
			name = sourceNames[newIndex];
			if (name == EMPTY)
				name = string.Empty;
			dirty = true;
		}
		if (useAsIndividualGroup)
			EditorGUILayout.EndHorizontal();
	}

	public static void DrawRange(string caption, ref Range range, ref bool dirty, bool useUIGrouping = true, float captionWidth = 100f, float fieldWidth = 50f)
	{
		float oldValue = 0;
		if (useUIGrouping)
			EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(captionWidth));
		oldValue = range.min;
		range.min = EditorGUILayout.FloatField(range.min, GUILayout.Width(fieldWidth));
		if (range.min != oldValue)
			dirty = true;
		GUILayout.Space(5);
		oldValue = range.max;
		range.max = EditorGUILayout.FloatField(range.max, GUILayout.Width(fieldWidth));
		if (range.max != oldValue)
			dirty = true;
		if (useUIGrouping)
			EditorGUILayout.EndHorizontal();
	}

	public static void DrawListOfIAPData(string caption, List<IAPData> list, ref bool dirty)
	{
		if (list == null)
			return;

		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		GUI.backgroundColor = greenColor;
		if (GUILayout.Button("Add", GUILayout.Width(60)))
		{
			list.Add(new IAPData(StoreType.All, ""));
			dirty = true;
		}
		GUI.backgroundColor = backgroundColor;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(30);
		EditorGUILayout.BeginVertical();
		int removedIndex = -1;
		for(int i=0; i<list.Count; ++i)
		{
			IAPData element = list[i];
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Store: ", GUILayout.Width(60));
			StoreType oldStore = element.store;
			element.store = (StoreType)EditorGUILayout.EnumPopup(element.store, GUILayout.Width(60));
			if (oldStore != element.store)
				dirty = true;
			string oldId = element.iapId;
			element.iapId = EditorGUILayout.TextField(element.iapId, GUILayout.Width(250));
			if (oldId != element.iapId)
				dirty = true;
			GUILayout.Space(20);
			GUI.backgroundColor = redColor;
			if (GUILayout.Button("Delete this entry", GUILayout.Width(100)))
			{
				removedIndex = i;
			}
			GUI.backgroundColor = backgroundColor;
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (removedIndex != -1)
		{
			list.RemoveAt(removedIndex);
			dirty = true;
		}
	}

	static string [] years = {"2016", "2017", "2018", "2019", "2020", "2021", "2022", "2023", "2024", "2025", "2026", "2027", "2028", "2029", "2030"};
	static string [] months = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"};
	static string [] days = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"};

	public static void DrawDateTime(string caption, ref DateTime dateTime, ref bool dirty)
	{
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(100));
		string year = dateTime.Year.ToString();
		string month = dateTime.Month.ToString();
		string day = dateTime.Day.ToString();
		bool localDirty = false;
		DrawStringAsPopup("", ref year, years, ref localDirty, 0, 60, false);
		DrawStringAsPopup("", ref month, months, ref localDirty, 0, 60, false);
		DrawStringAsPopup("", ref day, days, ref localDirty, 0, 60, false);
		int hour = dateTime.Hour;
		int newHour = EditorGUILayout.IntField(hour, GUILayout.Width(40));
		if (newHour != hour)
		{
			localDirty = true;
			newHour = Mathf.Clamp(newHour, 0, 23);
		}
		int minute = dateTime.Minute;
		int newMinute = EditorGUILayout.IntField(minute, GUILayout.Width(40));
		if (newMinute != minute)
		{
			localDirty = true;
			newMinute = Mathf.Clamp(newMinute, 0, 59);
		}
		if (localDirty)
		{
			dirty = localDirty;
			dateTime = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), newHour, newMinute, 0);
		}
		EditorGUILayout.EndHorizontal();
	}

	public static string [] storeNames = 
	{
		"all",
		"ios",
		"google",
		"amazon",
	};

	public static void InitNamesFromListOfConfigs<T>(List<T> configs, ref string [] names, bool allowEmptyEntry = false) where T:IConfigData
	{
		if (configs == null)
			return;
		int count = configs.Count;
		if (allowEmptyEntry)
			++count;
		names = new string[count];
		int s = 0;
		if (allowEmptyEntry)
		{
			names[s] = EMPTY;
			++s;
		}

		foreach(var p in configs)
		{
			names[s] = p.GetId();
			++s;
		}
	}

	public static bool DrawItemsFilter(string caption, float captionWidth = 100f, float filterWidth = 200f, bool useAsIndividualGroup = true)
	{
		return DrawFilter(caption, ref _itemConfigNamesFilter, _allItems, _allItemsFiltered, ref _itemConfigNamesFiltered, captionWidth, filterWidth, useAsIndividualGroup);
	}

	public static bool DrawFilter<T>(string caption, ref string filter, List<T> configs, List<T> filteredConfigs, ref string[] filteredNames, float captionWidth = 100f, float filterWidth = 200f, bool useAsIndividualGroup = true) where T:IConfigData
	{
		bool result = false;
		contentColor = GUI.contentColor;
		backgroundColor = GUI.backgroundColor;

		if (useAsIndividualGroup)
			EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(captionWidth));
		string newFilter = EditorGUILayout.DelayedTextField(filter, GUILayout.Width(filterWidth));
		if (newFilter != filter)
		{
			filter = newFilter;
			if (filter.Length > 0)
			{
				filteredConfigs.Clear();
				foreach(var c in configs)
				{
					if (c.GetId().Contains(filter))
					{
						filteredConfigs.Add(c);
					}
				}
				InitNamesFromListOfConfigs(filteredConfigs, ref filteredNames);
			}
			result = true;
		}
		if (useAsIndividualGroup)
			EditorGUILayout.EndHorizontal();
		return result;
	}


	#region State Raw Data

	public static List<StateRawData> AllStates = null;
	public static string[] StateNames = null;

	public static void LoadStateRawDatas (bool allowEmptyEntry = false)
	{
		if (gameDB == null) {
			gameDB = new GameDB ();
			gameDB.InitLocalDB ();
		}
		AllStates = gameDB.LoadStateRawData () as List<StateRawData>;
		InitStateNames (allowEmptyEntry);
	}

	public static void InitStateNames (bool allowEmptyEntry = false)
	{
		if (AllStates == null)
			return;
		int count = AllStates.Count;
		if (allowEmptyEntry)
			++count;
		StateNames = new string[count];
		int s = 0;
		if (allowEmptyEntry)
		{
			StateNames[s] = EMPTY;
			++s;
		}

		foreach(var p in AllStates)
		{
			StateNames[s] = p.ID;
			++s;
		}
	}

	private static void ClearStateRawData ()
	{
		if (AllStates != null) {
			AllStates.Clear ();
			AllStates = null;
		}
		StateNames = null;
	}

	#endregion
}

public class ConfigsFiltered<T> where T:IConfigData
{
	public static string EMPTY = "EMPTY";
	public List<T> configs = new List<T>();
	public string [] names = null;
	public List<T> configsFiltered = new List<T>();
	public string [] namesFiltered = null;
	public string filter = "";

	public List<T> allConfigs
	{
		get
		{
			if (filter.Length == 0)
				return configs;
			else
				return configsFiltered;
		}
		set
		{
			configs = value;
			filter = "";
		}
	}

	public string [] allNames
	{
		get
		{
			if (filter.Length == 0)
				return names;
			else
				return namesFiltered;
		}

		set
		{
			names = value;
			filter = "";
		}
	}

	public void InitNames(bool allowEmptyEntry = false)
	{
		int count = configs.Count;
		if (allowEmptyEntry)
			++count;
		names = new string[count];
		int s = 0;
		if (allowEmptyEntry)
		{
			names[s] = EMPTY;
			++s;
		}

		foreach(var p in configs)
		{
			names[s] = p.GetId();
			++s;
		}
	}

	public bool ApplyFilter(string newFilter, bool allowEmptyEntry = false)
	{
		bool result = false;
		if (newFilter != filter)
		{
			filter = newFilter;
			if (filter.Length > 0)
			{
				configsFiltered.Clear();
				foreach(var c in configs)
				{
					if (c.GetId().Contains(filter))
					{
						configsFiltered.Add(c);
					}
				}
				InitNames(allowEmptyEntry);
			}
			result = true;
		}
		return result;
	}

	public bool DrawFilter(string caption, float captionWidth = 100f, float filterWidth = 200f, bool useAsIndividualGroup = true)
	{
		bool result = false;

		if (useAsIndividualGroup)
			EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(caption, GUILayout.Width(captionWidth));
		string newFilter = EditorGUILayout.DelayedTextField(filter, GUILayout.Width(filterWidth));
		if (newFilter != filter)
		{
			result = ApplyFilter(newFilter);
		}
		if (useAsIndividualGroup)
			EditorGUILayout.EndHorizontal();
		return result;
	}

}