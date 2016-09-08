using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using straat.Control;
using straat.Model;
using straat.View.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace straat.View.Screen
{
	class GameScreen : IScreen
	{
		ScreenManager screenManager;
		Camera camera;
		World world;

		SpriteFont font;

		public GameScreen( ScreenManager screenManager, Rectangle bounds )
		{
			this.screenManager = screenManager;
			camera = new Camera( bounds );

			world = new World();
		}

		public void Initialize()
		{
			//
		}

		public void LoadContent()
		{
			font = screenManager.game.Content.Load <SpriteFont > ( "testfont" );
		}

		public void UnloadContent()
		{
			//
		}

		public void Draw( double deltaT )
		{
			// draw world
			foreach( GraphicsComponent gc in world.drawableObjects )
			{
				// todo: consider screen boundaries
				gc.Draw(deltaT);
			}

			screenManager.game.spriteBatch.DrawString( font, "This is the GameScreen", new Vector2( 100, 100 ), Color.White );

		}

		public void Update( double deltaT, Input input )
		{
			// move camera according to input
			if( input.peek( InputCommand.LEFT ) ) camera.position.X--; 
			if( input.peek( InputCommand.RIGHT ) ) camera.position.X++;
			if( input.peek( InputCommand.UP ) ) camera.position.Y--;
			if( input.peek( InputCommand.DOWN ) ) camera.position.Y++;
		}
        
	}
}
