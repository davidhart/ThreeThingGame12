using System;
using System.Collections.Generic;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;
using Sce.Pss.HighLevel.UI;

namespace TTG
{
	public class TitleScreen
	{
		Button startButton, helpButton, titleButton;
		Game theGame;
		
		public TitleScreen ()
		{
		}
		public void Initialise(GraphicsContext graphics, Game game)
		{
			UISystem.Initialize(graphics);
			Scene scene = new Scene();
			
			//Rendered title image as a button
			titleButton = new Button();
			titleButton.IconImage = new ImageAsset("assets/team.jpg");
			titleButton.X = 125.0f;
			titleButton.Y = 125.0f;
			scene.RootWidget.AddChildLast(titleButton);
			
			//Buttons
			startButton = new Button();
			startButton.Text = "Start";
			startButton.X = 960.0f-400.0f;
			startButton.Y = 100.0f;
			scene.RootWidget.AddChildLast(startButton);
			helpButton =  new Button();
			helpButton.Text = "Help";
			helpButton.X = 960.0f-400.0f;
			helpButton.Y = 200.0f;
			scene.RootWidget.AddChildLast(helpButton);
			
			UISystem.SetScene(scene, null);
			theGame = game;
		}
		
		public void Update(List<TouchData> dataList)
		{
			UISystem.Update(dataList);
			startButton.ButtonAction += HandleStartButtonButtonAction;
		}

		void HandleStartButtonButtonAction (object sender, TouchEventArgs e)
		{
			theGame.gameState = GameState.Playing;
		}
		public void Draw()
		{
			UISystem.Render();
		}
		
	}
}
