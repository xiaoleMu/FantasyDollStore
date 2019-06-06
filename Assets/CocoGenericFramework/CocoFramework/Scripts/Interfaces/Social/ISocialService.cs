using UnityEngine;
using System.Collections;

namespace TabTale 
{
	public interface ISocialService
	{		
		event System.Action<bool> onAuthenticateEvent;

		#if UNITY_ANDROID

		event System.Action onSignOutEvent;

		void SignOut ();

		#endif

		void Init ();

		void Authenticate ();
		
		void ReportProgress (string achievementID, double progress);
		
		void ResetAllAchievements ();
		
		void SetPlayerScore (string leaderboardId, long score);
		
		void ShowLeaderboard ();

        void ShowAchievements ();
		
		bool IsAuthenticated { get; }
		
		string SocialId { get; }
	}
}