using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace straat.Geometry
{
    public class Path
    {
        List<Point> points;

        public Path()
        {
            points = new List<Point>();
        }

        public void addNode(Point node)
        {
            points.Add(node);
        }
    }
}
