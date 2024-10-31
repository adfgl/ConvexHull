namespace ConvexHullLib
{
    public interface IReadOnlyHalfEdgeMesh
    {
        public IReadOnlyList<Vertex> Vertices { get; }
        public IReadOnlyList<Face> Faces { get; }
        public IReadOnlyList<HalfEdge> Edges { get; }
    }
}
