using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	public class EnemyType
	{
		public Model model;
		public float speed;
	}
	
	public class EnemyTypes
	{
		static public void Initialise()
		{
			if (basicEnemy != null)
				return;
			
			basicEnemy = new EnemyType();
			basicEnemy.model = new Model("penguin.mdx", 0);
			basicEnemy.speed = 1.0f;
		}
		
		static public EnemyType basicEnemy;
	}
	
	public enum Direction
	{
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3,
		Stop = 4
	}
	
	class EnemyData
	{
		static void Initialise()
		{
			orientationMatrices = new Matrix4[4];
			orientationMatrices[0] = new Matrix4(new Vector3(0, 0, -1), // left
			                                     new Vector3(0, 1, 0), 
			                                     new Vector3(-1, 0, 0), 
			                                     new Vector3(0, 0, 0));
				
			orientationMatrices[1] = new Matrix4(new Vector3(0, 0, 1),  // right
			                                     new Vector3(0, 1, 0), 
			                                     new Vector3(1, 0, 0), 
			                                     new Vector3(0, 0, 0));
			
			orientationMatrices[2] = new Matrix4(new Vector3(-1, 0, 0), // up
			                                     new Vector3(0, 1, 0), 
			                                     new Vector3(0, 0, -1), 
			                                     new Vector3(0, 0, 0));
				
			orientationMatrices[3] = new Matrix4(new Vector3(1, 0, 0),  // down
			                                     new Vector3(0, 1, 0), 
			                                     new Vector3(0, 0, 1), 
			                                     new Vector3(0, 0, 0));
			
			xOffsets = new int[5];
			xOffsets[0] = -1; // left
			xOffsets[1] = 1;  // right
			xOffsets[2] = 0;  // up
			xOffsets[3] = 0;  // down
			xOffsets[4] = 0;
			
			yOffsets = new int[5];
			yOffsets[0] = 0;  // left
			yOffsets[1] = 0;  // right
			yOffsets[2] = -1; // up
			yOffsets[3] = 1;  // down
			yOffsets[4] = 0;
		}
		
		public static Matrix4 GetOrientationMatrix(Direction direction)
		{
			if (orientationMatrices == null)
			{
				Initialise();
			}
			
			return orientationMatrices[(int)direction];
		}
		
		public static int GetXOffset(Direction direction)
		{
			if(xOffsets == null)
			{
				Initialise();	
			}
			
			return xOffsets[(int)direction];
		}
		
		public static int GetYOffset(Direction direction)
		{
			if(yOffsets == null)
			{
				Initialise();	
			}
			
			return yOffsets[(int)direction];
		}
		
		static int [] xOffsets; // x offset for each direction
		static int [] yOffsets; // y offset for each direction
		static Matrix4 [] orientationMatrices; // rotation matrix for each direction
	}
	
	public class Enemy
	{	    
		protected float health = 50;
		public float Health
		{
			get
			{
				return health;
			}
			set
			{
				health = value;
			}
		}
		
		private Level level;
		private int xTilePos;
		private int yTilePos;
		
		private AnimationState state;
		private EnemyType type;
		private BasicProgram program;
		
		private Direction direction;
		private float offset; // offset in direction, 0 is exactly on the tile, 1.0 is on the next tile
		private GraphicsContext graphics;
		
		public float SpawnTime;
		
		public Enemy (GraphicsContext graphics, EnemyType type, Level level, BasicProgram program)
		{
			this.graphics = graphics;
			this.type = type;
			this.level = level;
			this.program = program;
			
			direction = Direction.Down;			
			state = new AnimationState(type.model);
		}
		
		public void SetPosition(int tileX, int tileY, Direction direction)
		{
			xTilePos = tileX;
			yTilePos = tileY;
			this.direction = direction;
		}
		
		public void Draw ()
		{
			if (SpawnTime > 0 || health <= 0)
				return;
			
			if(direction != Direction.Stop)
			{
				Matrix4 world = Matrix4.Translation(GetPosition()) * Matrix4.Scale(new Vector3(0.6f)) * EnemyData.GetOrientationMatrix(direction);	
				type.model.SetWorldMatrix(ref world);			
				type.model.SetAnimationState(state);
				type.model.Update();
				type.model.Draw(graphics, program);
			}
		}
		
		public void Update (float dt)
		{
			if (SpawnTime > 0)
			{
				SpawnTime -= dt;
							
				if (SpawnTime > 0)
				{
					return;
				}
				else
				{
					dt = -SpawnTime;	
				}
			}
			
			if(direction != Direction.Stop)
			{
				state.Update(dt * type.speed);
				
				offset += dt * type.speed;
			
				while (offset > 1)
				{
					// Move one tile in current direction
					offset -= 1;
					xTilePos += EnemyData.GetXOffset(direction);
					yTilePos += EnemyData.GetYOffset(direction);
				
					// Check for direction changes in new cell
					PathOption p = level.GetCellPathOption(xTilePos, yTilePos);
				
					if (p == PathOption.Left)       direction = Direction.Left;
					else if (p == PathOption.Right) direction = Direction.Right;
					else if (p == PathOption.Down)  direction = Direction.Down;
					else if (p == PathOption.Up)	direction = Direction.Up;
					else if (p == PathOption.Stop)
					{
						health = 0;
						direction = Direction.Stop;
					}
					
				}
			}
		}
		
		public Vector3 GetPosition()
		{
			return new Vector3((xTilePos + EnemyData.GetXOffset(direction) * offset)* 2, 
			                   0, 
			                   (yTilePos + EnemyData.GetYOffset(direction) * offset)* 2);	
		}
	}
}

