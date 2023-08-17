Shader "Custom/Shockwave"
{
    Properties
    {
        _MainTex ("Distortion Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0,1)) = 0
        _Radius ("Radius", Range(0,1)) = 0
        _Center ("Center", Vector) = (0.5,0.5,0,0)
        _BlurAmount ("Blur Amount", Range(0,1)) = 0.05
        _NoiseScale ("Noise Scale", Range(0.01,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+50" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always

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

            sampler2D _MainTex;
            float _Distortion;
            float _Radius;
            float4 _Center;
            float _BlurAmount;
            float _NoiseScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 pos = i.uv - _Center.xy;
                float dist = length(pos);
                
                if(dist < _Radius)
                {
                    fixed4 noiseValue = tex2D(_MainTex, i.uv * _NoiseScale);
                    float2 direction = normalize(pos);
                    float2 distortion_direction = direction * (noiseValue.r * 2 - 1) * _Distortion * (_Radius - dist);

                    i.uv += distortion_direction;
                }

                // Sample the texture at the distorted coordinates
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
