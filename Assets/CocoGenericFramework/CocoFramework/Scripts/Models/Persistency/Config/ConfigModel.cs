using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using LitJson;
using strange.extensions.signal.impl;
using System.Linq;

namespace TabTale
{
	public class ConfigModel<TConfigData> where TConfigData : class, IConfigData, new()
	{
		[Inject]
		public IGameDB _gameDB{get;set;}

		[Inject]
		public SyncConfigsSignal syncConfigsSignal{get;set;}

		protected string _class;

		protected int _loggerModule;

		protected bool _shortLoadLog = false;

		protected ICollection<TConfigData> _configs;

		private int _maxLogItems = 25;

		[PostConstruct]
		public void Init(){
			_class = GetType().Name;
			_loggerModule = CoreLogger.RegisterModule(_class);

			Load ();

			syncConfigsSignal.AddListener(OnConfigSync);
		}
		
		protected bool Load()
		{
			if (_configs != null)
				_configs.Clear();

			_configs = _gameDB.LoadConfig<TConfigData>();

			if (_shortLoadLog)
				Debug.Log ("LOAD CONFIG (" + _class + ") got " + _configs.Count + " configs.\n(Set _fullLoadLog=true to get full loading log)");
			else
			{
				string log = "LOAD CONFIG (" + _class + ") got " + _configs.Count + " configs:";
				foreach(TConfigData _configItem in _configs.Take(_maxLogItems))
					log += ("\n" + _configItem.ToLogString());

				if(_configs.Count > _maxLogItems)
				{
					log += ("\n.... (Reached maximum items to display. Displayed " + _maxLogItems + " items out of " + _configs.Count + ")");
				}

				Debug.Log(log);
			}

			return true;

		}

		protected void OnConfigSync (ICollection<string> affectedTables)
		{
			string tableName = new TConfigData().GetTableName();
			if (affectedTables.Select(affectedTable => affectedTable.CompareTo(tableName) == 0).Count() > 0)
			    Load ();
		}

		public IEnumerable<TConfigData> GetAllConfigs() 
		{
			return _configs.Select(config => config.Clone() as TConfigData);
		}
		public TConfigData GetConfig(string id) 
		{
			TConfigData data = _configs.FirstOrDefault(config => config.GetId() == id);
			Debugger.Assert(data != null, _class + ".GetConfig(id) - Request for config data with illegal id: " + id);
			return data == null ? null : data.Clone() as TConfigData;
		}
	}

