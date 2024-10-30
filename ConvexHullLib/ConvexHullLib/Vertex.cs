namespace ConvexHullLib
{
    /// <summary>
    /// Represents a vertex in 3D space, defined by its coordinates and an index.
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

        public override string ToString()
        {
            return $"[{Index}] {X} {Y} {Z}";
        }
    }
}
