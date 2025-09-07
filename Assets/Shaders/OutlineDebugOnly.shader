Shader "Custom/FixedPushOutline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0.0, 1.0)) = 0.1
    }

    SubShader
    {
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite On
        Cull Front
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // Z方向に一定距離押し出す
                float4 offset = float4(0, 0, -_OutlineWidth, 0);
                o.pos = UnityObjectToClipPos(v.vertex + offset);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
Shader "Custom/FixedPushOutline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0.0, 1.0)) = 0.1
    }

    SubShader
    {
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite On
        Cull Front
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // Z方向に一定距離押し出す
                float4 offset = float4(0, 0, -_OutlineWidth, 0);
                o.pos = UnityObjectToClipPos(v.vertex + offset);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
