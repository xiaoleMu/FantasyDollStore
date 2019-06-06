using UnityEngine;
using System.Collections.Generic;


namespace TabTale
{
	public enum RewardType 
	{
		Advent=0,
		Consecutive=1,
		NonConsecutive=2,
		Random=3,
		MysteryBox=4
	}
	
	public enum DelayType
	{
		Hours=0,
		Days=1,
		Minutes=2,
		None=3
	}

	public class RewardConfigData  : IConfigData
	{
		public string id ;
		public RewardType rewardType ;
		public DelayType delayType ;
		public List<int> delays = new List<int>();
		public float socialFactor;
		public float rvFactor ;
		public bool doubleFactor ;
		public bool permanentSocialFactor ;
		public List<GameElementData> restrictions = new List<GameElementData>();
		#region IConfigData implementation

		public string GetTableName ()
		{
			return "reward_config";
		}

		public string GetId ()
		{
			return id;
		}

		public string ToLogString ()
		{
			return ToString();
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			RewardConfigData c = new RewardConfigData();
			c.id = id;
			c.rewardType = rewardType;
			c.delayType = delayType;
			c.delays = new List<int>(delays);
			c.socialFactor = socialFactor;
			c.rvFactor = rvFactor;
			c.doubleFactor = doubleFactor;
			c.permanentSocialFactor = permanentSocialFactor;
			c.restrictions = new List<GameElementData>(restrictions);

			return c;
		}
		public override string ToString ()
		{
			return string.Format ("[RewardConfigData: id={0}, rewardType={1}, delayType={2}, delays={3}, socialFactor={4}, rvFactor={5}, doubleFactor={6}, permanentSocialFactor={7}, restrictions={8}]", id, rewardType, delayType, delays, socialFactor, rvFactor, doubleFactor, permanentSocialFactor, restrictions);
		}
		#endregion

	}
}