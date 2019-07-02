using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CocoPlay;
using Game;

public class GameLoadingAnim : CocoSceneTransitionBase
{

	[SerializeField]
	Transform m_Logo;

	[SerializeField]
	GameObject m_Loading;

	[SerializeField]
	float m_LogoTargetY;

	[SerializeField]
	RectTransform[] m_Images;

	private List<int[]> m_CurPlayIndexs;
	private float m_PlayItemTime = 0.3f;
	private float m_DelayTime = 0.1f;

	private bool isOnlyX = true;

	void Awake ()
	{
		m_Logo.SetLocal_Y (800);
		m_Loading.SetActive (false);

		int playType = Random.Range (0, 5);
		isOnlyX = CCTool.RandomBool;

		m_CurPlayIndexs = new List<int[]> ();
//		if (playType == 0) {
//			m_CurPlayIndexs.Add (new int[]{ 1, 26 });
//			m_CurPlayIndexs.Add (new int[]{ 2, 3, 14, 25 });
//			m_CurPlayIndexs.Add (new int[]{ 9, 10, 4, 24, 20, 8 });
//			m_CurPlayIndexs.Add (new int[]{ 15, 16, 17, 11, 5, 23, 19, 13, 7, 22 });
//			m_CurPlayIndexs.Add (new int[]{ 21, 18, 12, 6 });
//		} else {
//			int[] block1 = new int[]{ 1, 2, 9, 15 };
//			int[] block2 = new int[]{ 3, 10, 16, 17, 21 };
//			int[] block3 = new int[]{ 4, 11, 18, 22 };
//			int[] block4 = new int[]{ 5, 12, 19, 23 };
//			int[] block5 = new int[]{ 6, 13, 20, 24 };
//			int[] block6 = new int[]{ 7, 8, 14, 25, 26 };
//
//			if (playType == 1) {
//				m_CurPlayIndexs.Add (block1);
//				m_CurPlayIndexs.Add (block2);
//				m_CurPlayIndexs.Add (block3);
//				m_CurPlayIndexs.Add (block4);
//				m_CurPlayIndexs.Add (block5);
//				m_CurPlayIndexs.Add (block6);
//				m_DelayTime = 0.15f;
//			} else if (playType == 2) {
//				m_CurPlayIndexs.Add (block6);
//				m_CurPlayIndexs.Add (block5);
//				m_CurPlayIndexs.Add (block4);
//				m_CurPlayIndexs.Add (block3);
//				m_CurPlayIndexs.Add (block2);
//				m_DelayTime = 0.15f;
//				m_CurPlayIndexs.Add (block1);
//			} else {
//				m_CurPlayIndexs.Add (new int[]{ 1, 2, 9, 15, 7, 8, 14, 25, 26 });
//				m_CurPlayIndexs.Add (new int[]{ 3, 10, 16, 17, 21, 6, 13, 20, 24 });
//				m_CurPlayIndexs.Add (new int[]{ 4, 11, 18, 22, 5, 12, 19, 23  });
//			}
//		}


		foreach (RectTransform rtf in m_Images) {
			Image mask = CocoLoad.InstantiateOrCreate<Image> (string.Empty, rtf.transform);
			CCTool.SetImageAlpha (mask, 0.3515f);
			mask.GetComponent<RectTransform> ().sizeDelta = rtf.sizeDelta;
			if (isOnlyX)
				rtf.localScale = new Vector3 (0, 1, 1);
			else
				rtf.localScale = new Vector3 (0, 0, 1);
		}
	}


	#region implemented abstract members of CocoSceneTransitionBase

	public override IEnumerator TransitionInAsync ()
	{
		yield return new WaitForEndOfFrame ();
		CocoAudio.PlaySound (CocoAudioID.Button_Click_07);
		int index = 0;
		while (index < m_CurPlayIndexs.Count) {
			int[] idxs = m_CurPlayIndexs [index];
			foreach (int idx in idxs)
				PlayItem (idx, true);

			index++;
			yield return new CCWait (m_PlayItemTime - m_DelayTime);
		}

		m_Loading.gameObject.SetActive (true);
		yield return new CCWait (0.3f);
//		CocoAudio.PlaySound (Game.CocoAudioID.Contest_Cards_In);
		LeanTween.moveLocalY (m_Logo.gameObject, m_LogoTargetY, 0.8f).setEase (LeanTweenType.easeOutBack).setIgnoreTimeScale (true);
		yield return new CCWait (0.8f);
	}

	public override IEnumerator TransitionOutAsync ()
	{
		m_Loading.gameObject.SetActive (false);
		CocoAudio.PlaySound (CocoAudioID.Button_Click_07);

		yield return new WaitForEndOfFrame ();
		LeanTween.moveLocalY (m_Logo.gameObject, 800, 0.5f).setEase (LeanTweenType.easeInBack).setIgnoreTimeScale (true);
		yield return new CCWait (0.3f);

		m_CurPlayIndexs.Reverse ();

		int index = 0;
		while (index < m_CurPlayIndexs.Count) {
			int[] idxs = m_CurPlayIndexs [index];
			foreach (int idx in idxs)
				PlayItem (idx, false);

			index++;
			yield return new CCWait (m_PlayItemTime - m_DelayTime);
		}

		yield return new CCWait (0.65f);

		foreach (var t in m_Images) {
			GameObject.Destroy (t.gameObject);
		}
		Debug.LogError (Time.time + ": ----- end" + m_Logo.transform.localPosition);
	}

	#endregion

	private void PlayItem (int index, bool isShow)
	{
		int idx = index - 1;
		RectTransform rtf = m_Images [idx];
		float tartScale = isShow ? 1 : 0;
		LeanTweenType tweenType = isShow ? LeanTweenType.easeInCubic : LeanTweenType.easeOutCubic;
		LeanTween.scaleX (rtf.gameObject, tartScale, m_PlayItemTime).setIgnoreTimeScale(true);//.setEase(tweenType);
		if (!isOnlyX)
			LeanTween.scaleY (rtf.gameObject, tartScale, m_PlayItemTime).setIgnoreTimeScale(true);
	}
}

