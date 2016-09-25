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
		public World world;
		Entity selection;

		MapDrawer mapDrawer;

		#region content
		SpriteFont font;
		string debugtext;
		#endregion

		Center curRegion;
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
				screenManager.game.spriteBatch.Draw(entity.gc.texture,drawingPos,scale:new Vector2(camera.zoomFactor),color:curCol);
			}
			#endregion

			screenManager.game.spriteBatch.DrawString( font, debugtext, new Vector2( 10, 10 ), Color.White );

			#region end_draw
			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
			//GeometryDrawer.setViewPort(original);
			#endregion
		}

		public void Update( double deltaT, Input input )
		{
			// move camera according to input
			int speed = 1;
			if( input.peek( InputCommand.SHIFT_CONT ) ) speed = 10;
			if( input.peek( InputCommand.LEFT_CONT ) ) 	camera.position.X-=speed; 
			if( input.peek( InputCommand.RIGHT_CONT ) ) camera.position.X+=speed;
			if( input.peek( InputCommand.UP_CONT ) ) 	camera.position.Y-=speed;
			if( input.peek( InputCommand.DOWN_CONT ) ) 	camera.position.Y+=speed;
			if( input.pop( InputCommand.SCROLL_UP ) ) 	camera.ZoomIn();
			if( input.pop( InputCommand.SCROLL_DOWN ) ) camera.ZoomOut();
		

			// debug
			if(input.pop(InputCommand.C)) mapDrawer.drawVoronoiCenters = !mapDrawer.drawVoronoiCenters;
			if(input.pop(InputCommand.D)) mapDrawer.drawDelaunayEdges = !mapDrawer.drawDelaunayEdges;
			if(input.pop(InputCommand.E)) mapDrawer.drawVoronoiEdges = !mapDrawer.drawVoronoiEdges;
			if(input.pop(InputCommand.F)) mapDrawer.drawVoronoiVertices = !mapDrawer.drawVoronoiVertices;
			if( input.pop( InputCommand.G ) ) mapDrawer.cycleShading();
		
			// get mouse position
			int x = input.pointerEvent.X;
			int y = input.pointerEvent.Y;
			Vector2 worldPos = camera.getWorldPos( new Vector2( x, y ) );
			debugtext = "screen: " + x + ", " + y + "\n";
			debugtext += "world: " + worldPos.X + ", " + worldPos.Y;
			curRegion = world.map.getRegionAt( worldPos.X, worldPos.Y );
			debugtext += ", elevation: " + curRegion.elevation + ", id: "+ curRegion.id + "\n";
			debugtext += "seed: " + world.seed + "\n";
			debugtext += mapDrawer.shadingStyle.ToString();

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

			camera.changeViewport(widthScale,heightScale);
			picker = new Picker( screenManager.game, viewport.Bounds.Width, viewport.Bounds.Height );
		}
        
	}
}
