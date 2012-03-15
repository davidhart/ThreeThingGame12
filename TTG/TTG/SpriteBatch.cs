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
		
		public void Draw(Texture2D texture, Vector2 position)
		{
			Draw (texture, position.Xy1, new Rgba(255, 255, 255, 255));
		}
		
		public void Draw(Texture2D texture, ImageRect destRect, ImageRect srcRect)
		{
			Draw (texture, destRect, srcRect, new Rgba(255, 255, 255, 255));
		}
		
		
		public void Draw(Texture2D texture, ImageRect destRect, ImageRect srcRect, Rgba color)
		{
			Draw (texture, 
			      new Vector2(destRect.X, destRect.Y),
			      new Vector2(destRect.X + destRect.Width, destRect.Y + destRect.Height),
			      new Vector2(srcRect.X / (float)texture.Width, srcRect.Y / (float)texture.Height),
				  new Vector2((srcRect.X + srcRect.Width) / (float)texture.Width, (srcRect.Y + srcRect.Height) / (float)texture.Height),
			      1.0f,
				  color);
		}
		
		public void Draw(Texture2D texture, Vector3 position, Rgba color)
		{			
			Draw(texture, 
			     position.Xy, 
			     position.Xy + new Vector2(texture.Width, texture.Height),
			     new Vector2(0, 0), new Vector2(1, 1),
			     position.Z,
			     color);
		}
		
		private void Draw(Texture2D texture, Vector2 destP1, Vector2 destP2, Vector2 srcP1, Vector2 srcP2, float depth, Rgba color)
		{
			if (currentTexture != texture)
			{
				Flush ();
				currentTexture = texture;
			}
			
			vertices[sprites * 4 * 3] = destP1.X;
			vertices[sprites * 4 * 3 + 1] = destP1.Y;
			vertices[sprites * 4 * 3 + 2] = -depth;
			
			vertices[sprites * 4 * 3 + 3] = destP2.X;
			vertices[sprites * 4 * 3 + 4] = destP1.Y;
			vertices[sprites * 4 * 3 + 5] = -depth;
			
			vertices[sprites * 4 * 3 + 6] = destP2.X;
			vertices[sprites * 4 * 3 + 7] = destP2.Y;
			vertices[sprites * 4 * 3 + 8] = -depth;
			
			vertices[sprites * 4 * 3 + 9] = destP1.X;
			vertices[sprites * 4 * 3 + 10] = destP2.Y;
			vertices[sprites * 4 * 3 + 11] = -depth;
			
			textureCoordinates[sprites * 8] = srcP1.X;
			textureCoordinates[sprites * 8 + 1] = srcP1.Y;
			
			textureCoordinates[sprites * 8 + 2] = srcP2.X;
			textureCoordinates[sprites * 8 + 3] = srcP1.Y;
			
			textureCoordinates[sprites * 8 + 4] = srcP2.X;
			textureCoordinates[sprites * 8 + 5] = srcP2.Y;
			
			textureCoordinates[sprites * 8 + 6] = srcP1.X;
			textureCoordinates[sprites * 8 + 7] = srcP2.Y;
			
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

