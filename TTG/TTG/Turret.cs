using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

using System.Collections;

namespace TTG
{
	
	
	public class TurretData
	{
		static int[] xOffsets; // x offset for each direction
		static int[] yOffsets; // y offset for each direction
		
		static void Initialise ()
		{
			xOffsets = new int[4];
			xOffsets [0] = -1; // left
			xOffsets [1] = 1;  // right
			xOffsets [2] = 0;  // up
			xOffsets [3] = 0;  // down
			
			yOffsets = new int[4];
			yOffsets [0] = 0;  // left
			yOffsets [1] = 0;  // right
			yOffsets [2] = -1; // up
			yOffsets [3] = 1;  // down
		}
		
		public static int GetXOffset (Direction direction)
		{
			if (xOffsets == null) {
				Initialise ();	
			}
			
			return xOffsets [(int)direction];
		}
		
		public static int GetYOffset (Direction direction)
		{
			if (yOffsets == null) {
				Initialise ();	
			}
			
			return yOffsets [(int)direction];
		}
		
	}

	public class Turret
	{
		Model model, turretPlacementModel;
		
		#region Stats
		protected byte atkDmg;

		public byte AtkDmg {
			get {
				return atkDmg;
			}
			set {
				atkDmg = value;
			}
		}
		
		protected float atkRange;

		public float AtkRange {
			get {
				return atkRange;
			}
			set {
				atkRange = value;
			}
		}
		
		protected float atkSpeed;

		public float AtkSpeed {
			get {
				return atkSpeed;
			}
			set {
				atkSpeed = value;
			}
		}
		
		#endregion
		
		Level level;
		int xTilePos;
		int yTilePos;
		BasicProgram program;
		float offset; // offset in direction, 0 is exactly on the tile, 1.0 is on the next tile
		
		GraphicsContext graphics;
		
		bool Placed = false;
		Enemy target = null;
		
		public Turret (GraphicsContext inGraphics, Level level, BasicProgram program, Vector2 tilePos)
		{
			graphics = inGraphics;
			this.level = level;
			this.program = program;
			turretPlacementModel = new Model("mapparts/turretbase.mdx", 0);
			xTilePos = (int)tilePos.X;
			yTilePos = (int)tilePos.Y;
		}
		
		public void SetModel(Model inModel)
		{
			model = inModel;
		}
		
		public void SetPosition (int tileX, int tileY)
		{
			xTilePos = tileX;
			yTilePos = tileY;
		}
		
		public void Update (float dt, Enemy[] enemies)
		{
			if (Placed) 
			{
				if (target == null) {
					for (int i = 0; i < enemies.Length; ++i) {
						float distance = Vector2.Distance (enemies[i].GetPosition().Xz, GetPosition().Xz);
						if (distance <= atkRange) {
							target = enemies [i];
							break;
						}
					}
				} 
				else {
					if (target.Health <= 0) {
						target = null;
					} else {
						target.Health -= (atkDmg) * dt;
						float distance = Vector2.Distance (target.GetPosition().Xy, GetPosition().Xz);
						if (distance > atkRange) {
							target = null;
						}
					}
				
				}
			}   
		}
		
		public void Draw ()
		{
			Matrix4 world = Matrix4.Translation (GetPosition ()) * Matrix4.Scale (new Vector3 (0.6f));	
			
			turretPlacementModel.SetWorldMatrix (ref world);
			turretPlacementModel.Update ();
			turretPlacementModel.Draw (graphics, program);
			
			if (Placed) 
			{
				model.SetWorldMatrix (ref world);			
				model.Update ();
				model.Draw (graphics, program);
			}
		}
		
		public Vector3 GetPosition ()
		{
			return new Vector3 ((xTilePos + EnemyData.GetXOffset (0) * offset) * 2, 
			                   -1, 
			                   (yTilePos + EnemyData.GetYOffset (0) * offset) * 2);	
		}
	}
}

