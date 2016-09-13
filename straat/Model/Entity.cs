using Microsoft.Xna.Framework;
using straat.View.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.Model
{
    class Entity
    {
        public GraphicsComponent gc { get; private set; }
		public SelectableComponent sc { get; }

		public Vector2 worldPos {get;}

		public int id { get; private set;}
		private static int idCounter = 0;
        
        public Entity()
        {
            worldPos = Vector2.Zero;

			id = idCounter;
			++idCounter;
        } 
    }
}
