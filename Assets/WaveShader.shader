//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/WaveShader"
{
	SubShader
	{
        Pass
        {
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vertIn
            {
                float4 vertex : POSITION;
            };

            struct vertOut
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            // Implementation of the vertex shader
            vertOut vert(vertIn v)
            {
                // Displace the original vertex in model space

                vertOut o;
                float4 vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                float4 displacement = float4(0.0f, 0.03 * sin(10*vertex.x + 2*_Time.y), 0.0f, 0.0f);
                vertex += displacement;
                o.vertex = vertex;
                o.color = float4(0.5f, 0.5f, 0.9f, 0.8f);
                return o;
            }

            // Implementation of the fragment shader
            fixed4 frag(vertOut v) : SV_Target
            {
                return v.color;
            }
            ENDCG
		}
	}
}