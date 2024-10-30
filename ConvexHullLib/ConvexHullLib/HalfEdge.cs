namespace ConvexHullLib
{
    public class HalfEdge
    {
        public Vertex Origin { get; set; } = null!;
        public Face Face { get; set; } = null!;
        public HalfEdge? Twin { get; set; } = null;
        public HalfEdge Next { get; set; } = null!;
        public HalfEdge Prev { get; set; } = null!;
    }
}
