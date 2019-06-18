using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale;
using System;
using CocoPlay;
using CocoPlay.ResourceManagement;
using Game;

public class CCCharacterAnimation : MonoBehaviour
{
	Animator m_Animator;
	ISceneAnimationData m_AniData;
	CCAnimationData m_CurrAnimData;

	Coroutine m_Coroutine;

	[SerializeField]
	string TestAnimation = "";

	bool m_AutoSwitchEanble = true;
	bool m_SoundEnable = true;

	bool m_CrossEnable = false;

	List<CCAnimationData> m_PlayAnimationList = new List<CCAnimationData>();

	public CCAnimationData CurrAnimData { get { return m_CurrAnimData; } set { m_CurrAnimData = value; } }
	public ISceneAnimationData SceneAnimData { get { return m_AniData; } }

	public Action<CCAnimationData> m_OnPlayAnimation = (t) => { };
	public Action<CCAnimationData> m_OnFinishAnimation = (t) => { };

	private RuntimeAnimatorController m_AnimatorControl;

	public  AnimationClip _CurPlayingClip
	{
		get;
		private set;
	}

	string stanceAnimStateName = "play";

	#if UNITY_EDITOR
	[SerializeField]
	bool m_LogEnable = false;
	#endif

	public void SetAutoSwithEnable(bool enable)
	{
		m_AutoSwitchEanble = enable;
	}

