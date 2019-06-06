
using System.Linq;
using System.Collections.Generic;


namespace TabTale
{
	public class RankConfigModel : ConfigModel<RankConfigData>
	{
		public RankConfigData GetRank (string id)
		{
			return _configs.FirstOrDefault (x => x.GetId () == id).Clone () as RankConfigData;
		}

		public RankConfigData GetRankForXp(int xp)
		{
			return _configs.Where (rankData => rankData.xp < xp).OrderBy(rankData => rankData.xp).LastOrDefault();
		}

		public string GetName (string id)
		{
			return GetConfig (id).name;
		}

		public int GetXp (string id)
		{
			return GetConfig (id).xp;
		}

		public string GetRankUpTextId (string id)
		{
			return GetConfig (id).rankUpTextId;
		}

		public int GetIndex (string id)
		{
			return GetConfig (id).index;
		}

		public List<GameElementData> GetRestrictions (string id)
		{
			return GetConfig (id).restrictions;
		}

		public IEnumerable<RankConfigData> GetRanks()
		{
			return _configs.OrderBy(r => r.index);
		}
	}
}