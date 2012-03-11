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

	/// @if LANG_JA
	/// <summary>モデルを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model</summary>
	/// @endif
	public class Model {
		/// @if LANG_JA
		/// <summary>モデルを作成する(ファイルから)</summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="index">ファイル内のモデル番号</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a model (from a file)</summary>
		/// <param name="fileName">Filename</param>
		/// <param name="index">Model number in a file</param>
		/// @endif
		public Model( string fileName, int index = 0 ) {
			WorldMatrix = Matrix4.Identity ;
			CurrentMotion = 0 ;

			var loader = new MdxLoader() ;
			loader.Load( this, fileName, index ) ;
		}
		/// @if LANG_JA
		/// <summary>モデルを作成する(ファイルイメージから)</summary>
		/// <param name="fileImage">ファイルイメージ</param>
		/// <param name="index">ファイル内のモデル番号</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a model (from a file image)</summary>
		/// <param name="fileImage">File image</param>
		/// <param name="index">Model number in a file</param>
		/// @endif
		public Model( byte[] fileImage, int index = 0 ) {
			WorldMatrix = Matrix4.Identity ;
			CurrentMotion = 0 ;

			var loader = new MdxLoader() ;
			loader.Load( this, fileImage, index ) ;
		}
		/// @if LANG_JA
		/// <summary>モデルのアンマネージドリソースを解放する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Frees unmanaged resources of a model</summary>
		/// @endif
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

		/// @if LANG_JA
		/// <summary>モデルにプログラムを関連づける</summary>
		/// <param name="container">プログラムコンテナ</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Links a program to a model</summary>
		/// <param name="container">Program Container</param>
		/// @endif
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
		/// @if LANG_JA
		/// <summary>モデルにテクスチャを関連づける</summary>
		/// <param name="container">テクスチャコンテナ</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Links a texture to a model</summary>
		/// <param name="container">Texture Container</param>
		/// @endif
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

		/// @if LANG_JA
		/// <summary>ワールド行列を設定する</summary>
		/// <param name="world">ワールド行列</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets a world matrix</summary>
		/// <param name="world">World matrix</param>
		/// @endif
		public void SetWorldMatrix( ref Matrix4 world ) {
			WorldMatrix = world ;
		}
		/// @if LANG_JA
		/// <summary>カレントモーションを設定する</summary>
		/// <param name="index">モーション番号</param>
		/// <param name="delay">遅延時間 (単位＝秒)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the current motion</summary>
		/// <param name="index">Motion number</param>
		/// <param name="delay">Delay time (unit = s)</param>
		/// @endif
		public void SetCurrentMotion( int index, float delay = 0.0f ) {
			CurrentMotion = index ;
			Motions[ CurrentMotion ].Frame = Motions[ CurrentMotion ].FrameStart ;
		}
		/// @if LANG_JA
		/// <summary>モデルのアニメーションを計算する</summary>
		/// <param name="step">ステップ時間 (単位＝秒)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Calculates the model animation</summary>
		/// <param name="step">Step time (unit = s)</param>
		/// @endif
		public void Animate( float step ) {
			if ( Motions.Length > 0 ) AnimateMotion( Motions[ CurrentMotion ], step ) ;
		}
		/// @if LANG_JA
		/// <summary>モデルのマトリクスを更新する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Updates the model matrix</summary>
		/// @endif
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
		/// @if LANG_JA
		/// <summary>モデルを描画する</summary>
		/// <param name="graphics">グラフィックスコンテキスト</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Renders a model</summary>
		/// <param name="graphics">Graphics Context</param>
		/// @endif
		public void Draw( GraphicsContext graphics ) { Draw( graphics, null ) ; }
		/// @if LANG_JA
		/// <summary>モデルを描画する (指定されたプログラムで)</summary>
		/// <param name="graphics">グラフィックスコンテキスト</param>
		/// <param name="program">指定されたプログラム</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Renders a model (with the specified program)</summary>
		/// <param name="graphics">Graphics Context</param>
		/// <param name="program">Specified program</param>
		/// @endif
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
						parameters.SetMaterialDiffuse( ref material.Diffuse ) ;
						parameters.SetMaterialSpecular( ref material.Specular ) ;
						parameters.SetMaterialEmission( ref material.Emission ) ;
						parameters.SetMaterialAmbient( ref material.Ambient ) ;
						parameters.SetMaterialOpacity( material.Opacity ) ;
						parameters.SetMaterialShininess( material.Shininess ) ;
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

		/// @if LANG_JA
		/// <summary>ワールド行列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>World matrix</summary>
		/// @endif
		public Matrix4 WorldMatrix ;
		/// @if LANG_JA
		/// <summary>カレントモーション番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Current motion number</summary>
		/// @endif
		public int CurrentMotion ;

		/// @if LANG_JA
		/// <summary>モデルに含まれるボーンの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone array included in a model</summary>
		/// @endif
		public BasicBone[] Bones ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるパートの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Part array included in a model</summary>
		/// @endif
		public BasicPart[] Parts ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるマテリアルの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material array included in a model</summary>
		/// @endif
		public BasicMaterial[] Materials ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるテクスチャの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture array included in a model</summary>
		/// @endif
		public BasicTexture[] Textures ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるモーションの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Motion array included in a model</summary>
		/// @endif
		public BasicMotion[] Motions ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるプログラムの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Program array included in a model</summary>
		/// @endif
		public BasicProgram[] Programs ;

		static Matrix4[] matrixBuffer = new Matrix4[ 0 ] ;
		static int[] defaultBlendSubset = { 0, 1, 2, 3 } ;
		static int[] fcurveElementStrides = { 1, 1, 3, 5, 1 } ;
	}

	//----------------------------------------------------------------
	//	BasicBone
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのボーンを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model bone</summary>
	/// @endif
	public class BasicBone {
		/// @if LANG_JA
		/// <summary>ボーンを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a bone</summary>
		/// @endif
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
		/// @if LANG_JA
		/// <summary>親ボーンの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Parent bone number</summary>
		/// @endif
		public int ParentBone ;
		/// @if LANG_JA
		/// <summary>ビジビリティ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Visibility</summary>
		/// @endif
		public uint Visibility ;
		/// @if LANG_JA
		/// <summary>描画パートの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Render part number</summary>
		/// @endif
		public int[] DrawParts ;
		/// @if LANG_JA
		/// <summary>スキニングの影響ボーンの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Skinning effect bone number</summary>
		/// @endif
		public int[] BlendBones ;
		/// @if LANG_JA
		/// <summary>スキニングのオフセット行列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Skinning offset matrix</summary>
		/// @endif
		public Matrix4[] BlendOffsets ;
		/// @if LANG_JA
		/// <summary>ピボット位置</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Pivot position</summary>
		/// @endif
		public Vector3 Pivot ;
		/// @if LANG_JA
		/// <summary>移動量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Movement amount</summary>
		/// @endif
		public Vector3 Translation ;
		/// @if LANG_JA
		/// <summary>回転量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Rotation amount</summary>
		/// @endif
		public Quaternion Rotation ;
		/// @if LANG_JA
		/// <summary>スケーリング</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Scaling</summary>
		/// @endif
		public Vector3 Scaling ;
		/// @if LANG_JA
		/// <summary>ワールド行列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>World matrix</summary>
		/// @endif
		public Matrix4 WorldMatrix ;
	}

	//----------------------------------------------------------------
	//	BasicPart / BasicMesh
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのパートを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model part</summary>
	/// @endif
	public class BasicPart {
		/// @if LANG_JA
		/// <summary>パートを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a part</summary>
		/// @endif
		public BasicPart() {
			Meshes = null ;
		}
		/// @if LANG_JA
		/// <summary>パートに含まれるメッシュの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Mesh array included in a part</summary>
		/// @endif
		public BasicMesh[] Meshes ;
	}

	/// @if LANG_JA
	/// <summary>モデルのメッシュを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model mesh</summary>
	/// @endif
	public class BasicMesh {
		/// @if LANG_JA
		/// <summary>メッシュを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a mesh</summary>
		/// @endif
		public BasicMesh() {
			Material = -1 ;
			BlendSubset = null ;
			VertexBuffer = null ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material number</summary>
		/// @endif
		public int Material ;
		/// @if LANG_JA
		/// <summary>スキニングの部分集合</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Skinning subset</summary>
		/// @endif
		public int[] BlendSubset ;
		/// @if LANG_JA
		/// <summary>頂点バッファ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>VertexBuffer</summary>
		/// @endif
		public VertexBuffer VertexBuffer ;
	}

	//----------------------------------------------------------------
	//	BasicMaterial / BasicLayer
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのマテリアルを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model material</summary>
	/// @endif
	public class BasicMaterial {
		/// @if LANG_JA
		/// <summary>マテリアルを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a material</summary>
		/// @endif
		public BasicMaterial() {
			Program = -1 ;
			FileName = null ;
			Diffuse = Vector3.One ;
			Specular = Vector3.Zero ;
			Ambient = Vector3.One ;
			Emission = Vector3.Zero ;
			Opacity = 1.0f ;
			Shininess = 0.0f ;
			Layers = null ;
		}
		/// @if LANG_JA
		/// <summary>プログラムの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Program number</summary>
		/// @endif
		public int Program ;
		/// @if LANG_JA
		/// <summary>プログラムのファイル名</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Program filename</summary>
		/// @endif
		public string FileName ;
		/// @if LANG_JA
		/// <summary>ディフューズカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Diffuse color</summary>
		/// @endif
		public Vector3 Diffuse ;
		/// @if LANG_JA
		/// <summary>スペキュラーカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Specular color</summary>
		/// @endif
		public Vector3 Specular ;
		/// @if LANG_JA
		/// <summary>アンビエントカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Ambient color</summary>
		/// @endif
		public Vector3 Ambient ;
		/// @if LANG_JA
		/// <summary>エミッションカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Emission color</summary>
		/// @endif
		public Vector3 Emission ;
		/// @if LANG_JA
		/// <summary>不透明度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Opacity</summary>
		/// @endif
		public float Opacity ;
		/// @if LANG_JA
		/// <summary>輝度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Luminance</summary>
		/// @endif
		public float Shininess ;
		/// @if LANG_JA
		/// <summary>マテリアルに含まれるレイヤーの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Layer array included in a material</summary>
		/// @endif
		public BasicLayer[] Layers ;
	}

	/// @if LANG_JA
	/// <summary>モデルのレイヤーを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model layer</summary>
	/// @endif
	public class BasicLayer {
		/// @if LANG_JA
		/// <summary>レイヤーを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a layer</summary>
		/// @endif
		public BasicLayer() {
			Texture = -1 ;
			TextureCrop = new Vector4( 0.0f, 0.0f, 1.0f, 1.0f ) ;
		}
		/// @if LANG_JA
		/// <summary>テクスチャの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture number</summary>
		/// @endif
		public int Texture ;
		/// @if LANG_JA
		/// <summary>テクスチャクロップ矩形</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture crop rectangle</summary>
		/// @endif
		public Vector4 TextureCrop ;
	}

	//----------------------------------------------------------------
	//	BasicTexture
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのテクスチャを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model texture</summary>
	/// @endif
	public class BasicTexture {
		/// @if LANG_JA
		/// <summary>テクスチャを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a texture</summary>
		/// @endif
		public BasicTexture() {
			Texture = null ;
			FileName = null ;
		}
		/// @if LANG_JA
		/// <summary>使用するテクスチャ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture to be used</summary>
		/// @endif
		public Texture Texture ;
		/// @if LANG_JA
		/// <summary>使用するテクスチャのファイル名</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Filename of texture to be used</summary>
		/// @endif
		public string FileName ;
	}

	//----------------------------------------------------------------
	//	BasicMotion / BasicFCurve
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのモーションを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model motion</summary>
	/// @endif
	public class BasicMotion {
		/// @if LANG_JA
		/// <summary>モーションを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a motion</summary>
		/// @endif
		public BasicMotion() {
			FrameStart = 0.0f ;
			FrameEnd = 1000000.0f ;
			FrameRate = 60.0f ;
			FrameRepeat = BasicMotionRepeatMode.Cycle ;
			Frame = 0.0f ;
			Weight = 0.0f ;
			FCurves = null ;
		}
		/// @if LANG_JA
		/// <summary>開始フレーム</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Start frame</summary>
		/// @endif
		public float FrameStart ;
		/// @if LANG_JA
		/// <summary>終了フレーム</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>End frame</summary>
		/// @endif
		public float FrameEnd ;
		/// @if LANG_JA
		/// <summary>フレームレート</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Frame rate</summary>
		/// @endif
		public float FrameRate ;
		/// @if LANG_JA
		/// <summary>繰り返しモード</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Repeat mode</summary>
		/// @endif
		public BasicMotionRepeatMode FrameRepeat ;
		/// @if LANG_JA
		/// <summary>現在のフレーム</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Current frame</summary>
		/// @endif
		public float Frame ;
		/// @if LANG_JA
		/// <summary>現在のブレンド係数</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Current blend coefficient</summary>
		/// @endif
		public float Weight ;
		/// @if LANG_JA
		/// <summary>モーションに含まれる関数カーブの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Function curve array included in a motion</summary>
		/// @endif
		public BasicFCurve[] FCurves ;
	}

	/// @if LANG_JA
	/// <summary>モデルの関数カーブを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model function curve</summary>
	/// @endif
	public class BasicFCurve {
		/// @if LANG_JA
		/// <summary>関数カーブを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a function curve</summary>
		/// @endif
		public BasicFCurve() {
			DimCount = 0 ;
			KeyCount = 0 ;
			InterpType = BasicFCurveInterpType.Constant ;
			TargetType = BasicFCurveTargetType.None ;
			ChannelType = BasicFCurveChannelType.None ;
			TargetIndex = -1 ;
			KeyFrames = null ;
		}
		/// @if LANG_JA
		/// <summary>次元数</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Dimensionality</summary>
		/// @endif
		public int DimCount ;
		/// @if LANG_JA
		/// <summary>キー数</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Number of keys</summary>
		/// @endif
		public int KeyCount ;
		/// @if LANG_JA
		/// <summary>アニメーションの補間タイプ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation interpolation type</summary>
		/// @endif
		public BasicFCurveInterpType InterpType ;
		/// @if LANG_JA
		/// <summary>アニメーションのターゲットタイプ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation target type</summary>
		/// @endif
		public BasicFCurveTargetType TargetType ;
		/// @if LANG_JA
		/// <summary>アニメーションのチャンネルタイプ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation channel type</summary>
		/// @endif
		public BasicFCurveChannelType ChannelType ;
		/// @if LANG_JA
		/// <summary>アニメーションのターゲット番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation target number</summary>
		/// @endif
		public int TargetIndex ;
		/// @if LANG_JA
		/// <summary>キーフレームの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Key frame array</summary>
		/// @endif
		public float[] KeyFrames ;
	} ;

	//----------------------------------------------------------------
	//	Public Enums
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モーションの繰り返しモード</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Motion repeat mode</summary>
	/// @endif
	public enum BasicMotionRepeatMode {
		/// @if LANG_JA
		/// <summary>単発再生</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Individual playback</summary>
		/// @endif
		Hold,
		/// @if LANG_JA
		/// <summary>繰り返し再生</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Repeat playback</summary>
		/// @endif
		Cycle
	}

	/// @if LANG_JA
	/// <summary>アニメーションの補間タイプ</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Animation interpolation type</summary>
	/// @endif
	public enum BasicFCurveInterpType {
		/// @if LANG_JA
		/// <summary>定数補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Constant interpolation</summary>
		/// @endif
		Constant,
		/// @if LANG_JA
		/// <summary>線形補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Linear interpolation</summary>
		/// @endif
		Linear,
		/// @if LANG_JA
		/// <summary>エルミート補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Hermite interpolation</summary>
		/// @endif
		Hermite,
		/// @if LANG_JA
		/// <summary>キュービック補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Cubic interpolation</summary>
		/// @endif
		Cubic,
		/// @if LANG_JA
		/// <summary>球面線形補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Spherical linear interpolation</summary>
		/// @endif
		Spherical
	} ;

	/// @if LANG_JA
	/// <summary>アニメーションのターゲットタイプ</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Animation target type</summary>
	/// @endif
	public enum BasicFCurveTargetType {
		/// @if LANG_JA
		/// <summary>なし</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>None</summary>
		/// @endif
		None,
		/// @if LANG_JA
		/// <summary>ボーン</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone</summary>
		/// @endif
		Bone,
		/// @if LANG_JA
		/// <summary>マテリアル</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material</summary>
		/// @endif
		Material,
	} ;

	/// @if LANG_JA
	/// <summary>アニメーションのチャンネルタイプ</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Animation channel type</summary>
	/// @endif
	public enum BasicFCurveChannelType {
		/// @if LANG_JA
		/// <summary>なし</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>None</summary>
		/// @endif
		None,
		/// @if LANG_JA
		/// <summary>ボーンのビジビリティ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone visibility</summary>
		/// @endif
		Visibility,
		/// @if LANG_JA
		/// <summary>ボーンの移動量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone movement amount</summary>
		/// @endif
		Translation,
		/// @if LANG_JA
		/// <summary>ボーンの回転量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone rotation amount</summary>
		/// @endif
		Rotation,
		/// @if LANG_JA
		/// <summary>ボーンのスケーリング</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone scaling</summary>
		/// @endif
		Scaling,
		/// @if LANG_JA
		/// <summary>マテリアルのディフューズカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material diffuse color</summary>
		/// @endif
		Diffuse,
		/// @if LANG_JA
		/// <summary>マテリアルのスペキュラーカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material specular color</summary>
		/// @endif
		Specular,
		/// @if LANG_JA
		/// <summary>マテリアルのエミッションカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material emission color</summary>
		/// @endif
		Emission,
		/// @if LANG_JA
		/// <summary>マテリアルのアンビエントカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material ambient color</summary>
		/// @endif
		Ambient,
		/// @if LANG_JA
		/// <summary>マテリアルの不透明度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material opacity</summary>
		/// @endif
		Opacity,
		/// @if LANG_JA
		/// <summary>マテリアルの輝度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material luminance</summary>
		/// @endif
		Shininess,
		/// @if LANG_JA
		/// <summary>レイヤーのテクスチャクロップ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Layer texture crop</summary>
		/// @endif
		TextureCrop
	} ;


} // namespace
