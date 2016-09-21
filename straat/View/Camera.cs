using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.View
{
    public class Camera
    {
        public Vector2 position;
        public float zoomFactor;
		private float zoomStep = 2.0f;
		Rectangle viewport;

		public Camera(Rectangle viewport)
        {
            position = Vector2.Zero;
            zoomFactor = 1.0f;
			this.viewport = viewport;
        }

		public void ZoomIn()
		{
			zoomFactor *= zoomStep;
		}

		public void ZoomOut()
		{
			zoomFactor /= zoomStep;
		}

		public Vector2 getDrawPos(Vector2 worldPos)
		{
			return (worldPos - position)*zoomFactor;
		}

		public Vector2 getWorldPos(Vector2 drawPos)
		{
			return drawPos/zoomFactor + position;
		}

		public bool isInBounds(Vector2 p)
		{
			return viewport.Contains( getDrawPos(p).ToPoint() + viewport.Location);
		}

		public bool isInBounds(Rectangle r)
		{
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
    }
}
