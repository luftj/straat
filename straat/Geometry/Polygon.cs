using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace straat.Geometry
{
    public class Polygon
    {
        public List<Point> points;
        List<Line> lines;
        //Path hull;

        public Polygon(List<Point> points)
        {
            this.points = new List<Point>(points);
            lines = new List<Line>();
            makeLines();
        }

        public Polygon(List<Line> lines)
        {
            this.lines = new List<Line>(lines);
            points = new List<Point>();
            foreach (Line item in lines)
                points.Add(item.start);
        }

        public float getArea()
        {
            float area = 0.0f;
            for (int i = 0; i < points.Count - 1; ++i)
                area += points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y;
            area /= 2.0f;
            return area;
        }

        public Point getCentroid()
        {
            float Cx = 0.0f, Cy = 0.0f;

            /*if (points.Count == 3)
            {
                Cx = points[0].X + points[1].X + points[2].X;
                Cy = points[0].Y + points[1].Y + points[2].Y;

                Cx /= 3.0f;
                Cy /= 3.0f;

                return new Point(Cx, Cy);
            }*/
            /*
            float area = getArea();

            for (int i = 0; i < points.Count - 1; ++i)
            {
                Cx += (points[i].X + points[i + 1].X) * (points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y);
                Cy += (points[i].Y + points[i + 1].Y) * (points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y);
            }
            Cx /= 6.0f * area;
            Cy /= 6.0f * area;*/
            for (int i = 0; i < points.Count; ++i)
            {
                Cx += points[i].X;
                Cy += points[i].Y;
            }
            Cx /= points.Count;
            Cy /= points.Count;

            return new Point(Cx, Cy);
        }

        public Polygon getConvexHull()
        {
            if (points.Count == 0)
                return null;

            //this.sortAntiClockwise(points[0]);

            List<Point> lp = new List<Point>();
            lp.Add(points[0]);
            lp.Add(points[1]);

            for (int k = 2; k < points.Count; ++k)
            {
                //int it = 0;
                if (points[k].equals(lp[lp.Count - 1], 0.0001f))
                    continue;
                while (lp.Count > 1 && !Point.isLessThan(points[k], lp[lp.Count - 1], lp[lp.Count - 2]))//todo eher andersrum?
                {
                    if (points[k].X == lp[lp.Count - 1].X || points[k].Y == lp[lp.Count - 1].Y)
                        //if (lp[lp.Count - 2].distanceTo(points[k]) < lp[lp.Count - 2].distanceTo(lp[lp.Count - 1]))
                            break;//todo: nur einen behalten!
                    lp.RemoveAt(lp.Count - 1);
                }
                /*if (!Point.isLessThan(points[k], points[k - 2], points[k - 1]))
                {
                    
                    lp.RemoveAt(lp.Count - 1);
                }*/
                //else
                lp.Add(points[k]);
            }


            Polygon ret = new Polygon(lp);
            return ret;
        }

        public Rectangle getBoundingRectangle()
        {
            float minX = float.MaxValue, maxX = float.MinValue, 
                minY = float.MaxValue, maxY = float.MinValue;
            foreach (Point point in points)
            {
                if (point.X < minX)
                    minX = point.X;
                else if (point.X > maxX)
                    maxX = point.X;
                if (point.Y < minY)
                    minY = point.Y;
                else if (point.Y > maxY)
                    maxY = point.Y;
            }
            return new Rectangle(minX, maxX, minY, maxY);
        }

        public bool splittable(Line line)
        {
            int intersections = 0;
            foreach (Line l in lines)
                if (l.intersectsSegmentRay(line))
                    return true;
                    //++intersections;
            if (intersections != 0) //eigentlich == 2
                return true;
            else
                return false;
        }

        public List<Polygon> split(Line line)
        {
            List<Polygon> ret = new List<Polygon>();

            List<Line> polyA = new List<Line>();
            List<Line> polyB = new List<Line>();

            Point intPointA=new Point(-1.0f,-1.0f);
            Point intPointB=new Point(-1.0f,-1.0f);

            bool a = true;

            for (int i = 0; i < lines.Count; ++i)
            {
                if (lines[i].intersectsSegmentRay(line))
                {
                    Point intPoint = lines[i].getIntersection(line);
                    if (a)
                        intPointA = intPoint;
                    else
                        intPointB = intPoint;
                    //wenn intPoint ==(toleranz 0.00001) start oder end
                    if (intPoint.equals(lines[i].start, 0.00001f))
                    {
                        //dann keine neue line
                        intPoint = lines[i].start;
                        (a ? polyA : polyB).Add(lines[i]);
                        continue;
                    }
                    else if (intPoint.equals(lines[i].end, 0.00001f))
                    {
                        intPoint = lines[i].end;
                        (a ? polyA : polyB).Add(lines[i]);
                        a = !a;
                        continue;
                    }
                    else
                    {
                        (a ? polyA : polyB).Add(new Line(lines[i].start, intPoint));
                        a = !a;
                        (a ? polyA : polyB).Add(new Line(intPoint, lines[i].end));
                    }
                }
                else
                    (a?polyA:polyB).Add(lines[i]);
            }
            polyA.Add(new Line(intPointA,intPointB));
            polyB.Add(new Line(intPointB,intPointA));
            ret.Add(new Polygon(polyA));
            ret.Add(new Polygon(polyB));

            return ret;
        }

        public void sortAntiClockwise()//Point centerPoint)
        {
            if (points.Count == 0)
                return;
            /*List<Point> sortedPoints = new List<Point>();

            sortedPoints.Add(points[0]);

            for (int i = 1; i < points.Count; ++i)
            {
                Point newPoint = points[i];

                int it = 0;
                //LinkedListNode<Point> iterator = sortedPoints[0];

                while (true)
                {
                    if (Point.isLessThan(newPoint, sortedPoints[it], centerPoint))
                    {
                        sortedPoints.Insert(it, newPoint);
                        break;
                    }
                    if (it == sortedPoints.Count)
                    {
                        sortedPoints.Add(newPoint);
                        break;
                    }
                    ++it;
                    iterator = iterator.Next;
                }
            }
            points = sortedPoints;*/

            //get rightmost lowest point
            Point rl = points[0];
            for (int i = 1; i < points.Count; ++i)
            {
                if (points[i].Y > rl.Y)
                    rl = points[i];
                else if (points[i].Y == rl.Y)
                    if (points[i].X > rl.X)
                        rl = points[i];
            }

            //centerPoint = tmpa;

            List<Point> ret = new List<Point>();
            ret.Add(rl);
            points.Remove(rl);
            while (points.Count>0)
            {
                Point next = points[0];
                for (int i = 1; i < points.Count; ++i)
                {
                    if (!Point.isLessThan(points[i], next, rl))
                        next = points[i];
                }
                if(!next.equals(ret[ret.Count-1],0.0001f))
                    ret.Add(next);
                points.Remove(next);
            }
            points = ret;
            //bubblesort
            /*int n = points.Count;
            bool swapped = false;
            while(true)
            {
                swapped = false;
                for (int i = 1; i < n; ++i)
                    if (!Point.isLessThan(points[i], points[i - 1], centerPoint))
                    {
                        Point tmp = points[i];
                        points[i] = points[i - 1];
                        points[i - 1] = tmp;
                        swapped = true;
                    }
                --n;
                if (!swapped)
                    break;
            }*/
            /*while (n != 0)
            {
                int newN = 0;
                for(int i = 1;i<n-1;++i)
                {
                    if(Point.isLessThan(points[i],points[i+1],centerPoint))
                    {
                        Point tmp = points[i];
                        points[i]=points[i+1];
                        points[i + 1] = tmp;
                            newN=i;
                    }
                }
                n=newN;
            }*/

            makeLines();
        }

        void makeLines()
        {
            lines.Clear();
            for (int i = 0; i < points.Count - 1; ++i)
            {
                lines.Add(new Line(points[i], points[i + 1]));
            }
            if (points.Count > 2)
                lines.Add(new Line(points[points.Count - 1], points[0]));
        }

        public bool contains(Point point)
        {
            return false;
        }

    }
}
