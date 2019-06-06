using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuideCircle : MonoBehaviour {

	public Vector3 StartScale = Vector3.one;
	public Vector3 EndScale = Vector3.one;
	public float m_time = 0.5f;
	public float m_delaytime = 0;
	public Color m_StartColor = Color.white;
	public Color m_EndAlpha = new Color(1,1,1,0);
	
	Coroutine m_Coroutine;
	
	void OnEnable()
	{
		Debug.Log("OnEnable");
		if(m_Coroutine == null)
		    m_Coroutine = StartCoroutine(Play());
		Play();
	}
	
	void OnDisable()
	{
        if (m_Coroutine != null)
        {
            m_Coroutine = null;
        }
		CCAction.Stop(gameObject);
		gameObject.transform.localScale = StartScale;
	}
	
	IEnumerator Play()
	{
		CCAction.Stop(gameObject);
		while(true)
		{
			gameObject.transform.localScale = StartScale;
			CCAction.ScaleTo(gameObject, EndScale, m_time).setIgnoreTimeScale(true);
			gameObject.GetComponent<Image>().color = m_StartColor;
			float time = 0;
			while(true)
			{
				time += Time.unscaledDeltaTime;
				if(time > m_time)
					time = m_time;
				float a = m_EndAlpha.a + (1- (time/m_time))*(m_StartColor.a - m_EndAlpha.a);
				gameObject.GetComponent<Image>().color = new Color(1,1,1,a);
				if(time >= m_time)
					break;
				yield return new WaitForEndOfFrame();
			}
			yield return new CCWait(m_delaytime);
		}
	}
}
