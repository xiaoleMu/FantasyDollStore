namespace CocoPlay
{
	public class CocoCurrencyStateModel : CocoNumericalStateModel<CocoCurrencyStateData, CocoCurrencyData>
	{
		/// <summary>
		/// Gets the total income.
		/// 获取总收入
		/// </summary>
		/// <returns>The total income.</returns>
		/// <param name="key">Key.</param>
		public int GetTotalIncome (string key)
		{
			return GetData (key).TotalIncome;
		}

		/// <summary>
		/// Gets the total spending.
		/// 获取总支出
		/// </summary>
		/// <returns>The total spending.</returns>
		/// <param name="key">Key.</param>
		public int GetTotalSpending (string key)
		{
			return GetData (key).TotalSpending;
		}
	}
}