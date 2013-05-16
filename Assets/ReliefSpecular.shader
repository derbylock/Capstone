// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'
// Upgrade NOTE: replaced '_PPLAmbient' with 'UNITY_LIGHTMODEL_AMBIENT'

Shader "Relief Specular"
{
	Properties
	{
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1)
		_Height("Height", Range(0, 0.2)) = 0.05
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range (0, 1)) = 0.078125
		_MainTex("Texture (RGB), Specmap (A)", 2D) = "white" {}
		_NormalTex("Normalmap (RGB), Heightmap (A)", 2D) = "white" {}
	}
	
	#warning Upgrade NOTE: SubShader commented out; uses _ObjectSpaceCameraPos which was removed. You can try computing that from _WorldSpaceCameraPos.
/*SubShader
	{
		Pass
		{
			Name "BASE"
			Tags {"LightMode" = "Always"}
			
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members texcoord,viewdir)
#pragma exclude_renderers d3d11 xbox360
				#pragma target 3.0
				#pragma profileoption MaxTexIndirections=256
				
				#pragma vertex vert
				#pragma fragment frag
				
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#include "UnityCG.cginc"
				
				#define LINEAR_SEARCH 20
				#define BINARY_SEARCH 10
				
				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform sampler2D _NormalTex;
				uniform float4 _NormalTex_ST;
				uniform float _Height;
				
				struct v2f
				{
					float4 pos : SV_POSITION;
					float4 texcoord;
					float3 viewdir;
				}; 

				v2f vert(appdata_tan v)
				{
					v2f o;
					
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _NormalTex);
					
					TANGENT_SPACE_ROTATION;
					o.viewdir = _ObjectSpaceCameraPos-v.vertex;
					o.viewdir = mul(rotation, o.viewdir);
					
					return o;
				}
				
				float ray_intersect_rm(in sampler2D reliefmap, in float2 dp, in float2 ds, in float  bias)
				{
					float size = 1.0/LINEAR_SEARCH; // current size of search window
					float depth = -bias;// current depth position
					int i;
					for(i = 0; i < LINEAR_SEARCH-1; i++)// search front to back for first point inside object 
					{
						float4 t = tex2D(reliefmap,dp+ds*depth).a;
						
						if(depth < t.w-bias)
							depth += size;
					}
					
					for(i = 0; i < BINARY_SEARCH; i++) // recurse around first point (depth) for closest match
					{
						size*=0.5;
						
						float4 t = tex2D(reliefmap,dp+ds*depth).a;
						
						if(depth < t.w-bias)
							depth += (2*size);
						
						depth -= size;
					}
					
					return depth;
				}
				
				float4 frag(v2f i) : COLOR
				{
					i.viewdir = normalize(i.viewdir);
					float2 view = i.viewdir.xy*_Height;
					float depth = ray_intersect_rm(_NormalTex, i.texcoord.zw, view, 1.0f);

					i.texcoord.xy += view*depth;
					float4 color = tex2D(_MainTex, i.texcoord.xy);
					
					return float4(color.rgb*UNITY_LIGHTMODEL_AMBIENT, 1.0);
				}
			ENDCG
		}
		
		Pass
		{
			Name "BASE"
			Tags {"LightMode" = "Pixel"}
			/* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */
			
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members texcoord,viewdir,lightdir)
#pragma exclude_renderers d3d11 xbox360
				#pragma target 3.0
				#pragma profileoption MaxTexIndirections=256
				
				#pragma vertex vert
				#pragma fragment frag
				
				#pragma multi_compile_builtin
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				
				#define LINEAR_SEARCH 20
				#define BINARY_SEARCH 10
				
				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform sampler2D _NormalTex;
				uniform float4 _NormalTex_ST;
				uniform float _Height;
				uniform float _Shininess;
				
				struct v2f
				{
					float4 pos : SV_POSITION;
					LIGHTING_COORDS
					float4 texcoord;
					float3 viewdir;
					float3 lightdir;
				}; 

				v2f vert(appdata_tan v)
				{
					v2f o;
					
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _NormalTex);
					
					TANGENT_SPACE_ROTATION;
					o.viewdir = _ObjectSpaceCameraPos-v.vertex;
					o.viewdir = mul(rotation, o.viewdir);
					
					o.lightdir = ObjSpaceLightDir(v.vertex);
					o.lightdir = mul(rotation, o.lightdir);
					
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					
					return o;
				}
				
				float ray_intersect_rm(in sampler2D reliefmap, in float2 dp, in float2 ds, in float  bias)
				{
					float size = 1.0/LINEAR_SEARCH; // current size of search window
					float depth = -bias;// current depth position
					int i;
					for(i = 0; i < LINEAR_SEARCH-1; i++)// search front to back for first point inside object 
					{
						float4 t = tex2D(reliefmap,dp+ds*depth).a;
						
						if(depth < t.w-bias)
							depth += size;
					}
					
					for(i = 0; i < BINARY_SEARCH; i++) // recurse around first point (depth) for closest match
					{
						size*=0.5;
						
						float4 t = tex2D(reliefmap,dp+ds*depth).a;
						
						if(depth < t.w-bias)
							depth += (2*size);
						
						depth -= size;
					}
					
					return depth;
				}
				
				float4 frag(v2f i) : COLOR
				{
					i.viewdir = normalize(i.viewdir);
					float2 view = i.viewdir.xy*_Height;
					float depth = ray_intersect_rm(_NormalTex, i.texcoord.zw, view, 1.0f);

					float2 offset = view*depth;
					float4 color = tex2D(_MainTex, i.texcoord.xy+offset);
					
					float4 normal = tex2D(_NormalTex, i.texcoord.zw+offset);
					normal.rgb = normalize(normal*2.0-1.0);
					
					return SpecularLight(i.lightdir, i.viewdir, normal.rgb, color, _Shininess*128, LIGHT_ATTENUATION(i));
				}
			ENDCG
		}
	}*/
	
	FallBack "Parallax Specular"
}
