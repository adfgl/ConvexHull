namespace ConvexHullLib
{
    public class Face
    {
        public Face(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public HalfEdge Edge { get; set; } = null!;

        public double NormalX { get; set; }
        public double NormalY { get; set; }
        public double NormalZ { get; set; }
        public double DistanceToOrigin { get; set; }
    }
}
