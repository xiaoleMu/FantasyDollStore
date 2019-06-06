using UnityEngine;
using System.Collections;


namespace CocoPlay
{
	[System.Serializable]
	public class CocoOptionalIntRangeProperty : CocoOptionalProperty<CocoIntRange>
	{
		public CocoOptionalIntRangeProperty () : base ()
		{
		}

		public CocoOptionalIntRangeProperty (CocoIntRange value) : base (value)
		{
		}

		public CocoOptionalIntRangeProperty (int from, int to) : base (new CocoIntRange (from, to))
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalFloatRangeProperty : CocoOptionalProperty<CocoFloatRange>
	{
		public CocoOptionalFloatRangeProperty () : base ()
		{
		}

		public CocoOptionalFloatRangeProperty (CocoFloatRange value) : base (value)
		{
		}

		public CocoOptionalFloatRangeProperty (float from, float to) : base (new CocoFloatRange (from, to))
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalVector3RangeProperty : CocoOptionalProperty<CocoVector3Range>
	{
		public CocoOptionalVector3RangeProperty () : base ()
		{
		}

		public CocoOptionalVector3RangeProperty (CocoVector3Range value) : base (value)
		{
		}

		public CocoOptionalVector3RangeProperty (Vector3 from, Vector3 to) : base (new CocoVector3Range (from, to))
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalColorRangeProperty : CocoOptionalProperty<CocoColorRange>
	{
		public CocoOptionalColorRangeProperty () : base ()
		{
		}

		public CocoOptionalColorRangeProperty (CocoColorRange value) : base (value)
		{
		}

		public CocoOptionalColorRangeProperty (Color from, Color to) : base (new CocoColorRange (from, to))
		{
		}
	}
}
