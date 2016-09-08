using System;
using straat.View.Screen;
using Microsoft.Xna.Framework;
using straat.Control;

namespace straat
{
	public class MainInterfaceScreen : InterfaceScreen
	{
		public MainInterfaceScreen(ScreenManager sm, Rectangle b ) : base(sm,b)
		{
			// list squads
			links.Add(InputCommand.S,new SquadInterfaceScreen(sm,b));
		}

		public new void Initialize()
		{
			base.Initialize();
		}

		public new void Update(double deltaT, Input input)
		{
			base.Update( deltaT, input );
		}

		public new void Draw(double deltaT)
		{
			// todo: draw content
			screenManager.game.spriteBatch.DrawString(font,"List _S_quads",new Vector2(10,50),Color.White);

			base.Draw(deltaT);
		}
	}
}

