using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using straat.Control;

namespace straat.View
{
    public class Camera
    {
        public Vector2 position;
		// todo: set origin to viewport center (so zooming happens concentrically)
        public float zoom;
		private float zoomFactor = 2.0f;
		Rectangle viewport;

		public Camera(Rectangle viewport)
        {
            position = Vector2.Zero;
            zoom = 1.0f;
			this.viewport = viewport;
        }

		public void ZoomIn()
		{
			zoom *= zoomFactor;
		}

		public void ZoomOut()
		{
			zoom /= zoomFactor;
		}

		public Vector2 getDrawPos(Vector2 worldPos)
		{
			return (worldPos - position)*zoom;
		}

		public Vector2 getWorldPos(Vector2 drawPos)
		{
			return drawPos / zoom + position;
		}

		public bool isInBounds(Vector2 p)
		{
			// todo: respect zoom level
			return viewport.Contains( getDrawPos(p).ToPoint() + viewport.Location);
		}

		public bool isInBounds(Rectangle r)
		{
			// todo: respect zoom level

			// assumes viewport is always bigger than r in every dimension
			//check every corner of the given rectangle
			Rectangle tmp = r;
			tmp.Location -= position.ToPoint() - viewport.Location;
			// top left
			if( viewport.Contains( tmp ))
				return true;
			// bottom right
			Point bottomright = tmp.Location + r.Size;
			if( viewport.Contains( bottomright ) )
				return true;
			// top right
			Point topright = bottomright;
			topright.Y -= r.Height;
			if( viewport.Contains( topright ) )
				return true;
			// bottom left
			Point bottomleft = bottomright;
			bottomleft.X -= r.Width;
			return viewport.Contains( bottomleft );
		}

		public void changeViewport(float widthScale, float heightScale)
		{
			viewport.X = (int)( viewport.X * widthScale );
			viewport.Y = (int)( viewport.Y * heightScale );
			viewport.Width = (int)( viewport.Width * widthScale );
			viewport.Height = (int)( viewport.Height * heightScale );
		}

		public void Update( Input input )
		{
			// move camera according to input
			int speed = 1;
			if( input.peek( InputCommand.SHIFT_CONT ) ) speed = 10;
			if( input.peek( InputCommand.LEFT_CONT ) ) 	position.X -= speed; 
			if( input.peek( InputCommand.RIGHT_CONT ) ) position.X += speed;
			if( input.peek( InputCommand.UP_CONT ) ) 	position.Y -= speed;
			if( input.peek( InputCommand.DOWN_CONT ) ) 	position.Y += speed;
			if( input.pop( InputCommand.SCROLL_UP ) ) 	ZoomIn();
			if( input.pop( InputCommand.SCROLL_DOWN ) ) ZoomOut();
		}
    }
}
