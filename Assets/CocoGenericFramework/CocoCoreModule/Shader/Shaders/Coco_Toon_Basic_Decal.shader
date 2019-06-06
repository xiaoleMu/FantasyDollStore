Shader "Coco/Toon/Basic (Decal)" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" {}
		_DecalTex ("Decal", 2D) = "black" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }

		Pass {
			Name "BASE"

			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _DecalTex;
            float4 _DecalTex_ST;

            #define COCO_APPDATA float2 texcoord1 : TEXCOORD1;
            #define COCO_V2F float2 texcoord1 : NORMAL;
            #define COCO_VERT o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _DecalTex);
            #define COCO_FRAG(tex, col) \
                float4 decal = tex2D(_DecalTex, i.texcoord1);\
                tex.rgb = lerp (tex.rgb, decal.rgb, decal.a);\
                tex *= col;
            
			#include "Assets/CocoGenericFramework/CocoCoreModule/Shader/CGIncludes/CocoCG_Toon.cginc"

			ENDCG
		}
	}

	Fallback "VertexLit"
}
