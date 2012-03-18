using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

using System.Diagnostics;

namespace TTG
{
	public class SplashScreen
	{
		Texture2D background;
		Texture2D TTG;
		
		Rgba colour1;
		Rgba colour2;
		float alpha1;
		float alpha2;
		
		float timeOnScreen = 9;
		public SplashScreen ()
		{
			alpha1 = 0;
			alpha2 = 0;
			colour1 = new Rgba(255, 255, 255, (int)alpha1);
			colour2 = new Rgba(255, 255, 255, (int)alpha2);
			
			background = new Texture2D("assets/Splash.png", false);
			TTG = new Texture2D("assets/Splash2.png", false);
		}
		
		public void Draw(SpriteBatch sb)
		{
			sb.Begin();
			if(timeOnScreen >= 4.5f)
			{
				sb.Draw (background, new Vector3(0,0, 0), colour1);
			}
			else
			{
				sb.Draw (TTG, new Vector3(0,0,0), colour2);
			}
			sb.End ();
		}
		
		public void Update(float dt, Game game)
		{
			if(timeOnScreen > 8.0f)
			{
				alpha1 += (dt * (255.0f / 1.0f));
				if(alpha1 > 255)
				{
					colour1 = new Rgba(255, 255, 255, 255);
				}
				else
				{
					colour1 = new Rgba(255, 255, 255, (int)alpha1);
				}
			}
			
			if(timeOnScreen < 6.0f)
			{
				alpha1 -= (dt * (255.0f / 1.0f));
				if(alpha1 < 0)
				{
					colour1 = new Rgba(255, 255, 255, 0);
				}
				else
				{
					colour1 = new Rgba(255, 255, 255, (int)alpha1);
				}
			}
			
			if(timeOnScreen < 5.0f && timeOnScreen > 4.0f)
			{
				alpha2 += (dt * (255.0f / 1.0f));
				if(alpha2 > 255)
				{
					colour2 = new Rgba(255, 255, 255, 255);
				}
				else
				{
					colour2 = new Rgba(255, 255, 255, (int)alpha2);
				}
			}
			
			if(timeOnScreen < 1.5f)
			{
				alpha2 -= (dt * (255.0f / 1.0f));
				if(alpha2 < 0)
				{
					colour2 = new Rgba(255, 255, 255, 0);
				}
				else
				{
					colour2 = new Rgba(255, 255, 255, (int)alpha2);
				}
			}
			
			timeOnScreen -= dt;
			if(timeOnScreen <= 0.0f)
			{
				Game.gameState = GameState.Title;
				background.Dispose();
				TTG.Dispose();
			}
		}
	}
}

