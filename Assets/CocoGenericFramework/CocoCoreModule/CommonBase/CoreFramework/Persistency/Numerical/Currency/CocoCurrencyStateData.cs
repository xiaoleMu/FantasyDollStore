namespace CocoPlay
{
	public class CocoCurrencyStateData : CocoNumericalStateData<CocoCurrencyData>
	{
		#region implemented abstract members of CocoAmountStateData

		public override string GetStateName ()
		{
			return "cocoCurrencyState";
		}

		protected override CocoNumericalStateData<CocoCurrencyData> Create ()
		{
			return new CocoCurrencyStateData ();
		}

		#endregion
	}
}