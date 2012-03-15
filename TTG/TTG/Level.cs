using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	enum CellType
	{
		Trench,
		Platform
	}
	
	struct LevelCell
	{
		public CellType type;
		public byte modelLookup;
		
		public bool IsTrench()
		{
			return type == CellType.Trench;	
		}
	}
	
	public class Level
	{	
		private LevelCell[,] levelData;
		private int width;
		private int height;
		
		private Model[] models;
		private BasicProgram program;
		private GraphicsContext graphics;
		
		public Level(GraphicsContext graphics, BasicProgram program)
		{
			this.graphics = graphics;
			this.program = program;
			
			models = new Model[17];
			for (int i = 0; i < models.Length; ++i)
			{
				models[i] = new Model("mapparts/part" + i.ToString() + ".mdx", 0);	
			}
		}
		
		public void Load(string filename)
		{
			try
			{				
				TextReader tr = new StreamReader(filename);
				
				string fileContents = tr.ReadToEnd();
				
				GenerateLevel(fileContents);
			}
			catch
			{
				throw new Exception("Cannot read level file");
			}
		}
		
		void GenerateLevel(string fileContents)
		{
			string [] lines = fileContents.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			
			width = lines[0].Length; // Assume all lines are the same length;
			height = lines.Length;
			levelData = new LevelCell[width, height];
			
			// map characters onto type
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					char c = lines[y][x];
					
					if (c == '.')
					{
						levelData[x,y].type = CellType.Trench;
					}
					else // represented by character #
					{
						levelData[x,y].type = CellType.Platform;
					}
				}
			}
			
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					if (!IsCellTrench(x, y))
					{
						levelData[x,y].modelLookup = 16;
					}
					else
					{
						byte index = 0;
						
						if (IsCellTrench(x - 1, y)) index += 1;
						if (IsCellTrench(x, y - 1)) index += 2;
						if (IsCellTrench(x + 1, y)) index += 4;
						if (IsCellTrench(x, y + 1)) index += 8;
						
						levelData[x,y].modelLookup = index;
					}
				}
			}
			
		}
		
		bool IsCellTrench(int x, int y)
		{
			if (x < 0)
				return false;
							
			if (y < 0)
				return false;
							
			if (x >= width)
				return false;
			
			if (y >= height)
				return false;
			
			return levelData[x, y].IsTrench();
		}
		
		public void Draw()
		{	
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byte index = levelData[x,y].modelLookup;
					Matrix4 world = Matrix4.Translation(new Vector3((- width / 2 + x) * 2.0f, 0.0f, (- height / 2 + y) * 2.0f));
					models[index].SetWorldMatrix( ref world );
					models[index].Update();
					models[index].Draw(graphics, program);
				}
			}
		}
		
		void Update()
		{
		}
	}
}

