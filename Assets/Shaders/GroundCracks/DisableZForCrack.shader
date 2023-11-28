Shader "Custom/DisableZForCrack"
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