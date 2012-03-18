using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	public class SplashScreen
	{
		Texture2D background;
		Texture2D TTG;
		
		float timeOnScreen = 9;
		public SplashScreen ()
		{
			background = new Texture2D("assets/Splash.png", false);
			TTG = new Texture2D("assets/Splash2.png", false);
		}
		
		public void Draw(SpriteBatch sb)
		{
			sb.Begin();
			if(timeOnScreen >= 4.5f)
			{
				sb.Draw (background, new Vector2(0,0));
			}
			else
			{
				sb.Draw (TTG, new Vector2(0,0));
			}
			sb.End ();
		}
		
		public void Update(float dt, Game game)
		{
			timeOnScreen -= dt;
			if(timeOnScreen <= 0.0f)
			{
				game.gameState = GameState.Title;
				background.Dispose();
				TTG.Dispose();
			}
		}
	}
}

