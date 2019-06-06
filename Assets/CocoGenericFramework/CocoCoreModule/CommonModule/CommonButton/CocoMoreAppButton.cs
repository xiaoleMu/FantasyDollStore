using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CocoPlay{
	public class CocoMoreAppButton : CocoUINormalButton
	{

		#region load icon

		public RawImage mIconRawTexture;
		public List<Texture2D> iconTexturesList;
		private int curDiplayIconIndex = 0;
		//放在Resources文件夹下的icon图标命名需要是icon_moreapps_0， icon_moreapps_5， icon_moreapps_12这种
		public string iconPathPrefix = "icon_moreapps_";
		public int iconNumLimit = 10;

		// Use this for initialization
		void Start ()
		{
			if(mTargetTransform == null)
				mTargetTransform = transform;
			mTargetTransform.localPosition = hidePosVec;

			if (mIconRawTexture == null) {
				mIconRawTexture = transform.GetComponent<RawImage> ();
			}

			if (iconTexturesList == null || iconTexturesList.Count < 1) {
				iconTexturesList = new List<Texture2D> ();
				for (int i = 0; i < iconNumLimit; i ++){
					Texture2D pTex = Resources.Load <Texture2D> (iconPathPrefix + i.ToString ());
					if (pTex != null)
						iconTexturesList.Add (pTex);
				}
			}

			displayImageCoro = StartCoroutine (DisplayImage ());
		}

		protected override void OnDestroy ()
		{
			mIconRawTexture = null;
			for (int i = 0; i < iconTexturesList.Count; i ++){
				iconTexturesList [i] = null;
			}

			base.OnDestroy ();
		}

		protected override void OnEnable ()
		{
			base.OnEnable ();

			if (displayImageCoro == null) {
				displayImageCoro = StartCoroutine (DisplayImage ());
			}
		}

		protected override void OnDisable ()
		{
			if (displayImageCoro != null) {
				StopCoroutine (displayImageCoro);
				displayImageCoro = null;
			}

			base.OnDisable ();
		}

		private Coroutine displayImageCoro;
		private IEnumerator DisplayImage ()
		{
			if (mIconRawTexture == null) {
				Debug.LogError ("CocoAppShelfFadeDisplay -> DisplayImage : mIconRawTexture is NULL");
				yield break;
			}

			if (iconTexturesList.Count < 1) {
				Debug.LogError ("CocoAppShelfFadeDisplay -> DisplayImage : icon Textures num is 0");
				yield break;
			}

			mIconRawTexture.texture = iconTexturesList [Random.Range (0, iconTexturesList.Count)];

			while (true) {
				Texture2D pNewTexture = iconTexturesList [curDiplayIconIndex];
				curDiplayIconIndex += 1;
				if (curDiplayIconIndex >= iconTexturesList.Count)
					curDiplayIconIndex = 0;

				float pDurationTime = Random.Range (1, 3f);
				yield return new WaitForSeconds (pDurationTime);
				yield return StartCoroutine (ChangColor (true));

				mIconRawTexture.texture = pNewTexture;

				yield return StartCoroutine (ChangColor (false));

				pDurationTime = Random.Range (1, 3f);
				yield return new WaitForSeconds (pDurationTime);
			}
		}

		private IEnumerator ChangColor (bool pIsClear)
		{
			Color pColor = pIsClear ? new Color (1, 1, 1, 0) : Color.white;
			Color pOldColor = mIconRawTexture.color;
			float pAddValue = 0;

			float pFadeTime = 0.25f;
			float pTimeCount = Time.time + pFadeTime;

			while (pTimeCount > Time.time){
				pAddValue += Time.deltaTime * 1 / pFadeTime;
				mIconRawTexture.color = Color.Lerp (pOldColor, pColor, pAddValue);
				yield return new WaitForEndOfFrame ();
			}
		}

		#endregion

		#region Icon Control

		[SerializeField]
		Vector3 showPosVec = new Vector3(-61, 50, 0);
		[SerializeField]
		Vector3 hidePosVec = new Vector3(-61, -110, 0);
		[SerializeField]
		Transform mTargetTransform;
		private bool mIsNeedShow = true;
		private bool hideIfNotReady = true;
		private float checkTime = 0;
		void Update()
		{
			if(mIsNeedShow)
			{
				if(hideIfNotReady && checkTime < Time.time)
				{
					checkTime = Time.time + 1f;
					hideIfNotReady = !CocoMainController.AdsControl.IsMoreAppsReady ();
					if(!hideIfNotReady)
						LeanTween.moveLocal(mTargetTransform.gameObject, showPosVec, 0.5f).setEase(LeanTweenType.easeOutElastic);
				}
			}
		}

		private void OnMoreAppsButtonShowOrHide(bool pIsShow)
		{
			if(pIsShow)
			{
				mIsNeedShow = true;
			}
			else
			{
				mIsNeedShow = false;
				LeanTween.moveLocal(mTargetTransform.gameObject, hidePosVec, 0.5f).setEase(LeanTweenType.easeInBack);
			}
		}

		#endregion

		#region click

		protected override void OnClick ()
		{
			CocoMainController.AdsControl.showMoreApps ();
		}

		#endregion
	}
}

