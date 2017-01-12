// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable

Shader "Unlit/UnlitTriplanar"
{
	Properties
	{
		_FrontTex("Texture", 2D) = "white" {}
		_WallTex("Texture", 2D) = "white" {}
		_FloorTex("Texture", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1.0
		_WallColor("Side Color", Color) = (1, 1, 1, 1)
		_FloorColor("Floor Color", Color) = (1, 1, 1, 1)
		_FrontColor("Front Color", Color) = (1, 1, 1, 1)
		_XOffset("X Offset", Range(0.0, 1.0)) = 0.0
		_YOffset("Y Offset", Range(0.0, 1.0)) = 0.0
		_Brightness("Front Brightness", Float) = 1.0
	}
		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

	struct v2f
	{
		half3 normal : TEXCOORD0;
		float2 uv : TEXCOORD2;
		float4 pos : SV_POSITION;
		float4 tangent : TANGENT;
		float3 worldPos : TEXCOORD3;
	};
	float _Tiling;
	float4 _WallColor;
	float4 _FloorColor;
	float4 _FrontColor;
	float _XOffset;
	float _YOffset;

	v2f vert(float4 pos : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0, float4 tangent : TANGENT)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, pos);
		o.normal = mul(unity_ObjectToWorld, normal);
		o.tangent = tangent;
		o.worldPos = mul(unity_ObjectToWorld, float4(pos.x - _XOffset, pos.y - _YOffset, pos.z, pos.w) * _Tiling);
		o.uv = uv;
		return o;
	}

	sampler2D _FrontTex;
	sampler2D _WallTex;
	sampler2D _FloorTex;
	float _Brightness;

	fixed4 frag(v2f i) : SV_Target
	{
		// use absolute value of normal as texture weights
		//float3 blend = min(1, max(1 - i.normal.z, 0));
		half3 blend = abs(i.normal);
		// make sure the weights sum up to 1 (divide by sum of x+y+z)
		blend /= dot(blend,1.0);
		// read the three texture projections, for x,y,z axes
		fixed4 frontTex = tex2D(_FrontTex, i.worldPos.xy);
		fixed4 sideTex = tex2D(_WallTex, i.worldPos.yz);
		fixed4 floorTex = tex2D(_FloorTex, i.worldPos.xz);
		// blend the textures based on weights

		fixed4 c = frontTex * _FrontColor * blend.z * _Brightness +
			sideTex * _WallColor *  blend.x * _Brightness +
			floorTex * _FloorColor * blend.y * _Brightness;
		return c;
	}
		ENDCG
	}
	}
}