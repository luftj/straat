using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace straat.View.Drawing
{
    class GraphicsComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D texture;

        public GraphicsComponent(Game game) : base(game)
        {
        }

        public void Draw(double deltaTime)
        {
            // draw this object
        }
    }
}
