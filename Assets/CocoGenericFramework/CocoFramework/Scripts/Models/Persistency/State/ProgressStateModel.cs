using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace TabTale
{
	public enum ProgressType
	{
		maxLevel,
		rank,
		xp,
		topScore
	}

	public partial class ProgressStateModel : StateModel<ProgressStateData>
	{
		[Inject]
		public RankConfigModel rankConfigModel { get; set; }

		[Inject]
		public RankUpSignal rankUpSignal { get; set; }

		[Inject]
		public TopScoreSignal topScoreSignal { get; set; }

		[Inject]
		public FirstWinInLevelSignal firstWinInLevelSignal { get; set; }

		[Inject]
		public SocialProgressUpdatedSignal socialProgressUpdatedSignal { get; set; }

		[Inject]
		public RestrictionVerifier restrictionVerifier { get; set; }

		#region Max Level

		public int GetMaxLevel()
		{
			return _data.maxLevel;
		}

		// Here the parameter is integer, since reporting the progress in levels is only relevant for indexed levels
		public bool ReportLevel(int level)
		{
			if(level > _data.maxLevel)
			{
				_data.maxLevel = level;
				firstWinInLevelSignal.Dispatch(_data.maxLevel);
				return Save ();
			}

			return false;
		}

		public bool ResetMaxLevel()
		{
			_data.maxLevel = 0;
			return Save ();
		}

		#endregion

		#region Rank

		public int GetRank()
		{
			return _data.rankIndex;
		}

		public bool ResetRank()
		{
			RankConfigData rankData = rankConfigModel.GetRankForXp(0);
			if(rankData != null)
				_data.rankIndex = rankData.index;
			return Save ();
		}

		private bool UpdateRank()
		{
			RankConfigData rankData = rankConfigModel.GetRankForXp(_data.xp);
			
			// Check if reached a new rank
			if(rankData != null && _data.rankIndex < rankData.index)
			{
				_data.rankIndex = rankData.index;
				rankUpSignal.Dispatch(rankData.index);
				return Save ();
			}
			
			return false;
		}

		#endregion

		#region xp

		public int GetXp()
		{
			return _data.xp;
		}

		public bool IncreaseXp(int deltaXp)
		{
			_data.xp += deltaXp;
			UpdateRank();
			return Save ();
		}

		public bool ResetXp()
		{
			_data.xp = 0;
			return Save ();
		}

		#endregion

		#region Top Score

		public int GetTopScore()
		{
			return _data.topScore;
		}

		public bool ReportScore(int score, bool notifyOnTopScore = true)
		{
			if(score > _data.topScore)
			{
				_data.topScore = score;

				Save ();

				if(notifyOnTopScore)
				{
					topScoreSignal.Dispatch(score);	
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ResetTopScore()
		{
			_data.topScore = 0;
			return Save ();
		}

		#endregion

		#region Generic Progress elements

		public GameElementData GetProgress(string key)
		{
			int index = _data.progress.GetIndex(key);
			if (  index == -1)
			{
				return null;
			}
			return _data.progress[index].Clone() as GameElementData;
		}

		public int GetProgressValue(string key)
		{
			GameElementData data = GetProgress(key);
			return (null != data) ? data.value : 0;

		}

		public bool IncreaseProgress (GameElementData progress)
		{
			_data.progress.Increase(progress);
			return Save();
		}
		
		public bool IncreaseProgress (string key, int value)
		{
			return IncreaseProgress (GameElementData.CreateState(key,value));
		}

		public void SetProgress(string key, int value = 0, GameElementType type = default(GameElementType))
		{
			GameElementData newData = null;

			int index = _data.progress.GetIndex(key);
			if (index == -1)
			{
				newData = new GameElementData(type, key, value);
				_data.progress.Add(newData);
				Save();
			}
			else
			{
				newData = _data.progress[index];
				newData.value = value;
				Save();
			}
		}

		public void SetProgress(GameElementData data)
		{
			int index = _data.progress.GetIndex(data.key);
			if (index == -1)
			{
				_data.progress.Add(data);
				Save();
			}
			else
			{
				_data.progress[index].type = data.type;
				_data.progress[index].value = data.value;
				Save();
			}
		}


		public bool ResetProgress (string key, int value = 0)
		{
			int index = _data.progress.GetIndex(key);
			if (index == -1)
				return false;

			_data.progress[index] = GameElementData.CreateState(key,value);

			return Save();
		}

		#endregion

		#region Restrictions

		public bool VerifyRestriction(GameElementData restriction)
		{
			if (restriction.type != GameElementType.State) 
			{
				return false;
			}

			if(restriction.key == ProgressType.maxLevel.ToString())
				return _data.maxLevel >= restriction.value;
			else if(restriction.key == ProgressType.rank.ToString())
				return _data.rankIndex >= restriction.value;
			else if(restriction.key == ProgressType.xp.ToString())
				return _data.xp >= restriction.value;
			else if(restriction.key == ProgressType.topScore.ToString())
				return _data.maxLevel >= restriction.value;

			int index = _data.progress.GetIndex(restriction);
			if (index == -1) 
			{
				return false;
			}
			return _data.progress[index].value >= restriction.value;
				
		}

		#endregion

		protected override void PerformAfterSync()
		{
			socialProgressUpdatedSignal.Dispatch();
		}

	}

	public class RankUpSignal : Signal<int> { }
	public class TopScoreSignal : Signal<int> { }
}
