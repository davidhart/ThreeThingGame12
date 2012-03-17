using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;


using System.Diagnostics;

namespace TTG
{
	public class Player
	{
		/*
		float speed = 0, direction = 0;
		const float acceleration = 5, turn = 3, deceleration = 3;*/
		
		Vector3 position;
		float angle;
		
		const float forwardSpeed = 7.5f;
		const float turnSpeed = 1.5f;
		Model lowerModel;
		Model upperModel;
		
		Enemy target = null;
		
		float health = 100, points = 0, fish = 1000;
		
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
		
		public float Points
		{
			get
			{
				return points;
			}
			set
			{
				points = value;
			}
		}
		
		public float Fish
		{
			get
			{
				return fish;
			}
			set
			{
				fish = value;
			}
		}
		
		float atkRange = 5;
		float atkDmg = 0.5f;
		
		GraphicsContext graphics;
		BasicProgram program;
		
		static Matrix4 tankScale = Matrix4.Scale(new Vector3(0.5f));
		
		public Player (GraphicsContext graphics, BasicProgram program) 
		{
			this.graphics = graphics;
			this.program = program;
			
			lowerModel = new Model("assets/tank_lower.mdx", 0);
			upperModel = new Model("assets/tank_upper.mdx", 0);
			
			position = new Vector3(0, 1.65f, 0);
		}
		
		public void Draw ()
		{
			//rotate the model by direction variable
			//translate the model by acceleration depending on direction
			//Matrix4 world = Matrix4.Identity;
			Matrix4 world = Matrix4.Translation(position) * tankScale;
			
			Matrix4 tank = world * Matrix4.RotationY(angle);
			
			Vector3 tUp = new Vector3(0,1,0);
			
			lowerModel.SetWorldMatrix( ref tank );
			lowerModel.Update();
			lowerModel.Draw(graphics, program);
			
			Vector3 turretTarget;
			
			if(target == null)
			{
				turretTarget = new Vector3(0, position.Y, 0);
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
				turret = world;	
			}
			
			upperModel.SetWorldMatrix( ref turret );
			upperModel.Update();
			upperModel.Draw(graphics, program);
			
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
			
			position += new Vector3(forward.X, 0, forward.Y) * dt * forwardSpeed;
			
		}
		
		public Vector3 GetPosition()
		{
			return position;
		}
		
		
	}
}

