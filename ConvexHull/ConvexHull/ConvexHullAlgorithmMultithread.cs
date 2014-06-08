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
        public List<HullPoint> InputPoints;
        public List<HullPoint> OutputPoints;

        public ConvexHullAlgorithmMultithread(List<HullPoint> InputPoints, int ThreadCount)
        {
            this.ThreadCount = ThreadCount;
            this.InputPoints = InputPoints;
        }

        public void run()
        {

            if (this.ThreadCount == 1)
            {
                this.OutputPoints = ComputeCovexHull(this.InputPoints, 0, this.InputPoints.Count - 1);
            }

            if (this.ThreadCount == 2)
            {
                // First divide Input points in 2 parts - A and B
                List<HullPoint> A, B;
                int mid = (InputPoints.Count - 1) / 2;

                // The convex hull of A is computed in the Charlie thread
                ConvexHullAlgorithmMultithread Charlie = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(0, mid), 1);
                Thread CharlieThread = new Thread(Charlie.run);

                CharlieThread.Priority = ThreadPriority.AboveNormal;
                CharlieThread.Start();

                // The convex hull of B is computed by the main thread
                B = this.InputPoints.GetRange(mid + 1, InputPoints.Count - 1 - mid);
                B = this.ComputeCovexHull(B, 0, B.Count - 1);

                CharlieThread.Join();

                A = Charlie.OutputPoints;
                this.OutputPoints = this.combine(A, B);
            }

            if (this.ThreadCount == 4)
            {
                // First Divide input in 4 parts - A, B, C, D
                List<HullPoint> A, B, C, D;

                // Important indices when dividing the Array in 4 parts
                int l = 0, r = InputPoints.Count - 1, mid = (l + r) / 2;
                int halfA = mid - l + 1;
                int halfB = r - mid;

                int l1 = l, r1 = mid, mid1 = (l1 + r1) / 2;
                int halfA1 = mid1 - l1 + 1;
                int halfB1 = r1 - mid1;

                int l2 = mid + 1, r2 = r, mid2 = (l2 + r2) / 2;
                int halfA2 = mid2 - l2 + 1;
                int halfB2 = r2 - mid2;

                // A, B and C are computed by the Charlie1-3
                ConvexHullAlgorithmMultithread Charlie1 = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(l1, halfA1), 1);
                ConvexHullAlgorithmMultithread Charlie2 = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(mid1 + 1, halfB1), 1);
                ConvexHullAlgorithmMultithread Charlie3 = new ConvexHullAlgorithmMultithread(this.InputPoints.GetRange(l2, halfA2), 1);
             


                Thread Charlie1Thread = new Thread(Charlie1.run);
                Thread Charlie2Thread = new Thread(Charlie2.run);
                Thread Charlie3Thread = new Thread(Charlie3.run);

                Charlie1Thread.Priority = ThreadPriority.AboveNormal;
                Charlie2Thread.Priority = ThreadPriority.AboveNormal;
                Charlie3Thread.Priority = ThreadPriority.AboveNormal;

                Charlie1Thread.Start(); Charlie2Thread.Start(); Charlie3Thread.Start();

                D = this.InputPoints.GetRange(mid2 + 1, halfB2);
                D = this.ComputeCovexHull(D, 0, D.Count - 1);


                Charlie1Thread.Join(); Charlie2Thread.Join(); Charlie3Thread.Join();

                A = Charlie1.OutputPoints; B = Charlie2.OutputPoints; C = Charlie3.OutputPoints;

                this.OutputPoints = combine(combine(A, B), combine(C, D));
            }
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
            while (!isTangent(a, b, U, orientation))
                b = direction == 1 ? b.next : b.prev;

            return b;
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
            if (debug) Console.WriteLine("\n***Upper tangent is : " + topA.index + " " + topB.index);

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
            if (debug) Console.WriteLine("\n*** Lower tangent is : " + topA.index + " " + topB.index);


            // "Stiching" up the top and bottom of the two Convex Hulls together
            topA.next = topB; topB.prev = topA;
            bottomA.prev = bottomB; bottomB.next = bottomA;

            // The Union of both convex hull can be found by starting at any of the Tangent points and walking untill the same point is reached again
            List<HullPoint> U = new List<HullPoint>();

            HullPoint x = topA;
            if (debug) Console.Write("\n\nThe run is : \n");
            do
            {
                U.Add(x);
                if (debug) Console.Write(x.X + ", " + x.Y + Environment.NewLine);
                x = x.next;
            } while (x != a);

            return U;
        }


        private List<HullPoint> ComputeCovexHull(List<HullPoint> U, int l, int r)
        {
            // Base case with 1 point is a Simple Convex Hull
            if (r - l + 1 == 1)
            {
                List<HullPoint> C = new List<HullPoint>();
                C.Add(U[l]);
                C[0].next = C[0];
                C[0].prev = C[0];
                return C;
            }

            // Base case with 2 points is a Convex Hull with both of them
            if (r - l + 1 == 2)
            {
                List<HullPoint> C = new List<HullPoint>();
                C.Add(U[l]); C.Add(U[r]);

                // Important they have to be linked together
                C[0].next = C[1]; C[0].prev = C[1];
                C[1].next = C[0]; C[1].prev = C[0];

                return C;
            }

            int mid = (l + r) / 2;

            List<HullPoint> A = ComputeCovexHull(U, l, mid);
            List<HullPoint> B = ComputeCovexHull(U, mid + 1, r);

            return combine(A, B);
        }
    }
}


