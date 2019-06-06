using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TweenAlpha : MonoBehaviour 
{
	public Color m_StartColor = Color.white;
	public Color m_EndAlpha = new Color(1,1,1,0);
	public float m_time = 0.5f;
	public float m_delaytime = 0;
	
	Coroutine m_Coroutine;
	
	void OnEnable()
	{
		Debug.Log("OnEnable");
		if(m_Coroutine != null)
			StopCoroutine(m_Coroutine);
		m_Coroutine = StartCoroutine(Play());
		Play();
	}
	
	void OnDisable()
	{
		Stop();
		CCAction.Stop(gameObject);
	}
	
	IEnumerator Play()
	{
		CCAction.Stop(gameObject);
		yield return new WaitForSeconds(m_delaytime);
		gameObject.GetComponent<Image>().color = m_StartColor;
		CCTool.SetColor(gameObject, m_StartColor);
		while(true)
		{
			CCAction.Color(gameObject, m_EndAlpha, m_time);
			yield return new WaitForSeconds(m_time);
			yield return new WaitForSeconds(m_time);
			gameObject.GetComponent<Image>().color = m_StartColor;
		}
	}
	
	void Stop()
	{
		if(m_Coroutine != null)
			StopCoroutine(m_Coroutine);
	}
}
