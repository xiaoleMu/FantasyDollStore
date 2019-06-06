using UnityEngine;
using LitJson;

namespace CocoPlay
{
	public class CocoNumericalData
	{
		public CocoNumericalData ()
		{
		}

		public CocoNumericalData (int amount)
		{
			Init (amount);
		}


		#region Property

		public int Value { get; private set; }

		public int Min { get; private set; }

		public int Max { get; private set; }

		[JsonIgnore]
		public bool IsInfinity {
			get { return Value == int.MaxValue; }
			set {
				if (value) {
					Value = int.MaxValue;
				}
			}
		}

		#endregion


		#region Init/Clone

		public virtual void Init (int amount, int min = 0, int max = int.MaxValue)
		{
			Min = min;
			Max = max;
			Value = Mathf.Clamp (amount, Min, Max);
		}

		public CocoNumericalData Clone ()
		{
			CocoNumericalData clone = Create ();
			clone.CloneContent (this);
			return clone;
		}


		public virtual CocoNumericalData Create ()
		{
			return new CocoNumericalData ();
		}

		public virtual void CloneContent (CocoNumericalData source)
		{
			Value = source.Value;
		}

		#endregion


		#region Increase / Decrease

		public bool IsSufficient (int amount)
		{
			if (IsInfinity) {
				return true;
			}

			return Value >= amount;
		}

		public bool Increase (int amount)
		{
			if (amount <= 0) {
				return false;
			}

			if (!IsInfinity) {
				int remaining = Max - Value;
				if (remaining <= 0) {
					return false;
				}

				if (remaining >= amount) {
					Value += amount;
				} else {
					amount = remaining;
					Value = Max;
				}
			}

			OnIncreased (amount);
			return true;
		}

		protected virtual void OnIncreased (int amount)
		{
		}

		public bool Decrease (int amount)
		{
			if (amount <= 0) {
				return false;
			}

			if (!IsInfinity) {
				int remaining = Value - Min;
				if (remaining <= 0) {
					return false;
				}

				if (remaining >= amount) {
					Value -= amount;
				} else {
					amount = remaining;
					Value = Min;
				}
			}

			OnDecreased (amount);
			return true;
		}

		protected virtual void OnDecreased (int amount)
		{
		}

		#endregion
	}
}