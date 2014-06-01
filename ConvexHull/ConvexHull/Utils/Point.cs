using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHull.Utils
{
    class HullPoint
    {
        public double x;
        public double y;
        
        public int index;

        public HullPoint next;
        public HullPoint prev;

        public HullPoint(double x, double y, int index)
        {
            this.x = x;
            this.y = y;
            this.index = index;
            this.next = this;
            this.prev = this;
        }

        static double Distance(HullPoint a, HullPoint b)
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

        static public double getCrossProductZ(HullPoint a, HullPoint b, HullPoint c)
        {
            return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
        }      
    }
}
