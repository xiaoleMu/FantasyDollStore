using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{

	public class ServerTime {
		
		//[Inject]
		//public GeneralParameterConfigModel _generalParameterConfigModel {get;set;}

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public ISyncService syncService { get; set; }

		[Inject]
		public GamePausedSignal gamePausedSignal { get; set; }

		[Inject]
		public GameQuitSignal gameQuitSignal { get; set; }

		private DateTime _serverTime;
		private TimeSpan _timeDiff;
		private int _newDayHour = 6;
		private DateTime _serverTimeNewDate;
		private bool _calculateTimeDiff = true;

		private const string Tag = "ServerTime";
		private const string TimeDiff = Tag + "_TimeDiff";

		private TimeSpan _skipTimeForDebug = TimeSpan.Zero;
		public TimeSpan SkipTimeForDebug
		{
			get { return _skipTimeForDebug; }
			set 
			{
				_skipTimeForDebug = value;
			}
		}

		private ConnectionHandler _connectionHandler;

		private Coroutine _loopRoutine = null;

		[PostConstruct]
		public void Init()
		{
			_serverTime = DateTime.Parse(TTPlayerPrefs.GetValue(Tag,_serverTime.ToString()));
			_timeDiff = TimeSpan.FromSeconds(TTPlayerPrefs.GetValue(TimeDiff, 0));

#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.Log(Tag,"Init - Set server time to: " + _serverTime + " Set time diff to: " + _timeDiff.TotalSeconds);
#else
			Debug.logger.Log(Tag,"Init - Set server time to: " + _serverTime + " Set time diff to: " + _timeDiff.TotalSeconds);
#endif

			ReCalculateTimeDiffFromServer();

			gamePausedSignal.AddListener(OnGamePausedOrQuit);
			gameQuitSignal.AddListener(OnGamePausedOrQuit);
		}

		private void OnGamePausedOrQuit()
		{
			int timeDiff = (int)_timeDiff.TotalSeconds;
#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.Log(Tag,"OnGamePaused - Saving Server Time " + _serverTime.ToString() + " ,time difference: " + timeDiff);
#else
			Debug.logger.Log(Tag,"OnGamePaused - Saving Server Time " + _serverTime.ToString() + " ,time difference: " + timeDiff);
#endif

			TTPlayerPrefs.SetValue(Tag,_serverTime.ToString());
			TTPlayerPrefs.SetValue(TimeDiff, timeDiff); 
			TTPlayerPrefs.Save();
		}

		public DateTime TruncateMillisecs(DateTime time)
		{
			return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
		}
		public DateTime TruncateTime(DateTime time)
		{
			return new DateTime(time.Year, time.Month, time.Day, 12, 0, 0);
		}

		public bool IsNewDay(string lastStoredDay)
		{
			DateTime storedDayStart = DateTime.Parse(lastStoredDay);
			DateTime localTime = GetLocalTime();
			DateTime todayStart = GetDateStart(localTime);

			//FIXME:
			_newDayHour = 6;//_generalParameterConfigModel.GetInt("DayStartHour", 6);

			TimeSpan diff = todayStart.Subtract(storedDayStart);
			if (diff.Days > 1)
				return true;
			if ( (diff.Days == 1) && (localTime.Hour > _newDayHour))
				return true;
			return false;
		}
		public string GetTodayStart()
		{
			return GetDateStart(GetLocalTime()).ToUniversalTime().ToString("o").Replace(".0000000","");;
		}

		public DateTime GetDateStart(DateTime date)
		{
			return (new DateTime(date.Year, date.Month, date.Day, _newDayHour, 0, 0));
		}

		public ServerTime()
		{
		}

		public void Set(string serverTimeStr)
		{
			_serverTime = DateTime.Parse(serverTimeStr);

			ReCalculateTimeDiffFromServer();
			Debug.Log ("ServerTime.Set: serverTime=" + _serverTime.ToString() + " timeDiff=" + _timeDiff.ToString());
		}
		public string GetAsServerString()
		{
			return GetLocalTime().ToUniversalTime().ToString("o").Replace(".0000000","");
		}
		public DateTime GetLocalTime()
		{
			return TruncateMillisecs(_serverTime.Add(_timeDiff).Add(_skipTimeForDebug));
		}

		public DateTime ToLocal(string serverTime)
		{
			return TruncateMillisecs(DateTime.Parse(serverTime));
		}

		public DateTime ToLocal(DateTime serverTime)
		{
			return TruncateMillisecs(serverTime);
		}

		public String ToServerString(DateTime localTime)
		{
			return localTime.Subtract(_timeDiff).ToUniversalTime().ToString("o").Replace(".0000000","");
		}
			
		private void ReCalculateTimeDiffFromServer()
		{
			if(_loopRoutine != null)
			{
				routineRunner.StopCoroutine(_loopRoutine);
			}

			_loopRoutine = routineRunner.StartCoroutine(CalculateTimeDiffCoro());
		}

		private IEnumerator CalculateTimeDiffCoro()
		{
			float dt = 1.0f;

			while(_calculateTimeDiff)
			{
				yield return new WaitForSeconds(dt);
				_timeDiff = _timeDiff.Add(TimeSpan.FromSeconds(dt));

				//Debug.Log ("*** Timediff: " + _timeDiff); // Uncomment for debugging
			}
		}

		public void Refresh()
		{
			syncService.GetServerTime();
		}

		public DateTime GetPersistedServerTime()
		{
			return new DateTime().AddSeconds(TTPlayerPrefs.GetValue(Tag, 0));
		}
	}

}
