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
		Vector2 forward = new Vector2(1,0);
		float angle;
		const float turnSpeed = 0.5f;
		Model lowerModel;
		Model upperModel;
		
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
			
			Matrix4 tank;
			
			Vector3 tUp = new Vector3(0,1,0);
			
			lowerModel.SetWorldMatrix( ref world );
			lowerModel.Update();
			lowerModel.Draw(graphics, program);
			
			Vector3 turretTarget = new Vector3(0, position.Y, 0);
			
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
		
		public void Update (GamePadData padData, float dt)
		{
			/*temporary*/
			const float CameraPanSpeed = 7.5f;
			
			Vector2 cameraDir = Vector2.Zero;
			
			if (padData.Buttons.HasFlag(GamePadButtons.Right))
			{
				cameraDir.X += 1;
				angle += turnSpeed;
				forward = forward.Rotate(angle);
				Debug.WriteLine("X:"+ forward.X + "Y:" + forward.Y);
			}
			
			if (padData.Buttons.HasFlag(GamePadButtons.Left))
			{
				cameraDir.X -= 1;
				angle -= turnSpeed;
				forward = forward.Rotate(angle);
				Debug.WriteLine("X:"+ forward.X + "Y:" + forward.Y);
			}
			
			if (padData.Buttons.HasFlag(GamePadButtons.Up))
			{
				//position += new Vector3(direction.X, 0, direction.Y) * dt * CameraPanSpeed;
				cameraDir.Y -= 1;
				position += new Vector3(forward.X, 0, forward.Y) * dt * CameraPanSpeed;
			}
			
			if (padData.Buttons.HasFlag(GamePadButtons.Down))
			{
				cameraDir.Y += 1;
			}
			
			
			//position += new Vector3(cameraDir.X, 0, cameraDir.Y) * dt * CameraPanSpeed;
			
			
			/*
			if(padData.AnalogLeftX > 0.25f || padData.Buttons.HasFlag(GamePadButtons.Right))
			{
				direction += turn;
			}
			if(padData.AnalogLeftX < -0.25f || padData.Buttons.HasFlag(GamePadButtons.Left))
			{
				direction += turn;
			}
			
			if(padData.Buttons.HasFlag(GamePadButtons.Cross) || padData.Buttons.HasFlag(GamePadButtons.R))
			{
				speed += acceleration;
			}
			else
			{
				if(speed > 0)
				{
					speed -= deceleration;
				}
			}
			*/
			
			
		}
		
		public Vector3 GetPosition()
		{
			return position;
		}
		
		
	}
}

