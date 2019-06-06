using System;
using System.Collections.Generic;

namespace TabTale
{
	public class MysteryBoxData
	{
		public List<GameElementData> rewards;
		public string rewardItemId;

		public MysteryBoxData(List<GameElementData> rewards, string rewardItemId)
		{
			this.rewards = rewards;
			this.rewardItemId = rewardItemId;
		}

		public MysteryBoxData() { }
	}
}

