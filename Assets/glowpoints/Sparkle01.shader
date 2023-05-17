Shader "Custom/Actor/Sparkle"
{
    Properties
    {
        _NoiseTex("Noise Tex", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Sparkle("_Sparkle", Range(0.75, 5)) = 1
        _HighlightRange("Highlight Range", Range(0, 5)) = 1
        [HDR]_SparkleColor("Sparkle Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            LOD 200

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _NoiseTex;
                float4 _NoiseTex_ST;
                half4 _Color;

                float _Sparkle;
                half4 _SparkleColor;
                float _HighlightRange;

                struct appdata
                {
                    float4 vertex	: POSITION;
                    float3 normal	: NORMAL;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos		: SV_POSITION;
                    float3 normal	: TEXCOORD0;
                    float3 worldPos	: TEXCOORD1;
                    float2 uv		: TEXCOORD2;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.normal = UnityObjectToWorldNormal(v.normal);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    half3 normal = normalize(i.normal);
                    half3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                    float ndotv = dot(normal, viewDir);
                    half noise0 = tex2D(_NoiseTex, half2(i.uv.x + ndotv, i.uv.y)).r;
                    half noise1 = tex2D(_NoiseTex, half2(i.uv.x, i.uv.y + ndotv)).r;

                    float sparkle = pow(noise0 * noise1, _Sparkle);
                    float highlight = pow(ndotv, _HighlightRange);

                    half3 sparkleColor = sparkle * _SparkleColor.rgb * highlight;

                    return half4(_Color.rgb + sparkleColor, _Color.a);
                }
                ENDCG
            }
        }
}
