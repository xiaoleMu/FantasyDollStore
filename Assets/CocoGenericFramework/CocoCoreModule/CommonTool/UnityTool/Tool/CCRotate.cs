using UnityEngine;
using System.Collections;

public class CCRotate : MonoBehaviour
{
	public float speed = 2;
	public bool m_Rotate_X;
	public bool m_Rotate_Y;
	public bool m_Rotate_Z;

	void Update () 
	{
		if (m_Rotate_X && m_Rotate_Y && m_Rotate_Z){
			transform.Rotate(new Vector3(0,0, speed * Time.deltaTime)); 
		}
		else {
			if(m_Rotate_X)
				transform.Rotate(new Vector3(speed * Time.deltaTime, 0, 0)); 
			if(m_Rotate_Y)
				transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0)); 
			if(m_Rotate_Z)
				transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime)); 
		}
	}
}



