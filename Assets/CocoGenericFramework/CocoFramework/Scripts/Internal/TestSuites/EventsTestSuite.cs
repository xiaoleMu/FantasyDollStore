using System;
using TabTale.Publishing;
using UnityEngine;
using System.Collections.Generic;

namespace TabTale
{
	public class EventsTestSuite : TestSuite
	{
		[Inject]
		public EventSystemService eventSystemService { get; set; }

		[Inject]
		public SocialStateModel socialStateModel { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[PostConstruct]
		public void Init()
		{
			// Preconditions:
			var noNetworkPrecondition = TestPrecondition.Create (networkCheck.HasInternetConnection, "No Network Connection");
			var hasAvailableEvent = TestPrecondition.Create(eventSystemService.HaveAvailableEvents,"No Available Events");
			var hasCurrentEvent = TestPrecondition.Create(eventSystemService.HaveCurrentEvents,"No Current Events");
			var playerHasAName = TestPrecondition.Create( () => ! socialStateModel.GetPlayerName().IsNullOrEmpty(),
				"Cannot register to an event without a defined player name");

			if(!socialStateModel.GetPlayerName().IsNullOrEmpty())
			{
				AddTest(TestSetPlayerNameForEvent, "Set Player Name", "");
			}

			AddTest(TestRegisterToEvent, "Register to Event", "", new List<TestPrecondition> { noNetworkPrecondition, hasAvailableEvent, playerHasAName });

			AddTest(TestGetTopLeaderboard, "Get top leaderboard", "", new List<TestPrecondition> { noNetworkPrecondition, hasCurrentEvent });

			AddTest(TestGetNeighborsLeaderboard, "Get neighbors leaderboard", "", new List<TestPrecondition> { noNetworkPrecondition, hasCurrentEvent });

			AddTest(TestUpdateEventScore, "Update Event Score", "", new List<TestPrecondition> { noNetworkPrecondition, hasCurrentEvent });
		}

		private void TestSetPlayerNameForEvent(Action<TestCaseResult> resultCallback)
		{
			string playerName = "TestUser" + UnityEngine.Random.Range(100, 1000).ToString();
			socialStateModel.SetPlayerName(playerName);

			resultCallback (TestCaseResult.Create (TestCaseResultCode.Success, "Set player name: " + playerName));
		}

		private void TestRegisterToEvent (Action<TestCaseResult> resultCallback)
		{
			eventSystemService.RegisterToEvent(eventSystemService.GetAvailableEvents()[0].id)
				.Catch(e => resultCallback (TestCaseResult.Create (TestCaseResultCode.Failure, e.Message)))
				.Done( ev => resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, ev.ToLogString())));
			
		}

		private void TestGetTopLeaderboard (Action<TestCaseResult> resultCallback)
		{
			eventSystemService.GetTopLeaderboard(eventSystemService.GetCurrentEvents()[0].id)
				.Catch(e => resultCallback (TestCaseResult.Create (TestCaseResultCode.Failure, e.Message)))
				.Done(leaderboard => {
					string result = String.Format("{0}\n{1}:{2}\n{3}:{4}","Got Top leaderboard:","Player Rank:",leaderboard.rank,"Leaderboard data:",leaderboard.leaderboardData.ArrayString());
					resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, result));
				});
		}

		private void TestGetNeighborsLeaderboard (Action<TestCaseResult> resultCallback)
		{
			eventSystemService.GetRankAndNeighbours(eventSystemService.GetCurrentEvents()[0].id)
				.Catch(e => resultCallback (TestCaseResult.Create (TestCaseResultCode.Failure, e.Message)))
				.Done(leaderboard => {
					string result = String.Format("{0}\n{1}:{2}\n{3}:{4}","Got Neighbors leaderboard:","Player Rank:",leaderboard.rank,"Leaderboard data:",leaderboard.leaderboardData.ArrayString());
					resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, result));
				});
		}

		private void TestUpdateEventScore (Action<TestCaseResult> resultCallback)
		{
			eventSystemService.UpdateEventScore(eventSystemService.GetCurrentEvents()[0].id, 10);
		}
	}
}

