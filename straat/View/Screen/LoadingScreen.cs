using System;
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

		Thread mapBuildingThread;
		bool done = false;

		string statusmsg = "";
		float status = 0.0f;

		public LoadingScreen(ScreenManager screenManager, Rectangle bounds)
		{
			this.screenManager = screenManager;
			viewport = new Viewport( bounds );
		}


		public void Initialize()
		{
			mapBuildingThread = new Thread( new ThreadStart(doThings) );
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

		private void doThings()
		{
			world = new World();
			statusmsg+="initialising mapbuilder...\n";
			world.mapBuilder = new MapBuilder( 2048.0f, 4096, 256.0f  , 4231337 );
			status = 0.125f;

			statusmsg+="generating Voronoi graph...\n";
			BenTools.Mathematics.VoronoiGraph voronoiGraph = world.mapBuilder.createVoronoiGraph();
			status = 0.25f;
			
			statusmsg+="converting Voronoi graph...\n";
			world.map = world.mapBuilder.buildMapFromGraph( voronoiGraph );
			status = 0.375f;
			statusmsg += "fixing Holes...\n";
			world.mapBuilder.fixHoles();
			status = 0.5f;

			statusmsg+="applying Elevation..\n";
			world.mapBuilder.applyElevation();
			status = 0.625f;
			statusmsg+="normalising Elevation...\n";
			world.mapBuilder.normaliseElevation();
			status = 0.75f;

			//mapBuilder.raiseElevation( -0.2f );
			//mapBuilder.normaliseElevation();

			statusmsg+="filling local minima...\n";
			world.mapBuilder.fillMinima();
			status += 0.125f;

			//mapBuilder.smoothenMinima( 0.5f, 1.0f );
			//mapBuilder.smoothenMinima( 0.4f, 0.6f );
			//mapBuilder.smoothenMinima( 0.1f, 0.3f );


			statusmsg+="drawing rivers\n";
			world.mapBuilder.applyRivers();

			status = 1.0f;
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


			Vector2 drawPos = new Vector2( 20, 20 );
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

