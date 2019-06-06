using UnityEngine;
using System.Collections;
using CocoPlay;
using System.Collections.Generic;
using TabTale;

public class GameGenericPopupBase : CocoGenericPopupBase {

	[SerializeField]
	GameObject content;

	protected override void ShowPopup ()
	{
		if (m_NeedAudio)
			soundManagerPop.PlaySound (new List<string> { "showPopupAudio" }, SoundLayer.Main);
		m_ScaleParent.transform.localScale = Vector3.zero;
		gameObject.SetActive(true);
		content.transform.localScale = new Vector3 (1,0,0);

		LeanTween.rotateAround(m_ScaleParent.GetComponent<RectTransform>(), new Vector3(-45f,-45f,0f), 0f , 0.6f).setEase(LeanTweenType.easeOutBack).setFrom(new Vector3(-45f,0f,0f));
		LeanTween.scale(m_ScaleParent.GetComponent<RectTransform>(), new Vector3(1f,1f,0f), 0.6f).setEase(LeanTweenType.easeOutBack).setFrom(new Vector3(0.6f,0.6f,0f)).setIgnoreTimeScale(true).setOnComplete(()=>{
			OnShowFinished ();
		});

		if(content!=null)
			LeanTween.scale(content.GetComponent<RectTransform>(), new Vector3(1f,1f,0f), 0.2f).setDelay(0.3f).setEase(LeanTweenType.easeOutBack).setFrom(new Vector3(1f,0f,0f));
	}
}

