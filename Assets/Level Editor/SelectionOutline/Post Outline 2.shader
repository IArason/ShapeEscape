Shader "Custom/Post Outline"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white"{}
		_OutlineWidth("Outline Width", Int) = 5
		_OutlineColor("Front Color", Color) = (0, 1, 1, 1)
		_DottedLine("Dotted Line", Range(0, 1)) = 0
		_HighlightIntensity("Highlight Intensity", Range(0, 1)) = 0.4
	}
		SubShader
	{
		Cull Off 
		ZWrite Off 
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
	{
		CGPROGRAM

		sampler2D _MainTex;

	//<SamplerName>_TexelSize is a float2 that says how much screen space a texel occupies.
	float2 _MainTex_TexelSize;

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uvs : TEXCOORD0;
	};

	v2f vert(appdata_base v)
	{
		v2f o;

		//Despite the fact that we are only drawing a quad to the screen, Unity requires us to multiply vertices by our MVP matrix, presumably to keep things working when inexperienced people try copying code from other shaders.
		o.pos = mul(UNITY_MATRIX_MVP,v.vertex);

		//Also, we need to fix the UVs to match our screen space coordinates. There is a Unity define for this that should normally be used.
		o.uvs = o.pos.xy / 2 + 0.5;

		return o;
	}

	int _OutlineWidth;
	float4 _OutlineColor;
	float _HighlightIntensity;
	int _DottedLine;

	half4 frag(v2f i) : COLOR
	{
	//arbitrary number of iterations for now
	int NumberOfIterations = _OutlineWidth * 2 + 1;

	//split texel size into smaller words
	float TX_x = _MainTex_TexelSize.x;
	float TX_y = _MainTex_TexelSize.y;

	//and a final intensity that increments based on surrounding intensities.
	float ColorIntensityInRadius;

	//if something already exists underneath the fragment, discard the fragment.


	//for every iteration we need to do horizontally

	for (int k = 0; k<1; k += 1)
	{
		//for every iteration we need to do vertically
		for (int j = 0; j<NumberOfIterations; j += 1)
		{
			//increase our output color by the pixels in the area
			ColorIntensityInRadius += tex2D(
				_MainTex,
				i.uvs.xy + float2
					(
					(k + NumberOfIterations / 2)*TX_x,
					0
					)
				).r // Apply dotted line
				* max(abs(_DottedLine - 1), clamp(round((abs((i.pos.x + i.pos.y - _Time * 500) / 20)) % 1.0), 0, 1));
		}
	}

	if (tex2D(_MainTex, i.uvs.xy).r>0)
	{
		ColorIntensityInRadius = _HighlightIntensity;
	}

	// Clamp to prevent overload artifacting
	ColorIntensityInRadius = clamp(ColorIntensityInRadius, 0, 1);

	//output some intensity of teal
	return ColorIntensityInRadius * _OutlineColor;
	}

		ENDCG

	}
		//end pass        
	}
		//end subshader
}
//end shader