	public Animator animator
	{
		get
		{
			if(m_Animator == null) {
				m_Animator = GetComponentInChildren<Animator>(true);
				m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
			return m_Animator;
		}
	}

	public void SetCrossEnable(bool enable)
	{
		m_CrossEnable = enable;
	}

	public bool GetCrossEnable()
	{
		return m_CrossEnable;
	}

	public float Speed {
		get { return animator.speed; }
		set { animator.speed = value; }
	}

    #if UNITY_EDITOR
	void Update()
	{
		if (CCInput.GetKeyDown(KeyCode.P) && TestAnimation != "")
		{
			CCAnimationData data = new CCAnimationData(TestAnimation);
			Play(data);
		}
	}
    #endif

	public void SetAnimationData(ISceneAnimationData data)
	{
		m_AniData = data;
	}

	public void PlayList(List<CCAnimationData> list)
	{
		if(list == null || list.Count == 0)
			return;
		m_PlayAnimationList = list;
		CCAnimationData pAnimData = list[0];

		if (pAnimData == null)
		{
			m_PlayAnimationList.Clear();
			return;
		}

		Stop();
		m_Coroutine = StartCoroutine(CoroutinePlay(pAnimData));
		m_PlayAnimationList.RemoveAt(0);
	}

	public void Play(string aniName)
	{
		CCAnimationData data = new CCAnimationData(aniName);
		Play(data);
	}

	public void Play(CCAnimationData pAnimData)
	{
		if (gameObject.activeInHierarchy == false)
			return;
		if (pAnimData == null)
			return;
		Stop();

		m_Coroutine = StartCoroutine(CoroutinePlay(pAnimData));
		m_PlayAnimationList.Clear();
	}

	public IEnumerator StartPlay(string aniName)
	{
		return StartPlay(new CCAnimationData(aniName));
	}

	public IEnumerator StartPlay(CCAnimationData pAnimData)
	{
		if (pAnimData == null)
			yield break;
		Stop();
		m_PlayAnimationList.Clear();
		m_Coroutine = StartCoroutine(CoroutinePlay(pAnimData));
		yield return m_Coroutine;
	}

	public void SwitchAnimation()
	{
		if (gameObject.activeInHierarchy == false)
			return;
		if (m_AniData == null)
			return;
		CCAnimationData pAnimData = m_AniData.GetNextAnimData();
		if (pAnimData != null)
			Play(pAnimData);
	}

	public void Stop()
	{
		if (m_Coroutine != null)
		{
			StopCoroutine(m_Coroutine);
			m_Coroutine = null;
		}
	}

	/// <summary>
	/// Modify support AnimationOverrideController dynamic loading animClip;
	/// </summary>
	/// <param name="pAnimData"></param>
	/// <returns></returns>
	IEnumerator CoroutinePlay(CCAnimationData pAnimData)
	{
		m_CurrAnimData = pAnimData;
		AnimationClip animClip = GetAnimationClip(pAnimData.AnimName);

		string animName = pAnimData.AnimName;
		if (animClip == null)
		{
			if (m_AnimatorControl is AnimatorOverrideController)
			{
				if (OverrideAnimatorControllerClip(pAnimData,ref animClip))
				{
					animName = stanceAnimStateName;
				}
				else
				{
					yield break;
				}
			}else
			{
				Debug.LogError("Animation lost : " + animName);
				yield break;
			}
		}

		_CurPlayingClip = animClip;
		if(m_OnPlayAnimation != null)
			m_OnPlayAnimation(pAnimData);

		if (m_CrossEnable)
		{
			animator.CrossFade(animName, 0.1f, 0, pAnimData.normalizedTime);
		}
		else
		{
			animator.Play(animName, 0, pAnimData.normalizedTime);
		}

		#if UNITY_EDITOR
		if(m_LogEnable)
			Debug.Log(Time.time + " - pAnimData : " + animName);
		#endif

		if (m_SoundEnable)
		{
			CocoAudio.StopOnTarget(gameObject);
			CocoAudio.PlayOnTarget(gameObject, pAnimData.Audio);
		}
		float animLength = animClip.length * (1 - pAnimData.normalizedTime) / animator.speed;
		switch (pAnimData.mWrapMode)
		{
			case WrapMode.Loop:
				while (true)
				{
					yield return new WaitForSeconds(animLength);
					if (m_SoundEnable)
					{
					CocoAudio.PlayOnTarget(gameObject, pAnimData.Audio);
					}
				}
				break;
			case WrapMode.ClampForever:
				yield return new WaitForSeconds(animLength);
				while (true)
				{
					yield return new WaitForEndOfFrame();
				}
				break;
			default:    // Clamp
				yield return new WaitForSeconds(animLength);
				break;
		}

		m_CurrAnimData = null;
		if(m_PlayAnimationList != null && m_PlayAnimationList.Count > 0)
		{
			PlayList(m_PlayAnimationList);
		}
		else if (m_AutoSwitchEanble)
		{
			SwitchAnimation();
		}

		if (m_OnFinishAnimation != null)
			m_OnFinishAnimation(pAnimData);
	}
	/// <summary>
	/// dynamic loading animClip
	/// </summary>
	/// <param name="pAnimData"></param>
	/// <param name="clip"></param>
	/// <returns></returns>
	bool OverrideAnimatorControllerClip (CCAnimationData pAnimData,ref AnimationClip clip)
	{
		string tAnimPath = pAnimData.AssetsPath;
		string tAnimClipName = pAnimData.AnimName;
		if (tAnimPath.IsNullOrEmpty() || tAnimClipName.IsNullOrEmpty())
		{
			return false;
		}
		var mAnimationClips = Resources.LoadAll<AnimationClip> (tAnimPath);
		if (mAnimationClips.Length <= 0)
			return false;

		AnimationClip targetClip = null;
		bool tOverrideClip = false;
		mAnimationClips.ForEach (item => {
			if (item.name == tAnimClipName) {
				(m_AnimatorControl as AnimatorOverrideController)[stanceAnimStateName] = item;
				targetClip = item;
				tOverrideClip = true;
			}
		});

		clip = targetClip;

		if (!tOverrideClip)
			return false;
		Resources.UnloadUnusedAssets ();
		return true;

	}

	public void SetSoundEnable(bool enable)
	{
		m_SoundEnable = enable;
	}

	public void StopSound()
	{
		CocoAudio.StopOnTarget(gameObject);
	}

	public float GetAnimatinClipLength(CCAnimationData pAnimData)
	{
		AnimationClip animClip = GetAnimationClip(pAnimData.AnimName);
		if (animClip == null)
			return 0;

		float animLength = animClip.length * (1 - pAnimData.normalizedTime) / animator.speed;
		return animLength;
	}

	public float GetCurAnimationClipLength()
	{
		if (_CurPlayingClip == null)
		{
			return 0;
		}
		return _CurPlayingClip.length;
	}


	Dictionary<string, AnimationClip> m_ClipDict;
	public void SetAnimatorController(string animator_name)
	{
		m_AnimatorControl = ResourceManager.Load<RuntimeAnimatorController>(animator_name);
		//m_AnimatorControl = Instantiate(animatorController);
		m_AnimatorControl = new AnimatorOverrideController (m_AnimatorControl);
		if (m_AnimatorControl == null)
		{
			Debug.LogError("RuntimeAnimatorController is null : " + animator_name);
			return;
		}
		animator.runtimeAnimatorController = m_AnimatorControl;
		m_ClipDict = new Dictionary<string, AnimationClip>();
		foreach (var clip in m_AnimatorControl.animationClips)
		{
			m_ClipDict.Add(clip.name, clip);
		}
	}



	public void CleanAnimatorController ()
	{
		if (animator.runtimeAnimatorController != null)
		{
			Destroy(animator.runtimeAnimatorController);
			animator.runtimeAnimatorController = null;
		}

		m_ClipDict = new Dictionary<string, AnimationClip>();
	}

	public AnimationClip GetAnimationClip(string clip_name)
	{
		if (!m_ClipDict.ContainsKey(clip_name))
		{
			Debug.LogError("CharacterAnimator doesn't contain :" + clip_name);
			return null;
		}

		return m_ClipDict[clip_name];
	}

    public void AddOverrideClip (CCAnimationData pAnimData)
    {
        string tAnimPath = pAnimData.AssetsPath;
        string tAnimClipName = pAnimData.AnimName;
        if (tAnimPath.IsNullOrEmpty() || tAnimClipName.IsNullOrEmpty())
        {
            return;
        }
        var mAnimationClips = Resources.LoadAll<AnimationClip> (tAnimPath);
        if (mAnimationClips.Length <= 0)
            return;

        AnimationClip targetClip = null;
        bool tOverrideClip = false;
        mAnimationClips.ForEach (item =>
        {
            if (item.name == tAnimClipName) {
                (m_AnimatorControl as AnimatorOverrideController)[stanceAnimStateName] = item;
                targetClip = item;
                tOverrideClip = true;
            }
        });

        if (tOverrideClip)
        {
            Resources.UnloadUnusedAssets ();
            m_ClipDict = new Dictionary<string, AnimationClip>();
            foreach (var clip in m_AnimatorControl.animationClips)
            {
                m_ClipDict.Add(clip.name, clip);
            }
        }
    }
}
