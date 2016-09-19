using System;
using straat.View.Screen;
using Microsoft.Xna.Framework;
using straat.Control;
using Microsoft.Xna.Framework.Graphics;

namespace straat
{
	public class SquadInterfaceScreen : InterfaceScreen
	{
		public SquadInterfaceScreen(ScreenManager sm, Rectangle b) : base(sm,b)
		{
		}

		public override void Initialize()
		{
			// return to main interface screen
			links.Add( InputCommand.EXIT, new MainInterfaceScreen( screenManager, viewport.Bounds ) );
		}

		public override void Update(double deltaT, Input input)
		{
			base.Update( deltaT, input );
		}

		public override void Draw(double deltaT)
		{
			// save previous viewport
			Viewport original = screenManager.game.graphics.GraphicsDevice.Viewport;
			screenManager.game.graphics.GraphicsDevice.Viewport = viewport;
			screenManager.game.spriteBatch.Begin();

			base.Draw( deltaT );


			// set initial drawing position within bounds
			Vector2 drawPos = new Vector2( 10, 10 );

			// todo: draw content
			screenManager.game.spriteBatch.DrawString( font, "Return", drawPos, Color.White );



			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
		}
	}
}

