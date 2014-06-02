using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ConvexHull;

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
            List<PointF> S = new List<PointF>();
            int pointLimit = 100;

            //pointLimit = Convert.ToInt32(Console.ReadLine());
            //for (int i = 0; i < pointLimit; i++)
            //    S.Add((Point)TypeDescriptor.GetConverter(typeof(Point)).ConvertFromString(Console.ReadLine()));          

            S = generatePoints(pointLimit, 1000000);


            ConvexHullAlgorithmMultithread Charlie = new ConvexHullAlgorithmMultithread(S);

            Charlie.Compute();

            

            S = Charlie.OutputPoints;

            printS(S);


            Console.Write("\n\nPress any key to exit ...");
            Console.ReadLine();
        }

        static void printS(List<PointF> S)
        {
            System.IO.File.WriteAllText("D:/points.txt", " ");
            for (int i = 0; i < S.Count; i++)
            {
                Console.WriteLine(S[i].X + ", " + S[i].Y);
                System.IO.File.AppendAllText("D:/points.txt", S[i].X + ", " + S[i].Y + Environment.NewLine);

            }
        }
    }
}
