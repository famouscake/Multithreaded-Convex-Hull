using ConvexHull.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConvexHull
{
    class ConvexHullAlgorithmMultithread
    {
        public int ThreadCount;
        public List<PointF> InputPoints;
        public List<PointF> OutputPoints;
        public List<HullPoint> CircularOutputPoints;

        static void printS(List<PointF> S)
        {
            System.IO.File.WriteAllText("D:/points.txt", " ");
            for (int i = 0; i < S.Count; i++)
            {
                //Console.WriteLine(S[i].X + ", " + S[i].Y);
                System.IO.File.AppendAllText("D:/points.txt", S[i].X + ", " + S[i].Y + Environment.NewLine);

            }
        }

        public ConvexHullAlgorithmMultithread(List<PointF> InputPoints, int ThreadCount)
        {
            this.ThreadCount = ThreadCount;
            this.InputPoints = InputPoints;
        }

        public List<HullPoint> ConvertPointsToHullPoints(List<PointF> Points)
        {
            List<HullPoint> HullPoints = new List<HullPoint>();

            for (int i = 0; i < Points.Count; i++)
            {
                HullPoints.Add(new HullPoint(Points[i].X, Points[i].Y, i));
            }

            return HullPoints;
        }




        public void run()
        {
            //printS(InputPoints);

            this.InputPoints.Sort(delegate(PointF a, PointF b) { return a.X.CompareTo(b.X); });

            List<HullPoint> HullPoints = this.ConvertPointsToHullPoints(this.InputPoints);
            List<HullPoint> U = new List<HullPoint>();


            if (ThreadCount == 1)
            {
                U = ComputeCovexHull(HullPoints, 0, InputPoints.Count - 1);
            }

            if (ThreadCount == 2)
            {
                List<HullPoint> A, B;

                int mid = (InputPoints.Count - 1) / 2;

                ConvexHullAlgorithmMultithread Charlie = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(0, mid), 1);
                Thread CharlieThread = new Thread(Charlie.run);

                CharlieThread.Priority = ThreadPriority.AboveNormal;
                CharlieThread.Start();

                

                B = this.ComputeCovexHull(HullPoints, mid + 1, InputPoints.Count - 1);
                CharlieThread.Join();

                A = Charlie.CircularOutputPoints;

                U = this.combine(A, B);
            }

            if (this.ThreadCount == 4)
            {
                List<HullPoint> A, B, C, D;

                int l = 0, r = InputPoints.Count - 1, mid = (l + r) / 2;
                int halfA = mid - l + 1;
                int halfB = r - mid;


                int l1 = l, r1 = mid, mid1 = (l1 + r1) / 2;
                int halfA1 = mid1 - l1 + 1;
                int halfB1 = r1 - mid1;

                int l2 = mid + 1, r2 = r, mid2 = (l2 + r2) / 2;
                int halfA2 = mid2 - l2 + 1;
                int halfB2 = r2 - mid2;



                ConvexHullAlgorithmMultithread Charlie1 = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(l1, halfA1), 1);
                ConvexHullAlgorithmMultithread Charlie2 = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(mid1 + 1, halfB1), 1);
                ConvexHullAlgorithmMultithread Charlie3 = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(l2, halfA2), 1);


                Thread Charlie1Thread = new Thread(Charlie1.run);

                Thread CharlieThread2 = new Thread(Charlie2.run);

                Thread CharlieThread3 = new Thread(Charlie3.run);

                Charlie1Thread.Start(); CharlieThread2.Start(); CharlieThread3.Start();


                D = this.ComputeCovexHull(HullPoints, mid2 + 1, r);

                Charlie1Thread.Join(); CharlieThread2.Join(); CharlieThread3.Join();

                A = Charlie1.CircularOutputPoints; B = Charlie2.CircularOutputPoints; C = Charlie3.CircularOutputPoints;

                U = combine(combine(A, B), combine(C, D));
            }








            HullPoint start = U.FirstOrDefault();

            HullPoint x = start;

            this.OutputPoints = new List<PointF>();
            this.CircularOutputPoints = new List<HullPoint>();
            do
            {
                this.CircularOutputPoints.Add(x);
                this.OutputPoints.Add(new PointF(x.X, x.Y));
                x = x.next;
            } while (x != start);
        }

        // orientation -1 : for vectors counterclockwise of a->b 
        // orientation  1 : for vectors clockwise of a->b

        // To see if a->b is a tangent we must see in all the points of U lie on either of the half planes defined by a->b
        // We use the Z coordinate of a->b and a->c cross product. We require all for points of U that to be > 0 (unless a->c is counterclockwise to a->b)
        private bool isTangent(HullPoint a, HullPoint b, List<HullPoint> U, int orientation)
        {
            for (int i = 0; i < U.Count; i++)
                if (Vector.getCrossProductZ(a, b, U[i]) * orientation > 0)
                    return false;
            return true;
        }

        // direction -1 : counterclockwise
        // direction  1 : clockwise

        // Simply walk along U in the given direction untill a tangent is found. That is guaranteed to happen!
        private HullPoint findTangent(HullPoint a, HullPoint b, List<HullPoint> U, int direction, int orientation)
        {
            while (true)
            {
                if (isTangent(a, b, U, orientation)) return b;
                b = direction == 1 ? b.next : b.prev;
            }
        }

        // A is the left Convex Hull B is the right Convex Hull
        private List<HullPoint> combine(List<HullPoint> A, List<HullPoint> B)
        {
            bool debug = false;

            // The rightmost point in A and the leftmost point in B are origins for the tangent search
            HullPoint rightA = A.Max();
            HullPoint leftB = B.Min();

            if (debug)
            {
                Console.WriteLine();
                Console.Write("\n\n## In A is : \n");
                foreach (HullPoint point in A)
                    Console.WriteLine(point.index + " " + point.X + ", " + point.Y);
                Console.Write("\n## In B is : \n");
                foreach (HullPoint point in B)
                    Console.WriteLine(point.index + " " + point.X + ", " + point.Y);
                Console.WriteLine();
            }

            // a and b are temporary variables for finding the upper tangent
            HullPoint a = rightA, b = leftB;
            while (true)
            {
                // Clockwise, Case 1
                b = findTangent(a, b, B, 1, 1);

                // Case 4
                if (isTangent(b, a, A, -1)) break;

                // Counterclockwise, Case 4
                a = findTangent(b, a, A, -1, -1);

                // Case 1
                if (isTangent(a, b, B, 1)) break;
            }
            HullPoint topA = a, topB = b;
            if (debug) Console.WriteLine("\n***Upper tangent is : " + a.index + " " + b.index);

            // a and b are temporary variables for finding the bottom tangent
            a = rightA; b = leftB;
            while (true)
            {
                // Counterclockwise, Case 2
                b = findTangent(a, b, B, -1, -1);

                // Case 3
                if (isTangent(b, a, A, 1)) break;

                // Clockwise, Case 3
                a = findTangent(b, a, A, 1, 1);

                // Case 2
                if (isTangent(a, b, B, -1)) break;
            }
            HullPoint bottomA = a, bottomB = b;
            if (debug) Console.WriteLine("\n*** Lower tangent is : " + a.index + " " + b.index);


            // "Stiching" up the top and bottom of the two Convex Hulls together
            topA.next = topB; topB.prev = topA;

            bottomA.prev = bottomB; bottomB.next = bottomA;

            // The Union of both convex hull can be found by starting at any of the Tangent points and walking untill the same point is reached again
            List<HullPoint> U = new List<HullPoint>();

            HullPoint x = a;
            if (debug) Console.Write("\n\nThe run is : \n");
            do
            {
                U.Add(x);
                if (debug) Console.Write(x.X + ", " + x.Y + Environment.NewLine);
                x = x.next;
            } while (x != a);

            return U;
        }


        private List<HullPoint> ComputeCovexHull(List<HullPoint> S, int l, int r)
        {
            // Base case with 1 point is a Simple Convex Hull
            if (r - l + 1 == 1)
            {
                List<HullPoint> C = new List<HullPoint>();
                C.Add(S[l]);
                C[0].next = C[0];
                C[0].prev = C[0];
                return C;
            }

            // Base case with 2 points is a Convex Hull with both of them
            if (r - l + 1 == 2)
            {
                List<HullPoint> C = new List<HullPoint>();

                C.Add(S[l]); C.Add(S[r]);

                // Important they have to be linked together
                C[0].next = C[1]; C[0].prev = C[1];

                C[1].next = C[0]; C[1].prev = C[0];

                return C;
            }

            int mid = (l + r) / 2;

            List<HullPoint> A = ComputeCovexHull(S, l, mid);
            List<HullPoint> B = ComputeCovexHull(S, mid + 1, r);

            return combine(A, B);
        }



    }
}


