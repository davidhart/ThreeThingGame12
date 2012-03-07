using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Imaging;

namespace TTG
{
	public class Sprite
	{
		#region Graphics
		protected GraphicsContext graphics;
		float[] vertices = new float[12];
		float[] texcoord =
		{
			0.0f, 0.0f,
			0.0f, 1.0f,
			1.0f, 0.0f,
			1.0f, 1.0f,
		};
		float[] colours =
		{
			1.0f, 1.0f, 1.0f, 1.0f,
			1.0f, 1.0f, 1.0f, 1.0f,
			1.0f, 1.0f, 1.0f, 1.0f,
			1.0f, 1.0f, 1.0f, 1.0f,
		};
		const int indexSize = 4;
		ushort[] indices;
		VertexBuffer vb;
		protected Texture2D texture;
		ShaderProgram shaderProg;
		#endregion
		
		#region Properties
		public Vector3 Position;
		public Vector2 Center;
		float height, width;
		public float Width()
		{
			return width;
		}
		public float Height()
		{
			return height;
		}
		#endregion
		
		#region Setters
		public void SetColour(Vector4 colour)
		{
			for(int i = 0; i < colours.Length; i+=4)
			{
				colours[i] = colour.R;
				colours[i+1] = colour.G;
				colours[i+2] = colour.B;
				colours[i+3] = colour.A;
				
			}
		}
		
		public void SetTexCoords(float x0, float y0, float x1, float y1)
		{
			texcoord[0] = x0 / texture.Width;
			texcoord[1] = y0 / texture.Height;
			texcoord[2] = x0 / texture.Width;
			texcoord[3] = y1 / texture.Height;
			texcoord[4] = x1 / texture.Width;
			texcoord[5] = y0 / texture.Height;
			texcoord[6] = x1 / texture.Width;
			texcoord[7] = y1 / texture.Height;
		}
		
		public void SetTexCoords(Vector2 topLeft, Vector2 bottomRight)
		{
			SetTexCoords(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
		}
		
		#endregion
		
		public Sprite (GraphicsContext inGraphics, Texture2D inTex2D)
		{
			if(inTex2D == null)
			{
				throw new Exception("ERROR: inTex2D cannot have a null value." +
					"Have you checked the texture is loaded in correctly?");
			}
			
			graphics = inGraphics;
			texture = inTex2D;
			width = texture.Width;
			height = texture.Height;
			
			indices = new ushort[indexSize];
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 3;
			
			vb = new VertexBuffer(
				4,
				indexSize,
				VertexFormat.Float3, //vertices
				VertexFormat.Float2, //Tex coords
				VertexFormat.Float4); //Colours
			
			shaderProg = new ShaderProgram("shaders/sprite.cgx");
			shaderProg.SetUniformBinding(0, "u_WorldMatrix");
		}
		
		public void Draw()
		{
			vertices[0]=Position.X + (-width * Center.X);
			vertices[1]=Position.Y+(-height * Center.Y);
			vertices[2]=Position.Z;
			vertices[3]=Position.X + (-width * Center.X);
			vertices[4]=Position.Y+( height * (1.0f-Center.Y));
			vertices[5]=Position.Z;
			vertices[6]=Position.X +  width * (1.0f-Center.X);
			vertices[7]=Position.Y+(-height * Center.Y);
			vertices[8]=Position.Z;
			vertices[9]=Position.X +  width * (1.0f-Center.X);
			vertices[10]=Position.Y+( height * (1.0f-Center.Y));
			vertices[11]=Position.Z;
			vb.SetVertices(0, vertices);
			vb.SetVertices(1, texcoord);
			vb.SetVertices(2, colours);
			vb.SetIndices(indices);
			graphics.SetVertexBuffer(0, vb);
			graphics.SetTexture(0, texture);
			graphics.DrawArrays(DrawMode.TriangleStrip, 0, indexSize);
		}
	}
}

