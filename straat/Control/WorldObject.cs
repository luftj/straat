using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace libertad
{
    public class WorldObject
    {
        public int id;
        static int idcounter=0;

        public Vector2 position;


        public WorldObject(int x, int y)
        {
            position.X = x;
            position.Y = y;

            id = idcounter;
            ++idcounter;
        }
    }
}
