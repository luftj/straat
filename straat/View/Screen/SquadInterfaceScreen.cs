using System;
using straat.View.Screen;
using Microsoft.Xna.Framework;
using straat.Control;

namespace straat
{
	public class SquadInterfaceScreen : InterfaceScreen
	{
		public SquadInterfaceScreen(ScreenManager sm, Rectangle b) : base(sm,b)
		{
			// return to main interface screen
			links.Add( InputCommand.EXIT, new MainInterfaceScreen( sm, b ) );
		}

		public new void Update(double deltaT, Input input)
		{
			base.Update( deltaT, input );
		}

		public new void Draw(double deltaT)
		{
			// todo: draw content
			screenManager.game.spriteBatch.DrawString(font,"Return",new Vector2(10,50),Color.White);

			base.Draw(deltaT);
		}
	}
}

