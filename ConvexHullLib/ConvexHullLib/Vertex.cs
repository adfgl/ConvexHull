namespace ConvexHullLib
{
    public class Vertex
    {
        public Vertex(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
