using System.Collections;
using UnityEngine;

namespace CocoPlay
{
	public class SafeAreaChildAntiFitter : MonoBehaviour
	{
		private void Start ()
		{
			if (!Init ()) {
				return;
			}

			StartCoroutine (RunAntiFit ());
		}

		private IEnumerator RunAntiFit ()
		{
			do {
				if (_parentFitter.AlreadyRefreshed) {
					AntiFit ();
				}

				yield return new WaitForEndOfFrame ();
			} while (_parentFitter.RefreshIsContinuous);
		}


		#region Anti-Fit

		[EnumFlag]
		[SerializeField]
		private SafeAreaFitter.FitMode _antiFitMode = SafeAreaFitter.FIT_MODE_ALL;

		private RectTransform _trans;
		private SafeAreaFitter _parentFitter;

		private Vector2 _originalAnchorMin;
		private Vector2 _originalAnchorMax;

		private bool Init ()
		{
			_trans = transform as RectTransform;
			if (_trans == null) {
				return false;
			}

			_parentFitter = GetComponentInParent<SafeAreaFitter> ();
			if (_parentFitter == null) {
				return false;
			}

			_originalAnchorMin = _trans.anchorMin;
			_originalAnchorMax = _trans.anchorMax;

			return true;
		}

		private void AntiFit ()
		{
			var anchorMin = _trans.anchorMin;
			var anchorMax = _trans.anchorMax;
			var parentAnchorMin = _parentFitter.Panel.anchorMin;
			var parentAnchorMax = _parentFitter.Panel.anchorMax;
			var parentSize = parentAnchorMax - parentAnchorMin;

			if (ShouldAntiFit (SafeAreaFitter.FitMode.Left)) {
				anchorMin.x = -parentAnchorMin.x / parentSize.x;
			} else {
				anchorMin.x = _originalAnchorMin.x;
			}
			if (ShouldAntiFit (SafeAreaFitter.FitMode.Right)) {
				anchorMax.x = 1f + (1f - parentAnchorMax.x) / parentSize.x;
			} else {
				anchorMax.x = _originalAnchorMax.x;
			}
			if (ShouldAntiFit (SafeAreaFitter.FitMode.Bottom)) {
				anchorMin.y = -parentAnchorMin.y / parentSize.y;
			} else {
				anchorMin.y = _originalAnchorMin.y;
			}
			if (ShouldAntiFit (SafeAreaFitter.FitMode.Top)) {
				anchorMax.y = 1f + (1f - parentAnchorMax.y) / parentSize.y;
			} else {
				anchorMax.y = _originalAnchorMax.y;
			}

			_trans.anchorMin = anchorMin;
			_trans.anchorMax = anchorMax;
		}

		private bool ShouldAntiFit (SafeAreaFitter.FitMode fitMode)
		{
			return MatchAntiFitMode (fitMode) && _parentFitter.MatchFitMode (fitMode);
		}

		private bool MatchAntiFitMode (SafeAreaFitter.FitMode fitMode)
		{
			return (_antiFitMode & fitMode) == fitMode;
		}

		#endregion
	}
}