using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;


using System.Diagnostics;

namespace TTG
{
	public class Player
	{		
		Vector3 position;
		float angle;
		
		const float forwardSpeed = 7.5f;
		const float turnSpeed = 1.5f;
		Model lowerModel;
		Model upperModel;
		
		Level level;
		
		Enemy target = null;
		
		float health = 100, points = 0, fish = 1000;
		
		float atkRange = 5;
		float atkDmg = 20.0f;
		
		GraphicsContext graphics;
		BasicProgram program;
		
		static Matrix4 tankScale = Matrix4.Scale(new Vector3(0.4f));
		
		bool drawBuyMenuIcon;
		Texture2D muzzleFlash;
		Texture2D buyMenuIcon;
		BillboardBatch billboardBatch;
		
		Vector3 buyMenuIconOffset;
		Vector2 buyMenuIconSize;
		float elapsed;
		
		public Player (GraphicsContext graphics, BasicProgram program, BillboardBatch billboardBatch, Level level) 
		{
			this.graphics = graphics;
			this.program = program;
			this.billboardBatch = billboardBatch;
			
			lowerModel = new Model("assets/tank_lower.mdx", 0);
			upperModel = new Model("assets/tank_upper.mdx", 0);

			// reference to level, to access tile data (used for collisions)
			this.level = level;
			
			buyMenuIcon = new Texture2D("assets/crossButton.png", false);
			muzzleFlash = new Texture2D("assets/muzzleFlash.png", false);
			
			drawBuyMenuIcon = false;
			buyMenuIconOffset = new Vector3(0, 4, 0);
			buyMenuIconSize = new Vector2(0.75f, 0.75f);
			
			position = new Vector3(0, 1.5f, 0);
		}
		
		public void DrawBuyMenuIcon(bool enable)
		{
			drawBuyMenuIcon = enable;	
		}
		
		public void Draw (Camera camera)
		{
			Matrix4 world = Matrix4.Translation(position) * tankScale;
			
			Matrix4 tank = world * Matrix4.RotationY(angle);
			
			Vector3 tUp = new Vector3(0,1,0);
			
			lowerModel.SetWorldMatrix( ref tank );
			lowerModel.Update();
			lowerModel.Draw(graphics, program);
			
			Vector3 turretTarget;
			
			if(target == null)
			{
				turretTarget = position;
			}
			else
			{
				turretTarget = new Vector3(target.GetPosition().X, position.Y, target.GetPosition().Z);
			}
			Vector3 turretDirection = (turretTarget - position).Normalize();
			
			Matrix4 turret;
			
			if (turretDirection.Length() > 0.2f)
			{
				Vector3 up = new Vector3(0, 1, 0);
			
				turret = world * new Matrix4(turretDirection, up, turretDirection.Cross(up), Vector3.Zero);
			}
			else
			{
				turret = tank;	
			}
			
			upperModel.SetWorldMatrix( ref turret );
			upperModel.Update();
			upperModel.Draw(graphics, program);
			
			if (target != null)
			{
				// draw muzzle flash
				if (Math.Sin(elapsed * 8.0f) > 0.3)
				{
					SoundSystem.PlaySound("turret", 0.5f, false);
					billboardBatch.Begin();
					billboardBatch.Draw(TurretModels.muzzleFlash, GetPosition() + new Vector3(0, 0.9f, 0) + turretDirection.Normalize() * 1.1f, new Vector2(0.6f, 0.6f));
					billboardBatch.End();
				}
			}
			
			
			if (drawBuyMenuIcon)
			{
				billboardBatch.Begin();
				billboardBatch.Draw(buyMenuIcon, GetPosition() + buyMenuIconOffset, buyMenuIconSize);
				billboardBatch.End();
			}
		}
		
		public void Update (GamePadData padData, float dt, Enemy [] enemies)
		{
			//Turret targeting
			if(target == null)
			{
				for(int i = 0; i < enemies.Length; ++i)
				{
					float distance = Vector2.Distance(enemies[i].GetPosition().Xz, position.Xz);
					if(distance <= atkRange && enemies[i].Health > 0)
					{
						target = enemies[i];
						break;

					}
				}
			}
			else
			{
				if(target.Health <= 0)
				{
					target =  null;
				}
				else
				{
					target.Health -= atkDmg * dt;
					float distance = Vector2.Distance(target.GetPosition().Xz, position.Xz);
					if(distance > atkRange)
					{
						target = null;
					}
				}
			}
			
			if (padData.Buttons.HasFlag(GamePadButtons.Right))
			{
				angle -= turnSpeed * dt;
			}
			
			if (padData.Buttons.HasFlag(GamePadButtons.Left))
			{
				angle += turnSpeed * dt;
			}
			
			Vector2 forward = new Vector2(1,0);
			forward = forward.Rotate(-angle);
					
			if (padData.Buttons.HasFlag(GamePadButtons.L) 
			    && !padData.Buttons.HasFlag(GamePadButtons.Left) 
			    && !padData.Buttons.HasFlag(GamePadButtons.Right))
			{
				forward *= -1;
			}
			else if (padData.Buttons.HasFlag(GamePadButtons.R)
			         && !padData.Buttons.HasFlag(GamePadButtons.Left) 
			         && !padData.Buttons.HasFlag(GamePadButtons.Right))
			{
				forward *= 1;	
			}
			else
			{
				forward *= 0;
			}
			
			// collision detection
			// bounding sphere
			float radius = 1.0f;
			Vector3 centre = new Vector3(0.4f, 0, 0.1f);
			
			
			
			Vector3 testPosition = position + new Vector3(forward.X, 0, forward.Y) * dt * forwardSpeed;
			bool test = level.CollisionDetection(testPosition);
			//Debug.WriteLine(test);
			if(test == false) position = testPosition;
			//position += new Vector3(forward.X, 0, forward.Y) * dt * forwardSpeed;
			//Debug.WriteLine(position.ToString());
			
			elapsed += dt;
			if (elapsed > 100)
				elapsed -= 100;
		}
		
		public Vector3 GetPosition()
		{
			return position;
		}
	}
}

