using System.Collections.Generic;

namespace TabTale
{
	public class RewardStateData : IStateData {

		public List<RewardItemData> rewards  = new List<RewardItemData>();



		#region IStateData implementation

		public string GetStateName ()
		{
			return "rewardState";
		}

		public string ToLogString ()
		{
			return ToString();
		}

		public IStateData Clone ()
		{
			RewardStateData c = new RewardStateData();
			c.rewards = new List<RewardItemData>(rewards);

			return c;
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[RewardStateData: rewards={0}]", rewards);
		}
	
	}
}