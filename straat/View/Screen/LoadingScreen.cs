﻿using System;
using straat.View.Screen;
using Microsoft.Xna.Framework.Graphics;
using straat.Model;
using Microsoft.Xna.Framework;
using System.Threading;
using straat.View.Drawing;

namespace straat.View.Screen
{
	public class LoadingScreen : IScreen
	{
		ScreenManager screenManager;
		Viewport viewport;

		World world;

		#region content
		SpriteFont font;
		#endregion

		MapBuilder mapBuilder;
		Thread mapBuildingThread;
		bool done = false;

		string statusmsg = "";
		float status = 0.0f;

		MapDrawer mapDrawer = null;
		Camera cam;

		public LoadingScreen(ScreenManager screenManager, Rectangle bounds)
		{
			this.screenManager = screenManager;
			viewport = new Viewport( bounds );
			cam = new Camera( viewport.Bounds );
		}


		public void Initialize()
		{
			mapBuildingThread = new Thread( new ThreadStart(generateMap) );
			mapBuildingThread.IsBackground = true;
			mapBuildingThread.Start();
		}

		public void LoadContent()
		{
			font = screenManager.game.Content.Load<SpriteFont>("testfont");
		}

		public void UnloadContent()
		{
			
		}

		private void generateMap()
		{
			float statusstep = 1.0f/11.0f;

			int seed = 4231337;
			world = new World();
			world.seed = seed;
			statusmsg+="initialising mapbuilder...\n";

			float dimension = 2048.0f;
			cam.zoomFactor /= 2.0f;
			cam.position -= new Vector2( dimension / 2.0f );
			cam.position.X -= viewport.Width / 2.0f;

			mapBuilder = new MapBuilder( dimension, 8194, 256.0f, seed );
			status += statusstep;

			statusmsg+="generating Voronoi graph...\n";
			BenTools.Mathematics.VoronoiGraph voronoiGraph = mapBuilder.createVoronoiGraph();
			status += statusstep;
			
			statusmsg+="converting Voronoi graph...\n";
			world.map = mapBuilder.buildMapFromGraph( voronoiGraph );
			mapDrawer = new MapDrawer( screenManager.game, world.map );
			status += statusstep;
			statusmsg += "fixing Holes...\n";
			mapBuilder.fixHoles();
			status += statusstep;

			statusmsg+="applying Elevation..\n";
			mapBuilder.applyElevation();
			//world.mapBuilder.applyCone(1.0f);
			status += statusstep;

			statusmsg+="normalising Elevation...\n";
			mapBuilder.normaliseElevation();
			status += statusstep;

			statusmsg+="filling local minima...\n";
			mapBuilder.fillMinima();
			status += statusstep;

			Thread.Sleep(2000);
			statusmsg+="smoothening elevation...\n";
			mapBuilder.smoothen();
			mapBuilder.smoothen();
			status += statusstep;
			Thread.Sleep(2000);


			statusmsg+="filling local minima...\n";
			mapBuilder.fillMinima();
			status += statusstep;

			statusmsg += "raising sea level...\n";
			mapBuilder.raiseElevation( -0.3f );
			//mapBuilder.normaliseElevation();
			status+=statusstep;

			statusmsg+="normalising Elevation...\n";
			mapBuilder.normaliseElevation();
			status += statusstep;

			//mapBuilder.smoothenMinima( 0.5f, 1.0f );
			//mapBuilder.smoothenMinima( 0.4f, 0.6f );
			//mapBuilder.smoothenMinima( 0.1f, 0.3f );

			mapDrawer.drawRivers = false;
			statusmsg+="drawing rivers\n";
			mapBuilder.applyRivers();
			mapDrawer.drawRivers = true;

			status = 1.0f;
			Thread.Sleep(2000);
			done = true;
		}

		public void Update(double deltaT, straat.Control.Input input)
		{
			
			if(done)
			{
				// split screen vertically in two halves
				Rectangle a = viewport.Bounds;
				a.Width /= 3;
				a.Width *= 2;
				Rectangle b = a;
				b.X += a.Width;

				// goto main game setup
				GameScreen gs = new GameScreen( screenManager, a );
				gs.world = world;
				screenManager.activateScreen(gs );
				screenManager.activateScreen( new MainInterfaceScreen( screenManager, b ) );
				screenManager.deactivateScreen( this );
			}
		}

		public void Draw(double deltaT)
		{
			// save previous viewport
			Viewport original = screenManager.game.graphics.GraphicsDevice.Viewport;
			screenManager.game.graphics.GraphicsDevice.Viewport = viewport;
			screenManager.game.spriteBatch.Begin();


			screenManager.game.GraphicsDevice.Clear(Color.DarkBlue);
			if( mapDrawer != null )
			{
				mapDrawer.Draw( cam );
			}

			Vector2 drawPos = new Vector2( 20, 20 );
			screenManager.game.spriteBatch.DrawString( font, "Behold the fancy loading screen!", drawPos, Color.Red );
			drawPos.Y += 50;
			screenManager.game.spriteBatch.DrawString( font, statusmsg, drawPos, Color.White );

			int maxWidth = viewport.Width - 50;
			GeometryDrawer.fillRect(25,viewport.Height-100,(int)(maxWidth*status),50,Color.Red);

			drawPos = new Vector2(50,viewport.Height-80);
			screenManager.game.spriteBatch.DrawString( font, "LOADING...", drawPos, Color.White );


			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
		}

		public void changeViewport(float widthScale, float heightScale)
		{
			viewport.X = (int)( viewport.X * widthScale );
			viewport.Y = (int)( viewport.Y * heightScale );
			viewport.Width = (int)( viewport.Width * widthScale );
			viewport.Height = (int)( viewport.Height * heightScale );
		}
	}
}

