namespace ConvexHullLib
{
    /// <summary>
    /// Represents a half-edge in a half-edge data structure, linking vertices and faces.
    /// This class is essential for representing the connectivity of a mesh, allowing traversal
    /// of the geometry in a structured manner.
    /// </summary>
    public class HalfEdge
    {
        /// <summary>
        /// Gets or sets the origin vertex of this half-edge.
        /// This vertex serves as the starting point for the half-edge.
        /// </summary>
        public Vertex Origin { get; set; } = null!;

        /// <summary>
        /// Gets or sets the face that this half-edge belongs to.
        /// This property establishes which polygonal face the half-edge is part of.
        /// </summary>
        public Face Face { get; set; } = null!;

        /// <summary>
        /// Gets or sets the twin half-edge that points in the opposite direction.
        /// This property enables traversal in both directions within the mesh.
        /// </summary>
        public HalfEdge? Twin { get; set; } = null;

        /// <summary>
        /// Gets or sets the next half-edge in the cyclic order around the face.
        /// This property allows iteration through the half-edges that form the boundary of a face.
        /// </summary>
        public HalfEdge Next { get; set; } = null!;

        /// <summary>
        /// Gets or sets the previous half-edge in the cyclic order around the face.
        /// This property complements the <see cref="Next"/> property, enabling bidirectional traversal.
        /// </summary>
        public HalfEdge Prev { get; set; } = null!;

        /// <summary>
        /// Determines whether the specified vertex is part of this half-edge or its next half-edge.
        /// </summary>
        /// <param name="vertex">The vertex to check for presence.</param>
        /// <returns><c>true</c> if the vertex is the origin or the next origin; otherwise, <c>false</c>.</returns>
        public bool Contains(Vertex vertex)
        {
            return Origin == vertex || Next.Origin == vertex;
        }

        public override string ToString()
        {
            return $"[{Origin.Index} {Next.Origin.Index}]";
        }
    }
}
