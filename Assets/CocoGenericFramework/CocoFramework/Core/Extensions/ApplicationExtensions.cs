using System;
using UnityEngine;

public static class ApplicationExtensions
{
	public static bool IsStandalone(this RuntimePlatform r)
	{
		return(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer);
	}
}


