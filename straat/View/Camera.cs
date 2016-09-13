using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.View
{
    class Camera
    {
        public Vector2 position;
        public float zoomFactor;
		Rectangle viewport;

		public Camera(Rectangle viewport)
        {
            position = Vector2.Zero;
            zoomFactor = 1.0f;
			this.viewport = viewport;
        }

		public bool isInBounds(Point p)
		{
			return viewport.Contains( p - position.ToPoint() + viewport.Location);
		}

		public bool isInBounds(Rectangle r)
		{
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
    }
}
