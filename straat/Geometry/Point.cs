using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace straat.Geometry
{
    public class Point
    {
        public float X;
        public float Y;

        int precision = 5;

        public Point(float x, float y)
        {
            this.X = (float)Math.Round((double)x, precision);
            this.Y = (float)Math.Round((double)y, precision);
        }

        public static bool isLessThan(Point a, Point b, Point center) 
        {
            if (a.equals(b, 0.00001f))
                return false;
            if (a.X - center.X >= 0 && b.X - center.X < 0)
                return false;
            if (a.X - center.X == 0 && b.X - center.X == 0)
            {
                /*if (a.Y - center.Y >= 0 || b.Y - center.Y >= 0)
                    return a.Y < b.Y;
                return b.Y < a.Y;*/
                if (-(a.Y - center.Y) >= 0 || -(b.Y - center.Y) >= 0)
                    return a.Y > b.Y;
                return a.Y < b.Y;
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            float det = (a.X - center.X) * (b.Y - center.Y) - (b.X - center.X) * (a.Y - center.Y);
            //float det = (a.X - center.X) * -(b.Y - center.Y) - (b.X - center.X) * -(a.Y - center.Y);
            if (det < 0)
                return false;
            if (det > 0)
                return true;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            float d1 = (a.X - center.X) * (a.X - center.X) + (a.Y - center.Y) * (a.Y - center.Y);
            float d2 = (b.X - center.X) * (b.X - center.X) + (b.Y - center.Y) * (b.Y - center.Y);
            return d1 < d2;
        }

        public bool equals(Point other, float threshhold)
        {
            if (X+threshhold < other.X) return false;
            if (X-threshhold > other.X) return false;
            if (Y+threshhold < other.Y) return false;
            if (Y-threshhold > other.Y) return false;
            return true;
        }

        public float distanceTo(Point other)
        {
            float dist = (this.X - other.X) * (this.Y - other.Y);
            dist = (float)Math.Sqrt(dist);
            return dist;
        }

        public Point getDeltaTo(Point other)
        {
            return new Point(other.X-this.X,other.Y-this.Y);
        }

        public float getAngleToVertical(Point other)
        {
            Point vector = getDeltaTo(other);
            vector.normalise();
            Point vertical = new Point(0.0f, 1.0f);

            return (float)Math.Acos(vertical.X * vector.X + vertical.Y + vector.Y);
        }

        public void normalise()
        {
            float magnitude = X * X + Y * Y;
            X /= magnitude;
            Y /= magnitude;
        }
    }
}
