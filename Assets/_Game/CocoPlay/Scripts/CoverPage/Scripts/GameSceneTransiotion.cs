using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CocoPlay;

public class GameSceneTransiotion : CocoSceneTransitionBase {
	[SerializeField]
	Animator loadingAni;

	[SerializeField]
	Image m_FillImage;
	[SerializeField]
	Text m_LoadingText;

	[SerializeField]
	Image m_SkierImage;

	private float m_LoadingFontValue;


	public override IEnumerator TransitionInAsync ()
	{
		float pTime = 1.0f;
//		loadingAni.Play ("loding");
		m_LoadingFontValue = Random.Range(0.4f,0.9f);
		m_FillImage.fillAmount = 0f;
		m_LoadingText.text = string.Format ("{0}%", m_FillImage.fillAmount);
		LeanTween.value (gameObject, 0f, m_LoadingFontValue, pTime).setOnUpdate ((value)=>{
			m_FillImage.fillAmount = value;
			m_LoadingText.text = string.Format ("{0}%", (int)(m_FillImage.fillAmount*100));
			m_LoadingText.transform.localPosition=new Vector3((value*250)-250,6,0);
			m_SkierImage.transform.localPosition=new Vector3((value*500)-250,6,0);
		});

		yield return new WaitForSeconds (pTime);
	}

	public override IEnumerator TransitionOutAsync ()
	{
		float pTime = 1.0f;
//		loadingAni.Play ("back");

		LeanTween.value (gameObject, m_LoadingFontValue, 1f, pTime).setOnUpdate ((value)=>{
			m_FillImage.fillAmount = value;
			m_LoadingText.text = string.Format ("{0}%", (int)(m_FillImage.fillAmount*100));
			m_LoadingText.transform.localPosition=new Vector3((value*250)-250,6,0);
			m_SkierImage.transform.localPosition=new Vector3((value*500)-250,6,0);
		});

		yield return new WaitForSeconds (pTime);
	}
}
