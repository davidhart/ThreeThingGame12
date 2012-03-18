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
	enum LevelState
	{
		Playing,
		Upgrading
	}
	enum CellType
	{
		Trench,
		Platform,
		Bridge,
		FishPile,
		TurretPlacement,
		Switch
	}
	
	
	public enum PathOption
	{
		Continue,
		Up,
		Down,
		Left,
		Right,
		Stop,
	}
	
	struct LevelCell
	{
		public CellType type;
		public PathOption pathOption;
		public PathOption switchOption;
		public byte modelLookup;
		
		
		
		public bool IsTrench()
		{
			return type == CellType.Trench || type == CellType.Bridge || type == CellType.FishPile || type == CellType.Switch;
		}
		
		public PathOption GetPathingOption()
		{
			Debug.Assert(IsTrench());
			if(type == CellType.Switch)
			{
				Switch ();
			}
			return pathOption;			
		}
		
		private void Switch(){
			PathOption temp = switchOption;
			switchOption = pathOption;
			pathOption = temp;
		}
	}
	
	class MapObject
	{
		public Matrix4 worldMatrix;	
	}
	
	public class Level
	{	
		LevelState levelState = LevelState.Playing;
		//Enemy waves
		
		public List<Enemy[]> waves = new List<Enemy[]>();
		public int Points = 1000;
		public int Fish =  100;
		public int WaveNumber = 0;
		int maxWaves, counter;
		
		Random rand = new Random();
		
		const float UPGRADEDIST = 3.5f;
		
		private LevelCell[,] levelData;
		private int width;
		private int height;
		
		private Model[] models;
		private Model fishPileModel;
		private Model bridgeModel;
		
		private BasicProgram program;
		private GraphicsContext graphics;
		
		public int lives;
		
		Vector2 spawnPos;
		Direction spawnDir;
		
		List<Turret> turretPlacements = new List<Turret>();
		
		List<MapObject> bridges;
		List<MapObject> fishPiles;
		
		UpgradeUI upgradeUI;
		SpriteBatch spritebatch;
		BillboardBatch billboardbatch;
		
		Enemy[] enemies;
		
		
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
		
		public Level(GraphicsContext graphics, 
		             BasicProgram program, 
		             UpgradeUI uUi, 
		             SpriteBatch sb,
		             BillboardBatch billboardbatch,
		             int waveAmount)
		{
			this.graphics = graphics;
			this.program = program;
			upgradeUI = uUi;
			maxWaves = waveAmount;
			lives = 0;
			
			models = new Model[17];
			for (int i = 0; i < models.Length; ++i)
			{
				models[i] = new Model("mapparts/part" + i.ToString() + ".mdx", 0);	
			}
			spritebatch = sb;
			this.billboardbatch = billboardbatch;
			bridgeModel = new Model("mapparts/bridge.mdx", 0);
			fishPileModel = new Model("mapparts/fish.mdx", 0);
			
			EnemyTypes.Initialise();
			for (int i = 0; i < waveAmount; ++i)
			{
				
				Enemy[] currentWave = new Enemy[waveAmount];
				
				for (int j = 0; j < currentWave.Length; ++j)
				{
					currentWave[j] = new Enemy(graphics, EnemyTypes.basicEnemy, this, program); 
				}
				
				if (i == waveAmount -1)
				{
					//boss
					for(int k = 0; k < currentWave.Length; ++k)
					{
						currentWave[k].type = EnemyTypes.boss;
					}
					waves.Add(currentWave);
				}
				else if(i%3 == 0)
				{
					//Fast enemy
					for(int k = 0; k < currentWave.Length; ++k)
					{
						currentWave[k].type = EnemyTypes.fastEnemy;
					}
					waves.Add(currentWave);
				}
				else if(i%5 == 0)
				{
					//slow enemy
					for(int k = 0; k < currentWave.Length; ++k)
					{
						currentWave[k].type = EnemyTypes.slowEnemy;
					}
					waves.Add(currentWave);
				}
				else
				{
					//standard enemies
					waves.Add(currentWave);
				}
			}
		}
		
		public void Load(string filename)
		{
		
				TextReader tr = new StreamReader(filename);
				
				string fileContents = tr.ReadToEnd();
				
				GenerateLevel(fileContents);


		}
		
		void GenerateLevel(string fileContents)
		{			
			string [] lines = fileContents.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			
			width = lines[0].Length; // Assume all lines are the same length;
			height = lines.Length;
			levelData = new LevelCell[width, height];
			bridges = new List<MapObject>();
			fishPiles = new List<MapObject>();
			
			Debug.WriteLine("Width: " + width + ", Height: " + height);
			
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
					// switches
					else if (c == 'Q')	// left-down switch
					{
						levelData[x,y].type = CellType.Switch;
						levelData[x,y].pathOption = PathOption.Left;
						levelData[x,y].switchOption = PathOption.Down;
					}
					else if (c == 'W') // left-up
					{
						levelData[x,y].type = CellType.Switch;
						levelData[x,y].pathOption = PathOption.Left;
						levelData[x,y].switchOption = PathOption.Up;
					}
					else if(c == 'E') // left-right
					{
						levelData[x,y].type = CellType.Switch;
						levelData[x,y].pathOption = PathOption.Left;
						levelData[x,y].switchOption = PathOption.Right;
					}
					else if(c == 'R') // right-down
					{
						levelData[x,y].type = CellType.Switch;
						levelData[x,y].pathOption = PathOption.Right;
						levelData[x,y].switchOption = PathOption.Down;
					}
					else if(c == 'Y') // right-up
					{
						levelData[x,y].type = CellType.Switch;
						levelData[x,y].pathOption = PathOption.Right;
						levelData[x,y].switchOption = PathOption.Up;
					}
					else if(c=='U') // up-down
					{
						levelData[x,y].type = CellType.Switch;
						levelData[x,y].pathOption = PathOption.Up;
						levelData[x,y].switchOption = PathOption.Down;
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
						levelData[x,y].pathOption = PathOption.Stop;
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
						turretPlacements.Add(new Turret(graphics, this, program, new Vector2(x,y), billboardbatch ));
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
						
						if (levelData[x,y].type == CellType.Bridge)
						{
							Matrix4 orientation;
							
							if (((index & 2) != 0) && ((index & 8) != 0))
								orientation = Matrix4.Identity;
							else
								orientation = new Matrix4(new Vector3(0, 0, 1),
								                          new Vector3(0, 1, 0),
								                          new Vector3(-1, 0, 0),
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
			
			InitialiseWaves();
			
			lives = 15;			 
		}
		
		public int GetLives()
		{
			return lives;	
		}
		
		public void RemoveLife()
		{
			lives--;
			
			if (lives == 0)
			{
				Reset();
				Game.gameState = GameState.GameOver;
			}
		}
		
		void InitialiseWaves()
		{
			//EnemyTypes.Initialise();
			Enemy[] currentWave = waves[WaveNumber];
			counter = waves[WaveNumber].Length;
			
			for (int i = 0; i < currentWave.Length; i++)
			{
				currentWave[i].SetPosition((int)SpawnPos.X, (int)spawnPos.Y, SpawnDir);
				currentWave[i].SpawnTime = 2 * i;
				currentWave[i].Health = currentWave[i].type.health;
			}
		}
		
		public void Reset()
		{
			for (int i = 0; i < turretPlacements.Count; i++)
			{
				turretPlacements[i].type = null;	
			}
			
			lives = 15;
			Points = 1000;
		}
		
		void NextWave()
		{
			bool alldead = true;
			Enemy[] currentWave = waves[WaveNumber];
			for (int i = 0; i < currentWave.Length; i++)
			{
				if(currentWave[i].Health > 0)
				{
					alldead = false;
					break;
				}
				
			}
			
			if (alldead && WaveNumber < maxWaves)
			{
				WaveNumber++;
				if(WaveNumber == maxWaves)
				{
					WaveNumber = 0;
					Game.gameState = GameState.Win;
					Reset ();
				}
				else
				{
					
				InitialiseWaves();
				}
			}
			
		}
		
		public Enemy[] GetEnemies()
		{
			return enemies;
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
		
		// Tank / World collision
		public bool CollisionDetection(Vector3 testPosition)
		{
			float cellX = (float)Math.Round(testPosition.X / 2.0f);
			float cellZ = (float)Math.Round(testPosition.Z / 2.0f);
			
			if(cellX < 0 || cellZ < 0 || cellX > width - 1 || cellZ > height - 1)  
			{
				//Debug.WriteLine("cell x = " + (int)cellX + ", pos x = " + testPosition.X/2.0f + " : cell z = " + (int)cellZ + ", pos z = " + testPosition.Z/2.0f);
				return true;
			}
			else
			{
				LevelCell cell = levelData[(int)cellX, (int)cellZ];
				//Debug.WriteLine("cell x = " + (int)cellX + ", pos x = " + testPosition.X/2.0f + " : cell z = " + (int)cellZ + ", pos z = " + testPosition.Z/2.0f);
				return (cell.type != CellType.Platform && cell.type != CellType.Bridge) ? true : false;
			}
		}
		
		public PathOption GetCellPathOption(int x, int y)
		{
			return levelData[x, y].GetPathingOption();	
		}
		
		public void Draw()
		{				
			graphics.Disable( EnableMode.CullFace );
			enemies = waves[WaveNumber];
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].Draw();	
			}
			graphics.Enable(EnableMode.CullFace);
			
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byte index = levelData[x,y].modelLookup;
					Matrix4 world = Matrix4.Translation(new Vector3( x * 2.0f, 0.0f,  y * 2.0f));
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
			for (int i = 0; i < turretPlacements.Count; ++i)
			{
				turretPlacements[i].Draw();
			}
			
			upgradeUI.Draw(spritebatch);
		}
		
		public void Update (float dt, UpgradeUI ui, GamePadData data, Player player)
		{
			enemies = waves[WaveNumber];
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].Update(dt);	
			}
			
			for (int i = 0; i < turretPlacements.Count; i++)
			{
				turretPlacements[i].Update(dt, enemies);
			}
			
			// If we moved away from selected turret, hide menu
			if (upgradeUI.show)
			{
				float dist = Vector2.Distance(player.GetPosition ().Xz, upgradeUI.turret.GetPosition ().Xz);
				if (dist > UPGRADEDIST)
				{
					upgradeUI.show = false;
				}
			}
			
			if (!upgradeUI.Update(data, ref Points))
			{
				player.DrawBuyMenuIcon (false);
			
				for (int i = 0; i < turretPlacements.Count; ++i)
				{
					float dist = Vector2.Distance (player.GetPosition ().Xz, turretPlacements [i].GetPosition ().Xz);
				
					if (dist <= UPGRADEDIST)
					{
						player.DrawBuyMenuIcon (true);
					
						if (data.ButtonsDown.HasFlag (GamePadButtons.Cross) && !upgradeUI.show)
						{
							upgradeUI.show = true;
							player.DrawBuyMenuIcon (false);
							upgradeUI.turret = turretPlacements [i];
						}
					}					
				}
			}
			NextWave();
		}
	}
}

