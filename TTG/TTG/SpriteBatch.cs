using System;
using System.Diagnostics;

using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Imaging;

namespace TTG
{
	public class SpriteBatch
	{	
		const int MAX_SPRITES_PER_FLUSH = 250;
		
		float [] vertices;
		float [] textureCoordinates;
		byte [] colours;
		ushort [] indices;
		
		int sprites;
		
		Texture currentTexture;
		
		GraphicsContext graphics;
		VertexBuffer buffer;
		ShaderProgram shaderProg;
		
		const int PositionAttributeIndex = 0;
		const int TexCoordAttributeIndex = 1;
		const int VertexColorAttributeIndex = 2;
		
		const int WorldMatrixUniformIndex = 0;
		
		public SpriteBatch (GraphicsContext graphics)
		{
			this.graphics = graphics;
			sprites = 0;
			
			shaderProg = new ShaderProgram("shaders/sprite.cgx", "shaders/sprite.cgx");
			
			shaderProg.SetAttributeBinding(PositionAttributeIndex, "a_Position");
			shaderProg.SetAttributeBinding(TexCoordAttributeIndex, "a_TexCoord");
			shaderProg.SetAttributeBinding(VertexColorAttributeIndex, "a_VertexColor");
			
			shaderProg.SetUniformBinding(WorldMatrixUniformIndex, "u_WorldMatrix");
			
									 // Vertex, Texture Coordinate, Color
			VertexFormat[] formats = { VertexFormat.Float3, VertexFormat.Float2, VertexFormat.UByte4N };
			
			buffer = new VertexBuffer(MAX_SPRITES_PER_FLUSH * 4, MAX_SPRITES_PER_FLUSH * 6, formats);
			
			vertices = new float[MAX_SPRITES_PER_FLUSH * 4 * 3];
			textureCoordinates = new float[MAX_SPRITES_PER_FLUSH * 4 * 2];
			colours = new byte[MAX_SPRITES_PER_FLUSH * 4 * 4];
			
			indices = new ushort[MAX_SPRITES_PER_FLUSH * 6];
			
			for (int i = 0; i < MAX_SPRITES_PER_FLUSH; ++i)
			{
				ushort quad = (ushort)(i*6);
				ushort vert = (ushort)(i*4);
				indices[quad + 0] = (ushort)(0 + vert);
				indices[quad + 1] = (ushort)(1 + vert);
				indices[quad + 2] = (ushort)(2 + vert);
				
				indices[quad + 3] = (ushort)(0 + vert);
				indices[quad + 4] = (ushort)(2 + vert);
				indices[quad + 5] = (ushort)(3 + vert);
			}
			
			buffer.SetIndices(indices);			
		}
		
		public void Begin()
		{
			Debug.Assert(sprites == 0);
			
			Matrix4 worldMatrix = Matrix4.Ortho(0, graphics.GetViewport().Width, graphics.GetViewport().Height, 0, 0, 1);			
			shaderProg.SetUniformValue(WorldMatrixUniformIndex, ref worldMatrix);
			
			graphics.SetShaderProgram(shaderProg);
		}
		
		public void End()
		{
			Flush();
		}
		
		public void Draw(Vector2 position, Texture texture)
		{
			Draw (position.Xy1, texture, new Rgba(255, 255, 255, 255));
		}
		
		public void Draw(Vector3 position, Texture texture, Rgba color)
		{
			if (currentTexture != texture)
			{
				Flush ();
				currentTexture = texture;
			}
			
			vertices[sprites * 4 * 3] = position.X;
			vertices[sprites * 4 * 3 + 1] = position.Y;
			vertices[sprites * 4 * 3 + 2] = -position.Z;
			
			vertices[sprites * 4 * 3 + 3] = position.X + texture.Width;
			vertices[sprites * 4 * 3 + 4] = position.Y;
			vertices[sprites * 4 * 3 + 5] = -position.Z;
			
			vertices[sprites * 4 * 3 + 6] = position.X + texture.Width;
			vertices[sprites * 4 * 3 + 7] = position.Y + texture.Height;
			vertices[sprites * 4 * 3 + 8] = -position.Z;
			
			vertices[sprites * 4 * 3 + 9] = position.X;
			vertices[sprites * 4 * 3 + 10] = position.Y + texture.Height;
			vertices[sprites * 4 * 3 + 11] = -position.Z;
			
			textureCoordinates[sprites * 8] = 0;
			textureCoordinates[sprites * 8 + 1] = 0;
			
			textureCoordinates[sprites * 8 + 2] = 1;
			textureCoordinates[sprites * 8 + 3] = 0;
			
			textureCoordinates[sprites * 8 + 4] = 1;
			textureCoordinates[sprites * 8 + 5] = 1;
			
			textureCoordinates[sprites * 8 + 6] = 0;
			textureCoordinates[sprites * 8 + 7] = 1;
			
			colours[sprites * 16] = color.R;
			colours[sprites * 16 + 1] = color.G;
			colours[sprites * 16 + 2] = color.B;
			colours[sprites * 16 + 3] = color.A;
			
			colours[sprites * 16 + 4] = color.R;
			colours[sprites * 16 + 5] = color.G;
			colours[sprites * 16 + 6] = color.B;
			colours[sprites * 16 + 7] = color.A;
			
			colours[sprites * 16 + 8] = color.R;
			colours[sprites * 16 + 9] = color.G;
			colours[sprites * 16 + 10] = color.B;
			colours[sprites * 16 + 11] = color.A;
			
			colours[sprites * 16 + 12] = color.R;
			colours[sprites * 16 + 13] = color.G;
			colours[sprites * 16 + 14] = color.B;
			colours[sprites * 16 + 15] = color.A;
			
			sprites++;
			
			if (sprites == MAX_SPRITES_PER_FLUSH)
				Flush ();
		}
		
		private void Flush()
		{		
			buffer.SetVertices(PositionAttributeIndex, vertices, 0, 0, sprites * 4);
			buffer.SetVertices(TexCoordAttributeIndex, textureCoordinates, 0, 0, sprites * 4);
			buffer.SetVertices(VertexColorAttributeIndex, colours, 0, 0, sprites * 4);
			
			graphics.SetVertexBuffer(0, buffer);
			
			graphics.SetTexture(0, currentTexture);
			
			graphics.DrawArrays(DrawMode.Triangles, 0, sprites * 6);
			
			sprites = 0;
		}
	}
}

