using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CocoPlay;

public enum GuideArrowType
{
	Left,
	Right,
	Up,
	Down
}

public class CCGuideManager : MonoBehaviour
{
	const int TipMaxNum = 5;
	const int FingerMaxNum = 3;
	[SerializeField]
	TweenPosition targetArrow;
	[SerializeField]
	GameObject targetCircle;
	public GameObject targetFinger;
	public GameObject targetTouch;
    public GameObject targetMirrorTouch;
	public CCGuideArea m_GuideArea;
	public GameObject m_HandAnimation;
    public GameObject m_HandClick;
	Vector3 mTipPos = Vector3.zero;
	Vector3 FingerStartPos;
	Vector3 FingerEndPos;
	GuideArrowType mType;

	[SerializeField]
	CCGuideDrag m_GuideDrag;

	public CCGuideDrag Drag{get {return m_GuideDrag;}}

	#region Circle Control

	bool CircleActive = false;
	int ShowedNum = 0;
	float ShowTime = 0;

	#endregion


	#region

	Coroutine m_CircleCoroutine;

	#endregion

	bool IgnoreShowNum = false;

	public void SetIgnoreNum (bool value)
	{
		IgnoreShowNum = value;
	}

	bool CircleEnable = true;
	bool ArrowEnable = true;

	public void SetCircleEnable (bool value)
	{
		CircleEnable = value;
	}

	public void SetArrowEnable (bool value)
	{
		ArrowEnable = value;
	}

	bool AutoDestory = false;

	public void SetAutoDestory (bool value)
	{
		AutoDestory = value;
	}

	public static CCGuideManager Create (Transform pParent = null)
	{
		if (pParent == null)
			pParent = GameObject.FindWithTag ("Canvas_UI").transform;

		GameObject obj = CocoLoad.Instantiate ("UI_Guide", pParent);
		obj.transform.localScale = Vector3.one;
		CCGuideManager guide = obj.GetComponent<CCGuideManager> ();

		Canvas uiCanvas = pParent.GetComponent<Canvas> ();
		if (uiCanvas != null) {
			guide.GuideCamera = uiCanvas.worldCamera;
		} else {
			guide.GuideCamera = CocoMainController.UICamera;
		}

		return guide;
	}

	public Camera GuideCamera { get; private set; }

	void Start ()
	{

	}

	void Update ()
	{

	}

	public void SetTipPosition (Vector3 tipPos, GuideArrowType type = GuideArrowType.Up)
	{
		mTipPos = tipPos;
		mType = type;
	}

	void InitPosition ()
	{
		targetCircle.transform.position = mTipPos;
		Vector3 arrowPos = targetCircle.transform.localPosition;
		targetArrow.move_time = 0.5f;
		switch (mType)
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
		mTipPos = tipPos;
		mType = type;
		ShowTip ();
	}

	public void ShowTip (int count = 8)
	{
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
		} 
		else 
		{
			while (true) 
			{
				yield return new WaitForSeconds (0.5f);
				targetCircle.gameObject.SetActive (!targetCircle.gameObject.activeSelf);
			}
		}
		HideTip ();
	}

	public void HideTip ()
	{
		if (m_CircleCoroutine != null)
		{
			StopCoroutine (m_CircleCoroutine);
			m_CircleCoroutine = null;
		}
			
		targetArrow.Stop ();
		targetArrow.gameObject.SetActive (false);
		targetCircle.gameObject.SetActive (false);
	}

	#region Finger

	public bool FingerLoop = false;

	public void ShowFinger (Vector3 startPos, Vector3 endPos,bool pLoop = false)
	{
		HideFinger ();
        FingerLoop = pLoop;
		targetFinger.transform.position = startPos;
		FingerStartPos = startPos;
		FingerEndPos = endPos;
		StartCoroutine ("FingerTip");
	}

	IEnumerator FingerTip ()
	{
		CCAction.Stop (targetFinger);
		yield return new WaitForSeconds (0.1f);
		targetFinger.SetActive (true);
		int i = 0;

		if (FingerLoop)
		{
			while (true)
			{
				targetFinger.transform.position = FingerStartPos;
				CCAction.Move (targetFinger, FingerEndPos, 0.8f);
				yield return new WaitForSeconds (1.3f);
			}
		}

		while (i < FingerMaxNum)
		{
			targetFinger.transform.position = FingerStartPos;
			CCAction.Move (targetFinger, FingerEndPos, 0.8f);
			yield return new WaitForSeconds (1.3f);
			i++;
		}
		targetFinger.SetActive (false);
	}

	public void StartOnceFingerTip (Vector3 startPos, Vector3 endPos)
	{
		HideFinger ();
		targetFinger.transform.position = startPos;
		FingerEndPos = endPos;
		StartCoroutine ("OnceFingerTip");
	}

	IEnumerator OnceFingerTip ()
	{
		targetFinger.SetActive (true);
		CCAction.Move (targetFinger, FingerEndPos, 0.8f);
		yield return new WaitForSeconds (1f);
		targetFinger.SetActive (false);
	}

	public void HideFinger ()
	{
		targetFinger.SetActive (false);
		CCAction.Stop (targetFinger);
		StopCoroutine ("FingerTip");
		StopCoroutine ("OnceFingerTip");
        FingerLoop = false;
	}

	public void Stop ()
	{
		HideFinger ();
		HideTip ();
		HideTouch ();
		HideTouchAnimation ();
        HideHandClick();
        HideMirrorTouch();

		m_GuideDrag.Stop();
	}

	#endregion

	#region touch

	public bool ToucPpointEnable = true;
	public Image TouchPoint;
    public TweenPosition touchTweenPosition;

	public void ShowTouch (Vector3 position)
	{
		targetTouch.transform.position = position;
		targetTouch.SetActive (true);
	}

	public void ShowTouch (Vector3 position, Vector2 size)
	{
		TouchPoint.gameObject.SetActive (ToucPpointEnable);
		targetTouch.transform.position = position;
		targetTouch.SetActive (true);
		m_GuideArea.gameObject.SetActive (true);
		m_GuideArea.SetArea (targetTouch.transform.localPosition, size);
	}

	public void HideTouch ()
	{
		targetTouch.SetActive (false);
		m_GuideArea.gameObject.SetActive (false);
	}


    public void ShowMirrorTouch(Vector3 position){

        targetMirrorTouch.transform.position = position;
        targetMirrorTouch.SetActive(true);
    }

    public void HideMirrorTouch(){
        targetMirrorTouch.SetActive(false);
        m_GuideArea.gameObject.SetActive (false);
    }

	public void ShowTouchAnimation (Vector3 position, Vector2 size)
	{
		//TouchPoint.gameObject.SetActive (ToucPpointEnable);
		m_HandAnimation.transform.position = position;
		m_HandAnimation.SetActive (true);
		m_GuideArea.gameObject.SetActive (true);
		m_GuideArea.SetArea (m_HandAnimation.transform.localPosition, size);
	}

	public void HideTouchAnimation ()
	{
		m_HandAnimation.SetActive (false);
		m_GuideArea.gameObject.SetActive (false);
	}

    #endregion

    #region click
    public void ShowHandClick(Vector3 position)
    {
        m_HandClick.transform.position = position;
        m_HandClick.SetActive(true);
    }

    public void HideHandClick()
    {
        m_HandClick.SetActive(false);
    }


    #endregion

}
