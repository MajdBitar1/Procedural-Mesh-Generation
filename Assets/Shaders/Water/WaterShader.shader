Shader "Custom/WaterShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _FlowMap ("Flow (RG, A noise)",2D) = "black" {}
        //[NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        [NoScaleOffset] _DerivHeightMap ("Deriv (AG) Height (B)", 2D) = "black" {}
        _UJump ("U Jump per Phase", Range(-0.25,0.25)) = 0.25
        _VJump ("V Jump per Phase", Range(-0.25,0.25)) = 0.25
        _Tiling ("Tiling", Float) = 1
        _Speed ("Speed", Float) = 1
        _FlowStrength ("Flow Strength", Float) = 1
        _FlowOffset ("Flow Offset", Float) = 0
        _HeightScale ("Height Scale, Constant", Float) = 0.25
        _HeightScaleModulated("Height Scale, Modulated", Float) = 0.75
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        #include "Flow.cginc"

        sampler2D _MainTex,_FlowMap,_DerivHeightMap; //,_NormalMap;
        float _UJump,_VJump,_Tiling,_Speed,_FlowStrength,_FlowOffset;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _HeightScale, _HeightScaleModulated;

        float3 UnPackDerivativeHeight (float4 textureData)
        {
            float3 dh = textureData.agb;
            dh.xy = dh.xy * 2 -1;
            return dh;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //float2 flowVector = tex2D (_FlowMap, IN.uv_MainTex.rg) * 2 - 1;
            //flowVector *= _FlowStrength;
            float3 flow = tex2D(_FlowMap, IN.uv_MainTex).rgb;
            flow.xy = flow.xy * 2 -1;
            flow *= _FlowStrength;
            float noise = tex2D(_FlowMap, IN.uv_MainTex).a;
            float time = _Time.y * _Speed + noise;
            float2 jump = float2(_UJump,_VJump);
            // Albedo comes from a texture tinted by color
            float3 uvwA = FlowUVW(IN.uv_MainTex,flow.xy,jump,
                                   _FlowOffset,_Tiling,time,false);
            float3 uvwB = FlowUVW(IN.uv_MainTex,flow.xy,jump,
                                   _FlowOffset,_Tiling,time,true);
            
            //float3 normalA = UnpackNormal( tex2D (_NormalMap, uvwA.xy)) * uvwA.z;
            //float3 normalB = UnpackNormal( tex2D (_NormalMap, uvwB.xy)) * uvwB.z;
            //o.Normal = normalize (normalA + normalB);
            float3 finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;
            float3 dhA = UnPackDerivativeHeight(tex2D (_DerivHeightMap, uvwA.xy)) * (uvwA.z * finalHeightScale);
            float3 dhB = UnPackDerivativeHeight(tex2D (_DerivHeightMap, uvwB.xy)) * (uvwB.z * finalHeightScale);
            o.Normal = normalize( float3(-(dhA.xy,dhB.xy), 1) );

            fixed4 texA = tex2D (_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D (_MainTex, uvwB.xy) * uvwB.z;

            fixed4 c = (texA + texB) * _Color;

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
