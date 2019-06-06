using UnityEngine;
using System.Collections;

public class CCGuideCircle : MonoBehaviour 
{
	[SerializeField]
	TweenPosition targetArrow;
	[SerializeField]
	GameObject targetCircle;

	Vector3 m_TipPos = Vector3.zero;
	GuideArrowType m_ArrowType;

	bool CircleEnable = true;
	bool ArrowEnable = true;

	Coroutine m_CircleCoroutine;

	public void SetCircleEnable (bool value)
	{
		CircleEnable = value;
	}

	public void SetArrowEnable (bool value)
	{
		ArrowEnable = value;
	}

	public void SetTipPosition (Vector3 tipPos, GuideArrowType type = GuideArrowType.Up)
	{
		m_TipPos = tipPos;
		m_ArrowType = type;
	}

	void InitPosition ()
	{
		targetCircle.transform.position = m_TipPos;
		Vector3 arrowPos = targetCircle.transform.localPosition;
		targetArrow.move_time = 0.5f;
		switch (m_ArrowType)
		{
		case GuideArrowType.Left:
			arrowPos.x -= 80f;
			targetArrow.end = arrowPos;
			arrowPos.x -= 60f;
			targetArrow.from = arrowPos;
			targetArrow.transform.localEulerAngles = new Vector3 (0f, 0f, -90f);
			break;
		case GuideArrowType.Right:
			arrowPos.x += 80f;
			targetArrow.end = arrowPos;
			arrowPos.x += 60f;
			targetArrow.from = arrowPos;
			targetArrow.transform.localEulerAngles = new Vector3 (0f, 0f, 90f);
			break;
		case GuideArrowType.Up:
			arrowPos.y += 80f;
			targetArrow.end = arrowPos;
			arrowPos.y += 60f;
			targetArrow.from = arrowPos;
			targetArrow.transform.localEulerAngles = new Vector3 (0f, 0f, 180f);
			break;
		case GuideArrowType.Down:
			arrowPos.y -= 80f;
			targetArrow.end = arrowPos;
			arrowPos.y -= 60f;
			targetArrow.from = arrowPos;
			targetArrow.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			break;
		default:
			break;
		}
	}

	public void ShowTip (Vector3 tipPos, GuideArrowType type = GuideArrowType.Up)
	{
		m_TipPos = tipPos;
		m_ArrowType = type;
		ShowTip ();
	}

	public void ShowTip (int count = 8)
	{
		HideTip();
		InitPosition ();
		if (ArrowEnable)
		{
			targetArrow.gameObject.SetActive (true);
		}
		m_CircleCoroutine = StartCoroutine (ShowCircle (count));
	}

	IEnumerator ShowCircle (int count)
	{
		yield return new WaitForSeconds (0.25f);
		targetCircle.gameObject.SetActive (true);

		if (count > 0) 
		{
			for (int i = 0; i < count; i++) 
			{
				yield return new WaitForSeconds (0.5f);
				targetCircle.gameObject.SetActive (!targetCircle.gameObject.activeSelf);
			}
			HideTip ();
		} 
		else 
		{
			while (true) 
			{
				yield return new WaitForSeconds (0.5f);
				targetCircle.gameObject.SetActive (!targetCircle.gameObject.activeSelf);
			}
		}
	}

	public void HideTip ()
	{
		StopAllCoroutines();
		if (m_CircleCoroutine != null)
		{
			StopCoroutine (m_CircleCoroutine);
			m_CircleCoroutine = null;
		}

		targetArrow.Stop ();
		targetArrow.gameObject.SetActive (false);
		targetCircle.gameObject.SetActive (false);
	}

}
