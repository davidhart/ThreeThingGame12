/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.97.21
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.Collections.Generic ;
using Sce.Pss.Core ;
using Sce.Pss.Core.Graphics ;

namespace TTG {

	//------------------------------------------------------------
	//	BasicModel
	//------------------------------------------------------------

	/// <summary>Class representing a model</summary>
	public class Model {
		/// <summary>Creates a model (from a file)</summary>
		/// <param name="fileName">Filename</param>
		/// <param name="index">Model number in a file</param>
		public Model( string fileName, int index = 0 ) {
			WorldMatrix = Matrix4.Identity ;
			CurrentMotion = 0 ;

			var loader = new MdxLoader() ;
			loader.Load( this, fileName, index ) ;
		}
		/// <summary>Creates a model (from a file image)</summary>
		/// <param name="fileImage">File image</param>
		/// <param name="index">Model number in a file</param>
		public Model( byte[] fileImage, int index = 0 ) {
			WorldMatrix = Matrix4.Identity ;
			CurrentMotion = 0 ;

			var loader = new MdxLoader() ;
			loader.Load( this, fileImage, index ) ;
		}

		/// <summary>Frees unmanaged resources of a model</summary>
		public void Dispose() {
			foreach ( var part in Parts ) {
				foreach ( var mesh in part.Meshes ) {
					mesh.VertexBuffer.Dispose() ;
					mesh.VertexBuffer = null ;
				}
			}
			foreach ( var texture in Textures ) {
				texture.Texture.Dispose() ;
				texture.Texture = null ;
			}
			foreach ( var program in Programs ) {
				program.Dispose() ;
			}
			Bones = null ;
			Parts = null ;
			Materials = null ;
			Textures = null ;
			Motions = null ;
			Programs = null ;
		}

		/// <summary>Links a program to a model</summary>
		/// <param name="container">Program Container</param>
		public void BindPrograms( BasicProgramContainer container ) {
			if ( Programs != null ) {
				foreach ( var program in Programs ) program.Dispose() ;
				Programs = null ;
			}
			var programs = new List<BasicProgram>() ;
			foreach ( var material in Materials ) {
				BasicProgram p = container.Find( material.FileName ) ;
				if ( p == null ) p = container.Find( null ) ;
				material.Program = programs.Count ;
				programs.Add( new BasicProgram( p ) ) ;
			}
			Programs = programs.ToArray() ;
		}
		
		/// <summary>Links a texture to a model</summary>
		/// <param name="container">Texture Container</param>
		public void BindTextures( BasicTextureContainer container ) {
			foreach ( var texture in Textures ) {
				if ( texture.Texture == null ) {
					Texture t = container.Find( texture.FileName ) ;
					if ( t == null ) t = container.Find( null ) ;
					if ( t is Texture2D ) texture.Texture = new Texture2D( t as Texture2D ) ;
					if ( t is TextureCube ) texture.Texture = new TextureCube( t as TextureCube ) ;
				}
			}
		}

		/// <summary>Sets a world matrix</summary>
		/// <param name="world">World matrix</param>
		public void SetWorldMatrix( ref Matrix4 world ) {
			WorldMatrix = world ;
		}
		
		/// <summary>Sets the current motion</summary>
		/// <param name="index">Motion number</param>
		/// <param name="delay">Delay time (unit = s)</param>
		public void SetCurrentMotion( int index, float delay = 0.0f ) {
			CurrentMotion = index ;
			Motions[ CurrentMotion ].Frame = Motions[ CurrentMotion ].FrameStart ;
		}

		/// <summary>Calculates the model animation</summary>
		/// <param name="step">Step time (unit = s)</param>
		public void Animate( float step ) {
			if ( Motions.Length > 0 ) AnimateMotion( Motions[ CurrentMotion ], step ) ;
		}
		
		/// <summary>Updates the model matrix</summary>
		public void Update() {
			foreach ( var bone in Bones ) {
				Matrix4 local = Matrix4.Transformation( bone.Translation, bone.Rotation, bone.Scaling ) ;
				BasicBone parent = ( bone.ParentBone < 0 ) ? null : Bones[ bone.ParentBone ] ;
				if ( parent == null ) {
					bone.WorldMatrix = WorldMatrix * local ;
				} else {
					bone.WorldMatrix = parent.WorldMatrix * local ;
				}
			}
		}

		/// <summary>Renders a model</summary>
		/// <param name="graphics">Graphics Context</param>
		public void Draw( GraphicsContext graphics ) { Draw( graphics, null ) ; }
		
