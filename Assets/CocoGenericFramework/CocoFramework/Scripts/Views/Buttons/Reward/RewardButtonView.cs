using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class RewardButtonView : GameView 
	{
		[Inject]
		public ServerTime serverTime { get; set; }

		void OnApplicationPause(bool paused)
		{
			if(!paused)
			{
				Debug.Log ("RewardButtonView - Requesting refresh of server time");
				serverTime.Refresh();
			}
		}
	}
}