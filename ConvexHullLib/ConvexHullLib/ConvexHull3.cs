namespace ConvexHullLib
{
    public class ConvexHull3<T>
    {
        const int NO_INDEX = -1;

        readonly HalfEdgeMesh _mesh;
        readonly List<HalfEdge> _horizon = new List<HalfEdge>();

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

            if (vertices.Count < 4)
            {
                throw new InvalidOperationException("At least 4 points are required to build the convex hull.");
            }

            GetFirstTwoPoints(_mesh.Vertices, out int p1, out int p2);
            GetThirdTetrahedronPoint(_mesh.Vertices, p1, p2, out int p3);
            GetForthTetrahedronPoint(_mesh.Vertices, p1, p2, p3, out int p4);

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                if (i != p1 && i != p2 && i != p3 && i != p4)
                {
                    T vertex = vertices[i];
                    double x = getX(vertex);
                    double y = getY(vertex);
                    double z = getZ(vertex);
                    Add(x, y, z);
                }
            }
        }

        public bool Add(double x, double y, double z)
        {
            if (RemoveVisibleFaces(_mesh, x, y, z, 0) == 0)
            {
                RebuildHorizon(_horizon, _mesh);
                BuildNewTriangles(_mesh, _mesh.Add(x, y, z), _horizon);
                return true;
            }
            return false;
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

            Plane plane = new Plane(a.X, a.Y, a.Z, b.X, b.Y, b.Z, c.X, c.Y, c.Z);

            double maxDistance = double.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                if (i != p1 && i != p2 && i != p3)
                {
                    var (x, y, z) = points[i];
                    double distance = Math.Abs(plane.SignedDistance(x, y, z));
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

        static double SquareDistanceFromPointToLine(
            double ax, double ay, double az,
            double bx, double by, double bz,
            double x, double y, double z)
        {
            // Direction vector of the line (from A to B)
            double ABx = bx - ax;
            double ABy = by - ay;
            double ABz = bz - az;

            // Vector from A to P
            double APx = x - ax;
            double APy = y - ay;
            double APz = z - az;

            // Calculate the dot products
            double AB_AB = ABx * ABx + ABy * ABy + ABz * ABz; // ||AB||^2
            double AP_AB = APx * ABx + APy * ABy + APz * ABz; // AP · AB

            // Projection of AP onto AB
            double projFactor = AP_AB / AB_AB;
            double projX = projFactor * ABx;
            double projY = projFactor * ABy;
            double projZ = projFactor * ABz;

            // Distance vector from P to the projection of P onto the line
            double Dx = APx - projX;
            double Dy = APy - projY;
            double Dz = APz - projZ;

            // Calculate the distance
            return Dx * Dx + Dy * Dy + Dz * Dz;
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

                    double distance = SquareDistanceFromPointToLine(x1, y1, z1, x2, y2, z2, x, y, z);
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

        static void SetTwin(HalfEdge edge, HalfEdge twin)
        {
            edge.Twin = twin;
            twin.Twin = edge;
        }

        static void BuildNewTriangles(HalfEdgeMesh mesh, Vertex vertex, IList<HalfEdge> horizon)
        {
            Face firstFace = null!;
            Face lastFace = null!;
            for (int i = 0; i < horizon.Count; i++)
            {
                HalfEdge edge = horizon[i];
                Face face = mesh.Add(edge.Next.Origin, edge.Origin, vertex);

                SetTwin(face.Edge, edge);
                if (lastFace != null)
                {
                    SetTwin(face.Edge.Prev, lastFace.Edge.Next);
                }
                else
                {
                    firstFace = face;
                }
                lastFace = face;
            }
            SetTwin(firstFace.Edge.Prev, lastFace.Edge.Next);
        }

        static HalfEdge GetNextHorizonEdge(HalfEdge he)
        {
            HalfEdge current = he.Prev;
            while (true)
            {
                if (current.Twin is null)
                {
                    return current;
                }
                current = current.Twin.Prev;
            }
        }

        static void RebuildHorizon(List<HalfEdge> horizon, HalfEdgeMesh mesh)
        {
            horizon.Clear();

            HalfEdge? horizonStart = null;
            foreach (Face face in mesh.Faces)
            {
                if (horizonStart is not null)
                {
                    break;
                }

                foreach (HalfEdge e in face.Forward())
                {
                    if (e.Twin is null)
                    {
                        horizonStart = e;
                        break;
                    }
                }
            }

            if (horizonStart is null)
            {
                throw new Exception("LOGIC ERROR: Horizon start not found.");
            }

            HalfEdge he = horizonStart;
            do
            {
                if (horizon.Count > mesh.Faces.Count * 2)
                {
                    throw new Exception($"Welcome to infinite loop! Tolerance is not sufficient for 'horizon' convergence.");
                }

                horizon.Add(he);
                he = GetNextHorizonEdge(he);
            } while (he != horizonStart);
        }

        static int RemoveVisibleFaces(HalfEdgeMesh mesh, double x, double y, double z, double tolerance)
        {
            int removed = 0;
            for (int i = mesh.Faces.Count - 1; i >= 0; i--)
            {
                Face face = mesh.Faces[i];
                if (face.SignedDistance(x, y, z) > tolerance) // TODO: a bit of a mess here
                {
                    mesh.RemoveFaceAt(i);
                    removed++;
                }
            }
            return removed;
        }
    }
}
