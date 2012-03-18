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
			Exit,
		};
		
		SoundSystem sound;
		Texture2D title;
		Selected selectedItem;
		
		public TitleScreen ()
		{
			title = new Texture2D("assets/title.png", false);
			selectedItem = Selected.Start;
		}
		
		public void Initialise()
		{
			sound = new SoundSystem();
			sound.Play(0, 100, true);
		}
		
		public void Update()
		{
			
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Begin();
			
			sb.Draw (title, new Vector2(0, 0));
			
			
			sb.End ();
		}
		
	}
}

