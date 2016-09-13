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
		Viewport viewport;

		Picker picker;

		Camera camera;
		World world;


		#region content
		SpriteFont font;
		string debugtext;
		#endregion

		public GameScreen( ScreenManager screenManager, Rectangle bounds )
		{
			this.screenManager = screenManager;
			viewport = new Viewport( bounds );

			picker = new Picker( screenManager.game, bounds.Width, bounds.Height );

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
			// save previous viewport
			Viewport original = screenManager.game.graphics.GraphicsDevice.Viewport;
			screenManager.game.graphics.GraphicsDevice.Viewport = viewport;

			// refresh selectable entities
			picker.Begin();
			// todo: handle everything selectable (implementing ISelectable)
			foreach(Entity entity in world.getSelectableEntities())
			{
				//  consider screen boundaries
				Vector2 drawingPos = entity.worldPos - camera.position;
				if( !camera.isInBounds( drawingPos.ToPoint() ) )					// todo: check for rectangle of texture instead // todo: respect zoom level
					continue;
				picker.Add( entity.sc, entity.gc.texture, drawingPos );
			}
			picker.End();


			// draw world
			screenManager.game.GraphicsDevice.Clear(Color.CornflowerBlue);
			screenManager.game.spriteBatch.Begin();

			foreach( Entity entity in world.getDrawableEntities() )
			{
				//  consider screen boundaries
				Vector2 drawingPos = entity.worldPos - camera.position;
				if( !camera.isInBounds( drawingPos.ToPoint() ) )					// todo: check for rectangle of texture instead // todo: respect zoom level
					continue;
				// is in view: draw world object
				screenManager.game.spriteBatch.Draw(entity.gc.texture,drawingPos,Color.White); // todo: reflect zoom level by scale argument
			}

			screenManager.game.spriteBatch.DrawString( font, debugtext, new Vector2( 100, 100 ), Color.White );

			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
		}

		public void Update( double deltaT, Input input )
		{
			// move camera according to input
			if( input.peek( InputCommand.LEFT_CONT ) ) 	camera.position.X--; 
			if( input.peek( InputCommand.RIGHT_CONT ) ) camera.position.X++;
			if( input.peek( InputCommand.UP_CONT ) ) 	camera.position.Y--;
			if( input.peek( InputCommand.DOWN_CONT ) ) 	camera.position.Y++;


			int x = input.pointerEvent.X;
			int y = input.pointerEvent.Y;
			debugtext = x + ", " + y + " : " + input.pointerEvent.Command.ToString();


			if( viewport.Bounds.Contains( x, y ) && ( input.pointerEvent.Command == PointerCommand.PRIMARY ) )
				picker.getSelection( x - viewport.Bounds.Left, y - viewport.Bounds.Top );
		}
        
	}
}
