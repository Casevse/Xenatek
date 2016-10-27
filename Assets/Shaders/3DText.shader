// Inspiración -> http://wiki.unity3d.com/index.php?title=3DText
// Hay que crear una copia editable de la fuente. Boton arriba a la derecha en la fuente del inspector.

Shader "Custom/3DText" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Font Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Back ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Color [_Color]
			SetTexture [_MainTex] {
				combine primary, texture * primary
			}
		}
	} 
}
