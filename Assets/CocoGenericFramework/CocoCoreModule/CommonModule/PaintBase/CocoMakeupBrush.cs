using UnityEngine;
using System.Collections;

namespace CocoPlay{
	public class CocoMakeupBrush : CocoMakeupMask
	{
		// basic
		Vector2 mSize;
		Vector2 mExtents;
		Vector2 mMaxSpacing;

		#region Constructor

		public CocoMakeupBrush (Texture2D pBrushMask, Vector2 pBrushSize, Color32 pBrushColor, Vector2 pBrushMaxSpacing) : base (pBrushMask)
		{
			mMaxSpacing = pBrushMaxSpacing;
			Size = pBrushSize;
			MixColor = pBrushColor;
		}

		public CocoMakeupBrush (Texture2D pBrushMask, Vector2 pBrushSize, Color32 pBrushColor) : base (pBrushMask)
		{
			mMaxSpacing = pBrushSize / 2;
			Size = pBrushSize;
			MixColor = pBrushColor;
		}

		public CocoMakeupBrush (Texture2D pBrushMask) : base (pBrushMask)
		{
			mMaxSpacing = new Vector2 (MaskWidth, MaskHeight) / 2;
			Size = new Vector2 (MaskWidth, MaskHeight);
			MixColor = Color.gray;
		}

		public CocoMakeupBrush (byte[] pBrushAlphas, int pMaskWidth, Vector2 pBrushSize, Color32 pBrushColor, Vector2 pBrushMaxSpacing) : base (pBrushAlphas, pMaskWidth)
		{
			mMaxSpacing = pBrushMaxSpacing;
			Size = pBrushSize;
			MixColor = pBrushColor;
		}

		public CocoMakeupBrush (byte[] pBrushAlphas, int pMaskWidth, Vector2 pBrushSize, Color32 pBrushColor) : base (pBrushAlphas, pMaskWidth)
		{
			mMaxSpacing = pBrushSize / 2;
			Size = pBrushSize;
			MixColor = pBrushColor;
		}

		public CocoMakeupBrush (byte[] pBrushAlphas, int pMaskWidth) : base (pBrushAlphas, pMaskWidth)
		{
			mMaxSpacing = new Vector2 (MaskWidth, MaskHeight) / 2;
			Size = new Vector2 (MaskWidth, MaskHeight);
			MixColor = Color.gray;
		}

		#endregion


		#region Basic

		public Vector2 Size {
			get {
				return mSize;
			}
			set {
				mSize = value;

				if (mSize.x < 1)
					mSize.x = 1;
				if (mSize.y < 1)
					mSize.y = 1;

				mExtents = mSize / 2;
				ClampMaxSpacing ();
			}
		}

		[System.Obsolete ("use [Size] instead")]
		public void SetSize (float value)
		{
			Size = new Vector2 (value, value);
		}

		void ClampMaxSpacing ()
		{
			if (mMaxSpacing.x > mSize.x)
				mMaxSpacing.x = mSize.x;
			else if (mMaxSpacing.x < mSize.x / 8)
				mMaxSpacing.x =  mSize.x / 8;

			if (mMaxSpacing.y > mSize.y)
				mMaxSpacing.y = mSize.y;
			else if (mMaxSpacing.y < mSize.y / 8)
				mMaxSpacing.y = mSize.y / 8;

			if (mMaxSpacing.x < 1)
				mMaxSpacing.x = 1;
			if (mMaxSpacing.y < 1)
				mMaxSpacing.y = 1;
		}

		public Vector2 Extents {
			get {
				return mExtents;
			}
		}

		public Vector2 PointMaxSpacing {
			get {
				return mMaxSpacing;
			}
			set {
				mMaxSpacing = value;
				ClampMaxSpacing ();
			}
		}
		
		public Color32 MixColor { get; set; }

		#endregion


	}
}
