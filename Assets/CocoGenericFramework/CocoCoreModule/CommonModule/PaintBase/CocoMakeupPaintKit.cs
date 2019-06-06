using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay
{
	public enum CCPaintMode
	{
		//混合背景颜色
		MixOrigin,
		//以最新图上的颜色为准
		NonMixOrigin
	}

	public class CocoMakeupPaintKit
	{
		#region Variables

		// target
		string mTargetMaterialProperty;
		Texture2D mTargetTexture;
		Color32[] mTargetOriginPixels;
		Color32[] mTargetPixels;

		// canvas layer
		Dictionary<int, CocoMakeupCanvasLayer> mCanvasLayers;
		List<CocoMakeupCanvasLayer> mSortedCanvasLayers;
		List<Color32> mPixelColorBuffers;

		// paint
		public delegate Color32 PaintPixelDelegate (Color32 pLayerColor, byte pMaskAlpha, byte pBrushAlpha, Color32 pBrushMixColor, Color32 pPatternColor);

		bool mIsPainting;
		Vector2 mLastPaintPos;

		CCPaintMode m_MixMode = CCPaintMode.MixOrigin;

		public void SetMixMode (CCPaintMode mode)
		{
			m_MixMode = mode;
		}

		#endregion

		#region Initialization and Clean

		// target
		public CocoMakeupPaintKit ()
		{
		}

		[System.Obsolete ("use [CocoMakeupPaintKit (pMaterial, pMaterialProperty)] instead")]
		public CocoMakeupPaintKit (GameObject pObj, string pMaterialProperty = null)
		{
			Renderer renderer = pObj != null ? pObj.GetComponent<Renderer> () : null;
			if (renderer == null || renderer.sharedMaterial == null) {
				Debug.LogError (GetType () + "->Constructor: target material not found in object !");
				return;
			}

			Target = pObj;
			if (!Init (renderer.material, pMaterialProperty))
				return;
			InitTexture ();
		}

		public CocoMakeupPaintKit (Material pMaterial, string pMaterialProperty = null)
		{
			if (!Init (pMaterial, pMaterialProperty))
				return;
			InitTexture ();
		}

		public CocoMakeupPaintKit (Material pMaterial, string pMaterialProperty, int pWidth, int pHeight, Color pColor)
		{
			if (!Init (pMaterial, pMaterialProperty))
				return;
			InitTexture (pWidth, pHeight, pColor);
		}

		bool Init (Material pMaterial, string pMaterialProperty)
		{
			TargetMaterial = pMaterial;
			mTargetMaterialProperty = string.IsNullOrEmpty (pMaterialProperty) ? "_MainTex" : pMaterialProperty;
			if (TargetMaterial == null || !TargetMaterial.HasProperty (mTargetMaterialProperty)) {
				Debug.LogError (GetType () + "->InitMaterial: target material or property not found !");
				return false;
			}

			// layer
			mCanvasLayers = new Dictionary<int, CocoMakeupCanvasLayer> ();
			mSortedCanvasLayers = new List<CocoMakeupCanvasLayer> ();
			mPixelColorBuffers = new List<Color32> () { new Color32 () };
			CurrentCanvasLayer = null;

			// unit
			CurrentBrush = null;
			CurrentPattern = null;
			CurrentPaintMask = null;
			IsEraseMode = false;

			// default paint function
			PaintPixelFunc = PaintPixel;

			return true;
		}

		public void InitTexture ()
		{
			Texture2D originTexture = (Texture2D)TargetMaterial.GetTexture (mTargetMaterialProperty);
			if (originTexture == null) {
				Debug.LogError (GetType () + "->InitTarget: target texture not found ! = " + mTargetMaterialProperty);
				return;
			}

			// target texture
			mTargetTexture = new Texture2D (originTexture.width, originTexture.height, TextureFormat.ARGB32, false);
			mTargetTexture.SetPixels32 (originTexture.GetPixels32 ());
			mTargetTexture.Apply ();
			TargetMaterial.SetTexture (mTargetMaterialProperty, mTargetTexture);

			// pixels
			mTargetPixels = mTargetTexture.GetPixels32 ();
			mTargetOriginPixels = (Color32[])mTargetPixels.Clone ();
		}

		void InitTexture (int pWidth, int pHeight, Color pColor)
		{
			// pixels
			mTargetPixels = CocoData.CreateArray<Color32> (pWidth * pHeight, pColor);
			mTargetOriginPixels = (Color32[])mTargetPixels.Clone ();

			// target texture
			mTargetTexture = new Texture2D (pWidth, pHeight, TextureFormat.ARGB32, false);
			mTargetTexture.SetPixels32 (mTargetPixels);
			mTargetTexture.Apply ();
			TargetMaterial.SetTexture (mTargetMaterialProperty, mTargetTexture);
		}

		public void ResetCurrentLayer ()
		{
			if (CurrentCanvasLayer == null)
				return;
			CurrentCanvasLayer.Reset ();
			for (int i = 0; i < mTargetPixels.Length; ++i)
				UpdateTargetPixel (i, true);

			//			mTargetPixels = (Color32[])mTargetOriginPixels.Clone();
			PaintPost ();
		}

		public void Reset ()
		{
			if (mSortedCanvasLayers == null)
				return;

			for (int i = mSortedCanvasLayers.Count - 1; i >= 0; --i)
				mSortedCanvasLayers [i].Reset ();
			mTargetPixels = (Color32[])mTargetOriginPixels.Clone ();
			PaintPost ();
		}

		#endregion


		#region Basic

		[System.Obsolete ("not available when use constructor [CocoMakeupPaintKit (pMaterial, pMaterialProperty)], use [TargetMaterial] instead")]
		public GameObject Target { get; private set; }

		public Material TargetMaterial { get; private set; }

		public CocoMakeupBrush CurrentBrush { get; set; }

		public CocoMakeupPattern CurrentPattern { get; set; }

		public CocoMakeupMask CurrentPaintMask { get; set; }

		public CocoMakeupCanvasLayer CurrentCanvasLayer { get; set; }

		public void SetCurrentCanvasLayer (int pLayerId)
		{
			if (mCanvasLayers.ContainsKey (pLayerId)) {
				CurrentCanvasLayer = mCanvasLayers [pLayerId];
				return;
			}

			CurrentCanvasLayer = new CocoMakeupCanvasLayer (pLayerId, mTargetOriginPixels.Length);
			mCanvasLayers.Add (pLayerId, CurrentCanvasLayer);

			mSortedCanvasLayers.Add (CurrentCanvasLayer);
			mSortedCanvasLayers.Sort ();
			mPixelColorBuffers.Add (new Color32 ());
		}

		public bool IsEraseMode { get; set; }

		public PaintPixelDelegate PaintPixelFunc { get; set; }

		#endregion


		#region Paint

		private bool CanPaint {
			get {
				return mTargetTexture != null && CurrentBrush != null && CurrentPattern != null && CurrentPaintMask != null && CurrentCanvasLayer != null;
			}
		}

		public void Paint (Vector2 pTextureCoord)
		{
			if (IsEraseMode)
				PaintProcess (pTextureCoord, ErasePixel, null, PaintPost);
			else
				PaintProcess (pTextureCoord, PaintPixelFunc, null, PaintPost);
		}

		private void PaintProcess (Vector2 pTextureCoord, PaintPixelDelegate pPaintPixelFunc, System.Action pPrePaintFunc, System.Action pPostPaintFunc)
		{
			if (!CanPaint) {
				PaintEnd ();
				return;
			}

			Vector2 pos = pTextureCoord;
			pos.x *= mTargetTexture.width;
			pos.y *= mTargetTexture.height;

			if (!mIsPainting) {
				mIsPainting = true;
				mLastPaintPos = pos;
			}

			if (pPrePaintFunc != null)
				pPrePaintFunc ();

			Vector2 delta = pos - mLastPaintPos;
			float distanceX = Mathf.Abs (delta.x);
			float distanceY = Mathf.Abs (delta.y);
			Vector2 paintPos = mLastPaintPos - CurrentBrush.Extents;
			if (distanceX < mTargetTexture.width * 0.25f && distanceY < mTargetTexture.height * 0.25f) {
				int tapCount = Mathf.CeilToInt (Mathf.Max (distanceX / CurrentBrush.PointMaxSpacing.x, distanceY / CurrentBrush.PointMaxSpacing.y));
				Vector2 step = delta / tapCount;
				for (int i = 1; i < tapCount; i++) {
					paintPos += step;
					PaintTap (paintPos, pPaintPixelFunc);
				}
			}

			paintPos = pos - CurrentBrush.Extents;
			PaintTap (paintPos, pPaintPixelFunc);

			if (pPostPaintFunc != null)
				pPostPaintFunc ();

			mLastPaintPos = pos;
		}

		public void PaintEnd ()
		{
			mIsPainting = false;
		}

		private void PaintPost ()
		{
			mTargetTexture.SetPixels32 (mTargetPixels);
			mTargetTexture.Apply ();
		}

		private void PaintTap (Vector2 pPaintPos, PaintPixelDelegate pPaintPixelFunc)
		{
			int startX = (int)pPaintPos.x, startY = (int)pPaintPos.y;
			int paintWidth = (int)CurrentBrush.Size.x, paintHeight = (int)CurrentBrush.Size.y;
			float brushMaskStepX = CurrentBrush.MaskWidth / (float)paintWidth, brushMaskStepY = CurrentBrush.MaskHeight / (float)paintHeight;
			float brushMaskStartX = brushMaskStepX * 0.5f, brushMaskStartY = brushMaskStepY * 0.5f;

			// adjust bound inside texture
			if (startX < 0) {
				paintWidth += startX;
				brushMaskStartX -= startX * brushMaskStepX;
				startX = 0;
			}
			if (startX + paintWidth >= mTargetTexture.width) {
				paintWidth = mTargetTexture.width - startX;
			}
			if (startY < 0) {
				paintHeight += startY;
				brushMaskStartY -= startY * brushMaskStepY;
				startY = 0;
			}
			if (startY + paintHeight >= mTargetTexture.height) {
				paintHeight = mTargetTexture.height - startY;
			}

			// paint mask
			float paintMaskStepX = CurrentPaintMask.MaskWidth / (float)mTargetTexture.width, paintMaskStepY = CurrentPaintMask.MaskHeight / (float)mTargetTexture.height;
			float paintMaskStartX = (startX + 0.5f) * paintMaskStepX, paintMaskStartY = (startY + 0.5f) * paintMaskStepY;

			// pattern
			float brushPatternStepX, brushPatternStepY;
			float brushPatternStartX, brushPatternStartY;
			switch (CurrentPattern.PatternFillMode) {
			case CocoMakeupPattern.FillMode.BrushArea:
				float scaleX = (float)CurrentPattern.MaskWidth / CurrentBrush.MaskWidth;
				float scaleY = (float)CurrentPattern.MaskHeight / CurrentBrush.MaskHeight;
				brushPatternStepX = brushMaskStepX * scaleX;
				brushPatternStepY = brushMaskStepY * scaleY;
				brushPatternStartX = brushMaskStartX * scaleX;
				brushPatternStartY = brushMaskStartY * scaleY;
				break;
			default:
				brushPatternStepX = CurrentPattern.MaskWidth / (float)mTargetTexture.width;
				brushPatternStepY = CurrentPattern.MaskHeight / (float)mTargetTexture.height;
				brushPatternStartX = (startX + 0.5f) * brushPatternStepX;
				brushPatternStartY = (startY + 0.5f) * brushPatternStepY;
				break;
			}

			// paint pixels
			int index = startY * mTargetTexture.width + startX;
			float paintMaskIndex = (int)paintMaskStartY * CurrentPaintMask.MaskWidth + paintMaskStartX;
			float brushMaskIndex = (int)brushMaskStartY * CurrentBrush.MaskWidth + brushMaskStartX;
			float brushPatternIndex = (int)brushPatternStartY * CurrentPattern.MaskWidth + brushPatternStartX;
			for (int y = 0; y < paintHeight; ++y) {
				for (int x = 0; x < paintWidth; ++x, ++index, paintMaskIndex += paintMaskStepX, brushMaskIndex += brushMaskStepX, brushPatternIndex += brushPatternStepX) {
					//Debug.LogWarning ("index : (" + x + ", " + y + "), (" + paintWidth + ", " + paintHeight + ")");
					//Debug.LogWarning ("paint mask : " + paintMaskIndex + " (" + (int)(paintMaskStartX + x * paintMaskStepX) + ", " + (int)(paintMaskStartY + y * paintMaskStepY) + "), (" + paintMaskStartX + ", " + paintMaskStartY + "), (" + paintMaskStepX + ", " + paintMaskStepY + ")");
					//Debug.LogWarning ("brush mask : " + brushMaskIndex + " (" + (int)(brushMaskStartX + x * brushMaskStepX) + ", " + (int)(brushMaskStartY + y * brushMaskStepY) + ")");
					//Debug.LogWarning ("brush pattern : " + brushPatternIndex + " (" + (int)(brushPatternStartX + x * brushPatternStepX) + ", " + (int)(brushPatternStartY + y * brushPatternStepY) + "), (" + brushPatternStartX + ", " + brushPatternStartY + "), (" + brushPatternStepX + ", " + brushPatternStepY + ")");
					//Debug.LogWarning ("target: (" + (startX + x) + ", " + (startY + y);
					CurrentCanvasLayer.LayerPixels [index] = pPaintPixelFunc (
						CurrentCanvasLayer.LayerPixels [index],
						CurrentPaintMask.MaskAlphas [(int)paintMaskIndex],
						CurrentBrush.MaskAlphas [(int)brushMaskIndex],
						CurrentBrush.MixColor,
						CurrentPattern.PatternPixels [(int)brushPatternIndex]
					);
					UpdateTargetPixel (index);
				}
				index += mTargetTexture.width - paintWidth;
				paintMaskIndex = (int)(paintMaskStartY + y * paintMaskStepY) * CurrentPaintMask.MaskWidth + paintMaskStartX;
				brushMaskIndex = (int)(brushMaskStartY + y * brushMaskStepY) * CurrentBrush.MaskWidth + brushMaskStartX;
				brushPatternIndex = (int)(brushPatternStartY + y * brushPatternStepY) * CurrentPattern.MaskWidth + brushPatternStartX;
			}
		}

		public void Paint (Vector2 pTextureCoord, bool pIsSave)
		{
			if (IsEraseMode) {
				if (pIsSave)
					PaintProcess (pTextureCoord, ErasePixel, null, PaintPost);
				else
					PaintProcess (pTextureCoord, ErasePixel, null, PaintPostNotSave);
			} else {
				if (pIsSave)
					PaintProcess (pTextureCoord, PaintPixelFunc, null, PaintPost);
				else
					PaintProcess (pTextureCoord, PaintPixelFunc, null, PaintPostNotSave);
			}
		}

		private void PaintPostNotSave ()
		{
			mTargetTexture.SetPixels32 (mTargetPixels);
			//			mTargetTexture.Apply ();
		}

		private void PaintProcessNew (Vector2 pTextureCoord, PaintPixelDelegate pPaintPixelFunc, System.Action pPrePaintFunc, System.Action pPostPaintFunc)
		{
			if (!CanPaint) {
				PaintEnd ();
				return;
			}

			Vector2 pos = pTextureCoord;
			pos.x *= mTargetTexture.width;
			pos.y *= mTargetTexture.height;

			if (!mIsPainting) {
				mIsPainting = true;
				mLastPaintPos = pos;
			}

			if (pPrePaintFunc != null)
				pPrePaintFunc ();

			//			Debug.LogWarning ("xxx " + pTextureCoord);

			if (pPostPaintFunc != null)
				pPostPaintFunc ();

			mLastPaintPos = pos;
		}

		#endregion


		#region Paint Pixel Functions

		private Color32 PaintPixel (Color32 pLayerColor, byte pMaskAlpha, byte pBrushAlpha, Color32 pBrushMixColor, Color32 pPatternColor)
		{
			if (pMaskAlpha == 0)
				return pLayerColor;
			if (pBrushAlpha == 0)
				return pLayerColor;

			// update layer color
			int layerAlpha = pLayerColor.a + pBrushAlpha;
			switch (CurrentPattern.PatternFillMode) {
			case CocoMakeupPattern.FillMode.BrushArea:
				if (layerAlpha > 255)
					layerAlpha = 255;
				break;
			default:
				if (layerAlpha > pPatternColor.a)
					layerAlpha = pPatternColor.a;
				break;
			}
			pLayerColor.r = (byte)(pPatternColor.r < 128 ? (2 * pPatternColor.r * pBrushMixColor.r / 255) : (255 - 2 * (255 - pPatternColor.r) * (255 - pBrushMixColor.r) / 255));
			pLayerColor.g = (byte)(pPatternColor.g < 128 ? (2 * pPatternColor.g * pBrushMixColor.g / 255) : (255 - 2 * (255 - pPatternColor.g) * (255 - pBrushMixColor.g) / 255));
			pLayerColor.b = (byte)(pPatternColor.b < 128 ? (2 * pPatternColor.b * pBrushMixColor.b / 255) : (255 - 2 * (255 - pPatternColor.b) * (255 - pBrushMixColor.b) / 255));
			pLayerColor.a = (byte)layerAlpha;

			return pLayerColor;
		}

		private Color32 ErasePixel (Color32 pLayerColor, byte pMaskAlpha, byte pBrushAlpha, Color32 pBrushMixColor, Color32 pPatternColor)
		{
			if (pMaskAlpha == 0)
				return pLayerColor;
			if (pBrushAlpha == 0)
				return pLayerColor;

			// update layer color
			int layerAlpha = pLayerColor.a - pBrushAlpha;
			if (layerAlpha < 0)
				layerAlpha = 0;
			pLayerColor.a = (byte)layerAlpha;

			return pLayerColor;
		}

		private void UpdateTargetPixel (int pTargetIndex, bool reset = false)
		{
			bool mixOrigin = m_MixMode == CCPaintMode.MixOrigin || reset;

			// collect mixing weights
			int totalAlpha = 0;
			int endIndex = 0;
			Color32 color;
			for (int i = mPixelColorBuffers.Count - 1; i > endIndex; --i) {
				color = mSortedCanvasLayers [i - 1].LayerPixels [pTargetIndex];
				totalAlpha += color.a;
				if (totalAlpha >= 255) {
					color.a -= (byte)(totalAlpha - 255);
					totalAlpha = 255;
					endIndex = i - 1;
				}
				mPixelColorBuffers [i] = color;
			}

			// mix origin
			if (mixOrigin && totalAlpha < 255) {
				color = mTargetOriginPixels [pTargetIndex];
				totalAlpha += color.a;
				if (totalAlpha >= 255) {
					color.a -= (byte)(totalAlpha - 255);
					totalAlpha = 255;
				}
			    endIndex = -1;
				mPixelColorBuffers [0] = color;
			}

			// mix all colors
			if (totalAlpha <= 0) {
				mTargetPixels [pTargetIndex] = mPixelColorBuffers [0];
				return;
			}

			int targetColor_r = 0;
			int targetColor_g = 0;
			int targetColor_b = 0;
			for (int i = mPixelColorBuffers.Count - 1; i > endIndex; --i) {
				targetColor_r += mPixelColorBuffers [i].r * mPixelColorBuffers [i].a * 255 / totalAlpha;
				targetColor_g += mPixelColorBuffers [i].g * mPixelColorBuffers [i].a * 255 / totalAlpha;
				targetColor_b += mPixelColorBuffers [i].b * mPixelColorBuffers [i].a * 255 / totalAlpha;
			}
			if (mixOrigin && totalAlpha < mTargetOriginPixels [pTargetIndex].a) {
				totalAlpha = mTargetOriginPixels [pTargetIndex].a;
			}

			// update target
			//Debug.Log (" - (" + targetColor_r / 255 + ", " + targetColor_g / 255 + ", " + targetColor_b / 255 + ", " + totalAlpha + ")");
			Color32 targetColor = new Color32 ();
			targetColor.r = (byte)(targetColor_r > 255 * 255 ? 255 : targetColor_r / 255);
			targetColor.g = (byte)(targetColor_g > 255 * 255 ? 255 : targetColor_g / 255);
			targetColor.b = (byte)(targetColor_b > 255 * 255 ? 255 : targetColor_b / 255);
			targetColor.a = (byte)totalAlpha;
			//Debug.LogWarning (pTargetIndex + ": " + mTargetOriginPixels [pTargetIndex] + " - " + mTargetPixels [pTargetIndex]);
			mTargetPixels [pTargetIndex] = targetColor;

		}

		#endregion

		public void ApplyToTarget ()
		{
			mTargetOriginPixels = mTargetPixels;
			Reset ();
		}

		public void CleanTexture ()
		{
			if (mTargetTexture != null) {
				Object.Destroy (mTargetTexture);
				mTargetTexture = null;
			}
		}

	    public void ResetByTexture(Texture2D originTexture)
	    {
	        if(mTargetTexture != null)
	            Texture2D.Destroy(mTargetTexture);
	            
	        mTargetPixels = originTexture.GetPixels32();
	        mTargetOriginPixels = (Color32[])mTargetPixels.Clone ();

	        
	        // target texture
	        mTargetTexture = new Texture2D (originTexture.width, originTexture.height, TextureFormat.ARGB32, false);
	        mTargetTexture.SetPixels32 (mTargetPixels);
	        mTargetTexture.Apply ();
	        TargetMaterial.SetTexture (mTargetMaterialProperty, mTargetTexture);

	        if (mSortedCanvasLayers == null)
	            return;

	        for (int i = mSortedCanvasLayers.Count - 1; i >= 0; --i)
	            mSortedCanvasLayers [i].Reset ();
	    }
	}
}
