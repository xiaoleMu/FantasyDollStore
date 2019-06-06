using UnityEngine;
using System.Collections;

namespace TabTale {

	[System.Serializable]
	public class GameIdentity
	{
		public string title;
	}

	//FIXME: this should be a data model obtained from the server
	[System.Serializable]
	public class GameProperties
	{
		public string leaderboardId;
	}
}
