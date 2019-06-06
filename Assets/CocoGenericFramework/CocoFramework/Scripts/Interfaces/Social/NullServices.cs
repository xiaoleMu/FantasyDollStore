using UnityEngine;
using System.Collections;
using TabTale.Publishing;

namespace TabTale {

	public class NullSocialProvider : NullService, ISocialService
	{
		#region ISocialService implementation

		public event System.Action<bool> onAuthenticateEvent;

		public event System.Action onSignOutEvent;

		public void Init ()
		{
			CoreLogger.LogDebug("NullSocialProvider", "init called");
		}

		public void Authenticate ()
		{
			CoreLogger.LogDebug("NullSocialProvider", "authenticate user called");
		}

		public void SignOut () 
		{
			CoreLogger.LogDebug("NullSocialProvider", "sign out");
		}

		public void ReportProgress (string achievementID, double progress)
		{
			CoreLogger.LogDebug("NullSocialProvider", string.Format("reporting achievement {0}: {1}", achievementID, progress));
		}

		public void ResetAllAchievements ()
		{
			CoreLogger.LogDebug("NullSocialProvider", string.Format("resetting achievements"));
		}

		public void SetPlayerScore (string leaderboardId, long score)
		{
			CoreLogger.LogDebug("NullSocialProvider", string.Format("reporting for leaderboard {0}: {1}", leaderboardId, score));
		}

		public void ShowLeaderboard ()
		{
			CoreLogger.LogDebug("NullSocialProvider", "Leaderboard showing");
		}

        public void ShowAchievements ()
        {
			CoreLogger.LogDebug("NullSocialProvider", "Achievements showing");
        }

		public bool IsAuthenticated 
		{
			get { return false; }
		}

		public string SocialId 
		{
			get { return ""; }
		}

		#endregion


	}
}
