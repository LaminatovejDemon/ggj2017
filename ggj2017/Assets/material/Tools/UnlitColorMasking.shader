// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ColorMasking"
{
 // Unlit alpha-blended shader.
 // - no lighting
 // - no lightmap support
 // - no per-material color
 
 Properties {
     _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 }
 
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 100
     
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    
	Pass {
		AlphaTest Greater 0

		Stencil {
			Ref 1
			Comp always
			Pass replace
		}

        Color [_TintColor]
	}	 
 }
}
