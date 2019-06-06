using UnityEngine;
using System.Collections;

public class CCAnimationTest : MonoBehaviour 
{
	[SerializeField]
	WrapMode warpmode = WrapMode.Clamp;

	[SerializeField]
	string m_AniName;

	Animation m_Animation;

	void Start () 
	{
		m_Animation = GetComponent<Animation>();
	}
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.P) && m_Animation!=null)
		{
			m_Animation.wrapMode = warpmode;
			m_Animation.Play(m_AniName);
		}
	}
}
