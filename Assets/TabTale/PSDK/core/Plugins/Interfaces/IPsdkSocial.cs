using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {

	public interface IPsdkSocial : IPsdkService 
	{
		void Init ();

		void Authenticate ();

		bool ReportProgress (string achievementID, double progress);

		void ResetAllAchievements ();

		void SetPlayerScore (string leaderboardId, long score);

		void ShowLeaderboard ();

		void ShowAchievements ();

		bool IsAuthenticated ();

		string SocialId ();

		void SignOut (); //TO-DO make sure to block when not android

	}
}
