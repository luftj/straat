using System;
using straat.Control;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using straat.Model;

namespace straat.Control
{
	public class Picker
	{
		private Game1 game;

		private Effect fillTexEffect;
		private RenderTarget2D objectMap;

		private Color curColour;
		private byte colourStep = 1;

		private Dictionary<Color, Entity> lookup;


		public Picker(Game1 game, int width, int height)
		{
			this.game = game;
			lookup = new Dictionary<Color, Entity>();

			objectMap = new RenderTarget2D( game.GraphicsDevice, width, height );
		}

		/// <summary>
		/// Begin handling of selectable objects.
		/// </summary>
		public void Begin()
		{
			lookup.Clear();
			curColour = new Color( 0, 0, 1 ); // set start color, can not be black

			game.GraphicsDevice.SetRenderTarget(objectMap);
			game.GraphicsDevice.Clear(Color.Black);

			game.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, fillTexEffect);
		}

		/// <summary>
		/// End and clean up for this frame
		/// </summary>
		public void End()
		{
			game.spriteBatch.End();

			// switch back to default render target
			game.GraphicsDevice.SetRenderTarget(null);
		}

		/// <summary>
		/// Adds an object, to handle clicks on it. Use inbetween Begin() and End() calls!
		/// </summary>
		/// <param name="worldObject">World object.</param>
		/// <param name="tex">Sprite of the object, which descirbes it's boundaries.</param>
		/// <param name="pos">Position, where to blit the sprite on the viewport</param>
		public void Add(Entity worldObject, Texture2D tex, Vector2 pos)
		{
			// call to begin

			nextColour();	// use a new color key for this object

			// draw this sprite on the backbuffer
			game.spriteBatch.Draw(tex, pos, curColour);

			//add object to color-lookup-table
			lookup.Add(curColour, worldObject);

			// call to end
		}

		/// <summary>
		/// Gets the selection at given screen coordinates.
		/// </summary>
		/// <returns>The selection. Or NULL if there was no object found.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Entity getSelection(int x, int y)
		{
			int i = x + y * objectMap.Width;
			Rectangle r = new Rectangle( x, y, 1, 1 );
			Color[] dat = new Color[1];
			objectMap.GetData<Color>(0,r, dat, 0, 1 );
			if( dat[0] != Color.Black )
				return lookup[dat[0]];
			else return null;
		}

		/// <summary>
		/// generates a new unique color
		/// </summary>
		private void nextColour()
		{
			if( curColour.R + colourStep < 255 )
				curColour.R += colourStep;
			else if( curColour.G + colourStep < 255 )
				curColour.G += colourStep;
			else if( curColour.B + colourStep < 255 )
				curColour.B += colourStep;
			else
				throw new ArgumentOutOfRangeException( "curColour", "Too many objects in scene for current colourkey settings" );
		}
	}
}

