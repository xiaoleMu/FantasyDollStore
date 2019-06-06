using UnityEngine;
using TabTale;

namespace CocoPlay
{
	public abstract class CocoNumericalLevelConfigData
	{
		public CocoNumericalLevelConfigData (int startValue, bool valueRising, int startLevel, int fullLevel)
		{
			StartValue = startValue;
			ValueIsRising = valueRising;
			StartLevel = startLevel;
			FullLevel = fullLevel;
		}

		protected int StartValue { get; private set; }

		protected bool ValueIsRising { get; private set; }

		protected int StartLevel { get; private set; }

		protected int FullLevel { get; private set; }

		private bool LevelIsAscending {
			get {
				return StartLevel < FullLevel;
			}
		}

		public bool LevelIsFull (int val)
		{
			Pair<int, int> levelData = GetLevelData (FullLevel);

			if (ValueIsRising) {
				return val >= levelData.First + levelData.Second;
			}

			return val <= levelData.First - levelData.Second;
		}

		public bool LevelIsEmpty (int val)
		{
			Pair<int, int> levelData = GetLevelData (StartLevel);

			if (ValueIsRising) {
				return val < levelData.First;
			}

			return val > levelData.First;
		}

		public Pair<int, int> GetLevelData (int level)
		{
			int levelIndex;

			if (LevelIsAscending) {
				levelIndex = Mathf.Clamp (level, StartLevel, FullLevel) - StartLevel;
			} else {
				levelIndex = StartLevel - Mathf.Clamp (level, FullLevel, StartLevel);
			}

			return GetLevelDataByIndex (levelIndex);
		}

		public int GetLevelByValue (int val)
		{
			int levelIndex = GetLevelIndexByValue (val);

			if (LevelIsAscending) {
				return Mathf.Clamp (StartLevel + levelIndex, StartLevel, FullLevel);
			} else {
				return Mathf.Clamp (StartLevel - levelIndex, FullLevel, StartLevel);
			}
		}

		public float GetLevelProgress (int val, out int level)
		{
			level = GetLevelByValue (val);
			Pair<int, int> levelData = GetLevelData (level);
			return Mathf.Clamp01 ((val - levelData.First) / (float)levelData.Second);
		}

		protected abstract Pair<int, int> GetLevelDataByIndex (int levelIndex);

		protected abstract int GetLevelIndexByValue (int val);

	}

	public class CocoNumericalLevelFixedIntervalConfigData : CocoNumericalLevelConfigData
	{
		public CocoNumericalLevelFixedIntervalConfigData (int interval, int startValue, bool valueRising = true, int startLevel = 0, int fullLevel = int.MaxValue)
			: base (startValue, valueRising, startLevel, fullLevel)
		{
			Interval = interval;
		}

		private int Interval { get; set; }

		#region implemented abstract members of CocoNumericalLevelInterval

		protected override int GetLevelIndexByValue (int val)
		{
			if (ValueIsRising) {
				return (val - StartValue) / Interval;
			}

			return (StartValue - val) / Interval;
		}

		protected override Pair<int, int> GetLevelDataByIndex (int levelIndex)
		{
			if (ValueIsRising) {
				return new Pair<int, int> (StartValue + Interval * levelIndex, Interval);
			}

			return new Pair<int, int> (StartValue - Interval * levelIndex, Interval);
		}

		#endregion
	}

	public class CocoNumericalLevelNonFixedIntervalConfigData : CocoNumericalLevelConfigData
	{
		public CocoNumericalLevelNonFixedIntervalConfigData (int[] intervals, int startValue, bool valueRising = true, int startLevel = 0, int fullLevel = int.MaxValue)
			: base (startValue, valueRising, startLevel, fullLevel)
		{
			LevelIntervals = intervals != null ? intervals : new int[0];
			LevelStartValues = new int[LevelIntervals.Length];

			int levelStartValue = startValue;
			for (int i = 0; i < LevelIntervals.Length; i++) {
				LevelStartValues [i] = levelStartValue;
				if (ValueIsRising) {
					levelStartValue += LevelIntervals [i];
				} else {
					levelStartValue -= LevelIntervals [i];
				}
			}
		}

		private int[] LevelIntervals { get; set; }

		private int[] LevelStartValues { get; set; }

		#region implemented abstract members of CocoNumericalLevelInterval

		protected override Pair<int, int> GetLevelDataByIndex (int levelIndex)
		{
			Pair<int, int> data = new Pair<int, int> ();
			if (LevelIntervals.Length <= 0) {
				return data;
			}

			if (levelIndex < 0) {
				if (ValueIsRising) {
					data.First = LevelStartValues [0] + LevelIntervals [0] * levelIndex;
				} else {
					data.First = LevelStartValues [0] - LevelIntervals [0] * levelIndex;
				}
				data.Second = LevelIntervals [0];
			} else if (levelIndex >= LevelIntervals.Length) {
				int start = LevelStartValues [LevelIntervals.Length - 1] + LevelIntervals [LevelIntervals.Length - 1];
				int interval = LevelIntervals [LevelIntervals.Length - 1];
				if (ValueIsRising) {
					data.First = start + interval * (levelIndex - LevelIntervals.Length);
				} else {
					data.First = start - interval * (levelIndex - LevelIntervals.Length);
				}
				data.Second = LevelIntervals [LevelIntervals.Length - 1];
			} else {
				data.First = LevelStartValues [levelIndex];
				data.Second = LevelIntervals [levelIndex];
			}
			return data;
		}

		protected override int GetLevelIndexByValue (int val)
		{
			if (LevelIntervals.Length <= 0) {
				return 0;
			}

			int targetValue = 0;

			if (ValueIsRising) {
				if (val < StartValue) {
					return (val - StartValue - LevelIntervals [0] + 1) / LevelIntervals [0];
				}

				for (int i = 0; i < LevelIntervals.Length; i++) {
					targetValue = LevelStartValues [i] + LevelIntervals [i];
					if (val < targetValue) {
						return i;
					}
				}

				return (val - targetValue) / LevelIntervals [LevelIntervals.Length - 1] + LevelIntervals.Length;
			}

			if (val > StartValue) {
				return (StartValue - val - LevelIntervals [0] + 1) / LevelIntervals [0];
			}

			for (int i = 0; i < LevelIntervals.Length; i++) {
				targetValue = LevelStartValues [i] - LevelIntervals [i];
				if (val > targetValue) {
					return i;
				}
			}

			return (targetValue - val) / LevelIntervals [LevelIntervals.Length - 1] + LevelIntervals.Length;
		}

		#endregion
	}
}
