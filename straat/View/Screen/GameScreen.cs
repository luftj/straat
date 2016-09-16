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
		Entity selection;

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

			// TESTING
			world.entities.Add(EntityFactory.Instance.createTestEntity());
		}

		public void UnloadContent()
		{
			//
		}

		public void Draw( double deltaT )
		{
			#region begin_draw
			// save previous viewport
			Viewport original = screenManager.game.graphics.GraphicsDevice.Viewport;
			screenManager.game.graphics.GraphicsDevice.Viewport = viewport;
			#endregion

			#region picker
			// refresh selectable entities
			picker.Begin();
			// todo: handle everything selectable (implementing ISelectable)
			foreach(Entity entity in world.getSelectableEntities())
			{
				//  consider screen boundaries
				Vector2 drawingPos = entity.worldPos - camera.position;
				if( !camera.isInBounds( drawingPos.ToPoint() ) )					// todo: check for rectangle of texture instead // todo: respect zoom level
					continue;
				picker.Add( entity, entity.gc.texture, drawingPos );
			}
			picker.End();
			#endregion


			#region scene_drawing
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
				Color curCol = entity.getAllegiance() == Allegiance.PLAYER ? Color.Blue : Color.Red;
				screenManager.game.spriteBatch.Draw(entity.gc.texture,drawingPos,curCol); // todo: reflect zoom level by scale argument
			}
			#endregion

			screenManager.game.spriteBatch.DrawString( font, debugtext, new Vector2( 10, 10 ), Color.Red );

			#region end_draw
			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
			#endregion
		}

		public void Update( double deltaT, Input input )
		{
			// move camera according to input
			if( input.peek( InputCommand.LEFT_CONT ) ) 	camera.position.X--; 
			if( input.peek( InputCommand.RIGHT_CONT ) ) camera.position.X++;
			if( input.peek( InputCommand.UP_CONT ) ) 	camera.position.Y--;
			if( input.peek( InputCommand.DOWN_CONT ) ) 	camera.position.Y++;

			// get mouse position
			int x = input.pointerEvent.X;
			int y = input.pointerEvent.Y;
			debugtext = x + ", " + y;

			// handle clicks
			if( viewport.Bounds.Contains( x, y ) )
			{
				if( input.pointerEvent.Command == PointerCommand.PRIMARY || input.pointerEvent.Command == PointerCommand.SECONDARY )
				{
					selection = picker.getSelection( x - viewport.Bounds.Left, y - viewport.Bounds.Top );

					// pass the new selection to all interested screens
					screenManager.addMessage( new ScreenMessage( typeof(InterfaceScreen), ScreenMessageType.SELECTION_CHANGE, selection ) );
				}
			}
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
