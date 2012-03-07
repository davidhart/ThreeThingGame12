using System;
using System.Collections.Generic;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

namespace TTG
{
	public class AppMain
	{
		private static GraphicsContext graphics;
		
		//GAME MAIN LOOP
		public static void Main (string[] args)
		{
			Initialize ();
			Load ();
			while (true) 
			{
				SystemEvents.CheckEvents ();
				Update ();
				Draw ();
			}
		}

		public static void Initialize ()
		{
			// Set up the graphics system
			graphics = new GraphicsContext ();
			
		}
		
		//LOAD
		public static void Load ()
		{
			//Load any content here
		}
		
		//UPDATE
		public static void Update ()
		{
			// Query gamepad for current state
			var gamePadData = GamePad.GetData (0);
			
		}
		
		//DRAW
		public static void Draw ()
		{
			// Clear the screen
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear ();

			// Present the screen
			graphics.SwapBuffers ();
		}
	}
}
