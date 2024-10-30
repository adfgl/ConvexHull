namespace ConvexHullLib
{
    /// <summary>
    /// Represents a half-edge in a half-edge data structure, linking vertices and faces.
    /// </summary>
    public class HalfEdge
    {
        /// <summary>
        /// Gets or sets the origin vertex of this half-edge.
        /// </summary>
        public Vertex Origin { get; set; } = null!;

        /// <summary>
        /// Gets or sets the face that this half-edge belongs to.
        /// </summary>
        public Face Face { get; set; } = null!;

        /// <summary>
        /// Gets or sets the twin half-edge that points in the opposite direction.
        /// </summary>
        public HalfEdge? Twin { get; set; } = null;

        /// <summary>
        /// Gets or sets the next half-edge in the cyclic order around the face.
        /// </summary>
        public HalfEdge Next { get; set; } = null!;

        /// <summary>
        /// Gets or sets the previous half-edge in the cyclic order around the face.
        /// </summary>
        public HalfEdge Prev { get; set; } = null!;
    }
}
