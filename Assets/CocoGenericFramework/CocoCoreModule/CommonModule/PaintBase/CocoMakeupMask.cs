using UnityEngine;
using System;

namespace CocoPlay{
	public class CocoMakeupMask
	{
		// mask
		int mMaskWidth;
		int mMaskHeight;
		byte[] mMaskAlphas;

		public CocoMakeupMask (Texture2D pMask)
		{
			Mask = pMask;
		}

		public CocoMakeupMask (byte[] pMaskAlphas, int maskWidth)
		{
			mMaskAlphas = pMaskAlphas;
			mMaskWidth = maskWidth;
			mMaskHeight = mMaskAlphas.Length / mMaskWidth;
		}

		#region Mask
		
		public Texture2D Mask {
			set {
				if (value != null) {
					mMaskWidth = value.width;
					mMaskHeight = value.height;
					Color32[] pixels = value.GetPixels32 ();
					mMaskAlphas = new byte[pixels.Length];
					for (int i = 0; i < pixels.Length; ++i)
						mMaskAlphas [i] = pixels [i].a;
				} else {	// default mask
					mMaskWidth = 1;
					mMaskHeight = 1;
					mMaskAlphas = new byte[] {255};
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
		
		public byte[] MaskAlphas {
			get {
				return mMaskAlphas;
			}
		}
		
		#endregion
	}
}
