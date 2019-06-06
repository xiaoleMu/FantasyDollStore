using System;

namespace TabTale
{
	public class RefreshTeamStateCommand : GameCommand
	{
		[Inject]
		public ITeamSyncService teamSyncService { get; set; }

		[Inject]
		public TeamStateModel teamStateModel { get; set; }

		[Inject]
		public MatchStateModel matchStateModel { get; set; }

		public override void Execute ()
		{
			logger.Log ("RefreshTeamScoreCommand.Execute");

			if(teamStateModel.PlayerHasATeam() && matchStateModel.GetScore() > 0)
			{
				teamSyncService.UpdateTeamScore(matchStateModel.GetScore())
					.Catch(e => logger.Log(Tag,"Team state score update failed"))
					.Done(teamScore => {

						logger.Log(Tag,"Team state score updated successfully :" + teamScore);

					});
			}
		}



	}
}

