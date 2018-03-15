// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "StencilMask/SpriteMasking"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaCutout ("AlphaCutout", Range(0.0,1.0)) = 0.05
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }

		Blend One OneMinusSrcAlpha
    
		Pass {
			AlphaTest Greater [_AlphaCutout]

			Stencil {
				Ref 1
				Comp always
				Pass replace
			}

			SetTexture [_MainTex] {combine previous * texture}
		}
	}
}
