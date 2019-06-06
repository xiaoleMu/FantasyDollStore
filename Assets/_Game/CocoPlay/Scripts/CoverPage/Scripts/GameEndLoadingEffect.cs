using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndLoadingEffect : MonoBehaviour {

	[SerializeField]
	Image m_FillImage;
	[SerializeField]
	Text m_LoadingText;

	[SerializeField]
	Image m_SkierImage;

	private float m_LoadingFontValue;


	public IEnumerator TransitionOut()
	{
		m_LoadingFontValue = 0.5f;
		
		float pTime = 1.0f;
		LeanTween.value (gameObject, m_LoadingFontValue, 1f, pTime).setOnUpdate ((value)=>{
			m_FillImage.fillAmount = value;
			m_LoadingText.text = string.Format ("{0}%", (int)(m_FillImage.fillAmount*100));
			m_LoadingText.transform.localPosition=new Vector3((value*250)-250,6,0);
			m_SkierImage.transform.localPosition=new Vector3((value*500)-250,6,0);
		});

		yield return new WaitForSeconds (pTime);
		gameObject.SetActive(false);
	}
}
