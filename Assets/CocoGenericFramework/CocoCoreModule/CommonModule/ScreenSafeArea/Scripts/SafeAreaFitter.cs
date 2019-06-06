using System;
using System.Collections;
using UnityEngine;

namespace CocoPlay
{
	public class SafeAreaFitter : MonoBehaviour
	{
		private void Awake ()
		{
			Refresh (RefreshFlag.OnAwake);
		}

		private void Start ()
		{
			Refresh (RefreshFlag.OnStart);

			if (_refreshMode == RefreshMode.AfterStart) {
				StartCoroutine (RefreshAfterStart ());
			}
		}

		private void Update ()
		{
			Refresh (RefreshFlag.OnUpdate);
		}

		private IEnumerator RefreshAfterStart ()
		{
			yield return new WaitForEndOfFrame ();
			Refresh (RefreshFlag.AfterStart);
		}


		#region Update Mode

		private enum RefreshMode
		{
			OnAwake = RefreshFlag.OnAwake | RefreshFlag.Once,
			OnStart = RefreshFlag.OnStart | RefreshFlag.Once,
			AfterStart = RefreshFlag.AfterStart | RefreshFlag.Once,
			OnUpdateOnce = RefreshFlag.OnUpdate | RefreshFlag.Once,
			OnUpdateContinuously = RefreshFlag.OnUpdate | RefreshFlag.Continuous
		}


		[Flags]
		private enum RefreshFlag
		{
			// frequency
			Once = 0x00,
			Continuous = 0x01,

			// point
			OnAwake = 0x0100,
			OnStart = 0x0200,
			AfterStart = 0x0400,
			OnUpdate = 0x0800
		}


		[SerializeField]
		private RefreshMode _refreshMode = RefreshMode.OnStart;

		private bool _alreadyRefreshed;

		public bool AlreadyRefreshed {
			get { return _alreadyRefreshed; }
		}

		public bool RefreshIsContinuous {
			get { return MatchRefreshFlag (RefreshFlag.Continuous); }
		}

		private void Refresh (RefreshFlag refreshPoint)
		{
			if (!ShouldRefresh (refreshPoint)) {
				return;
			}

			_alreadyRefreshed |= RefreshSafeArea ();
		}

		private bool ShouldRefresh (RefreshFlag refreshPoint)
		{
			if (!MatchRefreshFlag (refreshPoint)) {
				return false;
			}

			return !AlreadyRefreshed || RefreshIsContinuous;
		}

		private bool MatchRefreshFlag (RefreshFlag refreshFlag)
		{
			return ((RefreshFlag)_refreshMode & refreshFlag) == refreshFlag;
		}

		#endregion


		#region Fit Mode

		[Flags]
		public enum FitMode
		{
			Left = 0x01,
			Right = 0x02,
			Top = 0x04,
			Bottom = 0x08
		}


		public const FitMode FIT_MODE_ALL = FitMode.Left | FitMode.Right | FitMode.Top | FitMode.Bottom;

		[EnumFlag]
		[SerializeField]
		private FitMode _fitMode = FIT_MODE_ALL;

		private FitMode _lastFitMode;

		private Vector2 _originalAnchorMin;
		private Vector2 _originalAnchorMax;

		private void RecordOriginalAnchors ()
		{
			if (AlreadyRefreshed) {
				return;
			}

			_originalAnchorMin = _panel.anchorMin;
			_originalAnchorMax = _panel.anchorMax;
		}

		private void FitSafeArea (Rect safeArea)
		{
			RecordOriginalAnchors ();

			var anchorMin = _panel.anchorMin;
			var anchorMax = _panel.anchorMax;

			if (MatchFitMode (FitMode.Left)) {
				anchorMin.x = safeArea.xMin / Screen.width;
			} else {
				anchorMin.x = _originalAnchorMin.x;
			}
			if (MatchFitMode (FitMode.Right)) {
				anchorMax.x = safeArea.xMax / Screen.width;
			} else {
				anchorMax.x = _originalAnchorMax.x;
			}
			if (MatchFitMode (FitMode.Bottom)) {
				anchorMin.y = safeArea.yMin / Screen.height;
			} else {
				anchorMin.y = _originalAnchorMin.y;
			}
			if (MatchFitMode (FitMode.Top)) {
				anchorMax.y = safeArea.yMax / Screen.height;
			} else {
				anchorMax.y = _originalAnchorMax.y;
			}

			_panel.anchorMin = anchorMin;
			_panel.anchorMax = anchorMax;
		}

		public bool MatchFitMode (FitMode fitMode)
		{
			return (_fitMode & fitMode) == fitMode;
		}

		private bool NeedFitSafeArea (Rect safeArea)
		{
			return safeArea != _lastSafeArea || _fitMode != _lastFitMode;
		}

		#endregion


		#region Safe Area

		[SerializeField]
		private RectTransform _panel;

		public RectTransform Panel {
			get { return _panel; }
		}

		private Rect _lastSafeArea = Rect.zero;

		private bool RefreshSafeArea ()
		{
			if (_panel == null) {
				_panel = transform as RectTransform;
			}
			if (_panel == null) {
				return false;
			}

			var safeArea = SafeAreaBinding.SafeArea;
			if (!NeedFitSafeArea (safeArea)) {
				return true;
			}

			if (CocoDebugSettingsData.Instance.IsDebugLogEnabled) {
				Debug.LogErrorFormat (gameObject, "ScreenSafeAreaFitter->RefreshSafeArea: obj: [{0}] {1} -> {2}", name, _lastSafeArea, safeArea);
			}

			FitSafeArea (safeArea);
			_lastSafeArea = safeArea;
			_lastFitMode = _fitMode;

			return true;
		}

		#endregion
	}
}