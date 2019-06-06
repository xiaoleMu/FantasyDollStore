using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TabTale
{
	public class EventConfigModel : ConfigModel<EventConfigData>
	{
		#region Getters

		public List<EventPrizeData> GetProgressionPrizes(string eventId)
		{
			List<EventPrizeData> progressionPrizes = _configs.FirstOrDefault(x => x.GetId() == eventId).progressionPrizes;

			progressionPrizes.Sort((x, y) => {
				if(x.value > y.value)
					return -1;
				else if(x.value < y.value)
					return 1;

				return 0;
			});

			return progressionPrizes;
		}

		public List<EventPrizeData> GetRankPrizes(string eventId)
		{
			List<EventPrizeData> rankPrizes = _configs.FirstOrDefault(ev => ev.GetId() == eventId).rankPrizes;

			rankPrizes.Sort((x, y) => {
				if(x.value > y.value)
					return 1;
				else if(x.value < y.value)
					return -1;
		
				return 0;
			});

			return rankPrizes;
		}

		public string GetName (string id)
		{
			return _configs.FirstOrDefault (ev => ev.GetId () == id).name;
		}

		public string GetDescription (string id)
		{
			return _configs.FirstOrDefault (ev => ev.GetId () == id).description;
		}

		public List<string> GetLevels (string id)
		{
			return _configs.FirstOrDefault (ev => ev.GetId () == id).levels;
		}

		public List<string> GetAffinityItems (string id)
		{
			return _configs.FirstOrDefault (ev => ev.GetId () == id).affinityItems;
		}

		public List<AssetData> GetAssets (string id)
		{
			return _configs.FirstOrDefault(ev => ev.id == id).assets;
		}

		public List<GenericPropertyData> GetProperties(string id)
		{
			return _configs.FirstOrDefault(ev => ev.id == id).properties;
		}

		public List<GameElementData> GetRestrictions (string id)
		{
			return _configs.FirstOrDefault (group => group.GetId () == id).restrictions;
		}

		public bool EventExists(string id) {
			try {
				DateTime dt = _configs.FirstOrDefault (ev => ev.GetId () == id).endTime;
				return true;
			} catch (Exception ex) {
				return false;
			}
			return false;
		}

		public DateTime GetEventEndDate(string id)
		{
			return _configs.FirstOrDefault (ev => ev.GetId () == id).endTime;
		}

		#endregion
	}
}

