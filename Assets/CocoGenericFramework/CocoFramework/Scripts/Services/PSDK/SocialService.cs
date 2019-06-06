using System;
using UnityEngine;
using TabTale;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale 
{

	public class SocialService : ISocialService
	{
		private IPsdkSocial _psdkSocial;

		public event Action<bool> onAuthenticateEvent;
		public event Action       onSignOutEvent;

		#region ISocialService implementation

		[PostConstruct]
		public void Setup()
		{
			_psdkSocial = PSDKMgr.Instance.GetSocial();

			PsdkEventSystem.Instance.onSocialAuthenticate += onAuthenticateEvent;
			PsdkEventSystem.Instance.onSocialSignOut += onSignOutEvent;
		}

		public bool IsAuthenticated 
		{
			get 
			{
				return _psdkSocial.IsAuthenticated();
			}
		}

		public string SocialId 
		{
			get 
			{
				return _psdkSocial.SocialId();
			}
		}

		public void Init ()
		{
			_psdkSocial.Init();
		}
			
		public void Authenticate()
		{
			_psdkSocial.Authenticate();
		}
			

		public void SignOut ()
		{
			_psdkSocial.SignOut();
		}

		public void ReportProgress (string achievementID, double progress)
		{
			_psdkSocial.ReportProgress(achievementID, progress);
		}

		public void ResetAllAchievements ()
		{
			_psdkSocial.ResetAllAchievements();
		}

		public void SetPlayerScore (String leaderboardId, long score)
		{
			_psdkSocial.SetPlayerScore(leaderboardId, score);
		}

		public void ShowLeaderboard ()
		{
			_psdkSocial.ShowLeaderboard();
		}
		
		public void ShowAchievements ()
		{
			_psdkSocial.ShowAchievements();
		}
			
		#endregion
	}

}


