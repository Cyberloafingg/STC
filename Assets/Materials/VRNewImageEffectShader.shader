Shader "Custom/VRNewImageEffectShader" // ע�⣬�����Shader������Ҫ��"Custom/"ǰ׺��ͷ
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // ��ͼ
        _Progress("Progress", Range(0, 10)) = 1 // ��������������ϵ��绬����ֵ���ڲ�shader����ֵ
    }
    SubShader
    {
        // ѡ����ʵ���Ⱦƽ̨
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
        
        Pass
        {
            HLSLINCLUDE

            // ����ʹ��URP��Built-in��������ȡ���ݵĲ���
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            // ʹ��URP��Built-in������ȡTexture
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Progress;
            float4 _MainTex_ST;

            fixed4 frag (v2f i) : SV_Target
            {
                // ����UV Tiling��Offset
                _MainTex_ST.xy = 1 / _Progress;
                _MainTex_ST.zw = (1 / _Progress - 1) * -0.5;

                // ���ݼ������UVֵ���в���
                fixed4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);

                return col;
            }

            ENDHLSL
        }
    }
}
