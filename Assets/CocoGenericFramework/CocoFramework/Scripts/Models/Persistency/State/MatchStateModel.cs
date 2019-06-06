using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TabTale.Analytics;

namespace TabTale
{
	public class MatchStateModel : StateModel<MatchStateData>
	{
		#region Outgoing Signals
		
		[Inject]
		public MatchStartSignal matchStartSignal { get; set; }
		
		[Inject]
		public MatchEndSignal matchEndSignal { get; set; }

		[Inject]
		public MatchScoreUpdatedSignal matchScoreUpdateSignal {get;set;}
		
		#endregion

		#region Injections

		[Inject]
		public ProgressStateModel progressStateModel { get; set; }

		[Inject]
		public CurrencyStateModel currencyStateModel { get; set; }

		[Inject]
		public InventoryStateModel inventoryStateModel { get; set; }

		[Inject]
		public SocialStateModel socialStateModel { get; set; }

		[Inject]
		public LevelStateModel levelStateModel { get; set; }

		[Inject]
		public IAnalyticsService analyticsService { get; set; }

		[Inject]
		public ISyncService syncService {get;set;}
		#endregion

		private System.Random _randGenerator;

		private System.Enum _leadingProgressType = null;

		private bool _syncDataOnMatchEnd = true;
		public bool SyncDataOnMatchEnd
		{
			get { return _syncDataOnMatchEnd; }
			set { _syncDataOnMatchEnd = value; }
		}

		#region Getters

		public PlayerMatchData Player
		{
			get 
			{
				if(_data.playersMatchData.Count == 0)
					_data.playersMatchData.Add(new PlayerMatchData());

				return _data.playersMatchData.FirstOrDefault( player => player.playerType == PlayerType.Local);
			}
		}

		public MatchStatus MatchStatus
		{
			get 
			{
				return Player.matchStatus;
			}
		}
		
		#endregion

		#region Configuration

		public virtual void SetLeadingProgressType(System.Enum progressType)
		{
			_leadingProgressType = progressType;
		}

		#endregion

		#region Match Life Cycle

		/// <summary>
		/// Starts the match with no concept of level
		/// </summary>
		public virtual void StartMatch()
		{
			StartMatchWithSeed(-1);
		}

		/// <summary>
		/// Starts the match in a specific level
		/// </summary>
		public virtual void StartMatchInLevel(int levelConfigId)
		{
			StartMatchInLevel(levelConfigId.ToString());
		}

		public virtual void StartMatchInLevel(string levelConfigId)
		{
			StartMatchInLevelWithSeed(-1, levelConfigId);
		}

		/// <summary>
		/// Starts the match with a given seed and optional level name
		/// </summary>
		public virtual void StartMatchWithSeed(int seed)
		{
			StartMatchInLevelWithSeed(seed, "-1");
		}

		/// <summary>
		/// Starts the match in a specific level with a specific seed
		/// </summary>
		protected virtual void StartMatchInLevelWithSeed(int seed, string levelConfigId)
		{
			if(seed == -1)
			{
				seed = new System.Random().Next();
			}

			if(_data == null)
				Debug.LogError("Cannot Start Match - Match data is empty, this means serialisation failed. Check the data in the DB");

			if(Player.matchStatus == MatchStatus.Playing)
			{
				Debug.LogError(string.Format(
					"MatchStateModel: Starting a match when the current match status is {0}, " +
					"StartMatch was called multiple times without calling EndMatch", MatchStatus.Playing));
			}

			InitMatchData(levelConfigId);
			SetMatchSeed(seed);

			matchStartSignal.Dispatch(_data.Clone() as MatchStateData);
		}

