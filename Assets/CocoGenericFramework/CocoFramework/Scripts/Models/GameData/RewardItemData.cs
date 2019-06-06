using System;
using System.Collections.Generic;

namespace TabTale {
	public class RewardItemData  : ICloneable{
		public string rewardItemConfigId;
		public string rewardConfigId;
		public string rewardDate;


		#region ICloneable implementation

		public object Clone ()
		{
			RewardItemData c = new RewardItemData();
			c.rewardDate = rewardDate;
			c.rewardConfigId = rewardConfigId;
			c.rewardItemConfigId = rewardItemConfigId;

			return c;
		}

		#endregion
	}
}
