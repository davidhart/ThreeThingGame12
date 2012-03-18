using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

namespace TTG
{
	public class HelpScreen
	{
		Texture2D help1;
		Texture2D help2;
		
		enum HelpScreens
		{
			help1,
			help2,
		};
		
		HelpScreens state;
		
		public HelpScreen ()
		{
			state = HelpScreens.help1;
			help1 = new Texture2D("assets/help1.png", false);
			help2 = new Texture2D("assets/help2.png", false);
		}
		
		public void Update(GamePadData data)
		{
			if (data.ButtonsDown.HasFlag (GamePadButtons.Cross) &&
			    state == HelpScreens.help1)
			{
				state = HelpScreens.help2;
			}
			else if(data.ButtonsDown.HasFlag (GamePadButtons.Cross) &&
			    state == HelpScreens.help2)
			{
				state = HelpScreens.help1;
				Game.gameState = GameState.Title;
			}
		}
		
		public void Draw(SpriteBatch sb)
		{
			sb.Begin();
			
			if(state == HelpScreens.help1)
			{
				sb.Draw (help1, new Vector2(0,0));
			}
			else if(state == HelpScreens.help2)
			{
				sb.Draw (help2, new Vector2(0,0));
			}
			
			sb.End ();
		}
	}
}

