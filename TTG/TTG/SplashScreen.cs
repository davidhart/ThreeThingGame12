using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	public class SplashScreen
	{
		Texture2D background;
		float timeOnScreen = 3;
		public SplashScreen ()
		{
			background = new Texture2D("assets/Splash.png", false);
		}
		
		public void Draw(SpriteBatch sb)
		{
			sb.Begin();
			sb.Draw (background, new Vector2(0,0));
			sb.End ();
		}
		
		public void Update(float dt, Game game)
		{
			timeOnScreen -= dt;
			if(timeOnScreen <= 0.0f)
			{
				game.gameState = GameState.Title;
			}
		}
	}
}

