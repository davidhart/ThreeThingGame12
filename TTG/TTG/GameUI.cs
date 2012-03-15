using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Imaging;

namespace TTG
{
	public class GameUI
	{
		Texture2D bearTex, fishTex, pointsTex;
		public GameUI ()
		{
			bearTex = new Texture2D("assets/testsprite.png", false);
			fishTex = new Texture2D("assets/fishIcon.png", false);
			pointsTex = new Texture2D("assets/CoinIcon.png", false);
		}
		
		public void Draw(SpriteBatch spritebatch)
		{
			spritebatch.Begin();
			spritebatch.Draw(new Vector2(0,0), bearTex);
			spritebatch.Draw(new Vector2(0,32), pointsTex);
			spritebatch.Draw(new Vector2(0, 64), fishTex);
			spritebatch.End();
		}
	}
}

