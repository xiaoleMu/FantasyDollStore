using UnityEngine;
using System.Collections;

public class TweenPosition : MonoBehaviour
{
	public Vector3 from = Vector3.zero;
	public Vector3 end = Vector3.zero;
	public float move_time  = 0.5f;
	public bool m_bEnableBack = true;
	
	[SerializeField]
	LeanTweenType type = LeanTweenType.linear;

	Coroutine m_Coroutine;



	void OnEnable()
	{
		m_Coroutine = StartCoroutine(Play());
	}
	
	void OnDisable()
	{
		if(m_Coroutine != null)
			StopCoroutine(m_Coroutine);
	}

	public void Init(Vector3 fromPos, Vector3 endPos, float time)
	{
		from = fromPos;
		end = endPos;
		move_time = time;
	}

	IEnumerator Play()
	{
		CCAction.Stop(gameObject);
		gameObject.transform.localPosition = from;
		while(true)
		{
			gameObject.transform.localPosition = from;
			CCAction.MoveLocal(gameObject, end, move_time, type);
			yield return new WaitForSeconds(move_time);
			if(m_bEnableBack)
				CCAction.MoveLocal(gameObject, from, move_time, type);
			yield return new WaitForSeconds(move_time);
		}
		Stop();
	}
	
	public void Stop()
	{
		if(m_Coroutine != null)
			StopCoroutine(m_Coroutine);
	}


}
