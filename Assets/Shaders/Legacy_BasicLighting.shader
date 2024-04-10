Shader "Custom/BasicLighting"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _FresGlow ("Fresnel Glow", Color) = (0,0,1,1)
        _Maintex ("Texture", 2D) = "white" {}
        _Gloss ("Gloss", Range(0,1)) = 0.5
        }
    SubShader
    {
        Pass {
            Tags {
                "RenderType" = "Opaque"
                }
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityShaderVariables.cginc"
            #include "AutoLight.cginc"
            float4 _Tint;
            float4 _FresGlow;
            float _Gloss;
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
                float3 worldPos: TEXCOORD2;
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
                output.worldPos = mul(unity_ObjectToWorld, input.position).xyz;

                
                return output;
            }

            float4 MyFragmentProgram(fraginput input) : SV_TARGET
            {
                input.normal = normalize(input.normal); 
                // re-normalize cause when u pass from vertex to fragment phase the interpolator will cause in normals != 1, renormalize to achieve that.
                // the error is usually low, so for performance reasons you can skip this step

                // TO GET DIFFUSE LIGHT: LAMBERT'S COSINE LAW, WE NEED NORMAL AND DIRECTION OF LIGHT AND COMPUTE DOT PRODUCT
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 albedo = tex2D(_Maintex, input.uv).rgb * _Tint.rgb;
                float3 lambert =  DotClamped(lightDir,input.normal);
                float3 diffuse = albedo * lightColor * lambert;

                //Specular LIGHT
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPos);

                float3 halfvector = normalize(lightDir + viewDir);
                //float3 ReflectedLight = reflect(-lightDir, input.normal); // uses Phong
                float3 specularlight = DotClamped( halfvector, input.normal) * (lambert > 0 );
                //specular exponent
                float specularExponent = exp2(_Gloss * 6) + 2;
                specularlight = pow(specularlight, specularExponent); 
                specularlight *= _LightColor0.rgb;


                //Fresnel Effect!
                float fresnel = DotClamped(viewDir, input.normal); // This produces Brighter Center and darker Edges
                // For Darker Center and Ligher Edges use 1 - fresnel

                float3 effect = ( (1-fresnel) * (cos(_Time.y) + 1) * 0.5) * _FresGlow.rgb;

                return float4( diffuse + specularlight + effect,1);
            }
            ENDCG
            }
    }
}
