using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

#if COCO_FAKE
using CocoUIButtonID = CocoPlay.Fake.CocoUIButtonID;
using CocoAudioID = CocoPlay.Fake.CocoAudioID;
using CocoLanguage = CocoPlay.Fake.CocoLanguage;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#else
using CocoUIButtonID = Game.CocoUIButtonID;
using CocoAudioID = Game.CocoAudioID;
using CocoLanguage = Game.CocoLanguage;
using CocoSceneID = Game.CocoSceneID;
#endif

namespace CocoPlay
{
	public class CocoStartButton : CocoUINormalButton
	{
		bool InShow = false;

		protected override void OnClick ()
		{
			StopPingpong ();
			CocoMainController.EnterScene (CocoSceneID.Map);
		}

		protected virtual void PlayPingpong ()
		{
			transform.localScale = Vector3.zero;
			LeanTween.cancel(gameObject);
			LeanTween.scale (gameObject, Vector3.one, 0.8f).setEase (LeanTweenType.easeOutElastic).setOnComplete (()=>{
				LeanTween.scale (gameObject, Vector3.one * 1.1f, 1f).setLoopPingPong ();
			});
		}

		protected virtual void StopPingpong()
		{
			transform.localScale = Vector3.one;
			LeanTween.cancel(gameObject);
		}

		public void Show ()
		{
			if(InShow) return;
			InShow = true;
			gameObject.SetActive (true);

			PlayPingpong ();
		}

		public void Hide ()
		{
			if(!InShow) return;
			InShow = false;
			StopPingpong ();
			gameObject.SetActive (false);
		}
	}
}

