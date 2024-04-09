// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TextureSplatting"
{
    Properties
    {
        _Maintex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _Texture1("Texture 1", 2D) = "white" {}
        [NoScaleOffset] _Texture2("Texture 2", 2D) = "white" {}
        [NoScaleOffset] _Texture3("Texture 3", 2D) = "white" {}
        [NoScaleOffset] _Texture4("Texture 4", 2D) = "white" {}
        
    }
    SubShader
    {
        Pass {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #include "UnityCG.cginc"

            sampler2D _Maintex;
            float4 _Maintex_ST;

            sampler2D _Texture1,_Texture2,_Texture3,_Texture4;
            

            struct vertexinput 
            {
                float4 position: POSITION;
                float2 uv : TEXCOORD0;
            };
            struct fraginput 
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvsplat : TEXCOORD1;
            };

            fraginput MyVertexProgram(vertexinput input)
            {
                fraginput output;
                output.position = UnityObjectToClipPos(input.position); //Submit to Camera Space
                //output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.uv = input.uv * _Maintex_ST.xy + _Maintex_ST.zw; // Apply Tiling and Offset xy is tiling zw is offset
                output.uvsplat = input.uv;
                return output;
            }

            float4 MyFragmentProgram(fraginput input) : SV_TARGET
            {
                float4 splat = tex2D(_Maintex,input.uvsplat);
            return
                tex2D (_Texture1, input.uv) * splat.r + //we can use any color channel from the splat map, r is red but we could have used b or g as well
                tex2D (_Texture2, input.uv) * splat.g +
                tex2D (_Texture3, input.uv) * splat.b +
                tex2D (_Texture4, input.uv) * (1 - splat.r - splat.b - splat.g);
            }
            ENDCG
            }
    }
}
