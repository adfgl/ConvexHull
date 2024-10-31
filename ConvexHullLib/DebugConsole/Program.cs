using ConvexHullLib;

namespace DebugConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Point[] points =
            [
                new Point(0, 0, 0),
                new Point(1, 0, 0),
                new Point(0, 1, 0),
                new Point(0, 0, 1)
            ];

            ConvexHull<Point> hull = new ConvexHull<Point>(points, p => p.X, p => p.Y, p => p.Z);
        }

        public class Point
        {
            public Point(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }
    }
}
