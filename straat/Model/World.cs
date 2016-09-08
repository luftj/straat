using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using straat.View.Drawing;

namespace straat.Model
{
	class World
	{
		public List<GraphicsComponent> drawableObjects;

		public World()
		{
			drawableObjects = new List<GraphicsComponent>();
		}
	}
}
