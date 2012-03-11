using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;


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
		private static GraphicsContext graphics;
		private static Model model;
		private static BasicProgram program;
		private static Stopwatch stopwatch;
		public GameState gameState = GameState.Title;
		private static int frameCount;
		private static int prevTicks;
		TitleScreen titleScreen;
		public bool IsRunning = true;
		
		public Game ()
		{
		}
		
		public void Initialise()
		{
			// Set up the graphics system
			graphics = new GraphicsContext ();
			
			model = new Model("box.mdx");
			
			// Custom Program with color attribute
			program = new BasicProgram("shaders/model.cgx", "shaders/model.cgx");
			program.SetUniformBinding(4, "Color");
			Vector4 color = new Vector4(1, 1, 1, 1);
			program.SetUniformValue(4, ref color);
			
			// Default unlit program
			//program = new BasicProgram();
			
			stopwatch = new Stopwatch();
			stopwatch.Start();
			titleScreen = new TitleScreen();
			titleScreen.Initialise(graphics, this);
		}
		
		public void Load()
		{
		}
		
		public void Update()
		{
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
			int currTicks = (int)stopwatch.ElapsedTicks;
			if (frameCount ++ == 0)
				prevTicks = currTicks;
			float stepTime = (currTicks - prevTicks) / (float)Stopwatch.Frequency;
			prevTicks = currTicks;
			
			
			Matrix4 projectionMatrix = Matrix4.Perspective(FMath.Radians(45.0f), graphics.Screen.AspectRatio, 1.0f, 1000000.0f);
			Matrix4 viewMatrix = Matrix4.Translation(new Vector3(0, 0, -25));
			
			
			Vector3 litDirection = new Vector3 (0.0f, -1.0f, -1.0f).Normalize ();
			Vector3 litColor = new Vector3 (1.0f, 1.0f, 1.0f);

			Vector3 litAmbient = new Vector3 (0.3f, 0.3f, 0.3f);
			Vector3 fogColor = new Vector3 (0.0f, 0.5f, 1.0f);

			BasicParameters parameters = program.Parameters;

			parameters.SetProjectionMatrix (ref projectionMatrix);
			parameters.SetViewMatrix (ref viewMatrix);
			
			Matrix4 world = Matrix4.RotationY( FMath.Radians( 20.0f * stopwatch.ElapsedMilliseconds / 1000.0f ) ) ;
			model.SetWorldMatrix( ref world ) ;
			
			graphics.Enable( EnableMode.Blend ) ;
			graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) ;
			graphics.Enable( EnableMode.CullFace ) ;
			graphics.SetCullFace( CullFaceMode.Back, CullFaceDirection.Ccw ) ;
			graphics.Enable( EnableMode.DepthTest ) ;
			graphics.SetDepthFunc( DepthFuncMode.LEqual, true ) ;
			
			model.Animate(stepTime);
			model.Update();
			model.Draw(graphics, program);
		}
	}
}

