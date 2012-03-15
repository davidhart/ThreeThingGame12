using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Imaging;

namespace TTG
{
	public class GameUI
	{
		Texture2D bearTex, fishTex, pointsTex;
		BitmapFont font;
		public GameUI ()
		{
			bearTex = new Texture2D("assets/testsprite.png", false);
			fishTex = new Texture2D("assets/fishIcon.png", false);
			pointsTex = new Texture2D("assets/CoinIcon.png", false);
			font = new BitmapFont("assets/fonts/font.png", 37, 4);
		}
		
		public void Draw(SpriteBatch spritebatch, Player player)
		{
			spritebatch.Begin();
			spritebatch.Draw(bearTex, new Vector2(0,0));
			spritebatch.Draw(pointsTex, new Vector2(0,32));
			spritebatch.Draw(fishTex, new Vector2(0, 64));
			
			font.DrawText(spritebatch, player.Health.ToString(), new Vector2(34, 8), 2.0f);
			font.DrawText(spritebatch, player.Points.ToString(), new Vector2(34, 40), 2.0f);
			font.DrawText(spritebatch, player.Fish.ToString(), new Vector2(34, 72), 2.0f);
			spritebatch.End();
		}
	}
}

