using UnityEngine;
using System.Collections;

public class TweenScale : MonoBehaviour 
{
	public Vector3 StartScale = Vector3.one;
	public Vector3 EndScale = Vector3.one;
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
		gameObject.transform.localScale = StartScale;
	}

	IEnumerator Play()
	{
		CCAction.Stop(gameObject);
		gameObject.transform.localScale = StartScale;
		yield return new WaitForSeconds(m_delaytime);
		while(true)
		{
			CCAction.ScaleTo(gameObject, EndScale, m_time);
			yield return new WaitForSeconds(m_time);
			yield return new WaitForSeconds(m_time);
			gameObject.transform.localScale = StartScale;
		}
	}

	void Stop()
	{
		if(m_Coroutine != null)
			StopCoroutine(m_Coroutine);
	}
}
