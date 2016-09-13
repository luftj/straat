using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace straat.Geometry
{
    public class Rectangle
    {
        Point topLeft;
        public float width, height;
        public float left   { get { return topLeft.X; } }
        public float right  { get { return topLeft.X + width; } }
        public float top    { get { return topLeft.Y; } }
        public float bottom { get { return topLeft.Y + height; } }

        public Rectangle(float left, float right, float top, float bottom) : this(new Point(left, top), right - left, bottom - top) { }

        public Rectangle(Point topLeft, float width, float height)
        {
            this.topLeft = topLeft;
            this.width = width;
            this.height = height;
        }
    }
}
