using UnityEngine;
using System.Collections;

public class CCInput
{
	public static bool GetKeyDown(KeyCode code)
	{
		#if UNITY_EDITOR
		return Input.GetKeyDown(code);
		#endif
		return false;
	}

	public static bool Test()
	{
		#if UNITY_EDITOR
		return Input.GetKeyDown(KeyCode.A);
		#endif
		return false;
	}
}
