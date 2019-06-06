using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CCDark
{
	public static void SetImage(Image image, int value = 150)
	{
		if(image == null)
			return;
		float dark = value/255f;
		Color color = image.color;
		color.r = dark;
		color.g = dark;
		color.b = dark;
		image.color = color;
	}

	public static void SetImage(GameObject obj, int value = 150)
	{
		if(obj == null)
			return;
		Image img = obj.GetComponent<Image>();
		CCDark.SetImage(img, value);
	}
}
