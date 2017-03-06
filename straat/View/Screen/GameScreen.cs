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
		public GameManager world;
		Entity selection;
		Center curRegion;

		MapDrawer mapDrawer;

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
		}

		public void Initialize()
		{
			//
			mapDrawer = new MapDrawer(screenManager.game,world.map);
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
			GeometryDrawer.setViewPort(viewport);
			#endregion

			#region picker
			// refresh selectable entities
			picker.Begin();
			foreach(Entity entity in world.getSelectableEntities())
			{
				//  consider screen boundaries
				Vector2 drawingPos = camera.getDrawPos(entity.worldPos);
//				if( !camera.isInBounds( drawingPos.ToPoint() ) )					// todo: check for rectangle of texture instead 
//					continue;
				picker.Add( entity, entity.gc.texture, drawingPos );
			}
			picker.End();
			#endregion

			#region scene_drawing
			// draw world
			screenManager.game.GraphicsDevice.Clear(Color.DarkBlue);
			screenManager.game.spriteBatch.Begin();

			mapDrawer.Draw(camera);

			// highlight region under cursor
			Vector2 curr = curRegion.position;
			curr = camera.getDrawPos(curr);
			GeometryDrawer.fillRect((int)curr.X,(int)curr.Y,5,5,Color.Red);


			foreach( Entity entity in world.getDrawableEntities() )
			{
				//  consider screen boundaries
				if( !camera.isInBounds( entity.worldPos) ) 					// todo: check for rectangle of texture instead
					continue;

				Vector2 drawingPos = camera.getDrawPos(entity.worldPos);

				// is in view: draw world object
				Color curCol = entity.getAllegiance() == Allegiance.PLAYER ? Color.Blue : Color.Red;
				screenManager.game.spriteBatch.Draw(entity.gc.texture,drawingPos,scale:new Vector2(camera.zoom),color:curCol);
			}
			#endregion

			#region debug_output
			screenManager.game.spriteBatch.DrawString( font, debugtext, new Vector2( 10, 10 ), Color.White );
			#endregion

			#region end_draw
			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
			//GeometryDrawer.setViewPort(original);
			#endregion
		}

		public void Update( double deltaT, Input input )
		{
			camera.Update( input );		

			// debug
			mapDrawer.Update( input );
		
			// get mouse position
			int x = input.pointerEvent.X;
			int y = input.pointerEvent.Y;

			#region debug_output
			Vector2 worldPos = camera.getWorldPos( new Vector2( x, y ) );
			debugtext = "screen: " + x + ", " + y + "\n";
			debugtext += "world: " + worldPos.X + ", " + worldPos.Y;
			curRegion = world.map.getRegionAt( worldPos.X, worldPos.Y );
			debugtext += ", elevation: " + curRegion.elevation + ", id: "+ curRegion.id + "\n";
			debugtext += "seed: " + world.seed + "\n";
			debugtext += mapDrawer.shadingStyle.ToString();
			#endregion

			// handle clicks
			if( viewport.Bounds.Contains( x, y ) )
			{
				if( input.pointerEvent.Command == PointerCommand.PRIMARY || input.pointerEvent.Command == PointerCommand.SECONDARY )
				{
					// pass the new selection to all interested screens
					selection = picker.getSelection( x - viewport.Bounds.Left, y - viewport.Bounds.Top );
					if(selection != null)
						screenManager.addMessage( new ScreenMessage( typeof(InterfaceScreen), ScreenMessageType.SELECTION_CHANGE, selection ) );
					else
						screenManager.addMessage( new ScreenMessage( typeof(InterfaceScreen), ScreenMessageType.SELECTION_CHANGE, curRegion ) );
				}
			}
		}

		public void changeViewport(float widthScale, float heightScale)
		{
			viewport.X = (int)( viewport.X * widthScale );
			viewport.Y = (int)( viewport.Y * heightScale );
			viewport.Width = (int)( viewport.Width * widthScale );
			viewport.Height = (int)( viewport.Height * heightScale );

			camera.changeViewport( widthScale, heightScale );
			picker = new Picker( screenManager.game, viewport.Bounds.Width, viewport.Bounds.Height );
		}
        
	}
}
