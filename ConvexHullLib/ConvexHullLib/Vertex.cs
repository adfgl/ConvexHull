namespace ConvexHullLib
{
    /// <summary>
    /// Represents a vertex in 3D space, defined by its coordinates, an index, and its associated half-edge.
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class with the specified index.
        /// </summary>
        /// <param name="index">The index of the vertex.</param>
        public Vertex(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class with the specified index and coordinates.
        /// </summary>
        /// <param name="index">The index of the vertex.</param>
        /// <param name="x">The X coordinate of the vertex.</param>
        /// <param name="y">The Y coordinate of the vertex.</param>
        /// <param name="z">The Z coordinate of the vertex.</param>
        public Vertex(int index, double x, double y, double z) : this(index)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets the index of the vertex.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets or sets the half-edge that originates from this vertex.
        /// </summary>
        public HalfEdge Edge { get; set; } = null!;

        /// <summary>
        /// Gets or sets the X coordinate of the vertex.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the vertex.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the Z coordinate of the vertex.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Returns an enumerable collection of half-edges around this vertex in clockwise order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{HalfEdge}"/> representing the half-edges connected to this vertex in clockwise order.</returns>
        public IEnumerable<HalfEdge> AroundClockwise()
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
        /// Returns an enumerable collection of half-edges around this vertex in counterclockwise order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{HalfEdge}"/> representing the half-edges connected to this vertex in counterclockwise order.</returns>
        public IEnumerable<HalfEdge> AroundCounterclockwise()
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
            return $"[{Index}] {X} {Y} {Z}";
        }
    }
}
