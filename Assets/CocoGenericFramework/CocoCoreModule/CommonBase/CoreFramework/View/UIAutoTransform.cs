using UnityEngine;
using System.Collections;

namespace CocoPlay{
	public class UIAutoTransform : MonoBehaviour {
		public GameObject tweenTarget;

		//pos 
		public bool changePos = false;
		public bool isLocalPos = true;
		public bool forceStartPos = false;
		public Vector3 startPos = Vector3.zero;
		public Vector3 endPos = Vector3.zero;
		public float posTime = 1f;
		public LeanTweenType posTweenType = LeanTweenType.linear;
		public LeanTweenType posLoopType = LeanTweenType.linear;

		//rotate
		public bool changeEulr = false;
		public bool isLocalEulr = true;
		public bool forceStartEulr = false;
		public Vector3 startEulr = Vector3.zero;
		public Vector3 endEulr = Vector3.zero;
		public float eulrTime = 1f;
		public LeanTweenType eulrTweenType = LeanTweenType.linear;
		public LeanTweenType eulrLoopType = LeanTweenType.linear;

		//scale
		public bool changeScale = false;
		public bool forceStartScale = false;
		public Vector3 startScale = Vector3.zero;
		public Vector3 endScale = Vector3.zero;
		public float scaleTime = 1f;
		public LeanTweenType scaleTweenType = LeanTweenType.linear;
		public LeanTweenType scaleLoopType = LeanTweenType.linear;


		// Use this for initialization
		void Start () {
			if (tweenTarget == null)
				tweenTarget = gameObject;

			ChangePos ();
			ChangeEurlangle ();
			ChangeScale ();
		}

		private void ChangePos (){
			if (!changePos)
				return;

			if (isLocalPos) {
				tweenTarget.transform.localPosition = forceStartPos ? startPos : tweenTarget.transform.localPosition;
				LeanTween.moveLocal (tweenTarget, endPos, posTime).setEase (posTweenType).setLoopType (posLoopType);
			}
			else{
				tweenTarget.transform.position = forceStartPos ? startPos : tweenTarget.transform.position;
				LeanTween.move (tweenTarget, endPos, posTime).setEase (posTweenType).setLoopType (posLoopType);
			}
		}

		private void ChangeEurlangle (){
			if (!changeEulr)
				return;

			if (isLocalEulr) {
				tweenTarget.transform.localEulerAngles = forceStartPos ? startPos : tweenTarget.transform.localEulerAngles;
				LeanTween.rotateLocal (tweenTarget, endEulr, eulrTime).setEase (eulrTweenType).setLoopType (eulrLoopType);
			}
			else{
				tweenTarget.transform.eulerAngles = forceStartEulr? startEulr : tweenTarget.transform.eulerAngles;
				LeanTween.rotate (tweenTarget, endEulr, eulrTime).setEase (eulrTweenType).setLoopType (eulrLoopType);
			}
		}

		private void ChangeScale (){
			if (!changeScale)
				return;

			tweenTarget.transform.localScale = forceStartScale ? startScale : tweenTarget.transform.localScale;
			LeanTween.scale (tweenTarget, endScale, scaleTime).setEase (scaleTweenType).setLoopType (scaleLoopType);
		}
	}
}
