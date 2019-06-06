using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteAnimaton : MonoBehaviour
{
	[SerializeField]
	float m_Time = 1.0f;
	[SerializeField]
	Sprite[] m_Sprites;

	Image mImage;
	float m_Speed;

	Coroutine m_Coroutine;
	int m_Index = 1;

	[SerializeField]
	bool NeedNativeSize = true;

	IEnumerator PlayAnim()
	{
		if (m_Speed <= 0)
			yield break;

		while (true)
		{
			mImage.sprite = m_Sprites[m_Index];
			if(NeedNativeSize)
				mImage.SetNativeSize();

			m_Index++;
			if (m_Index >= m_Sprites.Length)
				m_Index = 0;
			yield return new CCWait(m_Speed);
		}
	}

	void OnEnable()
	{
		//        if (m_Coroutine != null)
		//		{
		//			StopCoroutine(m_Coroutine);
		//			m_Coroutine = null;
		//		}
		mImage = GetComponent<Image>();
		if (m_Sprites.Length != 0)
		{
			m_Speed = m_Time / (float)m_Sprites.Length;
			m_Coroutine = StartCoroutine(PlayAnim());
		}
	}
}
