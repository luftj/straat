using straat.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace straat.View.Screen
{
    class MainMenuScreen : IScreen
    {
        ScreenManager screenManager;
		Viewport viewport;

        List<string> entries;
        int selectedEntry = 0;

		#region content
		SpriteFont font;
		#endregion

		public MainMenuScreen(ScreenManager screenManager, Rectangle bounds)
        {
			this.screenManager = screenManager;
			viewport = new Viewport( bounds );

			entries = new List<string>();
        }

		public void Initialize()
		{
			// initialise anything non-content
			entries.Add("Start Game");
			entries.Add("Load Game");
			entries.Add("Multiplayer");
			entries.Add("Settings");
			entries.Add("Exit");

		}

		public void LoadContent()
		{
			font = screenManager.game.Content.Load<SpriteFont>("testfont");
		}

		/// <summary>
		/// unloads all content files, that are not handled by content manager (music, ...?)
		/// </summary>
		public void UnloadContent()
		{
			// 
		}

        public void Draw(double deltaT)
		{
			// save previous viewport
			Viewport original = screenManager.game.graphics.GraphicsDevice.Viewport;
			screenManager.game.graphics.GraphicsDevice.Viewport = viewport;
			screenManager.game.spriteBatch.Begin();


			// set initial drawing position within bounds
			Vector2 drawPos = Vector2.Zero;
			drawPos.X = viewport.Width / 2;
			drawPos.Y += 20;

			// draw entries
			Color color;
			for( int i = 0; i < entries.Count(); ++i )
			{
				if( i == selectedEntry )
					color = Color.Red;
				else
					color = Color.White;
				screenManager.game.spriteBatch.DrawString( font, entries[i], drawPos, color );

				drawPos.Y += 20;
			}


			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
		}

        //public void Enter()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Exit()
        //{
        //    throw new NotImplementedException();
        //}

        public void Update(double deltaT,Input input)
        {
            if (input.pop(InputCommand.DOWN))
                if (selectedEntry < entries.Count - 1)
                    selectedEntry++;

            if (input.pop(InputCommand.UP))
                if (selectedEntry > 0)
                    selectedEntry--;

            if(input.pop(InputCommand.SELECT))
            {
				switch( selectedEntry )
				{
				case 0:
					screenManager.activateScreen( new LoadingScreen( screenManager, viewport.Bounds ) );
//					// split screen vertically in two halves
//					Rectangle a = viewport.Bounds;
//					a.Width /= 3;
//					a.Width *= 2;
//					Rectangle b = a;
//					b.X += a.Width;
//
//					// goto main game setup
//					screenManager.activateScreen( new GameScreen( screenManager, a ) );
//					screenManager.activateScreen( new MainInterfaceScreen( screenManager, b ) );
					screenManager.deactivateScreen( this );
					break;
				case 1:
					screenManager.deactivateScreen( this );
					break;
				default:
					break;

				}
            }

            if (input.peek(InputCommand.EXIT))
                screenManager.deactivateScreen(this);
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
