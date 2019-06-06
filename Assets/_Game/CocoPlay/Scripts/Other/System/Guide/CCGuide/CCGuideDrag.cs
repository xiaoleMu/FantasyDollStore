using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CCGuideDrag : MonoBehaviour 
{
	[SerializeField]
	Image m_HandeImage;

	[SerializeField]
	List<Sprite> m_SpriteList;

	bool FingerLoop;
	Vector3 FingerStartPos;
	Vector3 FingerEndPos;

	int FingerMaxNum = 3;

	Coroutine m_DragCoroutine;
	public void Play(Vector3 startPos, Vector3 endPos, bool pLoop = false)
	{
		Stop ();
		FingerLoop = pLoop;
		transform.position = startPos;
		FingerStartPos = startPos;
		FingerEndPos = endPos;

		gameObject.SetActive (true);
		m_DragCoroutine = StartCoroutine (DragCoroutine());
	}

	public void Stop()
	{
		CCAction.Stop (gameObject);
		if(m_DragCoroutine != null)
			StopCoroutine(m_DragCoroutine);

		CCAction.Stop (gameObject);
		gameObject.SetActive (false);
	}

	IEnumerator DragCoroutine()
	{
		yield return new WaitForSeconds (0.1f);
		gameObject.SetActive (true);

		int index = 0;
		while (index < FingerMaxNum || FingerLoop)
		{
			transform.position = FingerStartPos;
			m_HandeImage.gameObject.SetActive(true);
			foreach(var sprite in m_SpriteList)
			{
				m_HandeImage.sprite = sprite;
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds (0.3f);
			CCAction.Move (gameObject, FingerEndPos, 0.8f);
			yield return new WaitForSeconds (1.3f);
			if(!FingerLoop)
				index++;
			m_HandeImage.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.3f);
		}
		gameObject.SetActive (false);
	}
}
