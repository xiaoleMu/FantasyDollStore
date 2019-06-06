using UnityEngine;
using System;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace TabTale
{
	public class TickerObject 
	{
		public Coroutine routine = null;
		public string lastSystemTime;
		public bool stop;
		public float minutes;
		public float seconds;
		public float timeLeft;
		public string timerCounter;
	}

	public class StopwatchObject
	{
		public Stopwatch stopwatch = new Stopwatch();
		public string id;
	}

	public class TickerService
	{
		[Inject]
		public ServerTime serverTime { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public TimerLoopCompleteSignal onTimerLoopCompleteSignal { get; set; }

		[Inject]
		public EnergyStateModel energyStateModel { get; set; }

		[Inject]
		public RewardConfigModel rewardConfigModel { get; set; }

		[Inject]
		public RewardStateModel rewardStateModel { get; set; }

		[Inject]
		public GeneralParameterConfigModel generalParameterConfigModel { get; set; }

		[Inject]
		public EnergySystemService energySystemService { get; set; }
		[Inject]
		public EventSystemService eventSystemService { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		private Dictionary<string,TickerObject> _runningTickers = new Dictionary<string,TickerObject> ();
		private Dictionary<string,StopwatchObject> _runningStopwatches = new Dictionary<string,StopwatchObject> ();
		float maxCooldown;

		[PostConstruct]
		public void Init()
		{
			if(GsdkSettingsData.Instance.IsRewardSystemEnabled)
				StartRewardTimer ();



		}

		public string EnergyTimerID 
		{
			get { return "energyTimer"; }
		} 

		public string RewardTimerID 
		{
			get { return "rewardTimer"; }
		} 

		public void StartTimer(float from, string id, bool looping=false)
		{
			TickerObject ticker = new TickerObject ();

			if (ticker.routine == null) 
			{
				ticker.lastSystemTime = "0";
				ticker.routine = routineRunner.StartCoroutine (UpdateTimerCoro (ticker, from, id, looping));
			}

			_runningTickers.Add (id, ticker);
		}

		public void StopTimer(string id)
		{
			foreach (KeyValuePair<string,TickerObject> keyValue in _runningTickers)
			{
				if(keyValue.Key == id)
					keyValue.Value.stop = true;
			}
			_runningTickers.Remove (id);

		}

		public string TimerCounter (string id)
		{
			string _counter = "00:00";

			if (id == EnergyTimerID)
				_counter = "Full";
			if (id == RewardTimerID)
				_counter = " ";

			foreach (KeyValuePair<string,TickerObject> keyValue in _runningTickers)
			{
				if (id == keyValue.Key) 
				{
					_counter = keyValue.Value.timerCounter; 
					break;
				}
			}
			return _counter;
		}

		public string TimerCounterInSeconds (string id)
		{
			string _counter = "0";

			if (id == EnergyTimerID)
				_counter = "Full";
			if (id == RewardTimerID)
				_counter = " ";

			foreach (KeyValuePair<string,TickerObject> keyValue in _runningTickers)
			{
				if (id == keyValue.Key) 
				{
					int timerInSec = (int) keyValue.Value.timeLeft;
					_counter = timerInSec.ToString(); 
					break;
				}
			}
			return _counter;
		}

		private IEnumerator UpdateTimerCoro(TickerObject ticker, float from, string id, bool looping)
		{
			ticker.timeLeft = from;

			while (!ticker.stop)
			{
				DateTime systemTime;
				if(networkCheck.HasInternetConnection())
					systemTime = serverTime.GetLocalTime();
				else
					systemTime = DateTime.Now;

				if (ticker.lastSystemTime == "0") 
				{		
					string time = systemTime.ToString();
					ticker.lastSystemTime = time;
				} 
				else 
				{	
					DateTime lastSysT = DateTime.Parse (ticker.lastSystemTime);	
					TimeSpan timediff = systemTime - lastSysT;
					ticker.timeLeft -= (float)timediff.TotalSeconds;
					ticker.lastSystemTime = systemTime.ToString();
				}

				if (ticker.timeLeft <= 0) 
				{

					if (looping) 
					{
						ticker.timeLeft = from;
					} 
					else 
					{
						ticker.stop = true;
					}

					onTimerLoopCompleteSignal.Dispatch (id);
				}

				ticker.minutes = Mathf.Floor(ticker.timeLeft / 60);
				ticker.seconds = Mathf.Ceil(ticker.timeLeft % 60);

				if (ticker.seconds == 60f) 
				{
					ticker.minutes += 1;
					ticker.seconds = 0;
				}

				ticker.timerCounter = string.Format("{0:0}:{1:00}", ticker.minutes, ticker.seconds);

				yield return new WaitForSeconds(1f);
			}

			routineRunner.StopCoroutine (ticker.routine);

			ticker.routine = null;
			_runningTickers.Remove (id);
		}

		public void StopAllTimers()
		{
			foreach (KeyValuePair<string,TickerObject> keyValue in _runningTickers)
			{
				keyValue.Value.stop = true;
			}
			_runningTickers.Clear ();
		}

		public void StartStopwatch(string _id)
		{
			StopwatchObject timer = new StopwatchObject();
			timer.id = _id;
			timer.stopwatch.Start ();
			_runningStopwatches.Add (_id, timer);
		}

		public string StopwatchCounter(string _id)
		{
			string _counter="-";

			foreach (KeyValuePair<string,StopwatchObject> keyValue in _runningStopwatches)
			{
				if (_id == keyValue.Key) 
				{
					_counter = keyValue.Value.stopwatch.Elapsed.TotalSeconds.ToString ("0.0");
				}
			}
			return _counter;
		}

		public void StopStopwatch(string _id)
		{
			foreach (KeyValuePair<string,StopwatchObject> keyValue in _runningStopwatches)
			{
				if (_id == keyValue.Key) 
				{
					keyValue.Value.stopwatch.Stop ();
					break;
				}
			}
			_runningStopwatches.Remove (_id);
		}

		public bool IsTimerRunning(string id = "any")
		{
			if (id == "any") 
			{
				if (_runningTickers.Count > 0)
					return true;
				else
					return false;
			} 
			else 
			{
				bool exists = _runningTickers.Any(b => (b.Value != null && b.Key == id));
				if (exists)
					return true;
				else
					return false;
			}
		}

		public bool IsStopwatchRunning(string id = "any")
		{
			if (id == "any") 
			{
				if (_runningStopwatches.Count > 0)
					return true;
				else
					return false;
			} 
			else 
			{
				bool exists = _runningStopwatches.Any(b => (b.Value != null && b.Key == id));
				if (exists)
					return true;
				else
					return false;
			}
		}

		public void StartRewardTimer()
		{
			if (GsdkSettingsData.Instance.IsRewardSystemEnabled) 
			{
				logger.Log ("<color=yellow>RewardTimer</color>", "Init");

				if (RewardTimerCooldown () > 0) 
				{
					TickerObject ticker = new TickerObject ();
					if (ticker.routine == null) {
						ticker.lastSystemTime = "0";
						ticker.routine = routineRunner.StartCoroutine (UpdateTimerCoro (ticker, RewardTimerCooldown (), RewardTimerID, false));
					}

					_runningTickers.Add (RewardTimerID, ticker);
				} 
			}
		}

		public void StartEventTimer(string eventID)
		{

			logger.Log ("<color=blue>EventTimer </color>", "Start");
			float timeTillEnd = EventTimerCooldown (eventID);

			if (timeTillEnd> 0) 
			{
				TickerObject ticker = new TickerObject ();
				if (ticker.routine == null) 
				{
					ticker.lastSystemTime = "0";
					ticker.routine = routineRunner.StartCoroutine ( UpdateTimerCoro (ticker, timeTillEnd, eventID, false));
				}

				_runningTickers.Add (eventID, ticker);
			} 
		}

		public float RewardTimerCooldown()
		{
			string lastRewardDate = rewardStateModel.GetLastRewardedDate ("gifts");
			DateTime lastReward = serverTime.ToLocal(lastRewardDate);
			DateTime now = serverTime.GetLocalTime ();
			TimeSpan difference = now - lastReward;
			logger.Log ("<color=yelow>RewardTimer: Last rewarded date </color> ", lastReward.ToString());
			logger.Log ("<color=yelow>RewardTimer: Reward cooldown </color> ", GetRewardDelayInSeconds ().ToString());

			float _difference = (float)difference.TotalSeconds;
			logger.Log ("<color=yelow>RewardTimer: Difference in time </color>", _difference.ToString());


			if (difference > TimeSpan.Zero && _difference < GetRewardDelayInSeconds ()) 
			{
				float newTimer = GetRewardDelayInSeconds () - _difference;
				logger.Log ("<color=yelow>RewardTimer: New time </color>", newTimer.ToString());
				return newTimer;
			} 
			else 
			{
				logger.Log("<color=yelow> RewardTimer: </color>", "Timer expired");

				return 0;
			}
		}

		public float EventTimerCooldown(string eventID)
		{
			DateTime endDate = eventSystemService.eventConfigModel.GetEventEndDate (eventID);
			DateTime endEvent = serverTime.ToLocal (endDate);
			DateTime now = serverTime.GetLocalTime ();
			TimeSpan difference = endEvent - now;
			logger.Log ("<color=blue>EventTimer: </color> End Event Date", endEvent.ToString());

			float _difference = (float)difference.TotalSeconds;
			logger.Log ("<color=blue>EventTimer: </color> Difference in time", _difference.ToString());

			if (difference > TimeSpan.Zero) 
			{
				logger.Log ("<color=blue>EventTimer: </color> New time", _difference.ToString());
				return _difference;
			} 
			else 
			{
				logger.Log ("<color=blue>EventTimer: </color>", "Event "+ eventID + " expired!");

				return 0;
			}
		}

		public bool RewardTimerExpired()
		{
			string lastRewardDate = rewardStateModel.GetLastRewardedDate ("gifts");
			DateTime lastReward = serverTime.ToLocal(lastRewardDate);
			DateTime now = serverTime.GetLocalTime ();
			TimeSpan difference = now - lastReward;
			float _difference = (float)difference.TotalSeconds;

			if (difference > TimeSpan.Zero && _difference < GetRewardDelayInSeconds ()) 
			{
				return false;
			}
			else 
			{
				return true;
			}
		}

		private float GetFloat(string stringValue)
		{
			float result = 0;
			float.TryParse(stringValue, out result);
			return result;
		}

		public float GetRewardDelayInSeconds()
		{
			RewardItemConfigData nextReward = rewardStateModel.NextReward("gifts");
			List<int> delays = rewardConfigModel.GetDelays("gifts");

			int delay = delays[nextReward.index % delays.Count];
			float seconds = 1;
			switch (rewardConfigModel.GetDelayType("gifts")) 
			{
			case DelayType.Days:
				int hours = delay * 24;
				seconds = (hours * 60) * 60;
				break;

			case DelayType.Hours:
				seconds = (delay * 60) * 60;
				break;

			case DelayType.Minutes:
				seconds = (delay * 60);
				break;
			}
			return seconds;
		}

		public string TimeInDays(string id)
		{
			string time = TimerCounter(id);
			string[] hours = time.Split(':');
			float _hours = float.Parse(hours[0]);
			_hours /= 60;

			if(_hours < 24)
			{
				return time;
			}
			else 
			{
				int days = (int)_hours/24;
				return days.ToString() + "D left";
			}
		}
	}
}