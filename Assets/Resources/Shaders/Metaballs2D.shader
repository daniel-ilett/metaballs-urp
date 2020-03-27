Shader "Hidden/Metaballs2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
			int _MetaballCount;
			float3 _MetaballData[100];

			float _CameraSize;

            float4 frag (v2f_img i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);

				float dist = 100;

				float h = 1.0f;

				for (int m = 0; m < _MetaballCount; ++m)
				{
					float2 metaballPos = _MetaballData[m].xy;

					float measure = distance(metaballPos, i.uv * _ScreenParams.xy);
					//measure *= _CameraSize / (_ScreenParams.y / 2.0f) / _MetaballData[m].z;

					// Calculate PIXEL distance.
					//dist = min(dist, measure);

					float radiusSize = _MetaballData[m].z * _ScreenParams.y * 2.0f / _CameraSize;

					h *= saturate(measure / radiusSize);

					//dist /= _MetaballData[m].z;

					// Convert to UV distance.
					//dist /= (_ScreenParams.y * 2);
				}

				//float threshold = _MetaballData[0].z * _ScreenParams.y / (_CameraSize * 2.0f);

				float threshold = 0.125f;

				col = (h > threshold) ? col : float4(0.0f, 0.0f, 0.0f, 1.0f);//((h < 0.5f) ? float4(1.0f, 0.0f, 0.0f, 1.0f) : float4(0.0f, 0.0f, 0.0f, 1.0f));

				return col;

				/*
				if (dist < _MetaballData[0].z * _ScreenParams.y / (_CameraSize * 2.0f))
				{
					return float4(0.0f, 0.0f, 0.0f, 1.0f);
				}
				else
				{
					return col;
				}
				*/
            }
            ENDCG
        }
    }
}
