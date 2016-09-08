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
        public float zoomFaktor;
		Rectangle viewport;

		public Camera(Rectangle viewport)
        {
            position = Vector2.Zero;
            zoomFaktor = 1.0f;
			this.viewport = viewport;
        }
    }
}
