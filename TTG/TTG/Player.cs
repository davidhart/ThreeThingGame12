using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

namespace TTG
{
	public class Player : GameObject3D
	{
		float speed = 0, direction = 0;
		const float acceleration = 5, turn = 3, deceleration = 3;
		
		
		public Player (GraphicsContext graphics, Model model) 
			: base(graphics, model)
		{
		}
		
		public override void Draw ()
		{
			//rotate the model by direction variable
			//translate the model by acceleration depending on direction
			base.Draw ();
		}
		
		public override void Update (GamePadData padData)
		{
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
				speed += accleration;
			}
			else
			{
				if(speed > 0)
				{
					speed -= deceleration;
				}
			}
			
			base.Update (padData);
		}
		

	}
}

