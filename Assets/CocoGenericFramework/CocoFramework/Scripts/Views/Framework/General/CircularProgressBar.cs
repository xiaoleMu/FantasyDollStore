using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CircularProgressBar : MonoBehaviour
{

	public Image progressBarFront;

	int fillDirection = 1;

	float resolution = 0.01f;
	float speed = 1.2f;


	void Start ()
	{
		StartProgressUpdate ();
	}

	void StartProgressUpdate ()
	{
		StartCoroutine ("FillCoro");
	}

	IEnumerator FillCoro ()
	{

		while (true) {
			float fillAmount = progressBarFront.fillAmount;
			
			if ((fillAmount <= 0 && fillDirection > 0) || (fillAmount >= 1 && fillDirection < 0)) {
				progressBarFront.fillClockwise = !progressBarFront.fillClockwise;
				fillDirection = -fillDirection;
			} else {
				progressBarFront.fillAmount -= resolution * speed * fillDirection;
			}
			
			yield return new WaitForSeconds (resolution);
		}
	}
}
