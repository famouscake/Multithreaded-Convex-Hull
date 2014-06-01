using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvexHull.Utils;
using System.ComponentModel;

namespace ConverHull
{

    class Program
    {
        static private List<Point> generatePoints(int pointLimit, int maxValue)
        {
            List<Point> U = new List<Point>();

            Random r = new Random();

            StringBuilder a = new StringBuilder();

            for (int i = 0; i < pointLimit; i++)
            {
                
                U.Add(new Point(r.Next(maxValue), r.Next(maxValue)));

                a.Append(U[i].X.ToString());
                a.Append(", ");
                a.Append(U[i].Y.ToString());
                a.Append(Environment.NewLine);
            }

            System.IO.File.WriteAllText(@"D:\output.txt", a.ToString());

            return U;
        }

        static public int getLeftMost(List<HullPoint> S)
        {
            int x = 0;

            for (int i = 0; i < S.Count; i++)
            {
                if (S[x].X > S[i].X)
                {
                    x = i;
                }
            }
            return x;
        }
        static public int getRightMost(List<HullPoint> S)
        {
            int x = 0;

            for (int i = 0; i < S.Count; i++)
            {
                if (S[x].X < S[i].X)
                {
                    x = i;
                }
            }
            return x;
        }







        // Case 1 and 4
        public static bool isTangent(HullPoint a, HullPoint b, List<HullPoint> U, int modifier)
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


        static public HullPoint findTangent(HullPoint a, HullPoint b, List<HullPoint> U, int direction, int modifier)
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


        static public List<HullPoint> combine(List<HullPoint> A, List<HullPoint> B)
        {           

            HullPoint rightA = A.Max();
            HullPoint leftB = B.Min();



            Console.WriteLine();
            Console.Write("\n\n## In A is : ");
            foreach (HullPoint x in A)
                Console.Write(x.index + " ");
            Console.Write("\n## In B is : ");
            foreach (HullPoint x in B)
                Console.Write(x.index + " ");

            Console.WriteLine();


            HullPoint a2, b2;
            a2 = rightA;
            b2 = leftB;

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



            HullPoint a = rightA;
            HullPoint b = leftB;

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

            List<HullPoint> P = new List<HullPoint>();

            HullPoint r = a;


            Console.Write("\n\nThe run is : \n");
            do
            {
                P.Add(r);
                Console.Write(r.X + ", " + r.Y + Environment.NewLine);
                r = r.next;
            } while (r != a);

            Console.WriteLine();

            return P;
        }


        static public List<HullPoint> f(List<HullPoint> S, int l, int r)
        {
            if (r - l + 1 == 1)
            {
                List<HullPoint> A = new List<HullPoint>();
                A.Add(S[l]);
                A[0].next = A[0];
                A[0].prev = A[0];
                return A;
            }

            if (r - l + 1 == 2)
            {
                List<HullPoint> A = new List<HullPoint>();

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

            List<HullPoint> S1 = f(S, l, mid);
            List<HullPoint> S2 = f(S, mid + 1, r);

            return combine(S1, S2);
        }


        static void init(List<Point> S)
        {
            S.Sort(delegate(Point a, Point b) { return a.X.CompareTo(b.X); });

            printS(S);

            List<HullPoint> SS = new List<HullPoint>();

            for (int i = 0; i < S.Count; i++)
            {
                SS.Add(new HullPoint(S[i].X, S[i].Y, i));
            }

             f(SS, 0, S.Count - 1);

        }



        static void Main(string[] args)
        {
            List<Point> S = new List<Point>();
            int pointLimit = 1000;

            //pointLimit = Convert.ToInt32(Console.ReadLine());
            //for (int i = 0; i < pointLimit; i++)
            //    S.Add((Point)TypeDescriptor.GetConverter(typeof(Point)).ConvertFromString(Console.ReadLine()));          
           
            S = generatePoints(pointLimit, 100000);

            init(S);

           






            Console.Write("\n\nPress any key to exit ...");
            Console.ReadLine();
        }

        static void printS(List<Point> S)
        {
            System.IO.File.WriteAllText("D:/points.txt", " ");
            for (int i = 0; i < S.Count; i++)
            {
                //Console.WriteLine(S[i].X + ", " + S[i].Y);
                System.IO.File.AppendAllText("D:/points.txt", S[i].X + ", " + S[i].Y + Environment.NewLine);

            }
        }
    }
}
