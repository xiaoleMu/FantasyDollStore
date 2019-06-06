using UnityEngine;
using System.Collections;

public class CCWait : IEnumerator 
{
	float m_end = 0;
	public CCWait(float time)  
	{  
		m_end = Time.unscaledTime + time; 
	}  
	//-- IEnumerator Interface  
	public object Current  
	{  
		get  
		{
			return MoveNext();
		}  
	}  

	//-- IEnumerator Interface  
	public bool MoveNext()  
	{  
		return Time.unscaledTime < m_end;
	}

	//-- IEnumerator Interface  
	public void Reset()  
	{  
		m_end = 0;
	}  
}  
