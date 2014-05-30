using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvexHull.Utils;

namespace ConverHull
{

    class Program
    {

        static public int getLeftMost(List<Point> S)
        {
            int x = 0;

            for (int i = 0; i < S.Count; i++)
            {
                if (S[x].x > S[i].x)
                {
                    x = i;
                }
            }
            return x;
        }
        static public int getRightMost(List<Point> S)
        {
            int x = 0;

            for (int i = 0; i < S.Count; i++)
            {
                if (S[x].x < S[i].x)
                {
                    x = i;
                }
            }
            return x;
        }







        //static public bool isTangent(Point a, Point b, List<Point> U)
        //{
        //    //Console.WriteLine("IS UPPER here at : " + a.index + " "+ b.index + " " + direction);


        //    int countAbouve = 0;
        //    int countBellow = 0;

        //    for (int i = 0; i < U.Count; i++)
        //    {
        //        // Console.WriteLine("With : " + U[i].index + " " + Point.getScalarGz(a, b, U[i]) * direction);

        //        if (Point.getScalarGz(a, b, U[i]) > 0)
        //            countAbouve++;
        //        else if (Point.getScalarGz(a, b, U[i]) < 0)
        //            countBellow++;
        //    }

        //    if (countBellow > 0 && countAbouve > 0)
        //    {
        //        return false;
        //    }

        //    return true;
        //}


        // Case 1 and 4
        public static bool isTangent(Point a, Point b, List<Point> U, int modifier)
        {
            for (int i = 0; i < U.Count; i++)
            {
                // Console.WriteLine("With : " + U[i].index + " " + Point.getScalarGz(a, b, U[i]) * direction);

                double x = Vector.getCrossProductZ(a, b, U[i]);

                if (modifier == 1 && x > 0)
                    return false;

                if (modifier == 2 && x < 0)
                    return false;

                if (modifier == 3 && x > 0)
                    return false;

                if (modifier == 4 && x < 0)
                    return false;
            }

            return true;


        }


        static public Point findTangent(Point a, Point b, List<Point> U, int direction, int modifier)
        {
            while (true)
            {
                if (isTangent(a, b, U, modifier)) return b;

                Console.WriteLine(b.index);

                if (direction == 1)
                    b = b.next;
                else
                    b = b.prev;
            }
        }


        static public List<Point> combine(List<Point> A, List<Point> B)
        {
            int rightA = getRightMost(A);
            int leftB = getLeftMost(B);



            Console.WriteLine();
            Console.Write("\n\n## In A is : ");
            foreach (Point x in A)
                Console.Write(x.index + " ");
            Console.Write("\n## In B is : ");
            foreach (Point x in B)
                Console.Write(x.index + " ");

            Console.WriteLine();


            Point a2, b2;
            a2 = A[rightA];
            b2 = B[leftB];

            while (true)
            {
                Console.WriteLine("Upper");
                Console.WriteLine("a2 - " + a2.index + " b2 - " + b2.index);

                // Case 1
                b2 = findTangent(a2, b2, B, 1, 1);
                Console.WriteLine("New B is : " + b2.index);

                // Case 4
                if (isTangent(b2, a2, A, 4)) break;


                // Case 4
                a2 = findTangent(b2, a2, A, -1, 4);
                Console.WriteLine("New A is : " + a2.index);

                // Case 1
                if (isTangent(a2, b2, B, 1)) break;
            }

            Console.WriteLine("\n***Upper tangent is : " + a2.index + " " + b2.index);



            Point a = A[rightA];
            Point b = B[leftB];

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("a - " + a.index + " b - " + b.index);


                // 2
                b = findTangent(a, b, B, -1, 2);

                Console.WriteLine("New B is : " + b.index);

                // 3
                if (isTangent(b, a, A, 3))
                {
                    Console.WriteLine("af");
                    break;
                }

                // 3
                a = findTangent(b, a, A, 1, 3);

                // 2
                if (isTangent(a, b, B, 2)) break;

                Console.WriteLine("fasdf");

            }

            Console.WriteLine("\n*** Lower tangent is : " + a.index + " " + b.index);



            a.prev = b;
            b.next = a;

            a2.next = b2;
            b2.prev = a2;

            List<Point> P = new List<Point>();

            Point r = a;


            Console.Write("\n\nThe run is : ");
            do
            {
                P.Add(r);
                Console.Write(r.index + " ( " + r.x + " , " + r.y + " ) " + Environment.NewLine);
                r = r.next;
            } while (r != a);

            Console.WriteLine();

            return P;
        }


        static public List<Point> f(List<Point> S, int l, int r)
        {
            if (r - l + 1 == 1)
            {
                List<Point> A = new List<Point>();
                A.Add(S[l]);
                A[0].next = A[0];
                A[0].prev = A[0];
                return A;
            }

            if (r - l + 1 == 2)
            {
                List<Point> A = new List<Point>();

                A.Add(S[l]);
                A.Add(S[r]);

                A[0].next = A[1];
                A[0].prev = A[1];

                A[1].next = A[0];
                A[1].prev = A[0];

                return A;
            }

            int mid = (l + r) / 2;

            Console.WriteLine("---------------------------------------------------" + (r - l + 1));

            List<Point> S1 = f(S, l, mid);
            List<Point> S2 = f(S, mid + 1, r);

            return combine(S1, S2);
        }




        static void Main(string[] args)
        {
            List<Point> S = new List<Point>();

            int N;

            N = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine(N);


            for (int i = 0; i < N; i++)
            {
                S.Add(new Point(Convert.ToDouble(Console.ReadLine()), Convert.ToDouble(Console.ReadLine()), i));
            }

            printS(S);
            S.Sort(delegate(Point a, Point b) { return a.x.CompareTo(b.x); });
            printS(S);
            f(S, 0, 9);






            Console.Write("\n\nPress any key to exit ...");
            Console.ReadLine();
        }

        static void printS(List<Point> S)
        {
            for (int i = 0; i < S.Count; i++)
            {
                Console.WriteLine(i + " (" + S[i].x + " , " + S[i].y + ")");
            }
        }
    }
}
