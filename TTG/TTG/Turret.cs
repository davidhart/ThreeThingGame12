using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

using System.Collections;
using System.Diagnostics;

namespace TTG
{
	public class TurretType
	{
		public Model model;
		public byte AtkDmg;
		public float AtkRange;
		public float AtkSpeed;
	}
	
	public class TurretTypes
	{
		public static void Initialise()
		{
			if (machineGunTurret != null)
				return;
			
			machineGunTurret = new TurretType();
			machineGunTurret.AtkRange = 4;
			machineGunTurret.AtkSpeed = 0.5f;
			machineGunTurret.AtkDmg = 2;
			machineGunTurret.model = TurretModels.machineGunTurret;
		}
		
		public static TurretType machineGunTurret;
	}
	
	public class TurretModels
	{
		public static void Initialise()
		{
			if (turretPlacement != null)
				return;
			
			turretPlacement = new Model("mapparts/turretbase.mdx", 0);
			machineGunTurret = new Model("assets/turret.mdx", 0);
		}
		
		public static Model turretPlacement;
		public static Model machineGunTurret;
	}

	public class Turret
	{
		Level level;
		int xTilePos;
		int yTilePos;
		BasicProgram program;

		GraphicsContext graphics;

		Enemy target = null;
		TurretType type;
		
		public Turret (GraphicsContext inGraphics, Level level, BasicProgram program, Vector2 tilePos)
		{
			TurretModels.Initialise();
			TurretTypes.Initialise();
			
			graphics = inGraphics;
			this.level = level;
			this.program = program;
			xTilePos = (int)tilePos.X;
			yTilePos = (int)tilePos.Y;
		}
		
		public void SetPosition (int tileX, int tileY)
		{
			xTilePos = tileX;
			yTilePos = tileY;
		}
		
		public void Update (float dt, Enemy[] enemies)
		{
			if (type == null)
				return;
			
			if (target == null)
			{
				for (int i = 0; i < enemies.Length; ++i)
				{
					float distance = Vector2.Distance (enemies[i].GetPosition().Xz, GetPosition().Xz);
					if (distance <= type.AtkRange)
					{
						target = enemies[i];
						break;
					}
				}
			} 
			
			if (target != null)
			{
				target.Health -= (type.AtkDmg) * dt;
				
				if (target.Health <= 0)
				{
					target = null;
				} else
				{
					float distance = Vector2.Distance (target.GetPosition().Xz, GetPosition().Xz);
					if (distance > type.AtkRange)
					{
						target = null;
					}
				}
			}  
		}
		
		public void SetType(TurretType type)
		{
			this.type = type;	
		}
		
		public void Draw ()
		{
			Matrix4 world = Matrix4.Translation (GetPosition());
			
			Matrix4 turret;
			
			if (target != null && type != null)
			{
				Vector3 turretTarget = target.GetPosition();
				
				Vector3 turretDirection = (turretTarget - new Vector3(GetPosition().X, turretTarget.Y, GetPosition().Z)).Normalize();
				Vector3 up = new Vector3(0, 1, 0);
			
				turret = world * new Matrix4(turretDirection, up, turretDirection.Cross(up), Vector3.Zero);
			}
			else
			{
				turret = world;	
			}			
			
			// no turret placed
			if (type == null)
			{			
				TurretModels.turretPlacement.SetWorldMatrix (ref turret);
				TurretModels.turretPlacement.Update ();
				TurretModels.turretPlacement.Draw (graphics, program);
			}
			// draw turret model
			else
			{
				type.model.SetWorldMatrix (ref turret);
				type.model.Update ();
				type.model.Draw (graphics, program);
			}
		}
		
		public Vector3 GetPosition ()
		{
			return new Vector3 ((xTilePos) * 2, 
			                   	0, 
			                   	(yTilePos) * 2);
		}
	}
}

