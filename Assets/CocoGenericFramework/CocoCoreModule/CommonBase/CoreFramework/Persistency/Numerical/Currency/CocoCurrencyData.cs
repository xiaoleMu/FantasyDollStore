namespace CocoPlay
{
	public class CocoCurrencyData : CocoNumericalData
	{
		public CocoCurrencyData ()
		{
		}

		public CocoCurrencyData (int amount) : base (amount)
		{
		}


		#region Property

		public int TotalSpending { get; private set; }

		public int TotalIncome { get; private set; }

		#endregion


		#region Init/Clone

		public override void Init (int amount, int min = 0, int max = int.MaxValue)
		{
			base.Init (amount, min, max);
			TotalIncome = 0;
			TotalSpending = 0;
		}

		public override CocoNumericalData Create ()
		{
			return new CocoCurrencyData ();
		}

		public override void CloneContent (CocoNumericalData source)
		{
			base.CloneContent (source);

			CocoCurrencyData sourceData = (CocoCurrencyData)source;
			TotalIncome = sourceData.TotalIncome;
			TotalSpending = sourceData.TotalSpending;
		}

		#endregion


		#region Increase / Decrease

		protected override void OnIncreased (int amount)
		{
			base.OnIncreased (amount);
			TotalIncome += amount;
		}

		protected override void OnDecreased (int amount)
		{
			base.OnDecreased (amount);
			TotalSpending += amount;
		}

		#endregion
	}
}