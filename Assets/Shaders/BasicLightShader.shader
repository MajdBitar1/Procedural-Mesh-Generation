Shader "Unlit/BasicLightShader"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _Maintex ("Texture", 2D) = "white" {}
        }
    SubShader
    {
        Pass {
            Tags {
                "LightMode" = "UniversalForward"
                }
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityShaderVariables.cginc"
            float4 _Tint;
            sampler2D _Maintex;
            float4 _Maintex_ST;
            

            struct vertexinput 
            {
                float4 position: POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            struct fraginput 
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            fraginput MyVertexProgram(vertexinput input)
            {
                fraginput output;
                output.position = UnityObjectToClipPos(input.position); // output.position = mul(UNITY_MATRIX_MVP, input.position);
                //output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.uv = input.uv * _Maintex_ST.xy + _Maintex_ST.zw;

                output.normal = UnityObjectToWorldNormal(input.normal); // output.normal = mul ( transpose ( (float3x3)unity_WorldToObject ),input.normal ); 
                // we can also multiply by the whole tranformation matrix
                // output.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                // unity will eliminate any multiplication with 0
                output.normal = normalize(output.normal);

                
                return output;
            }

            float4 MyFragmentProgram(fraginput input) : SV_TARGET
            {
                input.normal = normalize(input.normal); 
                // re-normalize cause when u pass from vertex to fragment phase the interpolator will cause in normals != 1, renormalize to achieve that.
                // the error is usually low, so for performance reasons you can skip this step

                // TO GET DIFFUSE LIGHT: LAMBERT'S COSINE LAW, WE NEED NORMAL AND DIRECTION OF LIGHT AND COMPUTE DOT PRODUCT
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                return  DotClamped(lightDir, input.normal);
            }
            ENDCG
            }
    }
}