		/// <summary>Renders a model (with the specified program)</summary>
		/// <param name="graphics">Graphics Context</param>
		/// <param name="program">Specified program</param>
		public void Draw( GraphicsContext graphics, BasicProgram program ) {
			bool worldDirty = false ;
			foreach ( var bone in Bones ) {
				int[] blendBones = bone.BlendBones ;
				if ( blendBones != null ) {
					int blendCount = blendBones.Length ;
					if ( blendCount > matrixBuffer.Length ) matrixBuffer = new Matrix4[ blendCount ] ;
					for ( int i = 0 ; i < blendCount ; i ++ ) {
						matrixBuffer[ i ] = Bones[ blendBones[ i ] ].WorldMatrix * bone.BlendOffsets[ i ] ;
					}
				}
				int[] blendSubset = defaultBlendSubset ;
				worldDirty = true ;
				foreach ( var index in bone.DrawParts ) {
					BasicPart part = Parts[ index ] ;
					foreach ( var mesh in part.Meshes ) {
						BasicMaterial material = Materials[ mesh.Material ] ;
						BasicProgram program2 = program ;
						if ( program2 == null ) {
							if ( material.Program < 0 ) continue ;
							program2 = Programs[ material.Program ] ;
						}
						if ( program2 != graphics.GetShaderProgram() ) {
							graphics.SetShaderProgram( program2 ) ;
							worldDirty = true ;
						}
						BasicParameters parameters = program2.Parameters ;
						if ( blendBones == null ) {
							if ( worldDirty ) {
								parameters.SetWorldCount( 1 ) ;
								parameters.SetWorldMatrix( 0, ref bone.WorldMatrix ) ;
								worldDirty = false ;
							}
						} else if ( blendSubset != mesh.BlendSubset ) {
							blendSubset = ( mesh.BlendSubset != null ) ? mesh.BlendSubset : defaultBlendSubset ;
							int blendCount = blendSubset.Length ;
							if ( blendCount > blendBones.Length ) blendCount = blendBones.Length ;
							parameters.SetWorldCount( blendCount ) ;
							for ( int i = 0 ; i < blendCount ; i ++ ) {
								parameters.SetWorldMatrix( i, ref matrixBuffer[ blendSubset[ i ] ] ) ;
							}
							worldDirty = true ;
						}
			
						Texture texture = null ;
						if ( material.Layers.Length > 0 ) {
							texture = Textures[ material.Layers[ 0 ].Texture ].Texture ;
						}
						graphics.SetTexture( 0, texture ) ;

						graphics.SetVertexBuffer( 0, mesh.VertexBuffer ) ;
						graphics.DrawArrays( null ) ;
					}
				}
			}
		}

		//	Subroutines
		
