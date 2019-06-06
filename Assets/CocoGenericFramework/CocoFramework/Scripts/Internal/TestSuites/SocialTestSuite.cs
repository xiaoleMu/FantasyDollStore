using System;
using TabTale.Publishing;
using UnityEngine;

namespace TabTale
{
	public class SocialTestSuite : TestSuite
	{
		[Inject]
		public ISocialService socialService { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[PostConstruct]
		public void Init()
		{
			string gamingPlatform = Application.platform == RuntimePlatform.Android ? "GooglePlay" : "Game Center";

			AddTest(TestConnectToSocial, "Connect to " + gamingPlatform, "", networkCheck.HasInternetConnection(), "No network connection");

			AddTest(TestShowLeaderboard, "Show Leaderboard", "", networkCheck.HasInternetConnection(), "No network connection");

			AddTest(TestShowAchievements, "Show Achievements", "", networkCheck.HasInternetConnection(), "No network connection");
		}

		private void TestConnectToSocial (Action<TestCaseResult> resultCallback)
		{
			socialService.Init();
			resultCallback(TestCaseResult.Create(TestCaseResultCode.Inconclusive, "Expecting user to see native UI notifying of connection"));
		}

		private void TestShowLeaderboard (Action<TestCaseResult> resultCallback)
		{
			socialService.ShowLeaderboard();
			resultCallback(TestCaseResult.Create(TestCaseResultCode.Inconclusive, "Expecting user to see native UI popup"));
		}

		private void TestShowAchievements (Action<TestCaseResult> resultCallback)
		{
			socialService.ShowAchievements();
			resultCallback(TestCaseResult.Create(TestCaseResultCode.Inconclusive, "Expecting user to see native UI popup"));
		}
	}
}

