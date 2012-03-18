using System;
using System.Collections.Generic;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

using Sce.Pss.Core.Audio;

namespace TTG
{
	public class TitleScreen
	{		
		private enum Selected
		{
			Start,
			Help,
		};
		
		SoundSystem sound;
		Texture2D title;
		Texture2D frost;
		Selected selectedItem;
		
		public TitleScreen ()
		{
			title = new Texture2D("assets/title.png", false);
			frost = new Texture2D("assets/selection.png", false);
			selectedItem = Selected.Start;
		}
		
		public void Initialise()
		{
			sound = new SoundSystem();
			sound.Play(0, 100, true);
		}
		
		public void Update(GamePadData data, Game game)
		{
			if (data.ButtonsDown.HasFlag (GamePadButtons.Down))
			{
				selectedItem = Selected.Help;
			}
			else if(data.ButtonsDown.HasFlag(GamePadButtons.Up))
			{
				selectedItem = Selected.Start;
			}
			else if(data.ButtonsDown.HasFlag(GamePadButtons.Cross) &&
			        selectedItem == Selected.Start)
			{
				game.gameState = GameState.Playing;
				
				sound.Stop(0);
			}
			else if(data.ButtonsDown.HasFlag(GamePadButtons.Cross) && 
			        selectedItem == Selected.Help)
			{
				game.gameState = GameState.Help;
				
			}
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Begin();
			
			sb.Draw (title, new Vector2(0, 0));
			
			if(selectedItem == Selected.Start)
			{
				sb.Draw (frost, new Vector2(321,220));
			}
			else if(selectedItem == Selected.Help)
			{
				sb.Draw (frost, new Vector2(321,261));
			}
			
			sb.End ();
		}
		
	}
}

