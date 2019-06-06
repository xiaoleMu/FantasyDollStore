using UnityEngine;
using System.Collections;

namespace CocoPlay{
	public class UIAutoRotate : MonoBehaviour {
		public float timeDuration = 1f;
		public float delayTime = 0f;
		public GameObject targetAnimObj;
		private float timeRecord;
		private bool stopEffect = false;

		public Vector3 rotateAxis = Vector3.up;
		public float rotateSpeed = 60f;

		// Use this for initialization
		void Start () {
			timeRecord = Time.time + delayTime;
		}
		
		// Update is called once per frame
		void Update () {
			if (targetAnimObj == null)
				return;

			if (stopEffect)
				return;

			targetAnimObj.transform.Rotate (rotateAxis * Time.deltaTime * rotateSpeed);
		}

		public void ShowEffect (bool pShow){
			stopEffect = pShow;
			timeRecord = pShow ? (Time.time + delayTime) : 0;
		}
	}
}
