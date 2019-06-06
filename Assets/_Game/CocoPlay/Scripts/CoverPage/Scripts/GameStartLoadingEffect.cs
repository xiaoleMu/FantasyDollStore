using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartLoadingEffect : MonoBehaviour {

	[SerializeField]
	Image m_FillImage;
	[SerializeField]
	Text m_LoadingText;

	[SerializeField]
	Image m_SkierImage;

	private float m_LoadingFontValue;

	private void Start()
	{
		StartCoroutine(TransitionIn());
	}

	public IEnumerator TransitionIn()
	{
		float pTime = 1.0f;
		m_LoadingFontValue = 0.5f;
		m_FillImage.fillAmount = 0f;
		m_LoadingText.text = string.Format ("{0}%", m_FillImage.fillAmount);
		
		LeanTween.value (gameObject, (value) => {
			m_FillImage.fillAmount = value;
			m_LoadingText.text = string.Format ("{0}%", (int)(m_FillImage.fillAmount*100));
			m_LoadingText.transform.localPosition=new Vector3((value*250)-250,6,0);
			m_SkierImage.transform.localPosition=new Vector3((value*500)-250,6,0);
		}, 0, 0.4f, 0.4f);
		
		yield return new WaitForSeconds (0.4f);
		
		LeanTween.value (gameObject, 0.4f, m_LoadingFontValue, pTime).setOnUpdate ((value)=>
		{
			m_FillImage.fillAmount = value;
			m_LoadingText.text = string.Format ("{0}%", (int)(m_FillImage.fillAmount*100));
			m_LoadingText.transform.localPosition=new Vector3((value*250)-250,6,0);
			m_SkierImage.transform.localPosition=new Vector3((value*500)-250,6,0);
		});

		yield return new WaitForSeconds (pTime);
	}

//	public IEnumerator TransitionOut()
//	{
//		float pTime = 1.0f;
//		LeanTween.value (gameObject, m_LoadingFontValue, 1f, pTime).setOnUpdate ((value)=>{
//			m_FillImage.fillAmount = value;
//			m_LoadingText.text = string.Format ("{0}%", (int)(m_FillImage.fillAmount*100));
//			m_LoadingText.transform.localPosition=new Vector3((value*250)-250,6,0);
//			m_SkierImage.transform.localPosition=new Vector3((value*500)-250,6,0);
//		});
//
//		yield return new WaitForSeconds (pTime);
//	}
}
