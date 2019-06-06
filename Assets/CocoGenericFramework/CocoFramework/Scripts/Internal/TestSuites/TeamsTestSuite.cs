using System;

namespace TabTale
{
	public class TeamsTestSuite : TestSuite
	{
		[Inject]
		public RegionalService regionalService { get; set; }

		[Inject]
		public ITeamSyncService teamSyncService { get; set; }

		[PostConstruct]
		public void Init()
		{
			AddTest(TestGetCountryName, "Get Country");
			AddTest(TestGetTeamsByName, "Get Teams by name");
		}


		private void TestGetCountryName (Action<TestCaseResult> resultCallback)
		{
			regionalService.GetCountryName ()
				.Catch( e => resultCallback(TestCaseResult.Create(TestCaseResultCode.Failure, e.ToString())))
				.Done (name => resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Country name:" + name)));
		}

		private void TestGetTeamsByName(Action<TestCaseResult> resultCallback)
		{
			teamSyncService.GetTeamsByName ("I").Done (teams => {
				foreach (TeamData team in teams) {
					logger.Log (team.ToString ());
				}
			});
		}

		public void TestAddTeam (Action<TestCaseResult> resultCallback)
		{
			/*
			teamSyncService.AddTeam (teamName, new Color (10, 10, 10, 10), new Color (20, 20, 20, 20), "IL").Done (teamState => {
				logger.Log (teamState.ToLogString ());
			});
			*/
		}

		private void TestJoinTeam (Action<TestCaseResult> resultCallback, string teamId)
		{
			teamSyncService.JoinTeam (teamId).Done (teamState => {
				logger.Log (teamState.ToLogString ());
			});
		}

		private void TestGetTeam (Action<TestCaseResult> resultCallback, string teamId)
		{
			teamSyncService.GetTeamData (teamId).Done (teamInfo => {
				logger.Log(teamInfo);
			});
		}
	}
}

