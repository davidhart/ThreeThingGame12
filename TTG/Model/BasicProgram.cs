/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.97.21
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.IO ;
using System.Reflection ;

using Sce.Pss.Core ;
using Sce.Pss.Core.Graphics ;

namespace TTG {

	//------------------------------------------------------------
	//  BasicProgram
	//------------------------------------------------------------


	/// <summary>Class representing a basic program</summary>
	public class BasicProgram : ShaderProgram {


		/// <summary>Creates a program</summary>
		/// <param name="parameters">Parameters (creates new when null)</param>
		public BasicProgram( BasicParameters parameters = null ) : base( loadDefaultProgram() ) {
			BasicParameters.InitProgram( this ) ;
			Parameters = ( parameters != null ) ? parameters : new BasicParameters() ;
		}

		/// <summary>Creates a program (from a file)</summary>
		/// <param name="vpFileName">Vertex shader filename</param>
		/// <param name="fpFileName">Fragment shader filename</param>
		/// <param name="parameters">Parameters (creates new when null)</param>
		public BasicProgram( string vpFileName, string fpFileName = null, BasicParameters parameters = null	 ) : base( vpFileName, fpFileName ) {
			BasicParameters.InitProgram( this ) ;
			Parameters = ( parameters != null ) ? parameters : new BasicParameters() ;
		}

		/// <summary>Creates a copy of a program</summary>
		/// <param name="program">Program</param>
		/// <remarks>Creates a copy of the program. The 2 programs will then share unmanaged resources. When Dispose() is called for all copies, the shared unmanaged resources will be freed.</remarks>
		public BasicProgram( BasicProgram program ) : base( program ) {
			parameters = program.parameters ;
		}

		/// <summary>Frees the unmanaged resources of the program</summary>
		public new void Dispose() {
			base.Dispose() ;
			parameters = null ;
		}

		/// <summary>Updates the shader state</summary>
		/// <remarks>A virtual function that is called from the graphics context before primitive rendering. This function is used when a cached state in a class is applied to the shader.</remarks>
		protected override void UpdateShader() {
			Parameters.UpdateProgram( this ) ;
		}

		/// <summary>Parameters</summary>
		public BasicParameters Parameters {
			get { return parameters ; }
			set { parameters = value ; parameters.TouchProgram( this ) ; }
		}

		//  load default program

		static byte[] defaultProgram ;
		static byte[] loadDefaultProgram() {
			if ( defaultProgram == null ) {
				Assembly assembly = Assembly.GetExecutingAssembly() ;
				string resname = "TTG.Model.shaders.Basic.cgx" ;
				if ( assembly.GetManifestResourceInfo( resname ) == null ) {
					throw new FileNotFoundException( "Resource not found.", resname ) ;
				}
				Stream stream = assembly.GetManifestResourceStream( resname ) ;
				defaultProgram = new Byte[ stream.Length ] ;
				stream.Read( defaultProgram, 0, defaultProgram.Length ) ;
			}
			return defaultProgram ;
		}

		internal BasicParameters parameters ;
		internal int updateView ;
		internal int updateWorld ;
	}

	//------------------------------------------------------------
	//  BasicParameters
	//------------------------------------------------------------

	/// <summary>Class representing basic program parameters</summary>
	public class BasicParameters {

		/// <summary>Creates a parameter</summary>
		public BasicParameters() {
			;
		}

		/// <summary>Sets the projection matrix</summary>
		/// <param name="projection">Projection matrix</param>
		public void SetProjectionMatrix( ref Matrix4 projection ) {
			projMatrix = projection ;
			viewProjection = projMatrix * viewMatrix ;
			updateView ++ ;
		}

