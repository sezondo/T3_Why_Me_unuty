// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_VFX_Dissolve_Newest"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_IsAdd("Is Add", Float) = 0
		_CameraOffset("Camera Offset", Float) = 0
		_Color("Color", Color) = (1,1,1,0)
		_Emissive("Emissive", Float) = 1
		_GlobalSpeed("Global Speed", Float) = 1
		_Dissolve("Dissolve", Float) = 0
		_OpacityBoost("Opacity Boost", Float) = 5
		_DepthFade("Depth Fade", Float) = 1
		[Space(33)][Header(Noise 01)][Space(13)]_Noise01Texture("Noise 01 Texture", 2D) = "white" {}
		_Noise01UVScale("Noise 01 UV Scale", Vector) = (0.8,0.8,0,0)
		_Noise01PanSpeed("Noise 01 Pan Speed", Vector) = (0.5,0.5,0,0)
		[Space(33)][Header(Noise 02)][Space(13)]_Noise02Texture("Noise 02 Texture", 2D) = "white" {}
		_Noise02UVScale("Noise 02 UV Scale", Vector) = (1,1,0,0)
		_Noise02PanSpeed("Noise 02 Pan Speed", Vector) = (-0.2,0.4,0,0)
		_NoisesPower("Noises Power", Float) = 1
		_NoisesMultiply("Noises Multiply", Float) = 1
		[Space(33)][Header(Mask)][Space(13)]_MaskTexture("Mask Texture", 2D) = "white" {}
		_MaskUVScale("Mask UV Scale", Vector) = (1,1,0,0)
		_MaskUVOffset("Mask UV Offset", Vector) = (0,0,0,0)
		_MasksSpeed("Masks Speed", Float) = 0
		_MaskPower("Mask Power", Float) = 1
		_MaskMultiply("Mask Multiply", Float) = 1
		_MaskDistortion("Mask Distortion", Float) = 0
		[Space(33)][Header(Mask Move)][Space(13)]_MaskMoveTexture("Mask Move Texture", 2D) = "white" {}
		_MaskMoveUVScale("Mask Move UV Scale", Vector) = (1,1,0,0)
		_MaskMovePower("Mask Move Power", Float) = 1
		_MaskMoveMultiply("Mask Move Multiply", Float) = 1
		[Space(33)][Header(Noise Distortion)][Space(13)]_DistortionTexture("Distortion Texture", 2D) = "white" {}
		_DistortionUVScale("Distortion UV Scale", Vector) = (1,1,0,0)
		_DistortionPanSpeed("Distortion Pan Speed", Vector) = (0.2,0.25,0,0)
		_DistortionMultiply("Distortion Multiply", Float) = 1
		_IsFresnel("Is Fresnel", Float) = 0
		_IsFresnelInverted("Is Fresnel Inverted", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 1
		[Space(33)][Header(AR)][Space(13)]_Cull("Cull", Float) = 2
		_Src("Src", Float) = 5
		_Dst("Dst", Float) = 10
		_ZWrite("ZWrite", Float) = 0
		_ZTest("ZTest", Float) = 2


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		//[HideInInspector][ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull [_Cull]
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend [_Src] [_Dst], One OneMinusSrcAlpha
			ZWrite [_ZWrite]
			ZTest [_ZTest]
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19701
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD3;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _MaskUVScale;
			float2 _MaskMoveUVScale;
			float2 _DistortionUVScale;
			float2 _DistortionPanSpeed;
			float2 _Noise01PanSpeed;
			float2 _Noise02PanSpeed;
			float2 _Noise01UVScale;
			float2 _Noise02UVScale;
			float2 _MaskUVOffset;
			float _NoisesPower;
			float _MaskPower;
			float _NoisesMultiply;
			float _MaskMoveMultiply;
			float _OpacityBoost;
			float _Dissolve;
			float _MaskMovePower;
			float _DepthFade;
			float _MaskMultiply;
			float _Cull;
			float _MasksSpeed;
			float _MaskDistortion;
			float _DistortionMultiply;
			float _GlobalSpeed;
			float _IsFresnel;
			float _IsFresnelInverted;
			float _FresnelPower;
			float _FresnelScale;
			float _CameraOffset;
			float _ZTest;
			float _ZWrite;
			float _Dst;
			float _Src;
			float _IsAdd;
			float _Emissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MaskTexture;
			sampler2D _DistortionTexture;
			sampler2D _MaskMoveTexture;
			sampler2D _Noise01Texture;
			sampler2D _Noise02Texture;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 texCoord97 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord4.xyz = ase_worldNormal;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord5.xy = input.ase_texcoord.xy;
				output.ase_texcoord6 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord4.w = 0;
				output.ase_texcoord5.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( ( ase_worldPos - _WorldSpaceCameraPos ) * ( ( _CameraOffset + texCoord97.y ) * 0.01 ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#ifdef ASE_FOG
					output.fogFactor = ComputeFogFactor( vertexInput.positionCS.z );
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord3 = input.ase_texcoord3;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
				#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
				#endif
				 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_worldNormal = input.ase_texcoord4.xyz;
				float fresnelNdotV134 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode134 = ( 0.0 + _FresnelScale * pow( max( 1.0 - fresnelNdotV134 , 0.0001 ), _FresnelPower ) );
				float temp_output_137_0 = saturate( fresnelNode134 );
				float lerpResult144 = lerp( temp_output_137_0 , ( 1.0 - temp_output_137_0 ) , _IsFresnelInverted);
				float lerpResult138 = lerp( 1.0 , lerpResult144 , _IsFresnel);
				float global_speed22 = ( _GlobalSpeed * _TimeParameters.x );
				float2 texCoord23 = input.ase_texcoord5.xy * _DistortionUVScale + float2( 0,0 );
				float2 panner26 = ( global_speed22 * _DistortionPanSpeed + texCoord23);
				float Distortion36 = ( ( tex2D( _DistortionTexture, panner26 ).g * 0.1 ) * _DistortionMultiply );
				float2 appendResult44 = (float2(_MasksSpeed , 0.0));
				float2 texCoord47 = input.ase_texcoord5.xy * _MaskUVScale + _MaskUVOffset;
				float2 panner56 = ( global_speed22 * appendResult44 + texCoord47);
				float2 texCoord59 = input.ase_texcoord5.xy * _MaskMoveUVScale + float2( 0,0 );
				float2 appendResult57 = (float2(input.ase_texcoord6.z , input.ase_texcoord6.w));
				float4 tex2DNode68 = tex2D( _MaskMoveTexture, ( texCoord59 + appendResult57 ) );
				float temp_output_83_0 = ( saturate( ( saturate( pow( tex2D( _MaskTexture, ( ( Distortion36 * _MaskDistortion ) + panner56 ) ).g , _MaskPower ) ) * _MaskMultiply ) ) * saturate( ( pow( tex2DNode68.g , _MaskMovePower ) * _MaskMoveMultiply ) ) );
				float2 texCoord40 = input.ase_texcoord5.xy * _Noise01UVScale + float2( 0,0 );
				float2 panner48 = ( global_speed22 * _Noise01PanSpeed + texCoord40);
				float2 texCoord38 = input.ase_texcoord5.xy * _Noise02UVScale + float2( 0,0 );
				float2 panner54 = ( global_speed22 * _Noise02PanSpeed + texCoord38);
				float temp_output_65_0 = ( tex2D( _Noise01Texture, ( Distortion36 + panner48 ) ).g * tex2D( _Noise02Texture, ( Distortion36 + panner54 ) ).g );
				float noise84 = saturate( ( saturate( pow( saturate( temp_output_65_0 ) , _NoisesPower ) ) * _NoisesMultiply ) );
				float temp_output_87_0 = ( saturate( temp_output_83_0 ) * noise84 );
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth94 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth94 = saturate( ( screenDepth94 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				float temp_output_120_0 = saturate( ( ( saturate( ( temp_output_87_0 * _OpacityBoost ) ) - ( input.ase_texcoord6.x + _Dissolve ) ) * distanceDepth94 ) );
				float temp_output_121_0 = saturate( ( input.ase_color.a * ( lerpResult138 * temp_output_120_0 ) ) );
				float4 lerpResult131 = lerp( input.ase_color , float4( ( ( (_Color).rgb * (input.ase_color).rgb ) * temp_output_121_0 ) , 0.0 ) , _IsAdd);
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( lerpResult131 * _Emissive ).rgb;
				float Alpha = temp_output_121_0;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(input.positionCS, Color);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, input.fogFactor );
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19701
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _MaskUVScale;
			float2 _MaskMoveUVScale;
			float2 _DistortionUVScale;
			float2 _DistortionPanSpeed;
			float2 _Noise01PanSpeed;
			float2 _Noise02PanSpeed;
			float2 _Noise01UVScale;
			float2 _Noise02UVScale;
			float2 _MaskUVOffset;
			float _NoisesPower;
			float _MaskPower;
			float _NoisesMultiply;
			float _MaskMoveMultiply;
			float _OpacityBoost;
			float _Dissolve;
			float _MaskMovePower;
			float _DepthFade;
			float _MaskMultiply;
			float _Cull;
			float _MasksSpeed;
			float _MaskDistortion;
			float _DistortionMultiply;
			float _GlobalSpeed;
			float _IsFresnel;
			float _IsFresnelInverted;
			float _FresnelPower;
			float _FresnelScale;
			float _CameraOffset;
			float _ZTest;
			float _ZWrite;
			float _Dst;
			float _Src;
			float _IsAdd;
			float _Emissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MaskTexture;
			sampler2D _DistortionTexture;
			sampler2D _MaskMoveTexture;
			sampler2D _Noise01Texture;
			sampler2D _Noise02Texture;


			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 texCoord97 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord3.xyz = ase_worldNormal;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord4.xy = input.ase_texcoord.xy;
				output.ase_texcoord5 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.w = 0;
				output.ase_texcoord4.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( ( ase_worldPos - _WorldSpaceCameraPos ) * ( ( _CameraOffset + texCoord97.y ) * 0.01 ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord3 = input.ase_texcoord3;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_worldNormal = input.ase_texcoord3.xyz;
				float fresnelNdotV134 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode134 = ( 0.0 + _FresnelScale * pow( max( 1.0 - fresnelNdotV134 , 0.0001 ), _FresnelPower ) );
				float temp_output_137_0 = saturate( fresnelNode134 );
				float lerpResult144 = lerp( temp_output_137_0 , ( 1.0 - temp_output_137_0 ) , _IsFresnelInverted);
				float lerpResult138 = lerp( 1.0 , lerpResult144 , _IsFresnel);
				float global_speed22 = ( _GlobalSpeed * _TimeParameters.x );
				float2 texCoord23 = input.ase_texcoord4.xy * _DistortionUVScale + float2( 0,0 );
				float2 panner26 = ( global_speed22 * _DistortionPanSpeed + texCoord23);
				float Distortion36 = ( ( tex2D( _DistortionTexture, panner26 ).g * 0.1 ) * _DistortionMultiply );
				float2 appendResult44 = (float2(_MasksSpeed , 0.0));
				float2 texCoord47 = input.ase_texcoord4.xy * _MaskUVScale + _MaskUVOffset;
				float2 panner56 = ( global_speed22 * appendResult44 + texCoord47);
				float2 texCoord59 = input.ase_texcoord4.xy * _MaskMoveUVScale + float2( 0,0 );
				float2 appendResult57 = (float2(input.ase_texcoord5.z , input.ase_texcoord5.w));
				float4 tex2DNode68 = tex2D( _MaskMoveTexture, ( texCoord59 + appendResult57 ) );
				float temp_output_83_0 = ( saturate( ( saturate( pow( tex2D( _MaskTexture, ( ( Distortion36 * _MaskDistortion ) + panner56 ) ).g , _MaskPower ) ) * _MaskMultiply ) ) * saturate( ( pow( tex2DNode68.g , _MaskMovePower ) * _MaskMoveMultiply ) ) );
				float2 texCoord40 = input.ase_texcoord4.xy * _Noise01UVScale + float2( 0,0 );
				float2 panner48 = ( global_speed22 * _Noise01PanSpeed + texCoord40);
				float2 texCoord38 = input.ase_texcoord4.xy * _Noise02UVScale + float2( 0,0 );
				float2 panner54 = ( global_speed22 * _Noise02PanSpeed + texCoord38);
				float temp_output_65_0 = ( tex2D( _Noise01Texture, ( Distortion36 + panner48 ) ).g * tex2D( _Noise02Texture, ( Distortion36 + panner54 ) ).g );
				float noise84 = saturate( ( saturate( pow( saturate( temp_output_65_0 ) , _NoisesPower ) ) * _NoisesMultiply ) );
				float temp_output_87_0 = ( saturate( temp_output_83_0 ) * noise84 );
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth94 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth94 = saturate( ( screenDepth94 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				float temp_output_120_0 = saturate( ( ( saturate( ( temp_output_87_0 * _OpacityBoost ) ) - ( input.ase_texcoord5.x + _Dissolve ) ) * distanceDepth94 ) );
				float temp_output_121_0 = saturate( ( input.ase_color.a * ( lerpResult138 * temp_output_120_0 ) ) );
				

				float Alpha = temp_output_121_0;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( input.positionCS );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19701
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _MaskUVScale;
			float2 _MaskMoveUVScale;
			float2 _DistortionUVScale;
			float2 _DistortionPanSpeed;
			float2 _Noise01PanSpeed;
			float2 _Noise02PanSpeed;
			float2 _Noise01UVScale;
			float2 _Noise02UVScale;
			float2 _MaskUVOffset;
			float _NoisesPower;
			float _MaskPower;
			float _NoisesMultiply;
			float _MaskMoveMultiply;
			float _OpacityBoost;
			float _Dissolve;
			float _MaskMovePower;
			float _DepthFade;
			float _MaskMultiply;
			float _Cull;
			float _MasksSpeed;
			float _MaskDistortion;
			float _DistortionMultiply;
			float _GlobalSpeed;
			float _IsFresnel;
			float _IsFresnelInverted;
			float _FresnelPower;
			float _FresnelScale;
			float _CameraOffset;
			float _ZTest;
			float _ZWrite;
			float _Dst;
			float _Src;
			float _IsAdd;
			float _Emissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MaskTexture;
			sampler2D _DistortionTexture;
			sampler2D _MaskMoveTexture;
			sampler2D _Noise01Texture;
			sampler2D _Noise02Texture;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 texCoord97 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				
				output.ase_texcoord.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord1.xyz = ase_worldNormal;
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				output.ase_texcoord4 = screenPos;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				output.ase_texcoord3 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.w = 0;
				output.ase_texcoord1.w = 0;
				output.ase_texcoord2.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( ( ase_worldPos - _WorldSpaceCameraPos ) * ( ( _CameraOffset + texCoord97.y ) * 0.01 ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord3 = input.ase_texcoord3;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 ase_worldPos = input.ase_texcoord.xyz;
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_worldNormal = input.ase_texcoord1.xyz;
				float fresnelNdotV134 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode134 = ( 0.0 + _FresnelScale * pow( max( 1.0 - fresnelNdotV134 , 0.0001 ), _FresnelPower ) );
				float temp_output_137_0 = saturate( fresnelNode134 );
				float lerpResult144 = lerp( temp_output_137_0 , ( 1.0 - temp_output_137_0 ) , _IsFresnelInverted);
				float lerpResult138 = lerp( 1.0 , lerpResult144 , _IsFresnel);
				float global_speed22 = ( _GlobalSpeed * _TimeParameters.x );
				float2 texCoord23 = input.ase_texcoord2.xy * _DistortionUVScale + float2( 0,0 );
				float2 panner26 = ( global_speed22 * _DistortionPanSpeed + texCoord23);
				float Distortion36 = ( ( tex2D( _DistortionTexture, panner26 ).g * 0.1 ) * _DistortionMultiply );
				float2 appendResult44 = (float2(_MasksSpeed , 0.0));
				float2 texCoord47 = input.ase_texcoord2.xy * _MaskUVScale + _MaskUVOffset;
				float2 panner56 = ( global_speed22 * appendResult44 + texCoord47);
				float2 texCoord59 = input.ase_texcoord2.xy * _MaskMoveUVScale + float2( 0,0 );
				float2 appendResult57 = (float2(input.ase_texcoord3.z , input.ase_texcoord3.w));
				float4 tex2DNode68 = tex2D( _MaskMoveTexture, ( texCoord59 + appendResult57 ) );
				float temp_output_83_0 = ( saturate( ( saturate( pow( tex2D( _MaskTexture, ( ( Distortion36 * _MaskDistortion ) + panner56 ) ).g , _MaskPower ) ) * _MaskMultiply ) ) * saturate( ( pow( tex2DNode68.g , _MaskMovePower ) * _MaskMoveMultiply ) ) );
				float2 texCoord40 = input.ase_texcoord2.xy * _Noise01UVScale + float2( 0,0 );
				float2 panner48 = ( global_speed22 * _Noise01PanSpeed + texCoord40);
				float2 texCoord38 = input.ase_texcoord2.xy * _Noise02UVScale + float2( 0,0 );
				float2 panner54 = ( global_speed22 * _Noise02PanSpeed + texCoord38);
				float temp_output_65_0 = ( tex2D( _Noise01Texture, ( Distortion36 + panner48 ) ).g * tex2D( _Noise02Texture, ( Distortion36 + panner54 ) ).g );
				float noise84 = saturate( ( saturate( pow( saturate( temp_output_65_0 ) , _NoisesPower ) ) * _NoisesMultiply ) );
				float temp_output_87_0 = ( saturate( temp_output_83_0 ) * noise84 );
				float4 screenPos = input.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth94 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth94 = saturate( ( screenDepth94 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				float temp_output_120_0 = saturate( ( ( saturate( ( temp_output_87_0 * _OpacityBoost ) ) - ( input.ase_texcoord3.x + _Dissolve ) ) * distanceDepth94 ) );
				float temp_output_121_0 = saturate( ( input.ase_color.a * ( lerpResult138 * temp_output_120_0 ) ) );
				

				surfaceDescription.Alpha = temp_output_121_0;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19701
			#define ASE_SRP_VERSION 170003
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _MaskUVScale;
			float2 _MaskMoveUVScale;
			float2 _DistortionUVScale;
			float2 _DistortionPanSpeed;
			float2 _Noise01PanSpeed;
			float2 _Noise02PanSpeed;
			float2 _Noise01UVScale;
			float2 _Noise02UVScale;
			float2 _MaskUVOffset;
			float _NoisesPower;
			float _MaskPower;
			float _NoisesMultiply;
			float _MaskMoveMultiply;
			float _OpacityBoost;
			float _Dissolve;
			float _MaskMovePower;
			float _DepthFade;
			float _MaskMultiply;
			float _Cull;
			float _MasksSpeed;
			float _MaskDistortion;
			float _DistortionMultiply;
			float _GlobalSpeed;
			float _IsFresnel;
			float _IsFresnelInverted;
			float _FresnelPower;
			float _FresnelScale;
			float _CameraOffset;
			float _ZTest;
			float _ZWrite;
			float _Dst;
			float _Src;
			float _IsAdd;
			float _Emissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MaskTexture;
			sampler2D _DistortionTexture;
			sampler2D _MaskMoveTexture;
			sampler2D _Noise01Texture;
			sampler2D _Noise02Texture;


			
			float4 _SelectionID;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 texCoord97 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				
				output.ase_texcoord.xyz = ase_worldPos;
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				output.ase_texcoord1.xyz = ase_worldNormal;
				float4 ase_clipPos = TransformObjectToHClip((input.positionOS).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				output.ase_texcoord4 = screenPos;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				output.ase_texcoord3 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.w = 0;
				output.ase_texcoord1.w = 0;
				output.ase_texcoord2.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( ( ase_worldPos - _WorldSpaceCameraPos ) * ( ( _CameraOffset + texCoord97.y ) * 0.01 ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				output.positionCS = TransformWorldToHClip(positionWS);
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord3 = input.ase_texcoord3;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 ase_worldPos = input.ase_texcoord.xyz;
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_worldNormal = input.ase_texcoord1.xyz;
				float fresnelNdotV134 = dot( ase_worldNormal, ase_viewDirWS );
				float fresnelNode134 = ( 0.0 + _FresnelScale * pow( max( 1.0 - fresnelNdotV134 , 0.0001 ), _FresnelPower ) );
				float temp_output_137_0 = saturate( fresnelNode134 );
				float lerpResult144 = lerp( temp_output_137_0 , ( 1.0 - temp_output_137_0 ) , _IsFresnelInverted);
				float lerpResult138 = lerp( 1.0 , lerpResult144 , _IsFresnel);
				float global_speed22 = ( _GlobalSpeed * _TimeParameters.x );
				float2 texCoord23 = input.ase_texcoord2.xy * _DistortionUVScale + float2( 0,0 );
				float2 panner26 = ( global_speed22 * _DistortionPanSpeed + texCoord23);
				float Distortion36 = ( ( tex2D( _DistortionTexture, panner26 ).g * 0.1 ) * _DistortionMultiply );
				float2 appendResult44 = (float2(_MasksSpeed , 0.0));
				float2 texCoord47 = input.ase_texcoord2.xy * _MaskUVScale + _MaskUVOffset;
				float2 panner56 = ( global_speed22 * appendResult44 + texCoord47);
				float2 texCoord59 = input.ase_texcoord2.xy * _MaskMoveUVScale + float2( 0,0 );
				float2 appendResult57 = (float2(input.ase_texcoord3.z , input.ase_texcoord3.w));
				float4 tex2DNode68 = tex2D( _MaskMoveTexture, ( texCoord59 + appendResult57 ) );
				float temp_output_83_0 = ( saturate( ( saturate( pow( tex2D( _MaskTexture, ( ( Distortion36 * _MaskDistortion ) + panner56 ) ).g , _MaskPower ) ) * _MaskMultiply ) ) * saturate( ( pow( tex2DNode68.g , _MaskMovePower ) * _MaskMoveMultiply ) ) );
				float2 texCoord40 = input.ase_texcoord2.xy * _Noise01UVScale + float2( 0,0 );
				float2 panner48 = ( global_speed22 * _Noise01PanSpeed + texCoord40);
				float2 texCoord38 = input.ase_texcoord2.xy * _Noise02UVScale + float2( 0,0 );
				float2 panner54 = ( global_speed22 * _Noise02PanSpeed + texCoord38);
				float temp_output_65_0 = ( tex2D( _Noise01Texture, ( Distortion36 + panner48 ) ).g * tex2D( _Noise02Texture, ( Distortion36 + panner54 ) ).g );
				float noise84 = saturate( ( saturate( pow( saturate( temp_output_65_0 ) , _NoisesPower ) ) * _NoisesMultiply ) );
				float temp_output_87_0 = ( saturate( temp_output_83_0 ) * noise84 );
				float4 screenPos = input.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth94 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth94 = saturate( ( screenDepth94 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				float temp_output_120_0 = saturate( ( ( saturate( ( temp_output_87_0 * _OpacityBoost ) ) - ( input.ase_texcoord3.x + _Dissolve ) ) * distanceDepth94 ) );
				float temp_output_121_0 = saturate( ( input.ase_color.a * ( lerpResult138 * temp_output_120_0 ) ) );
				

				surfaceDescription.Alpha = temp_output_121_0;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

        	#pragma multi_compile_instancing
        	#pragma multi_compile _ LOD_FADE_CROSSFADE
        	#define _SURFACE_TYPE_TRANSPARENT 1
        	#define ASE_VERSION 19701
        	#define ASE_SRP_VERSION 170003
        	#define REQUIRE_DEPTH_TEXTURE 1


        	#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            #if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_FRAG_SCREEN_POSITION


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _MaskUVScale;
			float2 _MaskMoveUVScale;
			float2 _DistortionUVScale;
			float2 _DistortionPanSpeed;
			float2 _Noise01PanSpeed;
			float2 _Noise02PanSpeed;
			float2 _Noise01UVScale;
			float2 _Noise02UVScale;
			float2 _MaskUVOffset;
			float _NoisesPower;
			float _MaskPower;
			float _NoisesMultiply;
			float _MaskMoveMultiply;
			float _OpacityBoost;
			float _Dissolve;
			float _MaskMovePower;
			float _DepthFade;
			float _MaskMultiply;
			float _Cull;
			float _MasksSpeed;
			float _MaskDistortion;
			float _DistortionMultiply;
			float _GlobalSpeed;
			float _IsFresnel;
			float _IsFresnelInverted;
			float _FresnelPower;
			float _FresnelScale;
			float _CameraOffset;
			float _ZTest;
			float _ZWrite;
			float _Dst;
			float _Src;
			float _IsAdd;
			float _Emissive;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MaskTexture;
			sampler2D _DistortionTexture;
			sampler2D _MaskMoveTexture;
			sampler2D _Noise01Texture;
			sampler2D _Noise02Texture;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 texCoord97 = input.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				
				output.ase_texcoord2.xyz = ase_worldPos;
				
				output.ase_color = input.ase_color;
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				output.ase_texcoord4 = input.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.w = 0;
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( ( ase_worldPos - _WorldSpaceCameraPos ) * ( ( _CameraOffset + texCoord97.y ) * 0.01 ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				output.normalWS = TransformObjectToWorldNormal( input.normalOS );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord3 = input.ase_texcoord3;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_texcoord1 = input.ase_texcoord1;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord3 = patch[0].ase_texcoord3 * bary.x + patch[1].ase_texcoord3 * bary.y + patch[2].ase_texcoord3 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag( PackedVaryings input
				, out half4 outNormalWS : SV_Target0
			#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
			#endif
				 )
			{
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float3 ase_worldPos = input.ase_texcoord2.xyz;
				float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float fresnelNdotV134 = dot( input.clipPosV.xyz, ase_viewDirWS );
				float fresnelNode134 = ( 0.0 + _FresnelScale * pow( max( 1.0 - fresnelNdotV134 , 0.0001 ), _FresnelPower ) );
				float temp_output_137_0 = saturate( fresnelNode134 );
				float lerpResult144 = lerp( temp_output_137_0 , ( 1.0 - temp_output_137_0 ) , _IsFresnelInverted);
				float lerpResult138 = lerp( 1.0 , lerpResult144 , _IsFresnel);
				float global_speed22 = ( _GlobalSpeed * _TimeParameters.x );
				float2 texCoord23 = input.ase_texcoord3.xy * _DistortionUVScale + float2( 0,0 );
				float2 panner26 = ( global_speed22 * _DistortionPanSpeed + texCoord23);
				float Distortion36 = ( ( tex2D( _DistortionTexture, panner26 ).g * 0.1 ) * _DistortionMultiply );
				float2 appendResult44 = (float2(_MasksSpeed , 0.0));
				float2 texCoord47 = input.ase_texcoord3.xy * _MaskUVScale + _MaskUVOffset;
				float2 panner56 = ( global_speed22 * appendResult44 + texCoord47);
				float2 texCoord59 = input.ase_texcoord3.xy * _MaskMoveUVScale + float2( 0,0 );
				float2 appendResult57 = (float2(input.ase_texcoord4.z , input.ase_texcoord4.w));
				float4 tex2DNode68 = tex2D( _MaskMoveTexture, ( texCoord59 + appendResult57 ) );
				float temp_output_83_0 = ( saturate( ( saturate( pow( tex2D( _MaskTexture, ( ( Distortion36 * _MaskDistortion ) + panner56 ) ).g , _MaskPower ) ) * _MaskMultiply ) ) * saturate( ( pow( tex2DNode68.g , _MaskMovePower ) * _MaskMoveMultiply ) ) );
				float2 texCoord40 = input.ase_texcoord3.xy * _Noise01UVScale + float2( 0,0 );
				float2 panner48 = ( global_speed22 * _Noise01PanSpeed + texCoord40);
				float2 texCoord38 = input.ase_texcoord3.xy * _Noise02UVScale + float2( 0,0 );
				float2 panner54 = ( global_speed22 * _Noise02PanSpeed + texCoord38);
				float temp_output_65_0 = ( tex2D( _Noise01Texture, ( Distortion36 + panner48 ) ).g * tex2D( _Noise02Texture, ( Distortion36 + panner54 ) ).g );
				float noise84 = saturate( ( saturate( pow( saturate( temp_output_65_0 ) , _NoisesPower ) ) * _NoisesMultiply ) );
				float temp_output_87_0 = ( saturate( temp_output_83_0 ) * noise84 );
				float4 ase_screenPosNorm = ScreenPos / ScreenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth94 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth94 = saturate( ( screenDepth94 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthFade ) );
				float temp_output_120_0 = saturate( ( ( saturate( ( temp_output_87_0 * _OpacityBoost ) ) - ( input.ase_texcoord4.x + _Dissolve ) ) * distanceDepth94 ) );
				float temp_output_121_0 = saturate( ( input.ase_color.a * ( lerpResult138 * temp_output_120_0 ) ) );
				

				float Alpha = temp_output_121_0;
				float AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float3 normalWS = normalize(input.normalWS);
					float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					float3 normalWS = input.normalWS;
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "MotionVectors"
			Tags { "LightMode"="MotionVectors" }

			ColorMask RG

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_VERSION 19701
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
		    #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
			#endif

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MotionVectorsCommon.hlsl"

			

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 positionOld : TEXCOORD4;
				#if _ADD_PRECOMPUTED_VELOCITY
					float3 alembicMotionVector : TEXCOORD5;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float2 _MaskUVScale;
			float2 _MaskMoveUVScale;
			float2 _DistortionUVScale;
			float2 _DistortionPanSpeed;
			float2 _Noise01PanSpeed;
			float2 _Noise02PanSpeed;
			float2 _Noise01UVScale;
			float2 _Noise02UVScale;
			float2 _MaskUVOffset;
			float _NoisesPower;
			float _MaskPower;
			float _NoisesMultiply;
			float _MaskMoveMultiply;
			float _OpacityBoost;
			float _Dissolve;
			float _MaskMovePower;
			float _DepthFade;
			float _MaskMultiply;
			float _Cull;
			float _MasksSpeed;
			float _MaskDistortion;
			float _DistortionMultiply;
			float _GlobalSpeed;
			float _IsFresnel;
			float _IsFresnelInverted;
			float _FresnelPower;
			float _FresnelScale;
			float _CameraOffset;
			float _ZTest;
			float _ZWrite;
			float _Dst;
			float _Src;
			float _IsAdd;
			float _Emissive;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			
			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				// Jittered. Match the frame.
				output.positionCS = vertexInput.positionCS;
				output.positionCSNoJitter = mul( _NonJitteredViewProjMatrix, mul( UNITY_MATRIX_M, input.positionOS ) );

				float4 prevPos = ( unity_MotionVectorsParams.x == 1 ) ? float4( input.positionOld, 1 ) : input.positionOS;

				#if _ADD_PRECOMPUTED_VELOCITY
					prevPos = prevPos - float4(input.alembicMotionVector, 0);
				#endif

				output.previousPositionCSNoJitter = mul( _PrevViewProjMatrix, mul( UNITY_PREV_MATRIX_M, prevPos ) );

				return output;
			}

			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}

			half4 frag(	PackedVaryings input  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODFadeCrossFade( input.positionCS );
				#endif

				return float4( CalcNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 0, 0 );
			}
			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.CommentaryNode;16;-5216,-1152;Inherit;False;2184.752;611.1584;DISTORTION;11;36;31;30;29;28;27;26;25;24;23;21;;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;17;-8368,-1184;Inherit;False;2458.886;1220.625;NOISE;28;84;80;77;75;71;69;65;62;61;60;58;54;51;48;46;43;42;41;40;39;38;33;32;146;147;148;149;150;;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;10;464,-48;Inherit;False;1252;163.3674;Ge Lush was here! <3;5;15;14;13;12;11;Ge Lush was here! <3;0.4902092,0.3301886,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-7552,384;Inherit;False;Property;_GlobalSpeed;Global Speed;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;18;-7552,512;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-7296,384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-7040,384;Inherit;False;global_speed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;21;-5184,-1056;Inherit;False;Property;_DistortionUVScale;Distortion UV Scale;28;0;Create;True;0;0;0;False;0;False;1,1;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-4928,-1072;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;24;-4832,-768;Inherit;False;22;global_speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;25;-4960,-912;Inherit;False;Property;_DistortionPanSpeed;Distortion Pan Speed;29;0;Create;True;0;0;0;False;0;False;0.2,0.25;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;26;-4672,-1072;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-4208,-944;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-4496,-1104;Inherit;True;Property;_DistortionTexture;Distortion Texture;27;0;Create;True;0;0;0;False;3;Space(33);Header(Noise Distortion);Space(13);False;-1;02b0d0a96eedb3d4fa5664350b35ba23;02b0d0a96eedb3d4fa5664350b35ba23;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-4064,-1072;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-4000,-944;Inherit;False;Property;_DistortionMultiply;Distortion Multiply;30;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-3824,-1072;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;33;-8304,-1040;Inherit;False;Property;_Noise01UVScale;Noise 01 UV Scale;9;0;Create;True;0;0;0;False;0;False;0.8,0.8;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;32;-8160,-400;Inherit;False;Property;_Noise02UVScale;Noise 02 UV Scale;12;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-3584,-1072;Inherit;False;Distortion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;38;-7936,-416;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-8096,-1056;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;41;-7856,-80;Inherit;False;22;global_speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-8080,-688;Inherit;False;22;global_speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;43;-7904,-240;Inherit;False;Property;_Noise02PanSpeed;Noise 02 Pan Speed;13;0;Create;True;0;0;0;False;0;False;-0.2,0.4;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;39;-8080,-864;Inherit;False;Property;_Noise01PanSpeed;Noise 01 Pan Speed;10;0;Create;True;0;0;0;False;0;False;0.5,0.5;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;37;-5824,-336;Inherit;False;Property;_MaskUVScale;Mask UV Scale;17;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;34;-5808,-160;Inherit;False;Property;_MaskUVOffset;Mask UV Offset;18;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;48;-7856,-1008;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-7568,-1136;Inherit;False;36;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;54;-7680,-416;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-7664,-512;Inherit;False;36;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-5568,-128;Inherit;False;Property;_MasksSpeed;Masks Speed;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-5584,-272;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0.28,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;49;-5264,-512;Inherit;False;36;Distortion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-5296,-384;Inherit;False;Property;_MaskDistortion;Mask Distortion;22;0;Create;True;0;0;0;False;0;False;0;3.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-5248,48;Inherit;False;22;global_speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-5392,-112;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-7360,-1040;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-7472,-448;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-5056,-416;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;56;-5200,-272;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;50;-5280,624;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;61;-7248,-480;Inherit;True;Property;_Noise02Texture;Noise 02 Texture;11;0;Create;True;0;0;0;False;3;Space(33);Header(Noise 02);Space(13);False;-1;2f822f6dfbe26f44eadb94a1b925409d;2f822f6dfbe26f44eadb94a1b925409d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;62;-7216,-864;Inherit;True;Property;_Noise01Texture;Noise 01 Texture;8;0;Create;True;0;0;0;False;3;Space(33);Header(Noise 01);Space(13);False;-1;301564cad965bbb4da5072b5cf9406cb;301564cad965bbb4da5072b5cf9406cb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.Vector2Node;52;-5520,272;Inherit;False;Property;_MaskMoveUVScale;Mask Move UV Scale;24;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-4944,-288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;57;-5056,688;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;59;-5264,256;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-6912,-768;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-4944,272;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-6864,-432;Inherit;False;Property;_NoisesPower;Noises Power;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;150;-6864,-848;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;70;-4736,-288;Inherit;True;Property;_MaskTexture;Mask Texture;16;0;Create;True;0;0;0;False;3;Space(33);Header(Mask);Space(13);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;66;-4544,-64;Inherit;False;Property;_MaskPower;Mask Power;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;126;-4368,-384;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;146;-6912,-1024;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;68;-4800,240;Inherit;True;Property;_MaskMoveTexture;Mask Move Texture;23;0;Create;True;0;0;0;False;3;Space(33);Header(Mask Move);Space(13);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;67;-4592,496;Inherit;False;Property;_MaskMovePower;Mask Move Power;25;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;130;-4181.181,-375.602;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;125;-4389.101,183.8754;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-6640,-416;Inherit;False;Property;_NoisesMultiply;Noises Multiply;15;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;147;-6784,-1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-4352,-48;Inherit;False;Property;_MaskMultiply;Mask Multiply;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-4304,544;Inherit;False;Property;_MaskMoveMultiply;Mask Move Multiply;26;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-4176,-272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-4128,304;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-6528,-1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;82;-3952,-256;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;81;-3952,320;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;149;-6272,-1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-6144,-656;Inherit;False;noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-3728,272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-3456,432;Inherit;False;84;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;124;-3518.101,184.8754;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-3248,272;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;89;-3056,800;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-640,768;Inherit;False;Property;_FresnelScale;Fresnel Scale;33;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-640,896;Inherit;False;Property;_FresnelPower;Fresnel Power;34;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-3104,944;Inherit;False;Property;_OpacityBoost;Opacity Boost;6;0;Create;True;0;0;0;False;0;False;5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-2624,1248;Inherit;False;Property;_Dissolve;Dissolve;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;93;-2688,1056;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-2864,752;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;134;-384,768;Inherit;False;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-2464,1072;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;95;-2592,768;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;137;-160,768;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-1664,1280;Inherit;False;Property;_DepthFade;Depth Fade;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;98;-2272,976;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;142;-128,896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;128,1024;Inherit;False;Property;_IsFresnelInverted;Is Fresnel Inverted;32;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;94;-1664,1152;Inherit;False;True;True;False;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;139;384,640;Inherit;False;Constant;_Float1;Float 1;38;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;384,896;Inherit;False;Property;_IsFresnel;Is Fresnel;31;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;144;128,896;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-1664,1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;97;-1184,2000;Inherit;False;3;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;103;-1920,-160;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;138;384,768;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;120;-1280,1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-1184,1840;Inherit;False;Property;_CameraOffset;Camera Offset;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;101;-1200,1360;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;102;-912,1920;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;104;-1216,1600;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WireNode;107;-1792,208;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-672.3269,596.7025;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;108;-928,1504;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-720,1920;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-512,192;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-6480,-656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;127;-2896,480;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;-1536,480;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;129;-1248,496;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-4384,272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;512,0;Inherit;False;Property;_Cull;Cull;35;0;Create;True;0;0;0;True;3;Space(33);Header(AR);Space(13);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;768,0;Inherit;False;Property;_Src;Src;36;0;Create;True;0;0;0;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;1024,0;Inherit;False;Property;_Dst;Dst;37;0;Create;True;0;0;0;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;1280,0;Inherit;False;Property;_ZWrite;ZWrite;38;0;Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;1536,0;Inherit;False;Property;_ZTest;ZTest;39;0;Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-768,0;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-1344,-176;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;114;-1808,-400;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ComponentMaskNode;115;-1552,-400;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-960,160;Inherit;False;Property;_Emissive;Emissive;3;0;Create;True;0;0;0;False;0;False;1;40;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-592,1504;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;119;-1616,-176;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;121;-272,192;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;85;-3520,272;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;131;-768,-288;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-768,-384;Inherit;False;Property;_IsAdd;Is Add;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-1536,0;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-1152,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;145;-5504,0;Inherit;False;Constant;_MaskSpeed;Mask Speed;40;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;80;-6304,-656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-6656,-656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;Vefects/SH_Vefects_VFX_Dissolve_Newest;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;True;_Cull;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;True;True;1;5;True;_Src;10;True;_Dst;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;True;_ZWrite;True;3;True;_ZTest;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;25;Surface;1;638463808423533785;  Blend;0;0;Two Sided;1;0;Forward Only;0;0;Alpha Clipping;0;638857638787406793;  Use Shadow Threshold;0;0;Cast Shadows;0;638463808434011469;Receive Shadows;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;0;638857642354498939;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;11;False;True;False;True;False;False;True;True;True;False;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;8;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;9;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;151;0,100;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;20;0;19;0
WireConnection;20;1;18;0
WireConnection;22;0;20;0
WireConnection;23;0;21;0
WireConnection;26;0;23;0
WireConnection;26;2;25;0
WireConnection;26;1;24;0
WireConnection;27;1;26;0
WireConnection;29;0;27;2
WireConnection;29;1;28;0
WireConnection;31;0;29;0
WireConnection;31;1;30;0
WireConnection;36;0;31;0
WireConnection;38;0;32;0
WireConnection;40;0;33;0
WireConnection;48;0;40;0
WireConnection;48;2;39;0
WireConnection;48;1;42;0
WireConnection;54;0;38;0
WireConnection;54;2;43;0
WireConnection;54;1;41;0
WireConnection;47;0;37;0
WireConnection;47;1;34;0
WireConnection;44;0;35;0
WireConnection;58;0;51;0
WireConnection;58;1;48;0
WireConnection;60;0;46;0
WireConnection;60;1;54;0
WireConnection;55;0;49;0
WireConnection;55;1;45;0
WireConnection;56;0;47;0
WireConnection;56;2;44;0
WireConnection;56;1;53;0
WireConnection;61;1;60;0
WireConnection;62;1;58;0
WireConnection;64;0;55;0
WireConnection;64;1;56;0
WireConnection;57;0;50;3
WireConnection;57;1;50;4
WireConnection;59;0;52;0
WireConnection;65;0;62;2
WireConnection;65;1;61;2
WireConnection;63;0;59;0
WireConnection;63;1;57;0
WireConnection;150;0;65;0
WireConnection;70;1;64;0
WireConnection;126;0;70;2
WireConnection;126;1;66;0
WireConnection;146;0;150;0
WireConnection;146;1;69;0
WireConnection;68;1;63;0
WireConnection;130;0;126;0
WireConnection;125;0;68;2
WireConnection;125;1;67;0
WireConnection;147;0;146;0
WireConnection;78;0;130;0
WireConnection;78;1;74;0
WireConnection;79;0;125;0
WireConnection;79;1;76;0
WireConnection;148;0;147;0
WireConnection;148;1;71;0
WireConnection;82;0;78;0
WireConnection;81;0;79;0
WireConnection;149;0;148;0
WireConnection;84;0;149;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;124;0;83;0
WireConnection;87;0;124;0
WireConnection;87;1;86;0
WireConnection;89;0;87;0
WireConnection;90;0;89;0
WireConnection;90;1;88;0
WireConnection;134;2;135;0
WireConnection;134;3;136;0
WireConnection;96;0;93;1
WireConnection;96;1;92;0
WireConnection;95;0;90;0
WireConnection;137;0;134;0
WireConnection;98;0;95;0
WireConnection;98;1;96;0
WireConnection;142;0;137;0
WireConnection;94;0;91;0
WireConnection;144;0;137;0
WireConnection;144;1;142;0
WireConnection;144;2;143;0
WireConnection;105;0;98;0
WireConnection;105;1;94;0
WireConnection;138;0;139;0
WireConnection;138;1;144;0
WireConnection;138;2;140;0
WireConnection;120;0;105;0
WireConnection;102;0;100;0
WireConnection;102;1;97;2
WireConnection;107;0;103;4
WireConnection;141;0;138;0
WireConnection;141;1;120;0
WireConnection;108;0;101;0
WireConnection;108;1;104;0
WireConnection;109;0;102;0
WireConnection;111;0;107;0
WireConnection;111;1;141;0
WireConnection;77;0;75;0
WireConnection;77;1;71;0
WireConnection;127;0;87;0
WireConnection;128;0;88;0
WireConnection;128;1;120;0
WireConnection;129;0;128;0
WireConnection;73;0;68;2
WireConnection;73;1;67;0
WireConnection;110;0;131;0
WireConnection;110;1;116;0
WireConnection;112;0;115;0
WireConnection;112;1;119;0
WireConnection;115;0;114;0
WireConnection;117;0;108;0
WireConnection;117;1;109;0
WireConnection;119;0;103;0
WireConnection;121;0;111;0
WireConnection;85;0;83;0
WireConnection;131;0;103;0
WireConnection;131;1;118;0
WireConnection;131;2;132;0
WireConnection;118;0;112;0
WireConnection;118;1;121;0
WireConnection;80;0;77;0
WireConnection;75;0;65;0
WireConnection;75;1;69;0
WireConnection;1;2;110;0
WireConnection;1;3;121;0
WireConnection;1;5;117;0
ASEEND*/
//CHKSM=8549768E5E8C0AC1A6233BC1237CBA76418B113D