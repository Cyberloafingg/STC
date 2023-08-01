Shader "Hidden/NewImageEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" { } //贴图
		_Progress("Progress", Range(0,10)) = 1 //缩放量，负责联系外界滑动数值与内部shader的数值
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			sampler2D _MainTex;
			float _Progress; //声明缩放量，负责参与shader中运算
			float4 _MainTex_ST; //声明一个四维数，负责进行UV缩放平移的计算
			fixed4 frag (v2f i) : SV_Target
			{
				_MainTex_ST.xy = 1/_Progress; //将缩放量赋给UV Tiling
				_MainTex_ST.zw = (1/_Progress - 1)*-0.5; //根据缩放量计算UV Offset
				fixed4 col = tex2D(_MainTex, i.uv*_MainTex_ST.xy+_MainTex_ST.zw); //最后将计算完的UV值附上
				return col;
			}
			ENDCG
		}
	}
}