		void AnimateMotion( BasicMotion motion, float step ) {
			motion.Frame += step * motion.FrameRate ;
			if ( motion.Frame > motion.FrameEnd ) motion.Frame = motion.FrameStart ;
			float weight = 1.0f ;

			float[] value = new float[ 4 ] ;
			foreach ( var fcurve in motion.FCurves ) {
				if ( fcurve.TargetType != BasicFCurveTargetType.Bone ) continue ;
				EvalFCurve( fcurve, motion.Frame, value ) ;
				BasicBone bone = Bones[ fcurve.TargetIndex ] ;
				switch ( fcurve.ChannelType ) {
					case BasicFCurveChannelType.Translation :
						Vector3 translation = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
						bone.Translation = bone.Translation.Lerp( translation, weight ) ;
						break ;
					case BasicFCurveChannelType.Rotation :
						Quaternion rotation ;
						if ( fcurve.DimCount == 4 ) {
							rotation = new Quaternion( value[ 0 ], value[ 1 ], value[ 2 ], value[ 3 ] ) ;
						} else {
							rotation = Quaternion.RotationZyx( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
						}
						bone.Rotation = bone.Rotation.Slerp( rotation, weight ) ;
						break ;
					case BasicFCurveChannelType.Scaling :
						Vector3 scaling = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
						bone.Scaling = bone.Scaling.Lerp( scaling, weight ) ;
						break ;
				}
			}
		}
		void EvalFCurve( BasicFCurve fcurve, float frame, float[] result ) {
			float[] data = fcurve.KeyFrames ;
			int lower = 0 ;
			int upper = fcurve.KeyCount - 1 ;
			int elementStride = fcurveElementStrides[ (int)fcurve.InterpType ] ;
			int stride = elementStride * fcurve.DimCount + 1 ;
			while ( upper - lower > 1 ) {
				int center = ( upper + lower ) / 2 ;
				float frame2 = data[ stride * center ] ;
				if ( frame < frame2 ) {
					upper = center ;
				} else {
					lower = center ;
				}
			}
			int k1 = stride * lower ;
			int k2 = stride * upper ;
			float f1 = data[ k1 ++ ] ;
			float f2 = data[ k2 ++ ] ;
			float t = f2 - f1 ;
			if ( t != 0.0f ) t = ( frame - f1 ) / t ;

			BasicFCurveInterpType interp = fcurve.InterpType ;
			if ( t <= 0.0f ) interp = BasicFCurveInterpType.Constant ;
			if ( t >= 1.0f ) {
				interp = BasicFCurveInterpType.Constant ;
				k1 = k2 ;
			}
			switch ( interp ) {
				case BasicFCurveInterpType.Constant :
					for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
						result[ i ] = data[ k1 ] ;
						k1 += elementStride ;
					}
					break ;
				case BasicFCurveInterpType.Linear :
					for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
						float v1 = data[ k1 ++ ] ;
						float v2 = data[ k2 ++ ] ;
						result[ i ] = ( v2 - v1 ) * t + v1 ;
					}
					break ;
				case BasicFCurveInterpType.Hermite :
					float s = 1.0f - t ;
					float b1 = s * s * t * 3.0f ;
					float b2 = t * t * s * 3.0f ;
					float b0 = s * s * s + b1 ;
					float b3 = t * t * t + b2 ;
					for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
						result[ i ] = data[ k1 ] * b0 + data[ k1 + 2 ] * b1 + data[ k2 + 1 ] * b2 + data[ k2 ] * b3 ;
						k1 += 3 ;
						k2 += 3 ;
					}
					break ;
				case BasicFCurveInterpType.Cubic :
					for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
						float fa = f1 + data[ k1 + 3 ] ;
						float fb = f2 + data[ k2 + 1 ] ;
						float u = FindCubicRoot( f1, fa, fb, f2, frame ) ;
						float v = 1.0f - u ;
						float c1 = v * v * u * 3.0f ;
						float c2 = u * u * v * 3.0f ;
						float c0 = v * v * v + c1 ;
						float c3 = u * u * u + c2 ;
						result[ i ] = data[ k1 ] * c0 + data[ k1 + 4 ] * c1 + data[ k2 + 2 ] * c2 + data[ k2 ] * c3 ;
						k1 += 5 ;
						k2 += 5 ;
					}
					break ;
				case BasicFCurveInterpType.Spherical :
					var q1 = new Quaternion( data[ k1 + 0 ], data[ k1 + 1 ], data[ k1 + 2 ], data[ k1 + 3 ] ) ;
					var q2 = new Quaternion( data[ k2 + 0 ], data[ k2 + 1 ], data[ k2 + 2 ], data[ k2 + 3 ] ) ;
					var q3 = q1.Slerp( q2, t ) ;
					result[ 0 ] = q3.X ;
					result[ 1 ] = q3.Y ;
					result[ 2 ] = q3.Z ;
					result[ 3 ] = q3.W ;
					break ;
			}
		}
		float FindCubicRoot( float f0, float f1, float f2, float f3, float f ) {
			float E = ( f3 - f0 ) * 0.01f ;
			if ( E < 0.0001f ) E = 0.0001f ;
			float t0 = 0.0f ;
			float t3 = 1.0f ;
			for ( int i = 0 ; i < 8 ; i ++ ) {
				float d = f3 - f0 ;
				if ( d > -E && d < E ) break ;
				float r = ( f2 - f1 ) / d - ( 1.0f / 3.0f ) ;
				if ( r > -0.01f && r < 0.01f ) break ;
				float fc = ( f0 + f1 * 3.0f + f2 * 3.0f + f3 ) / 8.0f ;
				if ( f < fc ) {
					f3 = fc ;
					f2 = ( f1 + f2 ) * 0.5f ;
					f1 = ( f0 + f1 ) * 0.5f ;
					f2 = ( f1 + f2 ) * 0.5f ;
					t3 = ( t0 + t3 ) * 0.5f ;
				} else {
					f0 = fc ;
					f1 = ( f1 + f2 ) * 0.5f ;
					f2 = ( f2 + f3 ) * 0.5f ;
					f1 = ( f1 + f2 ) * 0.5f ;
					t0 = ( t0 + t3 ) * 0.5f ;
				}
			}
			float c = f0 - f ;
			float b = 3.0f * ( f1 - f0 ) ;
			float a = f3 - f0 - b ;
			float x ;
			if ( a == 0.0f ) {
				x = ( b == 0.0f ) ? 0.5f : -c / b ;
			} else {
				float D2 = b * b - 4.0f * a * c ;
				if ( D2 < 0.0f ) D2 = 0.0f ;
				D2 = (float)Math.Sqrt( D2 ) ;
				if ( a + b < 0.0f ) D2 = -D2 ;
				x = ( -b + D2 ) / ( 2.0f * a ) ;
			}
			return ( t3 - t0 ) * x + t0 ;
		}

		public Matrix4 WorldMatrix;
		public int CurrentMotion;

		/// <summary>Bone array included in a model</summary>
		public BasicBone[] Bones;

		/// <summary>Part array included in a model</summary>
		public BasicPart[] Parts;

		/// <summary>Material array included in a model</summary>
		public BasicMaterial[] Materials;

		/// <summary>Texture array included in a model</summary>
		public BasicTexture[] Textures;

		
		/// <summary>Motion array included in a model</summary>
		public BasicMotion[] Motions;

		/// <summary>Program array included in a model</summary>
		public BasicProgram[] Programs;

		static Matrix4[] matrixBuffer = new Matrix4[ 0 ] ;
		static int[] defaultBlendSubset = { 0, 1, 2, 3 } ;
		static int[] fcurveElementStrides = { 1, 1, 3, 5, 1 } ;
	}

	//----------------------------------------------------------------
	//	BasicBone
	//----------------------------------------------------------------

	/// <summary>Class representing a model bone</summary>
	public class BasicBone {
		
		/// <summary>Creates a bone</summary>
		public BasicBone() {
			ParentBone = -1 ;
			Visibility = ~0U ;
			DrawParts = null ;
			BlendBones = null ;
			BlendOffsets = null ;
			Pivot = Vector3.Zero ;
			Translation = Vector3.Zero ;
			Rotation = Quaternion.Identity ;
			Scaling = Vector3.One ;
			WorldMatrix = Matrix4.Identity ;
		}

		
		/// <summary>Parent bone number</summary>
		public int ParentBone ;

		
		/// <summary>Visibility</summary>
		public uint Visibility ;

		/// <summary>Render part number</summary>
		public int[] DrawParts ;

		/// <summary>Skinning effect bone number</summary>
		public int[] BlendBones ;

		/// <summary>Skinning offset matrix</summary>
		public Matrix4[] BlendOffsets ;

		/// <summary>Pivot position</summary>if
		public Vector3 Pivot ;

		/// <summary>Movement amount</summary>
		public Vector3 Translation ;

		/// <summary>Rotation amount</summary>
		public Quaternion Rotation ;

		/// <summary>Scaling</summary>
		public Vector3 Scaling ;

		/// <summary>World matrix</summary>
		public Matrix4 WorldMatrix ;
	}

	//----------------------------------------------------------------
	//	BasicPart / BasicMesh
	//----------------------------------------------------------------

	/// <summary>Class representing a model part</summary>
	public class BasicPart {
		public BasicPart() {
			Meshes = null ;
		}

		/// <summary>Mesh array included in a part</summary>
		public BasicMesh[] Meshes ;
	}

	/// <summary>Class representing a model mesh</summary>
	public class BasicMesh {

		public BasicMesh() {
			Material = -1 ;
			BlendSubset = null ;
			VertexBuffer = null ;
		}

		/// <summary>Material number</summary>
		public int Material ;

		/// <summary>Skinning subset</summary>
		public int[] BlendSubset ;

		/// <summary>VertexBuffer</summary>
		public VertexBuffer VertexBuffer ;
	}

	//----------------------------------------------------------------
	//	BasicMaterial / BasicLayer
	//----------------------------------------------------------------

	/// <summary>Class representing a model material</summary>
	public class BasicMaterial {

		/// <summary>Creates a material</summary>
		public BasicMaterial() {
			Program = -1 ;
			FileName = null ;
			Diffuse = Vector3.One ;
			Specular = Vector3.Zero ;
			Ambient = Vector3.One ;
			Opacity = 1.0f ;
			Shininess = 0.0f ;
			Layers = null ;
		}
		
		/// <summary>Program number</summary>
		public int Program;
		
		/// <summary>Program filename</summary>
		public string FileName;

		/// <summary>Diffuse color</summary>
		public Vector3 Diffuse ;

		/// <summary>Specular color</summary>
		public Vector3 Specular ;

		/// <summary>Ambient color</summary>
		public Vector3 Ambient ;

		/// <summary>Opacity</summary>
		public float Opacity ;

		/// <summary>Luminance</summary>
		public float Shininess ;

		/// <summary>Layer array included in a material</summary>
		public BasicLayer[] Layers ;
	}


	/// <summary>Class representing a model layer</summary>
	public class BasicLayer {

		/// <summary>Creates a layer</summary>
		public BasicLayer() {
			Texture = -1 ;
			TextureCrop = new Vector4( 0.0f, 0.0f, 1.0f, 1.0f ) ;
		}

		/// <summary>Texture number</summary>
		public int Texture ;

		/// <summary>Texture crop rectangle</summary>
		public Vector4 TextureCrop ;
	}

	//----------------------------------------------------------------
	//	BasicTexture
	//----------------------------------------------------------------


	/// <summary>Class representing a model texture</summary>
	public class BasicTexture {

		public BasicTexture() {
			Texture = null ;
			FileName = null ;
		}

		public Texture Texture ;

		public string FileName ;
	}

	//----------------------------------------------------------------
	//	BasicMotion / BasicFCurve
	//----------------------------------------------------------------


	/// <summary>Class representing a model motion</summary>
	public class BasicMotion {

		/// <summary>Creates a motion</summary>
		public BasicMotion() {
			FrameStart = 0.0f ;
			FrameEnd = 1000000.0f ;
			FrameRate = 60.0f ;
			FrameRepeat = BasicMotionRepeatMode.Cycle ;
			Frame = 0.0f ;
			Weight = 0.0f ;
			FCurves = null ;
		}

		public float FrameStart ;
		public float FrameEnd ;
		public float FrameRate ;
		public BasicMotionRepeatMode FrameRepeat ;
		public float Frame ;
		public float Weight ;
		public BasicFCurve[] FCurves ;
	}

	/// <summary>Class representing a model function curve</summary>
	public class BasicFCurve {
		
		/// <summary>Creates a function curve</summary>
		public BasicFCurve() {
			DimCount = 0 ;
			KeyCount = 0 ;
			InterpType = BasicFCurveInterpType.Constant ;
			TargetType = BasicFCurveTargetType.None ;
			ChannelType = BasicFCurveChannelType.None ;
			TargetIndex = -1 ;
			KeyFrames = null ;
		}

		/// <summary>Dimensionality</summary>
		public int DimCount ;

		/// <summary>Number of keys</summary>
		public int KeyCount ;

		/// <summary>Animation interpolation type</summary>
		public BasicFCurveInterpType InterpType ;

		/// <summary>Animation target type</summary>
		public BasicFCurveTargetType TargetType ;

		/// <summary>Animation channel type</summary>
		public BasicFCurveChannelType ChannelType ;

		/// <summary>Animation target number</summary>
		public int TargetIndex ;

		/// <summary>Key frame array</summary>
		public float[] KeyFrames ;
	} ;

	//----------------------------------------------------------------
	//	Public Enums
	//----------------------------------------------------------------

	/// <summary>Motion repeat mode</summary>
	public enum BasicMotionRepeatMode {
		/// <summary>Individual playback</summary>
		Hold,

		/// <summary>Repeat playback</summary>
		Cycle
	}

	/// <summary>Animation interpolation type</summary>
	public enum BasicFCurveInterpType {

		/// <summary>Constant interpolation</summary>
		Constant,

		/// <summary>Linear interpolation</summary>
		Linear,

		/// <summary>Hermite interpolation</summary>
		Hermite,

		/// <summary>Cubic interpolation</summary>
		Cubic,

		/// <summary>Spherical linear interpolation</summary>
		Spherical
	} ;


	/// <summary>Animation target type</summary>
	public enum BasicFCurveTargetType {
		/// <summary>None</summary>
		None,
		
		/// <summary>Bone</summary>
		Bone,

		/// <summary>Material</summary>
		Material,
	} ;


	/// <summary>Animation channel type</summary>
	public enum BasicFCurveChannelType {
		/// <summary>None</summary>
		None,

		/// <summary>Bone visibility</summary>
		Visibility,

		/// <summary>Bone movement amount</summary>
		Translation,

		/// <summary>Bone rotation amount</summary>
		Rotation,

		/// <summary>Bone scaling</summary>
		Scaling,

		/// <summary>Material diffuse color</summary>
		Diffuse,
		
		/// <summary>Material specular color</summary>
		Specular,

		/// <summary>Material ambient color</summary>
		Ambient,

		/// <summary>Material opacity</summary>
		Opacity,

		/// <summary>Material luminance</summary>
		Shininess,

		/// <summary>Layer texture crop</summary>
		TextureCrop
	} ;


}
