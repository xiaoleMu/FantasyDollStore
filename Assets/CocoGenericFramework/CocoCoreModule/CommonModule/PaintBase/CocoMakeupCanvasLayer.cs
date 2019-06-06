using UnityEngine;
using System;

namespace CocoPlay{
	public class CocoMakeupCanvasLayer : IComparable<CocoMakeupCanvasLayer>
	{
		int mPixelCount;

		public CocoMakeupCanvasLayer (int pId, int pPixelCount)
		{
			LayerId = pId;
			mPixelCount = pPixelCount;
			Reset ();
		}

		public int LayerId { get; private set; }

		public Color32[] LayerPixels { get; private set; }

		public int CompareTo(CocoMakeupCanvasLayer pOther)
		{
			if (pOther == null)
				return 1;

			return LayerId.CompareTo (pOther.LayerId);
		}

		public void Reset ()
		{
			LayerPixels = new Color32[mPixelCount];
		}
	}
}
