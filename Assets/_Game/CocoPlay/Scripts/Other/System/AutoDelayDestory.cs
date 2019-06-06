using UnityEngine;
using System.Collections;

public class AutoDelayDestory : MonoBehaviour
{
	[SerializeField]
	private float delayTime = 1f;

	void Start ()
	{
		LeanTween.scale (gameObject, transform.localScale, delayTime).setOnComplete (ForDestory);
	}

	void ForDestory ()
	{
		Destroy (this.gameObject);
	}


	void Destory ()
	{
		LeanTween.cancel (gameObject);
	}
}
