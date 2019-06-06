#ifndef COCOCG_REFLECTIVE
#define COCOCG_REFLECTIVE


// pragmas -------------------------------------

// custom -------------------------------------
#ifndef COCO_FRAG_DISCOLOR
    #define COCO_FRAG_DISCOLOR(tex)
#endif

#ifndef COCO_INPUTDATA
	#define COCO_INPUTDATA
#endif
#ifndef COCO_SURF
	#define COCO_SURF(tex, botTex, col, texColorFactor) tex.rgb *= lerp (1, col.rgb, texColorFactor); botTex.rgb *= col.rgb;
#endif
#ifndef COCO_SURF_REFL
	#define COCO_SURF_REFL(reflTex, reflCol) reflTex *= reflCol;
#endif

// properties -------------------------------------
fixed4 _Color;
fixed4 _ReflectColor;
half _Shininess;

sampler2D _MainTex;
sampler2D _BumpMap;
samplerCUBE _Cube;

sampler2D _BottomTex;
sampler2D _PropertyTex;

// color blend factor for base texture
half _BaseColorFactor;

// structs -------------------------------------
struct Input {
	half2 uv_MainTex;
	half3 viewDir;
	float2 uv_BottomTex;
	float2 uv_PropertyTex;
	float2 uv_BumpMap;
	float3 worldRefl;
	
	COCO_INPUTDATA
	
	INTERNAL_DATA
};


// surface function -------------------------------------
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
	fixed4 botTex = tex2D(_BottomTex, IN.uv_BottomTex);
	fixed4 proTex = tex2D (_PropertyTex, IN.uv_PropertyTex);

    COCO_FRAG_DISCOLOR(tex)
	COCO_SURF(tex, botTex, _Color, _BaseColorFactor)

    o.Albedo = tex.rgb * proTex.r + botTex.rgb * (1 - proTex.r);
    
	o.Gloss = tex.a;
	o.Specular = _Shininess;
	
	if (proTex.r > 0) {
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	} else {
		// default value
		o.Normal = float3 (0, 0, 1);
		_SpecColor = 0;
	}
	
    float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	fixed4 reflcol = texCUBE (_Cube, worldRefl);
	reflcol *= tex.a * proTex.r;
	
	COCO_SURF_REFL(reflcol, _ReflectColor)
	
	o.Emission = reflcol.rgb;
	
	o.Alpha = proTex.g;
}

#endif