	/*
	public class LevelConfigModel : ConfigModel<LevelConfigData>
	{
		public LevelConfigData GetLevelConfig(int level)
		{
			return _configs.FirstOrDefault(config => (config as LevelConfigData).index == level).Clone() as LevelConfigData;
		}
		public int GetLevelStartXp(int level)
		{
			if (level == 1)
				return 0;
			LevelConfigData data = _configs.FirstOrDefault(config => (config as LevelConfigData).index == level-1) as LevelConfigData;
			return data == null ? 0 : data.xpToNext;
		}
	}
	

	public class RankConfigModel : ConfigModel<RankConfigData>
	{
		public RankConfigData GetRankConfig(int index)
		{
			return _configs.FirstOrDefault(config => (config as RankConfigData).index == index).Clone() as RankConfigData;
		}
	}
	
	public class BuildingConfigModel : ConfigModel<BuildingConfigData>
	{
		public BuildingConfigData GetBuildingConfigByIndex(int index)
		{
			return _configs.FirstOrDefault(config => (config as BuildingConfigData).index == index).Clone() as BuildingConfigData;
		}
		public BuildingConfigData GetBuildingConfigByName(string name)
		{
			BuildingConfigData data = _configs.FirstOrDefault(config => (config as BuildingConfigData).name.CompareTo(name) == 0);
			return (data == null) ? null : data.Clone() as BuildingConfigData;
		}
	}
	
	public class TextConfigModel : ConfigModel<TextConfigData>
	{
		public string GetText(string key)
		{
			TextConfigData data = _configs.FirstOrDefault(config => (config as TextConfigData).key == key) as TextConfigData;
			return (data == null) ? "" : data.value;
		}
	}

	
	public class RewardConfigModel : ConfigModel<RewardConfigData>
	{

		public IList<RewardItemData> GetReward(string key)
		{
			List<RewardItemData> rewards = new List<RewardItemData>();
			RewardConfigData data = _configs.FirstOrDefault(config => (config as RewardConfigData).key == key) as RewardConfigData;
			Debugger.Assert((data != null), "RewardConfigModel.GiveReward() called with non-existing reward name \"" + key + "\"");
			Debugger.Assert((data.rewardItems.Count() > 0), "RewardConfigModel.GiveReward() found zero reward items for reward \"" + key + "\"");
			if (data == null || data.rewardItems.Count() == 0)
				return rewards;

			int chanceCheck = 0;
			System.Random random = new System.Random();
			foreach(RewardItemData item in data.rewardItems)
			{
				chanceCheck = random.Next(1,100);
				if (chanceCheck <= item.chance)
					rewards.Add(item);
			}
			return rewards;
		}
	}
	
	public class CategoryConfigModel : ConfigModel<CategoryConfigData>
	{
		public IEnumerable<CategoryConfigData> GetMakupConfigs()
		{
			return _configs.Where(config => (config as CategoryConfigData).IsMakeupCategory()).Select(config => config.Clone() as CategoryConfigData);
		}
		public IEnumerable<CategoryConfigData> GetOutfitConfigs()
		{
			return _configs.Where(config => (config as CategoryConfigData).IsOutfitCategory()).Select(config => config.Clone() as CategoryConfigData);
		}
		public IEnumerable<CategoryConfigData> GetOutfitClothingConfigs()
		{
			return _configs.Where(config => (config as CategoryConfigData).IsClothingCategory()).Select(config => config.Clone() as CategoryConfigData);
		}
		public IEnumerable<CategoryConfigData> GetOutfitAccessoryConfigs()
		{
			return _configs.Where(config => (config as CategoryConfigData).IsAccessoryCategory()).Select(config => config.Clone() as CategoryConfigData);
		}
		public ICollection<string> GetColorsOfCategory(string id)
		{
			CategoryConfigData category = GetConfig(id);
			if (category.colorOptions != null)
				return category.colorOptions.ToList();
			return new List<string>();
		}
	}
	
	public class MakeupItemConfigModel : ConfigModel<MakeupItemConfigData>
	{
		public IEnumerable<MakeupItemConfigData> GetItemsOfCategory(string categoryId)
		{
			return _configs.Where(config => (config as MakeupItemConfigData).itemProperty.categoryId.CompareTo(categoryId) == 0).Select(config => config.Clone() as MakeupItemConfigData);
		}
	}
	
	public class OutfitItemConfigModel : ConfigModel<OutfitItemConfigData>
	{
		public IEnumerable<OutfitItemConfigData> GetItemsOfCategory(string categoryId)
		{
			return _configs.Where(config => (config as OutfitItemConfigData).itemProperty.categoryId.CompareTo(categoryId) == 0).Select(config => config.Clone() as OutfitItemConfigData);
		}
	}
	
	public class ShopConfigModel : ConfigModel<ShopConfigData>
	{
		public ShopConfigData GetShop(string shopName)
		{
			ShopConfigData data = _configs.FirstOrDefault(config => (config as ShopConfigData).name == shopName);
			Debugger.Assert((data != null), "ShopConfigModel.GetShop() - got illegal shop name (" + shopName + ")");
			return (data == null) ? null : data.Clone() as ShopConfigData;
		}
		public ShopItemData[] GetItemsOfShop(string shopName)
		{
			ShopConfigData data = _configs.FirstOrDefault(config => (config as ShopConfigData).name == shopName);
			Debugger.Assert((data != null), "ShopConfigModel.GetItemsOfShop() - got illegal shop name (" + shopName + ")");
			return (data == null) ? null : (data.Clone() as ShopConfigData).shopOutfitItems;
		}
		public string GetItemIdFromShop(string shopName, string placeHolderId)
		{
			ShopConfigData data = _configs.FirstOrDefault(config => (config as ShopConfigData).name == shopName);
			if (data == null)
				return "";
			Debugger.Assert((data != null), "ShopConfigModel.GetItemIdFromShop() - got illegal shop name (" + shopName + ")");
			ShopItemData itemData = data.shopOutfitItems.FirstOrDefault(item => item.placeholderId == placeHolderId);
			Debugger.Assert((itemData != null), 
			                "ShopConfigModel.GetItemIdFromShop() - got illegal placeHolderId (" + placeHolderId + ") for shop (" + shopName + ")");
			if (itemData == null)
				return "";
			return itemData.outfitItemId;
		}
	}
    */   
}

