using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay
{
	public class CocoPaintKit
	{
		#region Variables

		// target
		string mTargetMaterialProperty;
		Texture2D mTargetTexture;
		Color32[] mTargetPixels;

		// paint
		public delegate Color32 PaintPixelDelegate (Color32 pColor, byte pMaskAlpha, byte pBrushAlpha, Color32 pBrushMixColor, Color32 pPatternColor);

		bool mIsPainting;
		Vector2 mLastPaintPos;
		#endregion

		#region Initialization and Clean

		// target
		public CocoPaintKit ()
		{
		}

		[System.Obsolete ("use [CocoMakeupPaintKit (pMaterial, pMaterialProperty)] instead")]
		public CocoPaintKit (GameObject pObj, string pMaterialProperty = null)
		{
			Renderer renderer = pObj != null ? pObj.GetComponent <Renderer> () : null;
			if (renderer == null || renderer.sharedMaterial == null) {
				Debug.LogError (GetType () + "->Constructor: target material not found in object !");
				return;
			}

			Target = pObj;
			if (!Init (renderer.material, pMaterialProperty))
				return;
			InitTexture ();
		}

		public CocoPaintKit (Material pMaterial, string pMaterialProperty = null)
		{
			if (!Init (pMaterial, pMaterialProperty))
				return;
			InitTexture ();
		}

		public CocoPaintKit (Material pMaterial, string pMaterialProperty, int pWidth, int pHeight, Color pColor)
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
				
			// unit
			CurrentBrush = null;
			CurrentPattern = null;
			CurrentPaintMask = null;
			IsEraseMode = false;

			// default paint function
			PaintPixelFunc = PaintPixel;
			return true;
		}

		void InitTexture ()
		{
			Texture2D originTexture = (Texture2D)TargetMaterial.GetTexture (mTargetMaterialProperty);
			if (originTexture == null) {
				Debug.LogError (GetType () + "->InitTarget: target texture not found !");
				return;
			}

			// pixels
			mTargetPixels = originTexture.GetPixels32 ();

			// target texture
			mTargetTexture = new Texture2D (originTexture.width, originTexture.height, TextureFormat.ARGB32, false);
			mTargetTexture.SetPixels32 (mTargetPixels);
			mTargetTexture.Apply ();
			TargetMaterial.SetTexture (mTargetMaterialProperty, mTargetTexture);
		}

		void InitTexture (int pWidth, int pHeight, Color pColor)
		{
			// pixels
			mTargetPixels = CocoData.CreateArray<Color32> (pWidth * pHeight, pColor);

			// target texture
			mTargetTexture = new Texture2D (pWidth, pHeight, TextureFormat.ARGB32, false);
			mTargetTexture.SetPixels32 (mTargetPixels);
			mTargetTexture.Apply ();
			TargetMaterial.SetTexture (mTargetMaterialProperty, mTargetTexture);
		}
		#endregion


		#region Basic

		[System.Obsolete ("not available when use constructor [CocoMakeupPaintKit (pMaterial, pMaterialProperty)], use [TargetMaterial] instead")]
		public GameObject Target { get; private set; }

		public Material TargetMaterial { get; private set; }

		public CocoMakeupBrush CurrentBrush { get; set; }

		public CocoMakeupPattern CurrentPattern { get; set; }

		public CocoMakeupMask CurrentPaintMask { get; set; }

		public PaintPixelDelegate PaintPixelFunc { get; set; }

		public bool IsEraseMode { get; set; }

		#endregion


		#region Paint

		private bool CanPaint {
			get {
				return mTargetTexture != null && CurrentBrush != null && CurrentPattern != null && CurrentPaintMask != null;
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
			for (int y = 0; y < paintHeight; ++y) 
			{
				for (int x = 0; x < paintWidth; ++x, ++index, paintMaskIndex += paintMaskStepX, brushMaskIndex += brushMaskStepX, brushPatternIndex += brushPatternStepX) 
				{

					Color32 color = pPaintPixelFunc (
						mTargetPixels [index],
						CurrentPaintMask.MaskAlphas [(int)paintMaskIndex],
						CurrentBrush.MaskAlphas [(int)brushMaskIndex],
						CurrentBrush.MixColor,
						CurrentPattern.PatternPixels [(int)brushPatternIndex]
					);
					UpdateTargetPixel (index, color);
				}
				index += mTargetTexture.width - paintWidth;
				paintMaskIndex = (int)(paintMaskStartY + y * paintMaskStepY) * CurrentPaintMask.MaskWidth + paintMaskStartX;
				brushMaskIndex = (int)(brushMaskStartY + y * brushMaskStepY) * CurrentBrush.MaskWidth + brushMaskStartX;
				brushPatternIndex = (int)(brushPatternStartY + y * brushPatternStepY) * CurrentPattern.MaskWidth + brushPatternStartX;
			}
		}

		public void Paint (Vector2 pTextureCoord, bool pIsSave)
		{
			if (IsEraseMode){
				if (pIsSave)
					PaintProcess (pTextureCoord, ErasePixel, null, PaintPost);
				else
					PaintProcess (pTextureCoord, ErasePixel, null, PaintPostNotSave);
			}else{
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

		private Color32 PaintPixel (Color32 color, byte pMaskAlpha, byte pBrushAlpha, Color32 pBrushMixColor, Color32 pPatternColor)
		{
			if (pMaskAlpha == 0)
				return color;
			if (pBrushAlpha == 0)
				return color;

			// update layer color
			int layerAlpha = pBrushAlpha;
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
			color.r = (byte)(pPatternColor.r < 128 ? (2 * pPatternColor.r * pBrushMixColor.r / 255) : (255 - 2 * (255 - pPatternColor.r) * (255 - pBrushMixColor.r) / 255));
			color.g = (byte)(pPatternColor.g < 128 ? (2 * pPatternColor.g * pBrushMixColor.g / 255) : (255 - 2 * (255 - pPatternColor.g) * (255 - pBrushMixColor.g) / 255));
			color.b = (byte)(pPatternColor.b < 128 ? (2 * pPatternColor.b * pBrushMixColor.b / 255) : (255 - 2 * (255 - pPatternColor.b) * (255 - pBrushMixColor.b) / 255));
			color.a = (byte)layerAlpha;

			return color;
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

		private void UpdateTargetPixel (int pTargetIndex,  Color32 _color)
		{
			int targetColor_r = _color.r * 255;
			int targetColor_g = _color.g * 255;
			int targetColor_b = _color.b * 255;
			int targetAlpha = _color.a;

			Color32 targetColor = mTargetPixels [pTargetIndex];
//			if (targetAlpha < 255) {
//				targetColor_r += targetColor.r * (255 - targetAlpha);
//				targetColor_g += targetColor.g * (255 - targetAlpha);
//				targetColor_b += targetColor.b * (255 - targetAlpha);
//			}

			//			Debug.Log (" - (" + targetColor_r / 255 + ", " + targetColor_g / 255 + ", " + targetColor_b / 255 + ", " + targetAlpha + ")");
			targetColor.r = (byte)(targetColor_r > 255 * 255 ? 255 : targetColor_r / 255);
			targetColor.g = (byte)(targetColor_g > 255 * 255 ? 255 : targetColor_g / 255);
			targetColor.b = (byte)(targetColor_b > 255 * 255 ? 255 : targetColor_b / 255);
			if (targetAlpha > targetColor.a)
				targetColor.a = (byte)targetAlpha;


			mTargetPixels [pTargetIndex] = targetColor;
			//			Debug.LogWarning (pTargetIndex + ": " + mTargetOriginPixels [pTargetIndex] + " - " + mTargetPixels [pTargetIndex]);
		}

		#endregion

		public void CleanTexture()
		{
			if(mTargetTexture != null)
			{
				Object.Destroy(mTargetTexture);
				mTargetTexture = null;
			}
		}

		public void ResetTexture(Material mat, Texture2D _texture)
		{
			CleanTexture();

			if (_texture == null) {
				Debug.LogError (GetType () + "->InitTarget: target texture not found !");
				return;
			}

			// pixels
			mTargetPixels = _texture.GetPixels32 ();

			// target texture
			mTargetTexture = new Texture2D (_texture.width, _texture.height, TextureFormat.ARGB32, false);
			mTargetTexture.SetPixels32 (mTargetPixels);
			mTargetTexture.Apply ();

			TargetMaterial = mat;
			TargetMaterial.SetTexture (mTargetMaterialProperty, mTargetTexture);
		}
	}
}
