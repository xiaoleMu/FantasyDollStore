using UnityEngine;
using System.Collections;

public class SimpleColor {

	public float r;
	public float g;
	public float b;
	public float a;

	public SimpleColor()
	{
		
	}

	public SimpleColor(float r = 1f, float g = 1f, float b = 1f, float a = 1f)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}

	public Color ToUnityColor()
	{
		return new Color(r, g, b, a);
	}

	public static Color GetUnityColor(SimpleColor col)
	{
		return new Color(col.r, col.g, col.b, col.a);
	}

	public static SimpleColor GetSimpleColor(Color col)
	{
		return new SimpleColor(col.r, col.g, col.b, col.a);
	}
}
