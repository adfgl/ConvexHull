namespace ConvexHullLib
{
    public class ConvexHull3<T>
    {
        const int NO_INDEX = -1;

        readonly HalfEdgeMesh _mesh;

        public ConvexHull3(IList<T> vertices, Func<T, double> getX, Func<T, double> getY, Func<T, double> getZ)
        {
            _mesh = new HalfEdgeMesh(vertices.Count);
            foreach (T vertex in vertices)
            {
                double x = getX(vertex);
                double y = getY(vertex);
                double z = getZ(vertex);
                _mesh.Add(x, y, z);
            }

            if (_mesh.Vertices.Count < 4)
            {
                throw new InvalidOperationException("At least 4 points are required to build the convex hull.");
            }

            GetFirstTwoPoints(_mesh.Vertices, out int p1, out int p2);
            GetThirdTetrahedronPoint(_mesh.Vertices, p1, p2, out int p3);
            GetForthTetrahedronPoint(_mesh.Vertices, p1, p2, p3, out int p4);


        }

        static void GetFirstTwoPoints(IReadOnlyList<Vertex> vertices, out int p1, out int p2)
        {
            p1 = NO_INDEX;
            p2 = NO_INDEX;

            double minX, minY, minZ, maxX, maxY, maxZ;
            minX = minY = minZ = double.MaxValue;
            maxX = maxY = maxZ = double.MinValue;

            int[] min = new int[3];
            int[] max = new int[3];
            for (int i = 0; i < vertices.Count; i++)
            {
                var (x, y, z) = vertices[i];
                if (x < minX)
                {
                    minX = x;
                    min[0] = i;
                }

                if (y < minY)
                {
                    minY = y;
                    min[1] = i;
                }

                if (z < minZ)
                {
                    minZ = z;
                    min[2] = i;
                }

                if (x > maxX)
                {
                    maxX = x;
                    max[0] = i;
                }

                if (y > maxY)
                {
                    maxY = y;
                    max[1] = i;
                }

                if (z > maxZ)
                {
                    maxZ = z;
                    max[2] = i;
                }
            }

            if (p1 == NO_INDEX || p2 == NO_INDEX)
            {
                throw new InvalidOperationException("LOGIC ERROR: could not build initial tetrahedron.");
            }
        }

        static void GetForthTetrahedronPoint(IReadOnlyList<Vertex> points, int p1, int p2, int p3, out int p4)
        {
            p4 = NO_INDEX;

            Vertex a = points[p1];
            Vertex b = points[p2];
            Vertex c = points[p3];

            double nx, ny, nz, d;
            GeometryHelper.CalculatePlane(a.X, a.Y, a.Z, b.X, b.Y, b.Z, c.X, c.Y, c.Z, out nx, out ny, out nz, out d);

            double maxDistance = double.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                if (i != p1 && i != p2 && i != p3)
                {
                    var (x, y, z) = points[i];
                    double distance = Math.Abs(GeometryHelper.SignedDistanceToPlane(nx, ny, nz, d, x, y, z));
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        p4 = i;
                    }
                }
            }

            if (p4 == NO_INDEX)
            {
                throw new InvalidOperationException("LOGIC ERROR: could not build initial tetrahedron.");
            }
        }

        static void GetThirdTetrahedronPoint(IReadOnlyList<Vertex> points, int p1, int p2, out int p3)
        {
            p3 = NO_INDEX;

            double maxDistance = double.MinValue;
            var (x1, y1, z1) = points[p1];
            var (x2, y2, z2) = points[p2];
            for (int i = 0; i < points.Count; i++)
            {
                if (i != p1 && i != p2)
                {
                    var (x, y, z) = points[i];

                    double distance = GeometryHelper.SquareDistanceFromPointToLine(x1, y1, z1, x2, y2, z2, x, y, z);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        p3 = i;
                    }
                }
            }

            if (p3 == NO_INDEX)
            {
                throw new InvalidOperationException("LOGIC ERROR: could not build initial tetrahedron.");
            }
        }
    }
}
