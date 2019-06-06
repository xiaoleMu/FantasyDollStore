using System;
namespace TabTale
{
	public class LevelMatchData : ICloneable
	{
		public string levelConfigId;
		public int seed;

		public LevelMatchData() {}

		#region ICloneable implementation

		public object Clone ()
		{
			LevelMatchData c = new LevelMatchData();
			c.seed = seed;
			c.levelConfigId = levelConfigId;

			return c;
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[LevelMatchData: LevelConfigId={0}, Seed={1}]", levelConfigId, seed);
		}
	}
}
