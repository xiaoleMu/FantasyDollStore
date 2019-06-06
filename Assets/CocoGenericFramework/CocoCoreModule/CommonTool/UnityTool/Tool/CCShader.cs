using UnityEngine;
using System.Collections;

public enum CCShaderType
{
	Transparent_Non,
	Transparent_Half,
	Transparent_All
}

public class CCShader {

	public static void Set(MeshRenderer pRender, CCShaderType type)
	{
		if(pRender == null) return;
		switch(type)
		{
		case CCShaderType.Transparent_Non:
			pRender.sharedMaterial.shader = Shader.Find("CUSTOM/Toon/Transparent");
			break;
		case CCShaderType.Transparent_Half:
			pRender.sharedMaterial.shader = Shader.Find("CUSTOM/Toon/Transparent (Queue Deferred)");
			break;
		case CCShaderType.Transparent_All:
			pRender.sharedMaterial.shader = Shader.Find("CUSTOM/Toon/Transparent_Cutout");
			break;
		}
	}

	public const string MainTexture = "_MainTex";
}
