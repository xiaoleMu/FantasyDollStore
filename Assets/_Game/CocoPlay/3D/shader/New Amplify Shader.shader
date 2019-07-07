// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "diy"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "black" {}
		_decaltexture("decal texture", 2D) = "black" {}
		[NoScaleOffset]_BumpMap("Normal Map", 2D) = "bump" {}
		_tile("tile", Float) = 0
		[NoScaleOffset]_metallicRsmoothGdiffuseB("metallic(R)smooth(G)diffuse(B)", 2D) = "white" {}
		_df("df", Float) = 1.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _BumpMap;
		uniform float _tile;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _metallicRsmoothGdiffuseB;
		uniform float _df;
		uniform sampler2D _decaltexture;
		uniform float4 _decaltexture_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_tile).xx;
			float2 uv_TexCoord4 = i.uv_texcoord * temp_cast_0;
			o.Normal = UnpackNormal( tex2D( _BumpMap, uv_TexCoord4 ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode6 = tex2D( _MainTex, uv_MainTex );
			float4 tex2DNode94 = tex2D( _metallicRsmoothGdiffuseB, uv_TexCoord4 );
			float4 temp_cast_1 = (tex2DNode94.b).xxxx;
			float4 blendOpSrc163 = tex2DNode6;
			float4 blendOpDest163 = temp_cast_1;
			float4 temp_output_163_0 = ( saturate( (( blendOpDest163 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest163 - 0.5 ) ) * ( 1.0 - blendOpSrc163 ) ) : ( 2.0 * blendOpDest163 * blendOpSrc163 ) ) ));
			float4 temp_cast_2 = (_df).xxxx;
			float2 uv_decaltexture = i.uv_texcoord * _decaltexture_ST.xy + _decaltexture_ST.zw;
			float4 tex2DNode159 = tex2D( _decaltexture, uv_decaltexture );
			float4 lerpResult160 = lerp( pow( temp_output_163_0 , temp_cast_2 ) , tex2DNode159 , tex2DNode159.a);
			o.Albedo = lerpResult160.rgb;
			float lerpResult161 = lerp( tex2DNode94.r , 0.0 , tex2DNode159.a);
			o.Metallic = lerpResult161;
			float lerpResult139 = lerp( tex2DNode94.g , 0.0 , tex2DNode159.a);
			o.Smoothness = lerpResult139;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
7;29;1906;1004;3953.742;1592.98;1.580825;True;False
Node;AmplifyShaderEditor.RangedFloatNode;15;-4887.139,-307.2555;Float;False;Property;_tile;tile;3;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-4697.918,-325.4495;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;94;-3913.659,68.26485;Float;True;Property;_metallicRsmoothGdiffuseB;metallic(R)smooth(G)diffuse(B);4;1;[NoScaleOffset];Create;True;0;0;False;0;None;f3a23dc593a513e45a4e2bebc7554ffa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-3837.01,-810.574;Float;True;Property;_MainTex;Albedo;0;0;Create;False;0;0;True;0;83cb88a835d147e489695ca058a3d8c1;ff371780258216d42a3f9943a5958d57;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;-6.79;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;166;-2291.84,-1196.309;Float;False;Property;_df;df;5;0;Create;True;0;0;False;0;1.1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;163;-3193.279,-1319.995;Float;True;Overlay;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;159;-3434.256,-470.3911;Float;True;Property;_decaltexture;decal texture;1;0;Create;True;0;0;False;0;None;None;True;0;False;black;LockedToTexture2D;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;162;1785.708,-541.6381;Float;False;Constant;_Float3;Float 3;17;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;164;-1906.286,-1300.012;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;142;802,-390;Float;False;Constant;_Float0;Float 0;17;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;139;995.3999,-369;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;-2914.921,-774.754;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;160;-1590.801,-1221.315;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;161;1923.7,-686.2745;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-3913.898,-373.8042;Float;True;Property;_BumpMap;Normal Map;2;1;[NoScaleOffset];Create;False;0;0;True;0;None;86c9879e6fab7794c884e826137710c3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;96;-1069.108,-1131.872;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;diy;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;True;True;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;True;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;15;0
WireConnection;94;1;4;0
WireConnection;163;0;6;0
WireConnection;163;1;94;3
WireConnection;164;0;163;0
WireConnection;164;1;166;0
WireConnection;139;0;94;2
WireConnection;139;1;142;0
WireConnection;139;2;159;4
WireConnection;40;0;163;0
WireConnection;40;1;6;0
WireConnection;40;2;6;4
WireConnection;160;0;164;0
WireConnection;160;1;159;0
WireConnection;160;2;159;4
WireConnection;161;0;94;1
WireConnection;161;1;162;0
WireConnection;161;2;159;4
WireConnection;10;1;4;0
WireConnection;96;0;160;0
WireConnection;96;1;10;0
WireConnection;96;3;161;0
WireConnection;96;4;139;0
ASEEND*/
//CHKSM=48DCD51184336CBF4D40D59F1E52EDCC5262DC59