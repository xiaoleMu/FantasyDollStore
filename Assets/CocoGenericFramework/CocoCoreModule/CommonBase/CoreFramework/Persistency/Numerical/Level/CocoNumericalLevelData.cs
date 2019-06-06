using System;
using UnityEngine;
using System.Collections;
using LitJson;

namespace CocoPlay
{
	public class CocoNumericalLevelData : CocoNumericalData
	{
		public CocoNumericalLevelData () : base ()
		{
		}

		public CocoNumericalLevelData (int amount) : base (amount)
		{
		}

		public CocoNumericalLevelData (int amount, CocoNumericalLevelConfigData levelConfig) : base (amount)
		{
			LevelConfigData = levelConfig;
		}

		#region Init/Clone

		public override void Init (int amount, int min = 0, int max = Int32.MaxValue)
		{
			base.Init (amount, min, max);
			UpdateLevel ();
		}

		public override CocoNumericalData Create ()
		{
			return new CocoNumericalLevelData ();
		}

		public override void CloneContent (CocoNumericalData source)
		{
			base.CloneContent (source);

			CocoNumericalLevelData sourceData = (CocoNumericalLevelData)source;

			sourceData.LevelConfigData = LevelConfigData;
			sourceData.CurrLevel = CurrLevel;
			sourceData.CurrLevelProgress = CurrLevelProgress;
		}

		#endregion


		#region Level

		CocoNumericalLevelConfigData m_LevelConfigData = null;

		[JsonIgnore]
		public CocoNumericalLevelConfigData LevelConfigData {
			get {
				return m_LevelConfigData;
			}
			set {
				m_LevelConfigData = value;
				UpdateLevel ();
			}
		}

		public int CurrLevel { get; private set; }

		public float CurrLevelProgress { get; private set; }

		public void UpdateLevel ()
		{
			if (LevelConfigData == null) {
				return;
			}

			int level;
			CurrLevelProgress = LevelConfigData.GetLevelProgress (Value, out level);
			CurrLevel = level;
		}

		protected override void OnIncreased (int amount)
		{
			base.OnIncreased (amount);
			UpdateLevel ();
		}

		protected override void OnDecreased (int amount)
		{
			base.OnDecreased (amount);
			UpdateLevel ();
		}

		#endregion
	}
}
