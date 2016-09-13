using System;
using straat.View.Screen;
using Microsoft.Xna.Framework;
using straat.Control;
using Microsoft.Xna.Framework.Graphics;

namespace straat
{
	public class MainInterfaceScreen : InterfaceScreen
	{
		public MainInterfaceScreen(ScreenManager sm, Rectangle b ) : base(sm,b)
		{
		}

		public override void Initialize()
		{
			// list squads
			links.Add(InputCommand.S,new SquadInterfaceScreen(screenManager,viewport.Bounds));
			base.Initialize();
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

			// set initial drawing position
			Vector2 drawPos = new Vector2(10,10);

			// todo: draw content
			screenManager.game.spriteBatch.DrawString(font,"List _S_quads",drawPos,Color.White);

			base.Draw(deltaT);


			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
		}
	}
}

