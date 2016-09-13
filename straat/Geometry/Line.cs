using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace straat.Geometry
{
    public class Line
    {
        public Point start;
        public Point end;

        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        public bool intersectsRayRay(Line other)
        {
            float det = (end.Y - start.Y) * (other.end.X - other.start.X) - (end.X - start.X) * (other.end.Y - other.start.Y);
            if (det == 0)
                return false;
            else
                return true;
        }

        public bool intersectsSegmentRay(Line other)    //todo funktioniert nicht richtig?
        {
            float pbua1 = (end.X-start.X)*(other.start.Y-start.Y)-(end.Y-start.Y)*(other.start.X-start.X);
            float pbu2  = (end.Y-start.Y)*(other.end.X-other.start.X)-(end.X-start.X)*(other.end.Y-other.start.Y);
            float pbub1 = (other.end.X-other.start.X)*(other.start.Y-start.Y)-(other.end.Y-other.start.Y)*(other.start.X-start.X);
            if(pbu2==0)
                return false;
            float pbua = pbua1/pbu2;
            float pbub = pbub1/pbu2;
            if(pbub>=0&&pbub<=1)
                return true;
            return false;
        }

        public bool intersectsSegmentSegment(Line other)
        {
            //xy coordinates only
            float pbua1 = (end.X - start.X) * (other.start.Y - start.Y) - (end.Y - start.Y) * (other.start.X - start.X);
            float pbu2 = (end.Y - start.Y) * (other.end.X - other.start.X) - (end.X - start.X) * (other.end.Y - other.end.Y);
            float pbub1 = (other.end.X - other.start.X) * (other.start.Y - start.Y) - (other.end.Y - other.start.Y) * (other.start.X - start.X);
            if (pbu2 == 0)
                return false;
            float pbua = pbua1 / pbu2;
            float pbub = pbub1 / pbu2;
            
            if (pbub >= 0 && pbub <= 1 && pbua >= 0 && pbua <= 1)
                return true;

            return false;
        }

        public Point getIntersection(Line other)
        {
            float pbua1 = (end.X - start.X) * (other.start.Y - start.Y) - (end.Y - start.Y) * (other.start.X - start.X);
            float pbu2 = (end.Y - start.Y) * (other.end.X - other.start.X) - (end.X - start.X) * (other.end.Y - other.start.Y);
            if (pbu2 != 0)
            {
                float pbua = pbua1 / pbu2;
                float x = other.start.X + (other.end.X - other.start.X) * pbua;
                float y = other.start.Y + (other.end.Y - other.start.Y) * pbua;

                Point p = new Point(x, y);
                return p;
            }
            else
                return null;
        }
    }
}
