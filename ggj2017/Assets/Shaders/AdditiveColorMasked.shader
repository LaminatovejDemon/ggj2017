// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "StencilMask/AdditiveColorMasked"
{
	Properties {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    }
 
    Category {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha One
        AlphaTest Greater .01
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
        BindChannels {
            Bind "Color", color
            Bind "Vertex", vertex
        }
        
        // ---- Fragment program cards
        SubShader {
            Pass {

                Stencil {
                    Ref 1
                    Comp equal
                    Pass replace
                    Zfail decrWrap
                }
            
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma multi_compile_particles
    
                #include "UnityCG.cginc"
    
                sampler2D _MainTex;
                fixed4 _TintColor;
                
                struct appdata_t {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                };
    
                struct v2f {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                };
                
                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;                 
                    return o;
                }
                
                fixed4 frag (v2f i) : COLOR
                { 
                    return 2.0f * i.color * _TintColor;
                }
                ENDCG 
            }
        }     
        
    }
}
