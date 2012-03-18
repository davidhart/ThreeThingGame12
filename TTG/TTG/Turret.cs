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
		public TurretType nextUpgrade;
	}
	
	/*
	public struct LevelModifier
	{
		public int DamageIncrease;
		public int RangeIncrease;
		public int SpeedIncrease;
	}*/
	
	public class TurretTypes
	{
		public static void Initialise()
		{
			if (machineGunTurret != null)
				return;
			
		
			
			machineGunTurret = new TurretType();
			machineGunTurret2 = new TurretType();
			machineGunTurret3 = new TurretType();
			machineGunTurret4 = new TurretType();
			machineGunTurret5 = new TurretType();
			icerTurret = new TurretType();
		
			machineGunTurret.AtkRange = 4;
			machineGunTurret.AtkSpeed = 1.0f;
			machineGunTurret.AtkDmg = 10;
			machineGunTurret.model = TurretModels.machineGunTurret;
			machineGunTurret.nextUpgrade = machineGunTurret2;
			
			machineGunTurret2.AtkRange = 5;
			machineGunTurret2.AtkSpeed = 0.7f;
			machineGunTurret2.AtkDmg = 4;
			machineGunTurret2.model = TurretModels.machineGunTurret;
			machineGunTurret2.nextUpgrade = machineGunTurret3;
			
			machineGunTurret3.AtkRange = 5;
			machineGunTurret3.AtkSpeed = 1.0f;
			machineGunTurret3.AtkDmg = 5;
			machineGunTurret3.model = TurretModels.machineGunTurret;
			machineGunTurret3.nextUpgrade = machineGunTurret4;
			
			machineGunTurret4.AtkRange = 5;
			machineGunTurret4.AtkSpeed = 1.5f;
			machineGunTurret4.AtkDmg = 10;
			machineGunTurret4.model = TurretModels.machineGunTurret;
			machineGunTurret4.nextUpgrade = machineGunTurret5;
			
			machineGunTurret5.AtkRange = 5;
			machineGunTurret5.AtkSpeed = 1.5f;
			machineGunTurret5.AtkDmg = 12;
			machineGunTurret5.model = TurretModels.machineGunTurret;
			machineGunTurret5.nextUpgrade = null;
			
			icerTurret.AtkRange = 4;
			icerTurret.AtkSpeed = 0.3f;
			icerTurret.AtkDmg = 1;
			icerTurret.model = TurretModels.machineGunTurret;
			icerTurret.nextUpgrade = null;
		}
			public static TurretType machineGunTurret;
			public static TurretType machineGunTurret2;
			public static TurretType machineGunTurret3;
			public static TurretType machineGunTurret4;
			public static TurretType machineGunTurret5;
		
			public static TurretType icerTurret;

	}
	
	public class TurretModels
	{
		public static void Initialise()
		{
			if (turretPlacement != null)
				return;
			
			turretPlacement = new Model("mapparts/turretbase.mdx", 0);
			machineGunTurret = new Model("assets/turret.mdx", 0);
			
			muzzleFlash = new Texture2D("assets/muzzleFlash.png", false);
			bloodSplat = new Texture2D("assets/bloodSplat.png", false);
		}
		
		public static Model turretPlacement;
		public static Model machineGunTurret;
		
		public static Texture2D muzzleFlash;
		public static Texture2D bloodSplat;
	}

	public class Turret
	{
		Level level;
		int xTilePos;
		int yTilePos;
		BasicProgram program;
		
		public int Level = 1;
		
		GraphicsContext graphics;

		Enemy target = null;
		public TurretType type;
		
		float elapsed;
		BillboardBatch billboardBatch;
		
		public enum TurretState
		{
			Buy,
			Upgrade
		}
		
		public TurretState State = TurretState.Buy;
		
		
		public Turret (GraphicsContext inGraphics, Level level, BasicProgram program, Vector2 tilePos,
		               BillboardBatch billboardBatch)
		{
			TurretModels.Initialise();
			TurretTypes.Initialise();
			this.billboardBatch = billboardBatch;
			
			graphics = inGraphics;
			this.level = level;
			this.program = program;
			xTilePos = (int)tilePos.X;
			yTilePos = (int)tilePos.Y;
			elapsed = 0.0f;
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
					if (distance <= (type.AtkRange) && enemies[i].Health > 0 && enemies[i].SpawnTime <= 0)
					{
						target = enemies[i];
						break;
					}
				}
			} 
			
			if (target != null)
			{
				if(type == TurretTypes.icerTurret)
				{
					target.speedMultiplier = 0.5f;	
				}
				target.Health -= (type.AtkDmg ) * dt;
				
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
	
			elapsed += dt;
			
			if (elapsed > 100)
				elapsed -= 100;
		}
		
		public void SetType(TurretType type)
		{
			this.type = type;	
		}
		
		public void Draw ()
		{
			Matrix4 world = Matrix4.Translation (GetPosition());
			
			Matrix4 turret;
			
			Vector3 turretDirection = Vector3.Zero;
			
			if (target != null && type != null)
			{
				Vector3 turretTarget = target.GetPosition();
				
				turretDirection = (turretTarget - new Vector3(GetPosition().X, turretTarget.Y, GetPosition().Z)).Normalize();
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
				
				if (target != null)
				{
					billboardBatch.Begin();	
					
					if (type == TurretTypes.icerTurret)
					{
						// draw muzzle flash
								
						billboardBatch.Draw(TurretModels.muzzleFlash, GetPosition() + new Vector3(0, 3.3f, 0) + turretDirection.Normalize() * 1.2f, new Vector2(1, 1),
						                    new Rgba(120, 200, 255, (byte)(Math.Sin(elapsed * 6)*100 + 100)));
										
						float splashSize = 0.2f * (elapsed % 2) + 0.4f;
						float alpha = 1 - (elapsed % 2);
					
						billboardBatch.Draw(TurretModels.bloodSplat, target.GetPosition() + new Vector3(0, 1.0f, 0), new Vector2(splashSize),
					                    	new Rgba(255, 255, 255, (byte)(alpha * 255.0f)));
					}
					else
					{

				
						// draw muzzle flash
						if (Math.Sin(elapsed * 877.0f) > 0.3)
						{			
							billboardBatch.Draw(TurretModels.muzzleFlash, GetPosition() + new Vector3(0, 3.3f, 0) + turretDirection.Normalize() * 1.2f, new Vector2(1, 1));
						}
										
						float splashSize = 0.2f * (elapsed % 2) + 0.4f;
						float alpha = 1 - (elapsed % 2);
					
						billboardBatch.Draw(TurretModels.bloodSplat, target.GetPosition() + new Vector3(0, 1.0f, 0), new Vector2(splashSize),
					                    	new Rgba(255, 255, 255, (byte)(alpha * 255.0f)));
						
					}
					
					billboardBatch.End();
				}
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

