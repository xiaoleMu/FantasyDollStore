using UnityEngine;
using System.Collections;

namespace CocoPlay{
	public class UIAutoActive : MonoBehaviour {
		public float timeDuration = 1f;
		public float delayTime = 0f;
		public GameObject targetAnimObj;
		private float timeRecord;
		private bool stopEffect = false;

		void Start (){
			timeRecord = Time.time + delayTime;
		}
		
		// Update is called once per frame
		void Update () {
			if (targetAnimObj == null)
				return;

			if (stopEffect)
				return;

			if (timeRecord < Time.time){
				targetAnimObj.SetActive (!targetAnimObj.activeSelf);
				timeRecord = Time.time + timeDuration;
			}	
		}

		public void ShowEffect (bool pShow){
			stopEffect = pShow;
			targetAnimObj.SetActive (pShow);
			timeRecord = pShow ? (Time.time + delayTime) : 0;
		}
	}
}
