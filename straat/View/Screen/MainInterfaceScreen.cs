using System;
using straat.View.Screen;
using Microsoft.Xna.Framework;
using straat.Control;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using straat.Model;

namespace straat
{
	public class MainInterfaceScreen : InterfaceScreen
	{
		Entity selection;

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
			// handle passed messages
			List<ScreenMessage> msgs = screenManager.getRelevantMessages(this.GetType());
			foreach(ScreenMessage msg in msgs)
			{
				switch(msg.messageType)
				{
				case ScreenMessageType.SELECTION_CHANGE:
					selection = (Entity)msg.message[0];
					break;
				default:
					break;
				}
			}

			base.Update( deltaT, input );
		}

		public override void Draw(double deltaT)
		{
			// save previous viewport
			Viewport original = screenManager.game.graphics.GraphicsDevice.Viewport;
			screenManager.game.graphics.GraphicsDevice.Viewport = viewport;
			screenManager.game.spriteBatch.Begin();

			base.Draw(deltaT);

			// set initial drawing position
			Vector2 drawPos = new Vector2(10,10);

			// todo: draw content
			screenManager.game.spriteBatch.DrawString( font, "currently selected: " + selection?.id, drawPos, Color.White );
			drawPos.Y += 20;
			screenManager.game.spriteBatch.DrawString(font,"S: List squads",drawPos,Color.White);


			// debug
			drawPos.Y += 20;
			drawPos.Y += 20;
			screenManager.game.spriteBatch.DrawString(font,"C: draw region centers",drawPos,Color.White);
			drawPos.Y += 20;
			screenManager.game.spriteBatch.DrawString(font,"D: draw delaunay graph",drawPos,Color.White);
			drawPos.Y += 20;
			screenManager.game.spriteBatch.DrawString(font,"E: draw region edges",drawPos,Color.White);
			drawPos.Y += 20;
			screenManager.game.spriteBatch.DrawString(font,"F: draw region vertices",drawPos,Color.White);



			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
		}
	}
}

