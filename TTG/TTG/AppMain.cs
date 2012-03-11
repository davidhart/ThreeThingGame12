using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

namespace TTG
{
	public class AppMain
	{

		//GAME MAIN LOOP
		public static void Main (string[] args)
		{
			Game game = new Game();
			game.Initialise ();
			game.Load ();
			while(game.IsRunning) 
			{
				SystemEvents.CheckEvents ();
				game.Update ();
				game.Draw ();
			}
			game.Dispose();
		}
	}
}
