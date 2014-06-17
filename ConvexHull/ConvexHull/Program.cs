using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ConvexHull;
using System.Diagnostics;
using System.Threading;
using ConvexHull.Utils;

namespace ConverHull
{

    class Program
    {
        static private List<PointF> generatePoints(int pointLimit, int maxValue)
        {
            List<PointF> U = new List<PointF>();

            Random r = new Random();

            //StringBuilder a = new StringBuilder();

            for (int i = 0; i < pointLimit; i++)
            {

                U.Add(new PointF(Convert.ToSingle(r.NextDouble() * maxValue), Convert.ToSingle(r.NextDouble() * maxValue)));

                // a.Append(U[i].X.ToString());
                //a.Append(", ");
                //a.Append(U[i].Y.ToString());
                //a.Append(Environment.NewLine);
            }

            //System.IO.File.WriteAllText(@"D:\output.txt", a.ToString());

            return U;
        }

        static void Main(string[] args)
        {
            // Mah Nigga
            ConvexHullAlgorithmMultithread Charlie;

            // Hold the generated points
            List<PointF> S = new List<PointF>();
            int pointLimit = 1000000;

            // Holds Empirical data
            Stopwatch stopwatch = new Stopwatch();
            List<TimeSpan> T = new List<TimeSpan>();
            List<KeyValuePair<List<PointF>, string>> Results = new List<KeyValuePair<List<PointF>, string>>();


            Console.Write("Please enter the number ot points to be generated : ");
            pointLimit = Convert.ToInt32(Console.ReadLine());

            //for (int i = 0; i < pointLimit; i++)
            //    S.Add((Point)TypeDescriptor.GetConverter(typeof(Point)).ConvertFromString(Console.ReadLine()));          

            S = generatePoints(pointLimit, 10000000);


            // Step 1 : Sort all the points according to their X coordinate
            S.Sort(delegate(PointF a, PointF b) { return a.X.CompareTo(b.X); });

            // Step 2 : Convert all the points to HullPoints
            List<HullPoint> Input = HullPoint.PointsToHullPoints(S);

            Console.WriteLine("The War begins!");

            stopwatch.Start(); T.Add(stopwatch.Elapsed);



            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;


            // TEST 1 : 1 THREAD
            Charlie = new ConvexHullAlgorithmMultithread(Input, 1);
            Charlie.run();
            T.Add(stopwatch.Elapsed);
            Results.Add(new KeyValuePair<List<PointF>, string>(HullPoint.HullPointsToPoints(Charlie.OutputPoints), "One Thread"));


            // TEST 2 : 2 THREADS
            Charlie = new ConvexHullAlgorithmMultithread(Input, 2);
            Charlie.run();
            T.Add(stopwatch.Elapsed);
            Results.Add(new KeyValuePair<List<PointF>, string>(HullPoint.HullPointsToPoints(Charlie.OutputPoints), "Two Thread"));


            // TEST 3 : 4 THREADS
            Charlie = new ConvexHullAlgorithmMultithread(Input, 4);
            Charlie.run();
            T.Add(stopwatch.Elapsed);
            Results.Add(new KeyValuePair<List<PointF>, string>(HullPoint.HullPointsToPoints(Charlie.OutputPoints), "Four Thread"));

            for (int i = 0; i < Results.Count; i++)
            {
                Console.WriteLine("Test {0} {1} : {2}", i, Results[i].Value, T[i + 1] - T[i]);
               // printS(Results[i].Key);
                Console.WriteLine();
            }


            Console.WriteLine("Total time : " + stopwatch.Elapsed);


            Console.Write("\n\nPress any key to exit ...");
            Console.ReadLine();
        }

        static void printS(List<PointF> S)
        {

            //System.IO.File.WriteAllText("D:/points.txt", " ");
            for (int i = 0; i < S.Count; i++)
            {
                Console.WriteLine(S[i].X + ", " + S[i].Y);
                // System.IO.File.AppendAllText("D:/points.txt", S[i].X + ", " + S[i].Y + Environment.NewLine);

            }
        }
    }
}
