using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class TeamStateModel : StateModel<TeamStateData>
	{
		[Inject]
		public TeamStateUpdatedSignal teamStateUpdatedSignal { get; set; }

		public bool PlayerHasATeam ()
		{
			return ! GetTeamId ().IsNullOrEmpty();
		}

		public string GetTeamId ()
		{
			return _data.teamInfo.id;
		}

		public int GetScore ()
		{
			return _data.score;
		}

		public string GetTeamCountry ()
		{
			return _data.teamInfo.countryId;
		}

		public string GetTeamName ()
		{
			return _data.teamInfo.name;
		}

		public void SetPlayerName(string name)
		{
			_data.playerName = name;
			Save();
		}
		public void SetScore (int score)
		{
			if (score > _data.score) {
				_data.score = score;
				Save ();
			}
		}

		public string GetPlayerName ()
		{
			return this._data.playerName;
		}

		public TeamStateData GetState ()
		{
			return this._data;
		}

		public void SetState (TeamStateData teamStateData)
		{
			_data = teamStateData;
			Save ();

			teamStateUpdatedSignal.Dispatch ();
		}

		protected override void PerformAfterSync()
		{
			teamStateUpdatedSignal.Dispatch ();
		}
	}

}
