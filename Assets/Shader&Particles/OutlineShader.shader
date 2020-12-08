
Shader "Unlit/OutlineShader"
{
	Properties
	{
		_OutlineColor("OL Colour", Color) = (1,0,0,1)
		_Outline("OL width", Range(0.0, 0.15)) = 0.0347
		_OutlineOffset("OL Offset", Vector) = (0, 0, 0)
		_Alpha("Alpha", Float) = 1
	}

			CGINCLUDE
	#include "UnityCG.cginc"

				struct appdata {
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				half4 pos : POSITION;
				half2 uv : TEXCOORD0;
				half3 normalDir : NORMAL;
			};

			uniform half4 _Color;
			uniform half _Outline;
			uniform half4 _OutlineColor;

			ENDCG
				SubShader{
									Tags { "Queue" = "Transparent" }

									Pass {
										Name "STENCIL"
										ZWrite Off
										ZTest Always
										ColorMask 0

										Stencil {
											Ref 2
											Comp always
											Pass replace
											ZFail decrWrap
										}

										CGPROGRAM

										#pragma vertex vert2
										#pragma fragment frag

										v2f vert2(appdata v)
										{
											v2f o;
											o.pos = UnityObjectToClipPos(v.vertex);

											return o;
										}

										half4 frag(v2f i) : COLOR
										{
											return _Color;
										}

										ENDCG


									}

									Pass {
										Name "OUTLINE"
										Tags { "LightMode" = "Always" }
										Cull Off
										ZWrite Off
										ColorMask RGB

										Blend SrcAlpha OneMinusSrcAlpha

										Stencil {
											Ref 2
											Comp NotEqual
											Pass replace
											ZFail decrWrap
										}

										CGPROGRAM
										#pragma vertex vert
										#pragma fragment frag

										half3 _OutlineOffset;

										v2f vert(appdata v) {
											v2f o;

											half3 vertex = v.vertex.xyz;
											vertex -= _OutlineOffset;
											vertex.x *= _Outline + 1;
											vertex.y *= _Outline + 1;
											vertex.z *= _Outline + 1;
											vertex += _OutlineOffset;
											o.pos = UnityObjectToClipPos(half4(vertex, v.vertex.w));

											return o;
										}

										half _Alpha;
										half4 frag(v2f i) :COLOR {
											return half4(_OutlineColor.rgb, _Alpha);
										}
										ENDCG
									}

			}

				Fallback Off
	}
