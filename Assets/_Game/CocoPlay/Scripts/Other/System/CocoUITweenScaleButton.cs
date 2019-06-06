using UnityEngine;
using System.Collections;
using CocoPlay;

public class CocoUITweenScaleButton : CocoUINormalButton
{
	[Header ("")]
	public bool TweenScaleEnable = true;
	public bool startAutomatically = true;
	public bool ignoreTimeScale = true;
	public float pingpongTime = 0.33f;
	/// <summary>
	/// true-间隔缩放 false-持续缩放
	/// </summary>
	public bool isIntervalScale = true;
	public CocoVector3Range scaleRange = new CocoVector3Range (Vector3.one, Vector3.one * 1.1f);

	private bool m_isTweenScaleing = false;

	public bool isTweenScaleing{ get { return m_isTweenScaleing; } }

	private int m_TweenUniqueId = int.MaxValue;

	protected override void Start ()
	{
		base.Start ();
		if (isIntervalScale)
			pingpongTime = 0.33f;
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		if (startAutomatically && TweenScaleEnable)
			PlayTweenScale ();
	}

	protected override void OnButtonPress (bool press)
	{
		if (!press) {
			if (TweenScaleEnable) {
				PlayTweenScale ();
			}
		} else {
			if (TweenScaleEnable) {
				StopTweenScale ();
			}
		}
		base.OnButtonPress (press);
	}

	public void PlayTweenScale ()
	{
		if (m_isTweenScaleing)
			return;

		TweenScaleEnable = true;
		m_isTweenScaleing = true;

		if (m_TweenUniqueId != int.MaxValue)
			LeanTween.cancel (gameObject, m_TweenUniqueId);
		
		if (isIntervalScale) {
			OnPlayTweenScale ();
		} else {
			gameObject.transform.localScale = scaleRange.From;
			m_TweenUniqueId = LeanTween.scale (gameObject, scaleRange.To, pingpongTime).setIgnoreTimeScale (ignoreTimeScale).setEase (LeanTweenType.easeInOutQuad).setLoopPingPong ().uniqueId;
		}
	}

	public void StopTweenScale ()
	{
		if (!m_isTweenScaleing)
			return;

		m_isTweenScaleing = false;
		if (m_TweenUniqueId != int.MaxValue)
			LeanTween.cancel (gameObject, m_TweenUniqueId);
		gameObject.transform.localScale = scaleRange.From;
	}

	protected void OnPlayTweenScale (float delay = 0.0f)
	{
		if (m_TweenUniqueId != int.MaxValue)
			LeanTween.cancel (gameObject, m_TweenUniqueId);
		
		gameObject.transform.localScale = scaleRange.From;
		m_TweenUniqueId = LeanTween.scale (gameObject, scaleRange.To, pingpongTime)
			.setIgnoreTimeScale (ignoreTimeScale)
			.setEase (LeanTweenType.easeInOutQuad)
			.setLoopPingPong ().setRepeat (6).setOnComplete (() => {
			OnPlayTweenScale (4);
		}).setDelay (delay).uniqueId;
	}

	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		StopTweenScale ();
	}
}
