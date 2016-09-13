using System;
using straat.Control;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using straat.Model;

namespace straat
{
	public class Picker
	{
		Game1 game;

		Effect fillTexEffect;
		RenderTarget2D currentObjectMap;
		Texture2D objectMap = null; // todo: 7remove?

		private Color curColour;
		private byte colourStep = 20;

		Dictionary<Color, SelectableComponent> lookup;


		public Picker(Game1 game, int width, int height)
		{
			this.game = game;
			lookup = new Dictionary<Color, SelectableComponent>();

			currentObjectMap = new RenderTarget2D( game.GraphicsDevice, width, height );
		}

		public void Initialize()
		{
			curColour = new Color( 100, 100, 100 ); // set start color, can not be black
		}

		public void LoadContent()
		{
			//
		}

		public void UnloadContent()
		{
			//
		}

		/// <summary>
		/// Begin handling of selectable objects.
		/// </summary>
		public void Begin()
		{
			lookup.Clear();

			game.GraphicsDevice.SetRenderTarget(currentObjectMap);
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



			//save object mapping for handling in next frame
			objectMap = (Texture2D)currentObjectMap;
		}

		/// <summary>
		/// Adds an object, to handle clicks on it.
		/// </summary>
		/// <param name="worldObject">World object.</param>
		/// <param name="tex">Sprite of the object, which descirbes it's boundaries.</param>
		/// <param name="pos">Position, where to blit the sprite on the viewport</param>
		public void Add(SelectableComponent worldObject, Texture2D tex, Vector2 pos)
		{
			//begin

			nextColour();	// use a new color key for this object

			// draw this sprite on the backbuffer
			game.spriteBatch.Draw(tex, pos, curColour);

			//add object to color-lookup-table
			lookup.Add(curColour, worldObject);

			//end
		}

		/// <summary>
		/// Gets the selection at given screen coordinates.
		/// </summary>
		/// <returns>The selection. Or NULL if there was no object found.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public SelectableComponent getSelection(int x, int y)
		{
			// todo: only get single pixel data, not the whole rendertarget
			Color[] dat = new Color [objectMap.Width * objectMap.Height];
			objectMap.GetData(dat);
			int i = x + y * objectMap.Width;

			if( dat[i] != Color.Black )
				return lookup[dat[i]];
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

