using UnityEngine;

namespace CocoPlay
{
	public class CocoTweenAnchoredPosition : CocoTweenBase
	{
		[Header ("Tween Range")]
		public CocoVector3Range positionRange = new CocoVector3Range (Vector3.zero, Vector3.one);


		#region implemented abstract members of CocoTweenBase

		protected override void TweenInit (bool reversed)
		{
			var rectTrans = transform as RectTransform;
			if (rectTrans == null) {
				return;
			}

			var pos = reversed ? positionRange.To : positionRange.From;
			rectTrans.anchoredPosition3D = pos;
		}

		protected override LTDescr TweenRun (bool reversed, float time)
		{
			var rectTrans = transform as RectTransform;
			if (rectTrans == null) {
				return null;
			}

			var pos = reversed ? positionRange.To : positionRange.From;
			return LeanTween.move (rectTrans, pos, time);
		}

		#endregion
	}
}