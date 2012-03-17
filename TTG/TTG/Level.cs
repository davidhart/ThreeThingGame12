using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

using Sce.Pss.HighLevel.UI;

namespace TTG
{
	enum CellType
	{
		Trench,
		Platform,
		Bridge,
		FishPile,
		TurretPlacement
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
			return type == CellType.Trench || type == CellType.Bridge || type == CellType.FishPile;
		}
		
		public PathOption GetPathingOption()
		{
			Debug.Assert(IsTrench());
			
			return pathOption;			
		}
	}
	
	class MapObject
	{
		public Matrix4 worldMatrix;	
	}
	
	public class Level
	{	
		private LevelCell[,] levelData;
		private int width;
		private int height;
		//private int fish;
		
		private Model[] models;
		private Model fishPileModel;
		private Model bridgeModel;
		private Model turretPlacementModel;
		
		private BasicProgram program;
		private GraphicsContext graphics;
		
		Vector2 spawnPos;
		Direction spawnDir;
		
		List<Vector2> turretPos;
		
		List<MapObject> bridges;
		List<MapObject> fishPiles;
		List<MapObject> turretPlacements;
		
		Turret [] turrets;
		
		//Scene scene;
		//Label fishLabel, healthLabel, pointsLabel;
		//Button bearImage, fishImage, pointsImage;
		
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
			
			bridgeModel = new Model("mapparts/bridge.mdx", 0);
			fishPileModel = new Model("mapparts/fish.mdx", 0);
			turretPlacementModel = new Model("mapparts/turretbase.mdx", 0);
		}
		
		public void Load(string filename)
		{
			try
			{				
				TextReader tr = new StreamReader(filename);
				
				string fileContents = tr.ReadToEnd();
				
				GenerateLevel(fileContents);
				
				turrets = new Turret[turretPlacements.Count];
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
			bridges = new List<MapObject>();
			fishPiles = new List<MapObject>();
			turretPlacements = new List<MapObject>();
			turretPos = new List<Vector2>();
			
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
					// bridge
					else if (c == 'B')
					{
						levelData[x,y].type = CellType.Bridge;
						levelData[x,y].pathOption = PathOption.Continue;
					}
					// fish pile
					else if (c == 'F')
					{
						levelData[x,y].type = CellType.FishPile;
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
					else if(c == 'T')	// Turret placement
					{
						levelData[x,y].type = CellType.TurretPlacement;
						levelData[x,y].pathOption = PathOption.Continue;
						turretPos.Add(new Vector2(x,y));
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
						
						if(levelData[x,y].type == CellType.TurretPlacement)
						{
							Matrix4 world = Matrix4.Translation(new Vector3(x * 2.0f, 0.0f, y*2.0f));
							MapObject f = new MapObject();
							f.worldMatrix = world;
							turretPlacements.Add(f);
						}
					}
					else
					{
						byte index = 0;
						
						if (IsCellTrench(x - 1, y)) index += 1;
						if (IsCellTrench(x, y - 1)) index += 2;
						if (IsCellTrench(x + 1, y)) index += 4;
						if (IsCellTrench(x, y + 1)) index += 8;
						
						levelData[x,y].modelLookup = index;
						
						if (levelData[x,y].type == CellType.Bridge)
						{
							Matrix4 orientation;
							
							if (((index & 2) != 0) && ((index & 8) != 0))
								orientation = Matrix4.Identity;
							else
								orientation = new Matrix4(new Vector3(0, 0, 1),
								                          new Vector3(0, 1, 0),
								                          new Vector3(1, 0, 0),
								                          new Vector3(0, 0, 0));
							
							Matrix4 world = Matrix4.Translation(new Vector3(x * 2.0f, 0.0f, y * 2.0f)) * orientation;
							
							MapObject b = new MapObject();
							b.worldMatrix = world;
							
							bridges.Add(b);
						}
						
						if (levelData[x,y].type == CellType.FishPile)
						{
							Matrix4 world = Matrix4.Translation(new Vector3(x * 2.0f, 0.0f, y * 2.0f));
							MapObject f = new MapObject();
							f.worldMatrix = world;
							fishPiles.Add(f);
						}
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
			
			for (int i = 0; i < bridges.Count; ++i)
			{
				bridgeModel.SetWorldMatrix(ref bridges[i].worldMatrix);
				bridgeModel.Update();
				bridgeModel.Draw(graphics, program);
			}
			
			for (int i = 0; i < fishPiles.Count; ++i)
			{
				fishPileModel.SetWorldMatrix(ref fishPiles[i].worldMatrix);
				fishPileModel.Update();
				fishPileModel.Draw(graphics, program);
			}
			
			for(int i = 0 ; i < turretPlacements.Count; ++i)
			{
				turretPlacementModel.SetWorldMatrix(ref turretPlacements[i].worldMatrix);
				turretPlacementModel.Update();
				turretPlacementModel.Draw(graphics, program);
			}
		}
		
		public void Update(UpgradeUI ui, GamePadData data, Vector2 playerPos)
		{
			if(data.ButtonsDown == GamePadButtons.Triangle)
			{
				for(int i = 0; i < turrets.Length; ++i)
				{
					
					ui.show = !ui.show;
				}
			}
		}
	}
}

