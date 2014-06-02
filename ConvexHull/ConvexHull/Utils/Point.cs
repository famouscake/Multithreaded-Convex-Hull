using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHull.Utils
{
    class HullPoint : IComparable<HullPoint>
    {
        public float X;
        public float Y;
        
        public int index;

        public HullPoint next;
        public HullPoint prev;

        public HullPoint(float x, float y, int index)
        {
            this.X = x;
            this.Y = y;
            this.index = index;
            this.next = this;
            this.prev = this;
        }

        public int CompareTo(HullPoint that)
        {
            return this.X.CompareTo(that.X);
        }

        static double Distance(HullPoint a, HullPoint b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }     
   
        static public List<HullPoint> PointsToHullPoints(List<PointF> Points)
        {            
            List<HullPoint> HullPoints = new List<HullPoint>();

            for (int i = 0; i < Points.Count; i++)
            {
                HullPoints.Add(new HullPoint(Points[i].X, Points[i].Y, i));
            }

            return HullPoints;       
        }


        static public List<PointF> HullPointsToPoints(List<HullPoint> HullPoints)
        {
            HullPoint start = HullPoints.FirstOrDefault();

            HullPoint x = start;

            List<PointF> Points = new List<PointF>();

            do
            {
                Points.Add(new PointF(x.X, x.Y));
                x = x.next;
            } while (x != start);

            return Points;

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
            return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
        }      
    }
}
