Shader "Hidden/NewImageEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" { } //��ͼ
		_Progress("Progress", Range(0,10)) = 1 //��������������ϵ��绬����ֵ���ڲ�shader����ֵ
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
			float _Progress; //�������������������shader������
			float4 _MainTex_ST; //����һ����ά�����������UV����ƽ�Ƶļ���
			fixed4 frag (v2f i) : SV_Target
			{
				_MainTex_ST.xy = 1/_Progress; //������������UV Tiling
				_MainTex_ST.zw = (1/_Progress - 1)*-0.5; //��������������UV Offset
				fixed4 col = tex2D(_MainTex, i.uv*_MainTex_ST.xy+_MainTex_ST.zw); //��󽫼������UVֵ����
				return col;
			}
			ENDCG
		}
	}
}