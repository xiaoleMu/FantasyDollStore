using UnityEngine;
using System.Collections;

namespace CocoPlay{
	public class CocoMakeupPattern
	{
		public enum FillMode
		{
			CanvasArea,
			BrushArea
		}

		// pattern
		int mMaskWidth;
		int mMaskHeight;
		Color32[] mMaskPixels;

		public CocoMakeupPattern (Texture2D pMask, FillMode pFillMode)
		{
			PatternMask = pMask;
			PatternFillMode = pFillMode;
		}

		public Texture2D PatternMask {
			set {
				if (value != null) {
					mMaskPixels = value.GetPixels32 ();
					mMaskWidth = value.width;
					mMaskHeight = value.height;
				} else {	// default pattern
					mMaskPixels = new Color32[] {new Color32 (255, 255, 255, 255)};
					mMaskWidth = 1;
					mMaskHeight = 1;
				}
			}
		}
		
		public int MaskWidth {
			get {
				return mMaskWidth;
			}
		}
		
		public int MaskHeight {
			get {
				return mMaskHeight;
			}
		}
		
		public Color32[] PatternPixels {
			get {
				return mMaskPixels;
			}
		}
		
		public FillMode PatternFillMode { get; set; }
	}
}
