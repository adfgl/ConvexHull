namespace ConvexHullLib
{
    /// <summary>
    /// Represents a face in a mesh, defined by its edges and geometrice plane properties.
    /// </summary>
    public class Face
    {
        double _normalX, _normalY, _normalZ, _distanceToOrigin;

        public Face(int index)
        {
            Index = index;
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
        /// Gets or sets the X component of the face's normal vector.
        /// </summary>
        public double NormalX => _normalX;

        /// <summary>
        /// Gets or sets the Y component of the face's normal vector.
        /// </summary>
        public double NormalY => _normalY;

        /// <summary>
        /// Gets or sets the Z component of the face's normal vector.
        /// </summary>
        public double NormalZ => _normalZ;

        public Face Recalculate()
        {
            var (a, b, c) = (Edge.Prev.Origin, Edge.Origin, Edge.Next.Origin);

            // subtract vectors
            double xBA = b.X - a.X;
            double yBA = b.Y - a.Y;
            double zBA = b.Z - a.Z;

            double xCA = c.X - a.X;
            double yCA = c.Y - a.Y;
            double zCA = c.Z - a.Z;

            // cross product
            double cpx = yBA * zCA - yCA * zBA;
            double cpy = zBA * xCA - zCA * xBA;
            double cpz = xBA * yCA - xCA * yBA;

            // normalize
            double len = Math.Sqrt(cpx * cpx + cpy * cpy + cpz * cpz);
            _normalX = cpx / len;
            _normalY = cpy / len;
            _normalZ = cpz / len;

            // calculate distance to origin
            _distanceToOrigin = _normalX * a.X + _normalY * a.Y + _normalZ * a.Z;
            return this;
        }
    }
}
