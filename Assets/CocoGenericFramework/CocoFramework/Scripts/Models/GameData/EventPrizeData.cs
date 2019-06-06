using System;

namespace TabTale
{
	public class EventPrizeData : ICloneable
	{
		public GameElementData prize = new GameElementData(GameElementType.Item, "", 0);
		public int 		value;

		#region ICloneable implementation

		public object Clone ()
		{
			var c = new EventPrizeData();
			c.prize = (GameElementData)prize.Clone();
			c.value = value;

			return c;
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[EventPrizeData: ItemId={0}, Value={1}]", prize, value);
		}
	}
}

