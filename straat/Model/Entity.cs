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

        Vector2 worldPos;
        
        public Entity()
        {
            worldPos = Vector2.Zero;
        } 
    }
}
