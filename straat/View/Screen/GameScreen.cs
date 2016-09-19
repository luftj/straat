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

		Center curRegion;
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
			GeometryDrawer.setViewPort(viewport);
			#endregion

			#region picker
			// refresh selectable entities
			picker.Begin();
			foreach(Entity entity in world.getSelectableEntities())
			{
				//  consider screen boundaries
				Vector2 drawingPos = camera.getDrawPos(entity.worldPos);
//				if( !camera.isInBounds( drawingPos.ToPoint() ) )					// todo: check for rectangle of texture instead // todo: respect zoom level
//					continue;
				picker.Add( entity, entity.gc.texture, drawingPos );
			}
			picker.End();
			#endregion


			#region scene_drawing
			// draw world
			screenManager.game.GraphicsDevice.Clear(Color.CornflowerBlue);
			screenManager.game.spriteBatch.Begin();


			foreach(Center c in world.map.centers.Values)
			{
				List<Point> poly = new List<Point>();
				List<Color> polyCol = new List<Color>();

				// polygon nodes
				foreach(Corner co in c.polygon)
				{
					Vector2 f = camera.getDrawPos(co.position);

					if(float.IsInfinity(co.position.Length()))
						continue;

					poly.Add(f.ToPoint());

					if(co.isOcean)
						polyCol.Add(Color.DarkBlue);
					else
						polyCol.Add( Color.Lerp(Color.ForestGreen,Color.SlateGray,co.elevation/world.mapBuilder.maxElevation));
				}

				// region center
				Vector2 d = camera.getDrawPos(c.position);
				// draw topography
				Color cCol;
				if(c.isOcean)
				{
					cCol = Color.DarkBlue;
					GeometryDrawer.fillPoly(poly,cCol);
				}
				else
				{
					cCol = Color.Lerp(Color.ForestGreen,Color.SlateGray,c.elevation/world.mapBuilder.maxElevation);
					GeometryDrawer.fillPolyGradient(d,poly,cCol,polyCol.ToArray());
				}

				// draw voronoi edges
				foreach(VDEdge e in c.borders)
				{
					Vector2 a = camera.getDrawPos(e.endpoints[0].position);
					Vector2 b = camera.getDrawPos(e.endpoints[1].position);

					//GeometryDrawer.drawLine(a,b,Color.White);
				}

				// draw river
				foreach(River r in world.map.rivers)
				{
					for(int i = 1;i<r.path.Count;++i)
					{
						Vector2 a = camera.getDrawPos(r.path[i-1].position);
						Vector2 b = camera.getDrawPos(r.path[i].position);
						GeometryDrawer.drawLine(a,b,Color.Blue);
					}
				}

				// draw corners
				foreach(Corner co in c.polygon)
				{
					Vector2 f = camera.getDrawPos(co.position);
					//GeometryDrawer.fillRect((int)f.X,(int)f.Y,3,3,Color.LightBlue);
				}

				// draw region centers
				//GeometryDrawer.fillRect((int)d.X,(int)d.Y,5,5,Color.Blue);
				Vector2 curr = curRegion.position;
				curr = camera.getDrawPos(curr);
				GeometryDrawer.fillRect((int)curr.X,(int)curr.Y,5,5,Color.Red);
			}

			foreach( Entity entity in world.getDrawableEntities() )
			{
				//  consider screen boundaries
				if( !camera.isInBounds( entity.worldPos.ToPoint()) ) 					// todo: check for rectangle of texture instead // todo: respect zoom level
					continue;

				Vector2 drawingPos = camera.getDrawPos(entity.worldPos);

				// is in view: draw world object
				Color curCol = entity.getAllegiance() == Allegiance.PLAYER ? Color.Blue : Color.Red;
				screenManager.game.spriteBatch.Draw(entity.gc.texture,drawingPos,scale:new Vector2(camera.zoomFactor),color:curCol);//null,curCol,0.0f,Vector2.Zero,camera.zoomFactor,SpriteEffects.None,0.0f); // todo: reflect zoom level by scale argument
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
		
		
			// get mouse position
			int x = input.pointerEvent.X;
			int y = input.pointerEvent.Y;
			Vector2 worldPos = camera.getWorldPos( new Vector2( x, y ) );
			debugtext = "screen: " + x + ", " + y + "\n";
			debugtext += "world: " + worldPos.X + ", " + worldPos.Y;
			curRegion = world.map.getRegionAt( worldPos.X, worldPos.Y );
			debugtext += ", elevation: " + curRegion.elevation;

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
