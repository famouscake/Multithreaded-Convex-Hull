using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHull.Utils
{
    class Point
    {
        public double x;
        public double y;
        
        public int index;

        public Point next;
        public Point prev;

        public Point(double x, double y, int index)
        {
            this.x = x;
            this.y = y;
            this.index = index;
            this.next = this;
            this.prev = this;
        }

        static double Distance(Point a, Point b)
        {
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }        
    }

    class Vector
    {
        double x, y;

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        static public double getCrossProductZ(Point a, Point b, Point c)
        {
            return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
        }      
    }
}
