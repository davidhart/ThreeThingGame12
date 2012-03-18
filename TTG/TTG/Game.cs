using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Imaging;
using Sce.Pss.Core.Input;
using Sce.Pss.HighLevel.UI;

namespace TTG
{
	public enum GameState
	{
		Playing,
		Title,
		Help,
		SplashScreen,
		GameOver
	}
	
	public class Game
	{
		private GraphicsContext graphics;
		private BasicProgram program;
		private Stopwatch stopwatch;
		
		Texture2D gameOvertex;
		
		private int frameCount;
		private int prevTicks;
		
		public GameState gameState = GameState.SplashScreen;
		private TitleScreen titleScreen;
		private SplashScreen splashScreen;
		public bool IsRunning = true;
		
		private Vector3 cameraOffset;
		
		private Level level;
		private Player player;
		
		private SpriteBatch spriteBatch;
		
		private Camera camera;
		
		GameUI UI;
		UpgradeUI upgrade;
		BillboardBatch billboardBatch;
		Matrix4 projectionMatrix;
		
		public Game ()
		{
		}
		
		public void Initialise()
		{			
			// Set up the graphics system
			graphics = new GraphicsContext ();
			graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
			Debug.WriteLine(graphics.Screen.Width.ToString() + " " + graphics.Screen.Height.ToString());
			
			spriteBatch = new SpriteBatch(graphics);
			billboardBatch = new BillboardBatch(graphics);
			
			UISystem.Initialize(graphics);
			
			program = new BasicProgram("shaders/model.cgx", "shaders/model.cgx");
			
			cameraOffset = new Vector3(0, 13, 10);
			
			stopwatch = new Stopwatch();
			stopwatch.Start();
			
			titleScreen = new TitleScreen();
			titleScreen.Initialise();
			
			splashScreen = new SplashScreen();
			
			UI = new GameUI();
			upgrade = new UpgradeUI();

			camera = new Camera(Vector3.Zero, Vector3.Zero, new Vector3(0, 0, -1));
					
			projectionMatrix = Matrix4.Perspective(FMath.Radians(45.0f), graphics.Screen.AspectRatio, 1.0f, 1000000.0f);
		
			level = new Level(graphics, program, upgrade, spriteBatch, billboardBatch, 15);
			level.Load("testlevel.txt");
			
			player = new Player(graphics, program, billboardBatch, level);
			
			gameOvertex = new Texture2D("assets/gameOver.png", false);
		}
		
		public void Load()
		{
		}
		
		public void Update()
		{
			int currTicks = (int)stopwatch.ElapsedTicks;
			if (frameCount ++ == 0)
				prevTicks = currTicks;
			float dt = (currTicks - prevTicks) / (float)Stopwatch.Frequency;
			prevTicks = currTicks;
			
			float elapsed = stopwatch.ElapsedMilliseconds / 1000.0f;			
			
			var gamePadData = GamePad.GetData (0);
			
			List<TouchData> touchData = Touch.GetData(0);
			switch (gameState)
			{
			case GameState.SplashScreen:
			{
				splashScreen.Update(dt, this);
				break;
			}
			case GameState.Title:
			{
				titleScreen.Update();
				break;
			}
			case GameState.Help:
			{
				break;
			}
			case GameState.Playing:
			{
				player.Update(gamePadData, dt, level.waves[level.WaveNumber]);
				level.Update(dt, upgrade, gamePadData, player);
				if(level.lives <=0)
				{
					gameState = GameState.GameOver;
				}
				break;
			}
			case GameState.GameOver:
			{
				UpdateGameOver(gamePadData);
				break;
			}
			}
		}
		
		public void Draw()
		{
			// Clear the screen
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear ();
			
			switch (gameState)
			{
			case GameState.SplashScreen:
			{
				splashScreen.Draw (spriteBatch);
				break;
			}
				case GameState.Title:
				{
					titleScreen.Draw(spriteBatch);
					break;
				}
				case GameState.Help:
				{
					break;
				}
				case GameState.Playing:
				{
					DrawGame();
					break;
				}
			case GameState.GameOver:
			{
				DrawGameOver();
				break;
			}
			}
			
			// Present the screen
			graphics.SwapBuffers ();
		}
		
		public void Dispose()
		{
		}
		
		public void DrawGame()
		{
			graphics.Enable( EnableMode.Blend ) ;
			graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) ;
			graphics.Enable( EnableMode.CullFace ) ;
			graphics.SetCullFace( CullFaceMode.Back, CullFaceDirection.Ccw ) ;
			graphics.Enable( EnableMode.DepthTest ) ;
			graphics.SetDepthFunc( DepthFuncMode.LEqual, true ) ;						
			
			camera.SetPosition(cameraOffset + player.GetPosition());
			camera.SetLookAt(player.GetPosition());	
			
			billboardBatch.SetCamera(camera, projectionMatrix);

			BasicParameters parameters = program.Parameters;

			parameters.SetProjectionMatrix (ref projectionMatrix);
			
			Matrix4 viewMatrix = camera.GetMatrix();
			parameters.SetViewMatrix (ref viewMatrix);

			level.Draw();
			
			player.Draw(camera);
			
			graphics.Disable( EnableMode.CullFace );
			graphics.Clear(ClearMask.Depth);
			UI.Draw(spriteBatch, level.Points, level.lives, ref level.WaveNumber);					
			upgrade.Draw (spriteBatch);
		}
		
		public void DrawGameOver()
		{
			spriteBatch.Begin();
			spriteBatch.Draw(gameOvertex, new Vector2(0,0));
			spriteBatch.End();
		}
		
		public void UpdateGameOver(GamePadData data)
		{
			if (data.ButtonsDown.HasFlag (GamePadButtons.Cross)) 
			{
				level.Load("testlevel.txt");
				gameState = GameState.Title;
			}
		}
		
	}
}

