using TabTale;

namespace CocoPlay
{
	public class CocoNumericalStateModel<TStateData, TData> : StateModel<TStateData>
		where TStateData : CocoNumericalStateData<TData>, new ()
		where TData : CocoNumericalData, new ()
	{
		public TData GetData (string key)
		{
			TData data = _data.dataDic.GetValue (key);

			if (data == null) {
				data = new TData ();
				_data.dataDic.Add (key, data);
			}

			return data;
		}

		/// <summary>
		/// Gets the amount.
		/// 获取当前货币数
		/// </summary>
		/// <returns>The amount.</returns>
		/// <param name="key">Key.</param>
		public int GetValue (string key)
		{
			return GetData (key).Value;
		}

		public bool IsInfinity (string key)
		{
			return GetData (key).IsInfinity;
		}

		/// <summary>
		/// Sets the infinity.
		/// 设置无限货币 (不会再增加或减少)
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="infinity">If set to <c>true</c> infinity.</param>
		public void SetInfinity (string key, bool infinity)
		{
			GetData (key).IsInfinity = infinity;
			Save ();
		}

		/// <summary>
		/// Inits the.
		/// 初始化货币值
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="amount">Amount.</param>
		/// <param name="min">Min.</param>
		/// <param name="max">Max.</param>
		public void Init (string key, int amount, int min = 0, int max = int.MaxValue)
		{
			GetData (key).Init (amount, min, max);
		}

		/// <summary>
		/// Determines whether this instance is sufficient the specified key amount.
		/// </summary>
		/// <returns><c>true</c> amount is sufficient; otherwise, <c>not sufficient</c>.</returns>
		/// <param name="key">Key.</param>
		/// <param name="amount">Amount.</param>
		public bool IsSufficient (string key, int amount)
		{
			return GetData (key).IsSufficient (amount);
		}

		/// <summary>
		/// Increases the.
		/// 增加货币数量
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="amount">Amount.</param>
		public void Increase (string key, int amount)
		{
			GetData (key).Increase (amount);
			Save ();
		}

		/// <summary>
		/// Decreases the.
		/// 减少货币数量
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="amount">Amount.</param>
		public void Decrease (string key, int amount)
		{
			GetData (key).Decrease (amount);
			Save ();
		}
	}
}