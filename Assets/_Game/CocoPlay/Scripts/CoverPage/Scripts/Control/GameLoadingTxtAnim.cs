using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale;

public class GameLoadingTxtAnim : MonoBehaviour
{

	[SerializeField] private GameObject[] m_LoadingTxts;

	private List<float> m_PosY;

	void Start ()
	{
		StartCoroutine(Move());
	}

	private IEnumerator Move()
	{
		m_PosY = new List<float>();
		foreach (var item in m_LoadingTxts)
			m_PosY.Add(item.transform.localPosition.y);
		
		yield return new WaitForSeconds(1);
		while (true)
		{
			for (int i = 0; i < m_LoadingTxts.Length; i++)
			{
				var item = m_LoadingTxts[i];
				int off = i >= 7 ? 11 : 22;
				var tarY = m_PosY[i] + off;
				LeanTween.moveLocalY(item, tarY, 0.3f).setRepeat(2).setLoopPingPong().setDelay(i * 0.3f);
			}
			yield return new WaitForSeconds(2);
		}
	}


	void OnDestroy()
	{
		StopCoroutine(Move());
	}
}
