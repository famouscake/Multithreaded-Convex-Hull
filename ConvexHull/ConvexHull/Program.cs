using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ConvexHull;
using System.Diagnostics;

namespace ConverHull
{

    class Program
    {
        static private List<PointF> generatePoints(int pointLimit, int maxValue)
        {
            List<PointF> U = new List<PointF>();

            Random r = new Random();

            StringBuilder a = new StringBuilder();

            for (int i = 0; i < pointLimit; i++)
            {

                U.Add(new PointF(Convert.ToSingle(r.NextDouble()*maxValue), Convert.ToSingle(r.NextDouble()*maxValue)));

                a.Append(U[i].X.ToString());
                a.Append(", ");
                a.Append(U[i].Y.ToString());
                a.Append(Environment.NewLine);
            }

            System.IO.File.WriteAllText(@"D:\output.txt", a.ToString());

            return U;
        }



        static void Main(string[] args)
        {
            ConvexHullAlgorithmMultithread Charlie;
            List<PointF> S = new List<PointF>();
            int pointLimit = 1000000;
            Stopwatch stopwatch = new Stopwatch();

            //pointLimit = Convert.ToInt32(Console.ReadLine());
            //for (int i = 0; i < pointLimit; i++)
            //    S.Add((Point)TypeDescriptor.GetConverter(typeof(Point)).ConvertFromString(Console.ReadLine()));          

            S = generatePoints(pointLimit, 1000000000);

            Console.WriteLine("The War begins!");
            stopwatch.Start();



            Charlie = new ConvexHullAlgorithmMultithread(S, 1);
            Charlie.run();


            //printS(Charlie.OutputPoints);
            TimeSpan lastTime = stopwatch.Elapsed;
            Console.WriteLine("Time elapsed for single thread : {0}", stopwatch.Elapsed);



            Charlie = new ConvexHullAlgorithmMultithread(S, 2);
            Charlie.run();


            //printS(Charlie.OutputPoints);
            Console.WriteLine("Time elapsed for two threads: {0}", stopwatch.Elapsed - lastTime);



            //List<int> A = new List<int>();

            //int N = 6;

            //for (int i = 1; i <= N ;i++ )
            //{
            //    A.Add(i);
            //}


            //foreach(var x in A.GetRange(0, 2))
            //{
            //    Console.WriteLine(x + " ");
            //}

                Console.Write("\n\nPress any key to exit ...");
            Console.ReadLine();
        }

        static void printS(List<PointF> S)
        {
            Console.WriteLine(Environment.NewLine);
            System.IO.File.WriteAllText("D:/points.txt", " ");
            for (int i = 0; i < S.Count; i++)
            {
                Console.WriteLine(S[i].X + ", " + S[i].Y);
                System.IO.File.AppendAllText("D:/points.txt", S[i].X + ", " + S[i].Y + Environment.NewLine);

            }
        }
    }
}
