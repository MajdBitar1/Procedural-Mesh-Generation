// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DetailedBasicShader"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _Maintex ("Texture", 2D) = "white" {}
        _Detailtex ("Detail Texture", 2D) = "gray" {}
        }
    SubShader
    {
        Pass {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #include "UnityCG.cginc"
            float4 _Tint;
            sampler2D _Maintex, _Detailtex;
            float4 _Maintex_ST, _Detailtex_ST;
            

            struct vertexinput 
            {
                float4 position: POSITION;
                float2 uv : TEXCOORD0;
            };
            struct fraginput 
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvDetail : TEXCOORD1;
            };

            fraginput MyVertexProgram(vertexinput input)
            {
                fraginput output;
                output.position = UnityObjectToClipPos(input.position);
                //output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.uv = input.uv * _Maintex_ST.xy + _Maintex_ST.zw;
                output.uvDetail = input.uv * _Detailtex_ST.xy + _Detailtex_ST.zw;
                return output;
            }

            float4 MyFragmentProgram(fraginput input) : SV_TARGET
            {
            float4 color = tex2D(_Maintex, input.uv) * _Tint;
            color *= tex2D(_Detailtex, input.uvDetail) * unity_ColorSpaceDouble;
            return color;
            }
            ENDCG
            }
    }
}
