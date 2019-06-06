using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour {

	public void KillPopup()
	{
		Destroy (this.gameObject);
	}
}
