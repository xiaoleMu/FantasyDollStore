using UnityEngine;
using System.Collections.Generic;
using System;
using LitJson;

namespace CocoPlay
{
	public interface ICocoTimerData
	{
		bool IsTiming { get; }

		DateTime TargetTime { get; }

		TimeSpan IntervalTime { get; }

		bool IsTargetReached { get; }

		int FinishCount { get; }

		void ChangeInterval(TimeSpan pInterval, bool pIsChangeTagetTime = false);

		void ChangeNotificationText(string pNotificationText);
	}

	public class CocoTimerData : ICocoTimerData
	{
		public CocoTimerData ()
		{
			IsTiming = false;
			FinishCount = 0;
		}

		public CocoTimerData (TimeSpan interval, string notificationText) : base ()
		{
			IntervalTime = interval;
			NotificationText = notificationText;
		}

		public bool IsTiming { get; private set; }

		public DateTime TargetTime { get; private set; }

		[JsonIgnore]
		public TimeSpan IntervalTime { get; private set; }

		/// <summary>
		/// 修改计时器的间隔
		/// </summary>
		/// <param name="pInterval">间隔</param>
		/// <param name="pIsChangeTagetTime">如果计时器在运行，是否需要修改目标时间</param>
		public void ChangeInterval(TimeSpan pInterval,bool pIsChangeTagetTime = false)
		{
			if (pIsChangeTagetTime && IsTiming)
				TargetTime = TargetTime - IntervalTime + pInterval;

			IntervalTime = pInterval;
		}

		/// <summary>
		/// 修改通知文本
		/// </summary>
		/// <param name="pNotificationText">通知文本</param>
		public void ChangeNotificationText(string pNotificationText)
		{
			NotificationText = pNotificationText;
		}

		public int FinishCount { get; private set; }

		public string NotificationText { get; private set; }

		public string IntervalString {
			get {
				return IntervalTime.ToString ();
			}
			set {
				TimeSpan interval;
				if (TimeSpan.TryParse (value, out interval)) {
					IntervalTime = interval;
				}
			}
		}

		public bool StartTimer ()
		{
			IsTiming = true;
			TargetTime = DateTime.Now + IntervalTime;
			return true;
		}

		[JsonIgnore]
		public bool IsTargetReached {
			get {
				if (!IsTiming) {
					return true;
				}

				return DateTime.Now >= TargetTime;
			}
		}

		public void FinishTimer ()
		{
			IsTiming = false;
			TargetTime = DateTime.Now;
			FinishCount++;
		}

		public void ReachTargetNow ()
		{
			TargetTime = DateTime.Now;
		}

		public void ChangeTarget(DateTime pDateTime)
		{
			TargetTime = pDateTime;
		}
		
		public void ChangeTarget(TimeSpan pTimeSpan)
		{
			TargetTime = TargetTime + pTimeSpan;
		}

		public void ResetFinishCount ()
		{
			FinishCount = 0;
		}

		public CocoTimerData Clone ()
		{
			CocoTimerData clone = new CocoTimerData ();

			clone.IsTiming = IsTiming;
			clone.IntervalString = IntervalString;
			clone.TargetTime = TargetTime;

			return clone;
		}


	}
}
