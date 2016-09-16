using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using straat.Control;
using Microsoft.Xna.Framework.Graphics;
using straat.View.Drawing;

namespace straat.View.Screen
{
	public class InterfaceScreen : IScreen
	{
		protected ScreenManager screenManager;
		protected Viewport viewport;
		protected Dictionary<InputCommand,InterfaceScreen> links;


		#region content
		protected SpriteFont font;
		#endregion

		public InterfaceScreen(ScreenManager screenManager, Rectangle bounds)
		{
			this.screenManager = screenManager;
			viewport = new Viewport( bounds );

			links = new Dictionary<InputCommand, InterfaceScreen>();

		}

		public virtual void Initialize()
		{
			// use this to initialise stuff
		}

		public virtual void LoadContent()
		{
			// load content required for all interface screens
			font = screenManager.game.Content.Load<SpriteFont>("testfont");
		}

		public virtual void UnloadContent()
		{
			//
		}

		public virtual void Update(double deltaT, Input input)
		{
			// handle hotkeys for navigatiom
			bool screenChange = false;
			foreach(KeyValuePair<InputCommand,InterfaceScreen> pair in links)
			{
				if( input.pop( pair.Key ) )
				{
					// start new screen in place of this one
					screenManager.activateScreen( pair.Value );
					screenChange = true;
				}
			}
			// if a new screen is opened, close this one
			if(screenChange)
			{
				screenManager.deactivateScreen(this);
			}
		}

		public virtual void Draw(double deltaT)
		{
			// todo: draw background
			// todo: draw border
			GeometryDrawer.fillRect(new Rectangle(Point.Zero,viewport.Bounds.Size),Color.Black);

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

