Shader "Custom/VRNewImageEffectShader" // 注意，这里的Shader名称需要以"Custom/"前缀开头
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 贴图
        _Progress("Progress", Range(0, 10)) = 1 // 缩放量，负责联系外界滑动数值与内部shader的数值
    }
    SubShader
    {
        // 选择合适的渲染平台
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
        
        Pass
        {
            HLSLINCLUDE

            // 这里使用URP的Built-in函数来获取传递的参数
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

            // 使用URP的Built-in函数获取Texture
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Progress;
            float4 _MainTex_ST;

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算UV Tiling和Offset
                _MainTex_ST.xy = 1 / _Progress;
                _MainTex_ST.zw = (1 / _Progress - 1) * -0.5;

                // 根据计算出的UV值进行采样
                fixed4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);

                return col;
            }

            ENDHLSL
        }
    }
}
