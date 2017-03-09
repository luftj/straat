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
		public Vector3 position3f;

		// implement as variable, if camera rotation is desired
		Vector3 target3f { get { return new Vector3(position+Vector2.UnitY, 1.0f); } } // HACK: viewMatrix becomes NaN if target not offset

		public Vector2 position { get { return new Vector2(position3f.X, position3f.Y); } }
		public float height{ get { return position3f.Z;} set { position3f.Z = value; } }

		// todo: set origin to viewport center (so zooming happens concentrically)
        public float zoom;
		private float zoomFactor = 2.0f;

		public Matrix viewMatrix { get; private set;}

		Rectangle viewport;

		public float aspectRatio { get { return (float)viewport.Width / (float)viewport.Height;}}

		public Camera(Rectangle viewport)
        {
			this.viewport = viewport;
			zoom = 1.0f;

			position3f = Vector3.Zero;
			position3f.Z = 10000.0f; // MAGIC_NUMBER: get height from map?
			//target3f = new Vector3(position, 0.0f);
			viewMatrix = Matrix.CreateLookAt(position3f, target3f, Vector3.UnitZ);
        }

		public void ZoomIn()
		{
			zoom *= zoomFactor;
			height /= zoomFactor;
		}

		public void ZoomOut()
		{
			zoom /= zoomFactor;
			height *= zoomFactor;
		}

		public Vector2 getDrawPos(Vector2 worldPos)
		{
			Vector2 position = this.position;
			position.Y *= -1.0f;
			return (worldPos - position) * zoom;
		}

		public Vector2 getWorldPos(Vector2 drawPos)
		{
			Vector2 position = this.position;
			position.Y *= -1.0f;
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

		public void Update(Input input)
		{
			bool viewChanged = false;

			// move camera according to input
			int speed = 1 * (int)zoomFactor;
			if(input.peek(InputCommand.SHIFT_CONT)) speed = 10 * (int)zoomFactor;
			if(input.peek(InputCommand.LEFT_CONT))
			{
				position3f.X -= speed;
				//target3f.X -= speed;
				viewChanged = true;
			}
			if(input.peek(InputCommand.RIGHT_CONT))
			{
				position3f.X += speed;
				//target3f.X += speed;
				viewChanged = true;
			}
			if(input.peek(InputCommand.UP_CONT))
			{
				position3f.Y += speed;
				viewChanged = true;
			}
			if(input.peek(InputCommand.DOWN_CONT))
			{
				position3f.Y -= speed;
				viewChanged = true;
			}
			if(input.pop(InputCommand.SCROLL_UP))
			{
				ZoomIn();
				viewChanged = true;
			}
			if(input.pop(InputCommand.SCROLL_DOWN))
			{
				ZoomOut();
				viewChanged = true;
			}

			if(viewChanged) viewMatrix = Matrix.CreateLookAt(position3f, target3f, Vector3.UnitZ);
		}
    }
}