		private void InitMatchData(string levelConfigId)
		{	
			_data.playersMatchData.Clear();

			PlayerMatchData playerMatchData = new PlayerMatchData();
			playerMatchData.itemsInUse = inventoryStateModel.GetItemsInUse();
			playerMatchData.playerProfile = socialStateModel.GetState();
			playerMatchData.matchStatus = MatchStatus.Playing;
			_data.playersMatchData.Add(playerMatchData);

			_data.levelMatchData = new LevelMatchData();
			if(levelConfigId != "-1")
				_data.levelMatchData.levelConfigId = levelConfigId;
		}

		public virtual void EndMatch(MatchStatus matchStatus)
		{
			if (!(Player.matchStatus == MatchStatus.Playing || Player.matchStatus == MatchStatus.Pause)) 
			{
				Debug.LogError("Tried to end a match when status isnt Playing or Paused, make sure a match has been started.");
				return;
			}
			Player.matchStatus = matchStatus;

			ReportLevelSummaryToAnalytics();
			UpdatePlayerProgress();
			UpdateLevelState();
			matchEndSignal.Dispatch();

			if(_syncDataOnMatchEnd)
			{
				syncService.SyncData();
			}

			Save();
		}

		#endregion

		#region Analytics

		private void ReportLevelSummaryToAnalytics()
		{
			string level = _data.levelMatchData.levelConfigId;
			int levelNum;
			int.TryParse(level, out levelNum);

			bool won = Player.matchStatus == MatchStatus.Won;
			int score = Player.score;

			// Check if there already any score for the level to determine if played already played it
			bool firstTime = (levelStateModel.GetLevel(level).score == 0); 

			// If we won and the level is higher than the max level reached by the player then this is the players first win:
			bool firstWin = won && (levelNum > progressStateModel.GetMaxLevel());

			analyticsService.LevelReport(level, won, firstTime, firstWin, score);
		}

		#endregion

		#region Progress

		private void UpdatePlayerProgress()
		{
			// Report the current level (only relevant for indexed levels)
			int currentLevel;
			if(int.TryParse(_data.levelMatchData.levelConfigId, out currentLevel))
			{
				if(Player.matchStatus == MatchStatus.Won)
				{
					progressStateModel.ReportLevel(currentLevel);
				}
			}

			progressStateModel.ReportScore(Player.score);

			progressStateModel.IncreaseXp(Player.xpGained);

			foreach(GameElementData matchProgressData in Player.progress)
			{
				progressStateModel.IncreaseProgress(matchProgressData);
			}
		}

		public GameElementData GetProgress(System.Enum key)
		{
			int index = Player.progress.FirstIndex(p => p.key == key.ToString());
			if (  index == -1)
			{
				return GameElementData.CreateState(key.ToString(),0);
			}
			return Player.progress[index].Clone() as GameElementData;
		}

		public bool IncreaseProgress(System.Enum key, int value =1)
		{
			GameElementData progress = GetProgress(key);
			return SetProgress(key, progress.value + value);
		}

		/// <summary>
		/// Sets the progress.
		/// </summary>
		/// <param name="setOnlyIfHigher">If set to <c>true</c> will assign only if the value is higher.</param>
		public bool SetProgress(System.Enum key, int value, bool setOnlyIfHigher = false)
		{
			int index = Player.progress.FirstIndex(p => p.key == key.ToString());
			if (index == -1)
			{
				Player.progress.Add(GameElementData.CreateState(key.ToString(),value));
				return true;
			}
			else
			{
				if(value >= Player.progress[index].value || !setOnlyIfHigher)
				{
					Player.progress[index] = GameElementData.CreateState(key.ToString(),value);
					return true;
				}
			}
			return false;
		}

		#endregion

		#region Level Summary

