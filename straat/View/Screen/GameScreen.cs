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
using straat.Model.Entities;
using straat.Model.Map;

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
		SceneRenderer sceneRenderer;
		DropdownMenu contextMenu;

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

			sceneRenderer = new SceneRenderer(screenManager.game);
			sceneRenderer.camera = camera;
		}

		public void Initialize()
		{
			// initialise everything we couldn't do in ctor
			mapDrawer = new MapDrawer(world.map);
			sceneRenderer.mapDrawer = mapDrawer;
			sceneRenderer.getVertices(world.map);

			camera.zoom = 0.1f;
		}

		public void LoadContent()
		{
			font = screenManager.game.Content.Load<SpriteFont>( "testfont" );

			sceneRenderer.init(screenManager.game.Content.Load<Effect>("sceneShader"));
			// TESTING
			world.entities.Add(EntityFactory.Instance.createTestEntity(new Vector3(0,0,0)));

			contextMenu = new DropdownMenu(screenManager.game.spriteBatch, font);
			contextMenu.addEntry(OrderType.MOVE);
			contextMenu.addEntry(OrderType.AMBUSH);
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
			//Matrix transform = camera.viewMatrix * Matrix.CreateScale(viewport.Width, -viewport.Height, 1) * Matrix.CreateTranslation(viewport.Width / 2, viewport.Height / 2, 100);
			screenManager.game.GraphicsDevice.Clear(Color.LightBlue);
			screenManager.game.spriteBatch.Begin();



			// todo: 2d/3d select somewhere?
			mapDrawer.Draw(camera);

			// highlight region under cursor
			Vector2 curr = curRegion.position;
			curr = camera.getDrawPos(curr);
			GeometryDrawer.fillRect((int)curr.X,(int)curr.Y,5,5,Color.Red);


			foreach( Entity entity in world.getDrawableEntities() )
			{
				//  consider screen boundaries
				//if( !camera.isInBounds( entity.worldPos) ) 					// todo: check for rectangle of texture instead
				//	continue;

				Vector2 drawingPos = camera.getDrawPos(entity.worldPos);

				// is in view: draw world object
				Color curCol = entity.getAllegiance() == Allegiance.PLAYER ? Color.Blue : Color.Red;
				screenManager.game.spriteBatch.Draw(entity.gc.texture,drawingPos,color:curCol);
			}
			#endregion

			sceneRenderer.Draw(deltaT);

			#region debug_output
			screenManager.game.spriteBatch.DrawString( font, debugtext, new Vector2( 10, 10 ), Color.White );
			#endregion

			contextMenu.Draw();

			#region end_draw
			screenManager.game.spriteBatch.End();
			// switch back to standard viewport
			screenManager.game.graphics.GraphicsDevice.Viewport = original;
			//GeometryDrawer.setViewPort(original);
			#endregion
		}

		public void Update( double deltaT, Input input )
		{
			// update world
			world.Update(deltaT);

			camera.Update( input );

			// refresh scene
			sceneRenderer.Update(deltaT);

			// debug
			mapDrawer.Update( input );
		
			// get mouse position
			int x = input.pointerEvent.X;
			int y = input.pointerEvent.Y;

			// DEBUG: prints some text
			#region debug_output
			Vector3? r = sceneRenderer.rayCast(x, y); // todo: encapsulate screenPos->worldPos
			r = r ?? Vector3.Zero;
			Vector2 worldPos = new Vector2(((Vector3)r).X,((Vector3)r).Y);//camera.getWorldPos( new Vector2( x, y ) );
			debugtext = "screen: " + x + ", " + y + "\n";
			debugtext += "world: " + worldPos.X + ", " + worldPos.Y;
			curRegion = world.map.getRegionAt( worldPos.X, worldPos.Y );
			debugtext += ", elevation: " + curRegion.elevation + ", id: "+ curRegion.id + "\n";
			debugtext += "seed: " + world.seed + "\n";
			debugtext += mapDrawer.shadingStyle.ToString()+"\n";
			debugtext += "cam height: " + camera.height;
			#endregion

			// handle clicks
			if( viewport.Bounds.Contains( x, y ) )
			{
				if(input.pointerEvent.Command == PointerCommand.PRIMARY)
				{
					if(selection != null && contextMenu.isInBounds(new Point(x,y)))
					{
						r = sceneRenderer.rayCast(contextMenu.bounds.Location.X, contextMenu.bounds.Location.Y);
						r = r ?? Vector3.Zero;
						worldPos = new Vector2(((Vector3)r).X, ((Vector3)r).Y);
						selection.standingOrder = new Order((OrderType)contextMenu.click(new Point(x, y)), worldPos);
						return;
					}

					// pass the new selection to all interested screens
					selection = picker.getSelection(x - viewport.Bounds.Left, y - viewport.Bounds.Top);
					if(selection != null)
						screenManager.addMessage(new ScreenMessage(typeof(InterfaceScreen), ScreenMessageType.SELECTION_CHANGE, selection));
					else
						screenManager.addMessage(new ScreenMessage(typeof(InterfaceScreen), ScreenMessageType.SELECTION_CHANGE, curRegion));
				} else if(input.pointerEvent.Command == PointerCommand.SECONDARY)
				{
					if(selection != null)
					{
						// what is here?
						// other entity
						// player entity
						// city
						// just the scene
						contextMenu.open(new Point(x,y));
					}
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
