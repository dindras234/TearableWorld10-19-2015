Shader "Two Layer Terrain DOUBLE" {


Properties
{
	_Color ("Main Color", Color) = (1,1,1)
	_MainTex ("Main Texture (RGB)", 2D) = ""
	_PathTex ("Path Texture (RGB)", 2D) = ""
	_PathMask ("Path Mask (A)", 2D) = ""
}

SubShader 
{
	Lighting On
	
	Material 
	{
		Ambient [_Color]
		Diffuse [_Color]
	}

	Pass
	{		
		SetTexture [_MainTex]
		
		SetTexture [_PathMask]
		{
			combine previous, texture
		}
				
		SetTexture [_PathTex]
		{
			combine texture lerp(previous) previous
		}
		
		SetTexture [_MainTex]
		{
			combine previous * primary DOUBLE
		}
	}
}

SubShader 
{
	Lighting On
	
	Material 
	{
		Ambient [_Color]
		Diffuse [_Color]
	}
	
	/* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */

	Pass
	{	
		SetTexture [_MainTex]
		{
			combine texture * primary
		}
		
		SetTexture [_PathMask]
		{
			combine previous * one - texture alpha DOUBLE
		}
	}
	
	Pass
	{			
		SetTexture [_PathTex]
		{
			combine texture * primary
		}
		
		SetTexture [_PathMask]
		{
			combine previous * texture alpha DOUBLE
		}
	}
}

Fallback "Diffuse"

 
}