		/// <summary>
		/// Updates level summary when the player achieved a new top score for a specific level
		/// </summary>
		private void UpdateLevelState()
		{
			string level = GetLevel();

			// Will not update state in case there is no concept of level
			if(string.IsNullOrEmpty(level))
				return;

			LevelStateData levelSummary = levelStateModel.GetLevel(level);

			bool shouldUpdatePlayerProgress = false;

			bool hasLeadingProgressType = (_leadingProgressType != null);
			if(hasLeadingProgressType)
			{
				int curProgress = GetProgress(_leadingProgressType).value;
				int savedProgress = levelStateModel.GetLevelProgress(GetLevel(),_leadingProgressType).value;

				if(curProgress == savedProgress)
					shouldUpdatePlayerProgress = Player.score >= levelSummary.score;
				if(curProgress > savedProgress)
						shouldUpdatePlayerProgress = true;

			}
			else
			{
				shouldUpdatePlayerProgress = Player.score >= levelSummary.score;
			}

			if(shouldUpdatePlayerProgress)
			{
				levelSummary.id = level;
				levelSummary.score = Player.score;
				levelSummary.xpGained = Player.xpGained;
				levelSummary.itemsInUse = Player.itemsInUse;
				levelSummary.status = Player.matchStatus;
				levelSummary.progress = Player.progress.Select(p => p.Clone() as GameElementData).ToList();
				levelSummary.properties = Player.properties.Select(p => p.Clone() as GameElementData).ToList();
				levelSummary.currenciesEarned = Player.currenciesEarned.Select(c => c.Clone() as GameElementData).ToList();

				levelStateModel.SetLevel(levelSummary);
			}

		}
		#endregion

		#region Match Level Data

		public LevelMatchData GetLevelData()
		{
			return _data.levelMatchData.Clone() as LevelMatchData;
		}

		public string GetLevel()
		{
			return _data.levelMatchData.levelConfigId;
		}
		
		#endregion

		#region Player Match Data

		public PlayerMatchData GetPlayer(string profileId)
		{
			return _data.playersMatchData.FirstOrDefault(player => player.playerProfile.playerId == profileId).Clone() as PlayerMatchData;
		}

		public IEnumerable GetPlayers()
		{
			return _data.playersMatchData.Clone();
		}

		public bool SetPlayer(PlayerMatchData player)
		{
			for (int i = 0; i < _data.playersMatchData.Count; i++) 
			{
				if (_data.playersMatchData[i].playerProfile == player.playerProfile) 
				{
					_data.playersMatchData[i] = player;
					return true;
				}
			}

			return false;
		}

		#endregion

		#region Score

		public bool IncreaseScore(int score)
		{

			return SetScore (Player.score + score);
		}


		public bool SetScore(int score)
		{
			if(score != Player.score)
				matchScoreUpdateSignal.Dispatch(Player.score);

			Player.score = score;
			return true;
		}

		public int GetScore(string profileId = null)
		{
			if (string.IsNullOrEmpty(profileId)) 
			{
				return Player.score;
			}
			return GetPlayer(profileId).score;
		}

		#endregion

		#region Xp

		public bool IncreaseXp(int deltaXp)
		{
			Player.xpGained += deltaXp;
			return true;
		}

		#endregion

		#region Currency

		public bool IncreaseCurrencyEarned(System.Enum key, int value)
		{
			GameElementData currencyEarned = GameElementData.CreateCurrency(key,value);
			currencyStateModel.IncreaseCurrency(currencyEarned);
			Player.currenciesEarned.Increase(currencyEarned);

			return true;
	
		}

		#endregion

		#region Random Generator & Seed

		public int MatchSeed()
		{
			return _data.levelMatchData.seed;
		}
		
		public bool SetMatchSeed(int seed)
		{
			_data.levelMatchData.seed = seed;
			
			_randGenerator = new System.Random(seed);
			
			return true;
		}

		public float GetRandValue()
		{
			return (float)_randGenerator.NextDouble();
		}
		
		public int GetRandValue(int min, int max)
		{
			return Mathf.FloorToInt((float)(min + _randGenerator.NextDouble() * (max - min)));
		}
		
		public float GetRandValue(float min, float max)
		{
			return (float)(min + _randGenerator.NextDouble() * (max - min));
		}

		#endregion
	}
}
