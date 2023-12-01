Shader "Unlit/StencilMask"
{
	SubShader{
		Tags{
			"RenderType" = "Opaque"
		}

		Pass{
			ZWrite Off
		}
	}
}
