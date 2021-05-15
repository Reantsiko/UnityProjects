﻿Shader "Custom/PlaneShader"
{
    Properties
    {
        _R ("Red", Range(0,1)) = 1
        _B ("Blue", Range(0,1)) = 1
        _G ("Green", Range(0,1)) = .5
        _A ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 200
            

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float _R;
        float _B;
        float _G;
        float _A;

        struct Input
        {
            float2 uv_MainTex;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            _R = sin(_R + _SinTime.x);
            _G = cos(_G - _SinTime.y);
            _B = tan(_B + _SinTime.z);
            o.Albedo = float3(_R, _G, _B);
            o.Alpha = _A;
            
        }
        ENDCG
    }
    //FallBack "Diffuse"
}
