namespace ConvexHullLib
{
    /// <summary>
    /// Represents a face in a mesh, defined by its edges and geometrice plane properties.
    /// </summary>
    public class Face
    {
        double _normalX, _normalY, _normalZ;
        double _distanceToOrigin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class with the specified index.
        /// </summary>
        /// <param name="index">The index of the face.</param>
        public Face(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class with the specified index and half-edge.
        /// </summary>
        /// <param name="index">The index of the face.</param>
        /// <param name="edge">The half-edge that represents one of the edges of this face.</param>
        public Face(int index, HalfEdge edge) : this(index)
        {
            Edge = edge;
            RecalculatePlane();
        }

        /// <summary>
        /// Gets the index of the face.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets or sets the half-edge that represents one of the edges of this face.
        /// </summary>
        public HalfEdge Edge { get; set; } = null!;

        /// <summary>
        /// Recalculates the plane of the face based on its vertices and updates the normal vector and distance to the origin.
        /// </summary>
        /// <returns>The updated <see cref="Face"/> instance.</returns>
        public Face RecalculatePlane()
        {
            var (a, b, c) = (Edge.Prev.Origin, Edge.Origin, Edge.Next.Origin);
            GeometryHelper.CalculatePlane(a.X, a.Y, a.Z, b.X, b.Y, b.Z, c.X, c.Y, c.Z, out _normalX, out _normalY, out _normalZ, out _distanceToOrigin);
            return this;
        }

        /// <summary>
        /// Calculates the signed distance from a point to the face's plane.
        /// </summary>
        /// <param name="x">The X coordinate of the point.</param>
        /// <param name="y">The Y coordinate of the point.</param>
        /// <param name="z">The Z coordinate of the point.</param>
        /// <returns>
        /// The signed distance from the point to the face's plane. 
        /// A <b><c>positive</c></b> value indicates the point is in front of the plane, 
        /// a <b><c>negative</c></b> value indicates it is behind the plane, 
        /// and <b><c>zero</c></b> indicates it is directly on the plane.
        /// </returns>
        public double SignedDistance(double x, double y, double z)
        {
            return GeometryHelper.SignedDistanceToPlane(_normalX, _normalY, _normalZ, _distanceToOrigin, x, y, z);
        }

        /// <summary>
        /// Determines whether this face contains the specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex to check.</param>
        /// <returns><c>true</c> if the face contains the vertex; otherwise, <c>false</c>.</returns>
        public bool Contains(Vertex vertex)
        {
            return Edge.Origin == vertex || Edge.Next.Origin == vertex || Edge.Next.Next.Origin == vertex;
        }

        /// <summary>
        /// Determines whether this face contains the specified half-edge.
        /// </summary>
        /// <param name="edge">The half-edge to check.</param>
        /// <returns><c>true</c> if the face contains the half-edge; otherwise, <c>false</c>.</returns>
        public bool Contains(HalfEdge edge)
        {
            return Edge == edge || Edge.Next == edge || Edge.Next.Next == edge;
        }

        /// <summary>
        /// Determines whether this face is a neighbor of the specified face.
        /// </summary>
        /// <param name="face">The face to check for adjacency.</param>
        /// <returns><c>true</c> if this face is a neighbor of the specified face; otherwise, <c>false</c>.</returns>
        public bool IsNeighbor(Face face)
        {
            return Contains(face.Edge) || Contains(face.Edge.Next) || Contains(face.Edge.Next.Next);
        }

        /// <summary>
        /// Iterates through the half-edges in the order they are connected, starting from the edge of this face.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{HalfEdge}"/> representing the half-edges in forward order.</returns>
        public IEnumerable<HalfEdge> Forward()
        {
            HalfEdge he = Edge;
            HalfEdge current = he;
            do
            {
                yield return current;
                current = current.Next;
            } while (current != he);
        }

        /// <summary>
        /// Iterates through the half-edges in the opposite order they are connected, starting from the edge of this face.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{HalfEdge}"/> representing the half-edges in backward order.</returns>
        public IEnumerable<HalfEdge> Backward()
        {
            HalfEdge he = Edge;
            HalfEdge current = he;
            do
            {
                yield return current;
                current = current.Prev;
            } while (current != he);
        }

        public override string ToString()
        {
            var (a, b, c) = (Edge.Prev.Origin, Edge.Origin, Edge.Next.Origin);
            return $"[{Index}] v{a.Index} v{b.Index}) v{c.Index}";
        }
    }
}
