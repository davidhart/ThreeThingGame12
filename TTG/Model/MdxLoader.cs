/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.97.21
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.IO ;
using System.Text ;
using System.Collections.Generic ;
using Sce.Pss.Core ;
using Sce.Pss.Core.Graphics ;

namespace TTG {

	//------------------------------------------------------------
	//  MdxLoader
	//------------------------------------------------------------

	public class MdxLoader {

		public MdxLoader() {
			encoding = Encoding.ASCII ;
		}
		public void Load( Model model, string fileName, int index ) {
			byte[] fileImage ;
			using ( var reader = new BinaryReader( File.OpenRead( fileName ) ) ) {
				fileImage = reader.ReadBytes( (int)reader.BaseStream.Length ) ;
			}
			Load( model, fileImage, index ) ;
		}
		public void Load( Model model, byte[] fileImage, int index ) {
			this.model = model ;
			this.fileImage = fileImage ;
			ReadHeader() ;
			LoadFile( ReadChunk( 16 ), index ) ;
		}

		//  Loading Functions

		void LoadFile( Chunk chunk, int index ) {
			if ( chunk.Type != ChunkType.File ) index = -1 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				if ( index -- == 0 ) {
					LoadModel( child ) ;
					return ;
				}
			}
			throw new FileLoadException( "out of range" ) ;
		}
		void LoadModel( Chunk chunk ) {
			int nBones = CountChild( chunk, ChunkType.Bone ) ;
			int nParts = CountChild( chunk, ChunkType.Part ) ;
			int nMaterials = CountChild( chunk, ChunkType.Material ) ;
			int nTextures = CountChild( chunk, ChunkType.Texture ) ;
			int nMotions = CountChild( chunk, ChunkType.Motion ) ;
			model.Bones = new BasicBone[ nBones ] ;
			model.Parts = new BasicPart[ nParts ] ;
			model.Materials = new BasicMaterial[ nMaterials ] ;
			model.Textures = new BasicTexture[ nTextures ] ;
			model.Motions = new BasicMotion[ nMotions ] ;
			model.Programs = new BasicProgram[ 0 ] ;
			for ( int i = 0 ; i < nBones ; i ++ ) model.Bones[ i ] = new BasicBone() ;
			for ( int i = 0 ; i < nParts ; i ++ ) model.Parts[ i ] = new BasicPart() ;
			for ( int i = 0 ; i < nMaterials ; i ++ ) model.Materials[ i ] = new BasicMaterial() ;
			for ( int i = 0 ; i < nTextures ; i ++ ) model.Textures[ i ] = new BasicTexture() ;
			for ( int i = 0 ; i < nMotions ; i ++ ) model.Motions[ i ] = new BasicMotion() ;
			int iBone = 0, iPart = 0, iMaterial = 0, iTexture = 0, iMotion = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				switch ( child.Type ) {
					case ChunkType.Bone :
						LoadBone( child, model.Bones[ iBone ++ ] ) ;
						break ;
					case ChunkType.Part :
						LoadPart( child, model.Parts[ iPart ++ ] ) ;
						break ;
					case ChunkType.Material :
						LoadMaterial( child, model.Materials[ iMaterial ++ ] ) ;
						break ;
					case ChunkType.Texture :
						LoadTexture( child, model.Textures[ iTexture ++ ] ) ;
						break ;
					case ChunkType.Motion :
						LoadMotion( child, model.Motions[ iMotion ++ ] ) ;
						break ;
				}
			}
		}
		void LoadBone( Chunk chunk, BasicBone bone ) {
			int nParts = CountChild( chunk, ChunkType.DrawPart ) ;
			bone.DrawParts = new int[ nParts ] ;
			int iPart = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.ParentBone :
						bone.ParentBone = ReadIndex( args ) ;
						break ;
					case ChunkType.Visibility :
						bone.Visibility = ReadUInt32( args ) ;
						break ;
					case ChunkType.BlendBones :
						int nBones = ReadInt32( args ) ;
						bone.BlendBones = new int[ nBones ] ;
						for ( int i = 0 ; i < nBones ; i ++ ) {
							bone.BlendBones[ i ] = ReadIndex( args + 4 + 4 * i ) ;
						}
						break ;
					case ChunkType.BlendOffsets : {
						int nOffsets = ReadInt32( args ) ;
						bone.BlendOffsets = new Matrix4[ nOffsets ] ;
						for ( int i = 0 ; i < nOffsets ; i ++ ) {
							bone.BlendOffsets[ i ] = ReadMatrix4( args + 4 + 64 * i ) ;
						}
						break ;
					}
					case ChunkType.Pivot :
						bone.Pivot = ReadVector3( args ) ;
						break ;
					case ChunkType.Translate :
						bone.Translation = ReadVector3( args ) ;
						break ;
					case ChunkType.RotateZYX :
						bone.Rotation = Quaternion.RotationZyx( ReadVector3( args ) ) ;
						break ;
					case ChunkType.RotateYXZ :
						bone.Rotation = Quaternion.RotationYxz( ReadVector3( args ) ) ;
						break ;
					case ChunkType.RotateQ :
						bone.Rotation = ReadQuaternion( args ) ;
						break ;
					case ChunkType.Scale :
						bone.Scaling = ReadVector3( args ) ;
						break ;
					case ChunkType.Scale2 :
						bone.Scaling = ReadVector3( args ) ;
						break ;
					case ChunkType.DrawPart :
						bone.DrawParts[ iPart ++ ] = ReadIndex( args ) ;
						break ;
				}
			}
		}
		void LoadPart( Chunk chunk, BasicPart part ) {
			int nMeshes = CountChild( chunk, ChunkType.Mesh ) ;
			part.Meshes = new BasicMesh[ nMeshes ] ;
			for ( int i = 0 ; i < nMeshes ; i ++ ) part.Meshes[ i ] = new BasicMesh() ;
			int iMesh = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				switch ( child.Type ) {
					case ChunkType.Mesh :
						LoadMesh( child, chunk, part.Meshes[ iMesh ++ ] ) ;
						break ;
				}
			}
		}
		void LoadMesh( Chunk chunk, Chunk parent, BasicMesh mesh ) {
			var prims = new List<Primitive>() ;
			var indices = new List<ushort>() ;
			int iArrays = -1 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.SetMaterial :
						mesh.Material = ReadIndex( args ) ;
						break ;
					case ChunkType.BlendSubset :
						int nIndices = ReadInt32( args ) ;
						mesh.BlendSubset = new int[ nIndices ] ;
						for ( int i = 0 ; i < nIndices ; i ++ ) {
							mesh.BlendSubset[ i ] = ReadIndex( args + 4 + 4 * i ) ;
						}
						break ;
					case ChunkType.DrawArrays :
						iArrays = ReadIndex( args + 0 ) ;
						int mode = ReadInt32( args + 4 ) ;
						int first = indices.Count ;
						int nVerts = ReadInt32( args + 8 ) ;
						int nPrims = ReadInt32( args + 12 ) ;
						int total = nVerts * nPrims ;

						var prim = new Primitive( GetDrawMode( mode ), first, nVerts, 0 ) ;
						for ( int i = 0 ; i < nPrims ; i ++ ) {
							prims.Add( prim ) ;
							prim.First += (ushort)nVerts ;
						}
						bool sequential = ( ( mode & 0x8000 ) != 0 ) ;
						int index = ReadUInt16( args + 16 ) ;
						for ( int i = 0 ; i < total ; i ++ ) {
							if ( !sequential ) index = ReadUInt16( args + 16 + i * 2 ) ;
							indices.Add( (ushort)( index ++ ) ) ;
						}
						break ;
				}
			}

			if ( iArrays < 0 ) return ;
			int pArrays = FindChild( parent, ChunkType.Arrays, iArrays ) ;
			Chunk chunk2 = ReadChunk( pArrays ) ;

			int args2 = chunk2.Args ;
			int format = ReadInt32( args2 + 0 ) ;
			int count = ReadInt32( args2 + 4 ) ;
			int data2 = chunk2.Data ;

			VertexFormat[] formats = GetVertexFormats( format ) ;
			int[] offsets = GetVertexOffsets( format ) ;
			int stride = GetVertexStride( format ) ;

			mesh.VertexBuffer = new VertexBuffer( count, indices.Count, prims.Count, formats ) ;
			mesh.VertexBuffer.SetPrimitives( prims.ToArray() ) ;
			mesh.VertexBuffer.SetIndices( indices.ToArray() ) ;
			for ( int i = 0 ; i < formats.Length ; i ++ ) {
				if ( formats[ i ] == VertexFormat.None ) continue ;
				mesh.VertexBuffer.SetVertices( i, fileImage, data2 + offsets[ i ], stride ) ;
			}
		}
		void LoadMaterial( Chunk chunk, BasicMaterial material ) {
			int nLayers = CountChild( chunk, ChunkType.Layer ) ;
			material.Layers = new BasicLayer[ nLayers ] ;
			for ( int i = 0 ; i < nLayers ; i ++ ) material.Layers[ i ] = new BasicLayer() ;
			int iLayer = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.Layer :
						LoadLayer( child, chunk, material.Layers[ iLayer ++ ] ) ;
						break ;
					case ChunkType.Diffuse :
						material.Diffuse = ReadVector3( args + 0 ) ;
						material.Opacity = ReadFloat( args + 12 ) ;
						break ;
					case ChunkType.Specular :
						material.Specular = ReadVector3( args + 0 ) ;
						material.Shininess = ReadFloat( args + 16 ) ;
						break ;
					case ChunkType.Ambient :
						material.Ambient = ReadVector3( args + 0 ) ;
						material.Opacity = ReadFloat( args + 12 ) ;
						break ;
					case ChunkType.Emission :
						material.Emission = ReadVector3( args ) ;
						break ;
					case ChunkType.FileName	:
						material.FileName = ReadString( args, args + child.ArgsSize ) ;
						break ;
				}
			}
		}
		void LoadLayer( Chunk chunk, Chunk parent, BasicLayer layer ) {
			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.SetTexture :
						layer.Texture = ReadIndex( args ) ;
						break ;
					case ChunkType.TexCrop :
						layer.TextureCrop = ReadVector4( args ) ;
						break ;
				}
			}
		}
		void LoadTexture( Chunk chunk, BasicTexture texture ) {
			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.FileImage :
						int size = ReadInt32( args ) ;
						var image = new byte[ size ] ;
						Array.Copy( fileImage, args + 4, image, 0, size ) ;
						texture.Texture = new Texture2D( image, true ) ;
						break ;
					case ChunkType.FileName	:
						texture.FileName = ReadString( args, args + child.ArgsSize ) ;
						break ;
				}
			}
		}
		void LoadMotion( Chunk chunk, BasicMotion motion ) {
			int nFCurves = CountChild( chunk, ChunkType.Animate ) ;
			motion.FCurves = new BasicFCurve[ nFCurves ] ;
			for ( int i = 0 ; i < nFCurves ; i ++ ) motion.FCurves[ i ] = new BasicFCurve() ;
			int iFCurve = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.Animate :
						LoadFCurve( child, chunk, motion.FCurves[ iFCurve ++ ] ) ;
						break ;
					case ChunkType.FrameRate :
						motion.FrameRate = ReadFloat( args ) ;
						break ;
					case ChunkType.FrameLoop :
						motion.FrameStart = ReadFloat( args + 0 ) ;
						motion.FrameEnd = ReadFloat( args + 4 ) ;
						break ;
					case ChunkType.FrameRepeat :
						motion.FrameRepeat = (BasicMotionRepeatMode)ReadInt32( args ) ;
						break ;
				}
			}
		}
		void LoadFCurve( Chunk chunk, Chunk parent, BasicFCurve fcurve ) {
			int args = chunk.Args ;
			int target = ReadInt32( args + 0 ) ;
			int channel = ReadInt32( args + 4 ) ;
			//int index = ReadInt32( args + 8 ) ;
			int iFCurve = ReadIndex( args + 12 ) ;
			int pFCurve = FindChild( parent, ChunkType.FCurve, iFCurve ) ;
			Chunk chunk2 = ReadChunk( pFCurve ) ;

			int args2 = chunk2.Args ;
			int format = ReadInt32( args2 + 0 ) ;
			int nDims = ReadInt32( args2 + 4 ) ;
			int nKeys = ReadInt32( args2 + 8 ) ;
			int data = chunk2.Data ;
			int nData = chunk2.DataSize / 4 ;
			fcurve.KeyFrames = new float[ nData ] ;
			for ( int i = 0 ; i < nData ; i ++ ) {
				fcurve.KeyFrames[ i ] = ReadFloat( data + i * 4 ) ;
			}
			fcurve.DimCount = nDims ;
			fcurve.KeyCount = nKeys ;
			fcurve.InterpType = GetFCurveInterpType( format ) ;
			fcurve.TargetType = GetFCurveTargetType( target ) ;
			fcurve.ChannelType = GetFCurveChannelType( channel ) ;
			fcurve.TargetIndex = GetReferenceIndex( target ) ;
		}

		//  Subroutines ( Enum )

		VertexFormat[] GetVertexFormats( int format ) {
			VertexFormat[] vFormats = { VertexFormat.Byte3N, VertexFormat.Short3N, VertexFormat.Float3 } ;
			VertexFormat[] tFormats = { VertexFormat.UByte2N, VertexFormat.UShort2N, VertexFormat.Float2 } ;
			VertexFormat[] wFormats = { VertexFormat.ByteN, VertexFormat.ShortN, VertexFormat.Float } ;
			int tType = ( format >> 0 ) & 0x03 ;
			int cType = ( format >> 2 ) & 0x07 ;
			int nType = ( format >> 5 ) & 0x03 ;
			int pType = ( format >> 7 ) & 0x03 ;
			int wType = ( format >> 9 ) & 0x03 ;
			int wCount = ( ( format >> 14 ) & 0x07 ) + 1 ;
			var formats = new VertexFormat[ 5 ] ;
			if ( pType != 0 ) formats[ 0 ] = vFormats[ pType - 1 ] ;
			if ( nType != 0 ) formats[ 1 ] = vFormats[ nType - 1 ] ;
			if ( cType != 0 ) formats[ 2 ] = VertexFormat.UByte4N ;
			if ( tType != 0 ) formats[ 3 ] = tFormats[ tType - 1 ] ;
			if ( wType != 0 ) formats[ 4 ] = (VertexFormat)( (int)wFormats[ wType - 1 ] + ( wCount - 1 ) ) ;
			return formats ;
		}
		int[] GetVertexOffsets( int format ) {
			var offsets = new int[ 5 ] ;
			int offset = 0 ;
			int esize = ( ( format >> 9 ) & 3 ) - 1 ;
			int vsize = esize ;
			if ( esize >= 0 ) {
				int num = ( ( format >> 14 ) & 7 ) + 1 ;
				offsets[ 4 ] = 0 ;
				offset -= num << esize ;
			}
			esize = ( format & 3 ) - 1 ;
			if ( esize >= 0 ) {
				if ( esize > vsize ) vsize = esize ;
				offset = offset >> esize << esize ;
				offsets[ 3 ] = -offset ;
				offset -= 2 << esize ;
			}
			esize = ( format >> 2 ) & 7 ;
			if ( esize != 0 ) {
				esize = ( esize + 1 ) >> 2 ;
				if ( esize > vsize ) vsize = esize ;
				offset = offset >> esize << esize ;
				offsets[ 2 ] = -offset ;
				offset -= 1 << esize ;
			}
			esize = ( ( format >> 5 ) & 3 ) - 1 ;
			if ( esize >= 0 ) {
				if ( esize > vsize ) vsize = esize ;
				offset = offset >> esize << esize ;
				offsets[ 1 ] = -offset ;
				offset -= 3 << esize ;
			}
			esize = ( ( format >> 7 ) & 3 ) - 1 ;
			if ( esize >= 0 ) {
				if ( esize > vsize ) vsize = esize ;
				offset = offset >> esize << esize ;
				offsets[ 0 ] = -offset ;
				offset -= 3 << esize ;
			}
			return offsets ;
		}
		int GetVertexStride( int format ) {
			return (int)( (uint)format >> 24 ) ;
		}
		DrawMode GetDrawMode( int mode ) {
			DrawMode[] modes = {
				DrawMode.Points, DrawMode.Lines, DrawMode.LineStrip,
				DrawMode.Triangles, DrawMode.TriangleStrip, DrawMode.TriangleFan
			} ;
			return modes[ mode & 0x0f ] ;
		}
		BasicFCurveInterpType GetFCurveInterpType( int format ) {
			return (BasicFCurveInterpType)( format & 0x0f ) ;
		}
		BasicFCurveTargetType GetFCurveTargetType( int target ) {
			ChunkType type = GetReferenceType( target ) ;
			if ( type == ChunkType.Bone ) return BasicFCurveTargetType.Bone ;
			return BasicFCurveTargetType.None ;
		}
		BasicFCurveChannelType GetFCurveChannelType( int channel ) {
			ChunkType type = (ChunkType)channel ;
			if ( type == ChunkType.Translate ) return BasicFCurveChannelType.Translation ;
			if ( type == ChunkType.RotateZYX ) return BasicFCurveChannelType.Rotation ;
			if ( type == ChunkType.RotateYXZ ) return BasicFCurveChannelType.Rotation ;
			if ( type == ChunkType.RotateQ ) return BasicFCurveChannelType.Rotation ;
			if ( type == ChunkType.Scale ) return BasicFCurveChannelType.Scaling ;
			if ( type == ChunkType.Scale2 ) return BasicFCurveChannelType.Scaling ;
			return BasicFCurveChannelType.None ;
		}
		ChunkType GetReferenceType( int reference ) {
			return (ChunkType)( ( reference >> 16 ) & 0x7fff ) ;
		}
		int GetReferenceIndex( int reference ) {
			return ( reference & 0x0fff ) ;
		}

		//  Subroutines ( Format Reading )

		void ReadHeader() {
			header.Signature = BitConverter.ToUInt32( fileImage, 0 ) ;
			header.Version = BitConverter.ToUInt32( fileImage, 4 ) ;
			header.Style = BitConverter.ToUInt32( fileImage, 8 ) ;
			header.Option = BitConverter.ToUInt32( fileImage, 12 ) ;
			if ( header.Signature != 0x2e4d4458 ) throw new FileLoadException( "wrong format" ) ;
			if ( header.Version != 0x302E3935 ) throw new FileLoadException( "wrong version" ) ;
			if ( header.Style != 0x00505353 ) throw new FileLoadException( "wrong style" ) ;
		}
		Chunk ReadChunk( int pos ) {
			Chunk chunk ;
			chunk.Pos = pos ;
			chunk.Name = null ;
			int type = BitConverter.ToUInt16( fileImage, pos + 0 ) ;
			chunk.Args = pos + BitConverter.ToUInt16( fileImage, pos + 2 ) ;
			chunk.Next = pos + BitConverter.ToInt32( fileImage, pos + 4 ) ;
			if ( ( type & 0x8000 ) != 0 ) {
				chunk.Args = pos + 8 ;
				chunk.Child = chunk.Next ;
				chunk.Data = chunk.Next ;
			} else {
				chunk.Name = ReadString( pos + 16, chunk.Args ) ;
				chunk.Child = pos + BitConverter.ToInt32( fileImage, pos + 8 ) ;
				chunk.Data = pos + BitConverter.ToInt32( fileImage, pos + 12 ) ;
			}
			chunk.Type = (ChunkType)( type & ~0x8000 ) ;
			return chunk ;
		}
		int CountChild( Chunk chunk, ChunkType type ) {
			int count = 0 ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; ) {
				ChunkType type2 = (ChunkType)( BitConverter.ToUInt16( fileImage, pos ) & ~0x8000 ) ;
				if ( type2 == type ) count ++ ;
				pos += BitConverter.ToInt32( fileImage, pos + 4 ) ;
			}
			return count ;
		}
		int FindChild( Chunk chunk, ChunkType type, int index ) {
			for ( int pos = chunk.Child ; pos < chunk.Next ; ) {
				ChunkType type2 = (ChunkType)( BitConverter.ToUInt16( fileImage, pos ) & ~0x8000 ) ;
				if ( type2 == type && index -- == 0 ) return pos ;
				pos += BitConverter.ToInt32( fileImage, pos + 4 ) ;
			}
			return 0 ;
		}

		//  Subroutines ( Data Reading )

		string ReadString( int pos, int end ) {
			int term = pos ;
			while ( term < end && fileImage[ term ] != 0 ) term ++ ;
			return ( term == pos ) ? "" : encoding.GetString( fileImage, pos, term - pos ) ;
		}
		int ReadIndex( int pos ) { return GetReferenceIndex( ReadInt32( pos ) ) ; }
		int ReadInt32( int pos ) { return BitConverter.ToInt32( fileImage, pos ) ; }
		uint ReadUInt32( int pos ) { return BitConverter.ToUInt32( fileImage, pos ) ; }
		short ReadInt16( int pos ) { return BitConverter.ToInt16( fileImage, pos ) ; }
		ushort ReadUInt16( int pos ) { return BitConverter.ToUInt16( fileImage, pos ) ; }
		sbyte ReadInt8( int pos ) { return (sbyte)fileImage[ pos ] ; }
		byte ReadUInt8( int pos ) { return fileImage[ pos ] ; }
		float ReadFloat( int pos ) { return BitConverter.ToSingle( fileImage, pos ) ; }
		Matrix4 ReadMatrix4( int pos ) {
			Vector4 x = ReadVector4( pos + 0 ) ;
			Vector4 y = ReadVector4( pos + 16 ) ;
			Vector4 z = ReadVector4( pos + 32 ) ;
			Vector4 w = ReadVector4( pos + 48 ) ;
			return new Matrix4( x, y, z, w ) ;
		}
		Vector4 ReadVector4( int pos ) {
			float x = BitConverter.ToSingle( fileImage, pos + 0 ) ;
			float y = BitConverter.ToSingle( fileImage, pos + 4 ) ;
			float z = BitConverter.ToSingle( fileImage, pos + 8 ) ;
			float w = BitConverter.ToSingle( fileImage, pos + 12 ) ;
			return new Vector4( x, y, z, w ) ;
		}
		Vector3 ReadVector3( int pos ) {
			float x = BitConverter.ToSingle( fileImage, pos + 0 ) ;
			float y = BitConverter.ToSingle( fileImage, pos + 4 ) ;
			float z = BitConverter.ToSingle( fileImage, pos + 8 ) ;
			return new Vector3( x, y, z ) ;
		}
		Vector2 ReadVector2( int pos ) {
			float x = BitConverter.ToSingle( fileImage, pos + 0 ) ;
			float y = BitConverter.ToSingle( fileImage, pos + 4 ) ;
			return new Vector2( x, y ) ;
		}
		Quaternion ReadQuaternion( int pos ) {
			Vector4 v = ReadVector4( pos ) ;
			return new Quaternion( v.X, v.Y, v.Z, v.W ) ;
		}

		//  Private Data

		Encoding encoding ;
		Model model ;
		byte[] fileImage ;
		Header header ;

		//  Private Types

		struct Header {
			public uint Signature ;
			public uint Version ;
			public uint Style ;
			public uint Option ;
		} ;
		struct Chunk {
			public int Pos ;
			public ChunkType Type ;
			public string Name ;
			public int Args ;
			public int Child ;
			public int Data ;
			public int Next ;
			public int ArgsSize { get { return Data - Args ; } }
			public int DataSize { get { return Child - Data ; } }
			public int ChildSize { get { return Next - Child ; } }
			public int TotalSize { get { return Next - Pos ; } }
		} ;
		enum ChunkType {
			Block			= 0x0001,
			File			= 0x0002,
			Model			= 0x0003,
			Bone			= 0x0004,
			Part			= 0x0005,
			Mesh			= 0x0006,
			Arrays			= 0x0007,
			Material		= 0x0008,
			Layer			= 0x0009,
			Texture			= 0x000a,
			Motion			= 0x000b,
			FCurve			= 0x000c,
			BlindBlock		= 0x000f,

			Command			= 0x0011,
			FileName		= 0x0012,
			FileImage		= 0x0013,
			BoundingBox		= 0x0014,
			BoundingPoints	= 0x0016,
			VertexOffset	= 0x0015,

			DefineEnum		= 0x0021,
			DefineBlock		= 0x0022,
			DefineCommand	= 0x0023,
			ConvertOption	= 0x0024,

			ParentBone		= 0x0041,
			Visibility		= 0x0042,
			MorphWeights	= 0x0043,
			MorphIndex		= 0x004f,
			BlendBones		= 0x0044,
			BlendOffsets	= 0x0045,
			Pivot			= 0x0046,
			MultMatrix		= 0x0047,
			Translate		= 0x0048,
			RotateZYX		= 0x0049,
			RotateYXZ		= 0x004a,
			RotateQ			= 0x004b,
			Scale			= 0x004c,
			Scale2			= 0x004d,
			Scale3			= 0x00e1,
			DrawPart		= 0x004e,
			BoneState		= 0x00e2,

			SetMaterial		= 0x0061,
			BlendSubset		= 0x0062,
			Subdivision		= 0x0063,
			KnotVectorU		= 0x0064,
			KnotVectorV		= 0x0065,
			DrawArrays		= 0x0066,
			DrawParticle	= 0x0067,
			DrawBSpline		= 0x0068,
			DrawRectMesh	= 0x0069,
			DrawRectPatch	= 0x006a,
			MeshType		= 0x006b,
			MeshLevel		= 0x006c,
			EdgeFlags		= 0x006d,
			EdgeFaces		= 0x006e,

			RenderState		= 0x0081,
			Diffuse			= 0x0082,
			Specular		= 0x0083,
			Emission		= 0x0084,
			Ambient			= 0x0085,
			Reflection		= 0x0086,
			Refraction		= 0x0087,
			Bump			= 0x0088,

			SetTexture		= 0x0091,
			MapType			= 0x0092,
			MapFactor		= 0x0093,
			BlendFunc		= 0x0094,
			TexFunc			= 0x0095,
			TexFilter		= 0x0096,
			TexWrap			= 0x0097,
			TexCrop			= 0x0098,
			TexGen			= 0x0099,
			TexMatrix		= 0x009a,

			FrameLoop		= 0x00b1,
			FrameRate		= 0x00b2,
			FrameRepeat		= 0x00b4,
			Animate			= 0x00b3,

			BlindData		= 0x00f1,
			FileInfo		= 0x00ff
		} ;
	}
	
} // namespace
	