		/// <summary>Sets a view matrix</summary>
		/// <param name="view">View matrix</param>
		public void SetViewMatrix( ref Matrix4 view ) {
			viewMatrix = view ;
			viewProjection = projMatrix * viewMatrix ;

			// - viewMatrix.Transpose().TransformVector( view.AxisW )
			eyePosition.X = -( viewMatrix.M11 * viewMatrix.M41 + viewMatrix.M12 * viewMatrix.M42 + viewMatrix.M13 * viewMatrix.M43 ) ;
			eyePosition.Y = -( viewMatrix.M21 * viewMatrix.M41 + viewMatrix.M22 * viewMatrix.M42 + viewMatrix.M23 * viewMatrix.M43 ) ;
			eyePosition.Z = -( viewMatrix.M31 * viewMatrix.M41 + viewMatrix.M32 * viewMatrix.M42 + viewMatrix.M33 * viewMatrix.M43 ) ;

			// ( viewMatrix.RowZ + ( 0, 0, 0, fs ) ) / ( fs - fe )
			float fs = fogRange.X ;
			float fe = fogRange.Y ;
			float fr = 1.0f / ( fs - fe ) ;
			fogVector.X = viewMatrix.M13 * fr ;
			fogVector.Y = viewMatrix.M23 * fr ;
			fogVector.Z = viewMatrix.M33 * fr ;
			fogVector.W = ( viewMatrix.M43 + fs ) * fr ;

			updateView ++ ;
		}

		/// <summary>Sets the number of world matrices</summary>
		/// <param name="count">Number of world matrices</param>
		public void SetWorldCount( int count ) {
			worldCount = count ;
			updateWorld ++ ;
		}

		/// <summary>Sets a world matrix</summary>
		/// <param name="index">World matrix number</param>
		/// <param name="world">World matrix</param>
		public void SetWorldMatrix( int index, ref Matrix4 world ) {
			worldMatrix[ index ] = world ;
			updateWorld ++ ;
		}

		
		static bool TryCatchSetUniform(BasicProgram program, int id, string uniform)
		{
			try
			{
				program.SetUniformBinding( id, uniform );
				return true;
			}
			catch
			{
				return false;
			}
		}

		internal static void InitProgram( BasicProgram program ) {
			
			TryCatchSetUniform(program, idWorldCount, "WorldCount" ) ;
			TryCatchSetUniform(program, idViewProjection, "ViewProjection" ) ;
			TryCatchSetUniform(program, idEyePosition, "EyePosition" ) ;
			TryCatchSetUniform(program, idWorldMatrix, "WorldMatrix" ) ;

			program.SetAttributeBinding( idAttribPosition, "a_Position" ) ;
			program.SetAttributeBinding( idAttribNormal, "a_Normal" ) ;
			// program.SetAttributeBinding( idAttribColor, "a_Color" ) ;
			program.SetAttributeBinding( idAttribTexCoord, "a_TexCoord" ) ;
			program.SetAttributeBinding( idAttribWeight, "a_Weight" ) ;
		}
		internal void TouchProgram( BasicProgram program ) {
			program.updateView = updateView - 1 ;
			program.updateWorld = updateWorld - 1 ;
		}
		internal void UpdateProgram( BasicProgram program ) {
			if ( program.updateView != updateView ) {
				program.updateView = updateView ;
				program.SetUniformValue( idViewProjection, ref viewProjection ) ;
				program.SetUniformValue( idEyePosition, ref eyePosition ) ;
			}
			if ( program.updateWorld != updateWorld ) {
				program.updateWorld = updateWorld ;
				program.SetUniformValue( idWorldCount, worldCount ) ;
				program.SetUniformValue( idWorldMatrix, worldMatrix, 0, 0, worldCount ) ;
			}
		}

		//  Const

		const int idWorldCount = 0 ;
		const int idViewProjection = 1 ;
		const int idEyePosition = 2 ;
		const int idWorldMatrix = 3 ;

		const int idAttribPosition = 0 ;
		const int idAttribNormal = 1 ;
		const int idAttribColor = 2 ;
		const int idAttribTexCoord = 3 ;
		const int idAttribWeight = 4 ;

		//  Status

		int updateView ;
		int updateWorld ;

		Matrix4 projMatrix = Matrix4.Identity ;
		Matrix4 viewMatrix = Matrix4.Identity ;
		Matrix4 viewProjection = Matrix4.Identity ;
		Vector3 eyePosition = Vector3.Zero ;
		Vector2 fogRange = new Vector2( 1000000.0f, 1000001.0f ) ;
		Vector3 fogColor = Vector3.Zero ;
		Vector4 fogVector = new Vector4( 0.0f, 0.0f, 1.0f, 1.0f ) ;

		int worldCount = 1 ;
		Matrix4[] worldMatrix = new Matrix4[ 4 ] ;
	}

} // namespace
