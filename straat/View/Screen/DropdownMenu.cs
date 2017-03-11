using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using straat.View.Drawing;

namespace straat.View.Screen
{
	public class DropdownMenu
	{
		public Rectangle bounds;
		Point screenPosition { get { return bounds.Location; } }

		bool visible = false;

		List<object> entries;
		int entryHeight;

		SpriteBatch spriteBatch;
		SpriteFont font;

		public DropdownMenu(SpriteBatch spriteBatch, SpriteFont font)
		{
			entries = new List<object>();
			bounds = new Rectangle();
			entryHeight = font.LineSpacing;
			this.spriteBatch = spriteBatch;
			this.font = font;
		}

		public void open(Point screenPos)
		{
			bounds.Location = screenPos;
			visible = true;
		}

		public bool isInBounds(Point p)
		{
			if(!visible) return false;
			return bounds.Contains(p);
		}

		public void addEntry(object entry)
		{
			entries.Add(entry);

			var textwidth = font.MeasureString(entry.ToString()).X;
			//if(textheight.Y > entryHeight) entryHeight = (int)textheight.Y;
			if(textwidth > bounds.Width) bounds.Width = (int)textwidth;

			bounds.Height += entryHeight;
		}

		public object click(Point p)
		{
			if(!isInBounds(p)) return null;

			int selectionY = (p - bounds.Location).Y;
			var selectedEntry = entries[selectionY / entryHeight];

			visible = false;
			return selectedEntry;
		}

		public void Draw()
		{
			if(!visible) return;

			GeometryDrawer.fillRect(bounds, Color.Black);

			Vector2 drawPos = bounds.Location.ToVector2();
			foreach(var item in entries)
			{
				spriteBatch.DrawString(font, item.ToString(), drawPos, Color.White);
				drawPos.Y += entryHeight;
			}
		}
	}
}
