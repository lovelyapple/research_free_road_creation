Shader "Custom/OutlinedUnlit"
{
    Properties
    {
        _BaseColor   ("Base Color", Color) = (1,0.3,0.3,0.35) // 本体色（半透明推奨）
        _OutlineColor("Outline Color (HDR)", Color) = (1,0.2,0.2,1)
        _OutlineWidth("Outline Width (world)", Range(0,0.5)) = 0.02
    }
    SubShader
    {
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" }

        // --- パス1：本体（半透明） ---
        Pass
        {
            ZWrite Off
            Cull Back
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _BaseColor;
            struct app { float4 v:POSITION; };
            struct v2f { float4 pos:SV_Position; };

            v2f vert(app i){
                v2f o; o.pos = UnityObjectToClipPos(i.v); return o;
            }
            fixed4 frag(v2f i):SV_Target { return _BaseColor; }
            ENDHLSL
        }

        // --- パス2：アウトライン（法線押し出し＆前面カリング） ---
        Pass
        {
            ZWrite On
            // 前面をカリングして“シルエットだけ”見せる
            Cull Front
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            float  _OutlineWidth;

            struct app { float4 v:POSITION; float3 n:NORMAL; };
            struct v2f { float4 pos:SV_Position; };

            v2f vert(app i){
                v2f o;
                // オブジェクト空間の法線方向に押し出し
                float3 n = normalize(i.n);
                float3 offset = n * _OutlineWidth;
                float4 v = i.v + float4(offset, 0);
                o.pos = UnityObjectToClipPos(v);
                return o;
            }
            fixed4 frag(v2f i):SV_Target { return _OutlineColor; }
            ENDHLSL
        }
    }
    FallBack Off
}
