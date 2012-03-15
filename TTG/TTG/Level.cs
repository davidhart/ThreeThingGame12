using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

using Sce.Pss.HighLevel.UI;

namespace TTG
{
	enum CellType
	{
		Trench,
		Platform
	}
	
	public enum PathOption
	{
		Continue,
		Up,
		Down,
		Left,
		Right,
	}
	
	struct LevelCell
	{
		public CellType type;
		public PathOption pathOption;
		
		public byte modelLookup;
		
		public bool IsTrench()
		{
			return type == CellType.Trench;
		}
		
		public PathOption GetPathingOption()
		{
			Debug.Assert(IsTrench());
			
			return pathOption;			
		}
	}
	
	public class Level
	{	
		private LevelCell[,] levelData;
		private int width;
		private int height;
		private int fish;
		
		private Model[] models;
		private BasicProgram program;
		private GraphicsContext graphics;
		
		Vector2 spawnPos;
		Direction spawnDir;
		
		Scene scene;
		Label fishLabel, healthLabel, pointsLabel;
		Button bearImage, fishImage, pointsImage;
		
		public Vector2 SpawnPos
		{
			get
			{
				return spawnPos;
			}
			set
			{
				spawnPos = value;
			}
		}
		public Direction SpawnDir
		{
			get
			{
				return spawnDir;
			}
			set
			{
				spawnDir = value;
			}
		}
		
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
					
					//trench
					if (c == '.') 
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Continue;
					}
					
					//
					
					//penguin orientation
					else if (c == '>')
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Right;
					}
					else if (c == '<')
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Left;
					}
					else if (c == 'v')
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Down;	
					}
					else if (c == '^')
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Up;	
					}
					else if(c == '+')
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Continue;
					}
					
					//spawners
					else if(c == '1') //spawn dacing downwards
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Down;	
						spawnPos = new Vector2(x,y);
						spawnDir = Direction.Down;
					}
					else if(c == '2')//upwards
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Up;	
						spawnPos = new Vector2(x,y);
						spawnDir = Direction.Up;
					}
					else if(c == '3')//left
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Left;	
						spawnPos = new Vector2(x,y);
						spawnDir = Direction.Left;
					}
					else if(c == '4')//right
					{
						levelData[x,y].type = CellType.Trench;
						levelData[x,y].pathOption = PathOption.Right;	
						spawnPos = new Vector2(x,y);
						spawnDir = Direction.Right;
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
		
		public PathOption GetCellPathOption(int x, int y)
		{
			return levelData[x, y].GetPathingOption();	
		}
		
		public void Draw()
		{	
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byte index = levelData[x,y].modelLookup;
					Matrix4 world = Matrix4.Translation(new Vector3(x * 2.0f, 0.0f, y * 2.0f));
					models[index].SetWorldMatrix( ref world );
					models[index].Update();
					models[index].Draw(graphics, program);
				}
			}
		}
		
		public void Update()
		{
		}
	}
}

