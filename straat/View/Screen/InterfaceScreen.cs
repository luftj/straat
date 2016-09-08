using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using straat.Control;
using Microsoft.Xna.Framework.Graphics;

namespace straat.View.Screen
{
	public class InterfaceScreen : IScreen
	{
		protected ScreenManager screenManager;
		protected Rectangle bounds;
		protected Dictionary<InputCommand,InterfaceScreen> links;

		#region content
		protected SpriteFont font;
		#endregion

		public InterfaceScreen(ScreenManager screenManager, Rectangle bounds)
		{
			this.screenManager = screenManager;
			this.bounds = bounds;
			links = new Dictionary<InputCommand, InterfaceScreen>();
		}

		public void Initialize()
		{
			
		}

		public void LoadContent()
		{
			// load content required for all interface screens
			font = screenManager.game.Content.Load<SpriteFont>("testfont");
		}

		public void UnloadContent()
		{
			//
		}

		public void Update(double deltaT, Input input)
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

		public void Draw(double deltaT)
		{
			// todo: draw background
			// todo: draw border
		}
	}
}

