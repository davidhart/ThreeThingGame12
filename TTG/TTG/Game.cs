using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;
using Sce.Pss.HighLevel.UI;

namespace TTG
{
	public enum GameState
	{
		Playing,
		Title,
		Help
	}
	
	public class Game
	{
		private GraphicsContext graphics;
		private BasicProgram program;
		private Stopwatch stopwatch;
		
		private int frameCount;
		private int prevTicks;
		
		public GameState gameState = GameState.Title;
		private TitleScreen titleScreen;
		public bool IsRunning = true;
		
		private Vector3 cameraOffset;
		
		private Level level;
		private Player player;
		
		private EnemyType basicEnemy;
		private Enemy testEnemy;
		
		private SpriteBatch spriteBatch;
		
		GameUI UI;
		
		public Game ()
		{
		}
		
		public void Initialise()
		{			
			// Set up the graphics system
			graphics = new GraphicsContext ();
			graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
			UISystem.Initialize(graphics);
			
			// Custom Program with color attribute (for demonstration)
			program = new BasicProgram("shaders/model.cgx", "shaders/model.cgx");
			program.SetUniformBinding(4, "Color");
			Vector4 color = new Vector4(1, 1, 1, 1);
			program.SetUniformValue(4, ref color);
			
			
			level = new Level(graphics, program);
			level.Load("testlevel.txt");
			
			basicEnemy = new EnemyType();
			basicEnemy.model = new Model("penguin.mdx", 0);
			basicEnemy.speed = 1.2f;
			
			testEnemy = new Enemy(graphics, basicEnemy, level, program);
			testEnemy.SetPosition((int)level.SpawnPos.X, (int)level.SpawnPos.Y, level.SpawnDir);
			
			cameraOffset = new Vector3(0, 13, 10);
			
			stopwatch = new Stopwatch();
			stopwatch.Start();
			titleScreen = new TitleScreen();
			titleScreen.Initialise(graphics, this);
			
			player = new Player(graphics, program);
			
			spriteBatch = new SpriteBatch(graphics);
			UI = new GameUI();
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
			case GameState.Title:
			{
				titleScreen.Update(touchData);
				break;
			}
			case GameState.Help:
			{
				break;
			}
			case GameState.Playing:
			{
				player.Update(gamePadData, dt);
				testEnemy.Update(dt);
				level.Update();
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
			case GameState.Title:
			{
				titleScreen.Draw();
				break;
			}
			case GameState.Help:
			{
				break;
			}
			case GameState.Playing:
			{
				RenderModel();
				graphics.Clear(ClearMask.Depth);
				UI.Draw(spriteBatch);
				break;
			}
			}
			
			
			
			
			// Present the screen
			graphics.SwapBuffers ();
		}
		
		public void Dispose()
		{
		}
		
		public void RenderModel()
		{
			graphics.Enable( EnableMode.Blend ) ;
			graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) ;
			//graphics.Enable( EnableMode.CullFace ) ;
			graphics.SetCullFace( CullFaceMode.Back, CullFaceDirection.Ccw ) ;
			graphics.Enable( EnableMode.DepthTest ) ;
			graphics.SetDepthFunc( DepthFuncMode.LEqual, true ) ;						
			
			Matrix4 projectionMatrix = Matrix4.Perspective(FMath.Radians(45.0f), graphics.Screen.AspectRatio, 1.0f, 1000000.0f);
			Matrix4 viewMatrix = Matrix4.Translation(new Vector3(0, 0, -10));
			viewMatrix = Matrix4.LookAt(cameraOffset + player.GetPosition(), player.GetPosition(), new Vector3(0, 0, -1));			
			
			Vector3 litDirection = new Vector3 (0.0f, -1.0f, -1.0f).Normalize ();
			Vector3 litColor = new Vector3 (1.0f, 1.0f, 1.0f);

			Vector3 litAmbient = new Vector3 (0.3f, 0.3f, 0.3f);
			Vector3 fogColor = new Vector3 (0.0f, 0.5f, 1.0f);

			BasicParameters parameters = program.Parameters;

			parameters.SetProjectionMatrix (ref projectionMatrix);
			parameters.SetViewMatrix (ref viewMatrix);

			level.Draw();
			testEnemy.Draw();
			player.Draw();
		}
	}
}

