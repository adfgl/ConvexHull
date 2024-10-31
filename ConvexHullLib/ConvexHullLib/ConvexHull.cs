namespace ConvexHullLib
{
    public class ConvexHull<T>
    {
        const int NO_INDEX = -1;

        readonly HalfEdgeMesh _mesh;
        readonly List<HalfEdge> _horizon = new List<HalfEdge>();

        public ConvexHull(IList<T> vertices, Func<T, double> getX, Func<T, double> getY, Func<T, double> getZ)
        {
            _mesh = BuildTetrahedron(vertices, getX, getY, getZ);
            int p1 = _mesh.Vertices[0].Index;
            int p2 = _mesh.Vertices[1].Index;
            int p3 = _mesh.Vertices[2].Index;
            int p4 = _mesh.Vertices[3].Index;

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                if (i != p1 && i != p2 && i != p3 && i != p4)
                {
                    T vertex = vertices[i];
                    Add(getX(vertex), getY(vertex), getZ(vertex));
                }
            }
        }

        public IReadOnlyHalfEdgeMesh Mesh => _mesh;

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

        public static HalfEdgeMesh BuildTetrahedron(IList<T> points, Func<T, double> getX, Func<T, double> getY, Func<T, double> getZ)
        {
            if (points.Count < 4)
            {
                throw new InvalidOperationException("At least 4 points are required to build tetrahedron.");
            }

            GetFirstTwoPoints(points, getX, getY, getZ, out int p1, out int p2);
            GetThirdTetrahedronPoint(points, getX, getY, getZ, p1, p2, out int p3);
            GetForthTetrahedronPoint(points, getX, getY, getZ, p1, p2, p3, out int p4);

            HalfEdgeMesh mesh = new HalfEdgeMesh(points.Count);

            Vertex v1 = mesh.Add(getX(points[p1]), getY(points[p1]), getZ(points[p1]));
            Vertex v2 = mesh.Add(getX(points[p2]), getY(points[p2]), getZ(points[p2]));
            Vertex v3 = mesh.Add(getX(points[p3]), getY(points[p3]), getZ(points[p3]));
            Vertex v4 = mesh.Add(getX(points[p4]), getY(points[p4]), getZ(points[p4]));

            Plane plane = new Plane(v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z, v3.X, v3.Y, v3.Z);

            // point 4 must be 'behind' the f123 plane
            Face tetraBase;
            if (plane.SignedDistance(v4.X, v4.Y, v4.Z) < 0)
            {
                tetraBase = mesh.Add(v1, v2, v3);
            }
            else
            {
                tetraBase = mesh.Add(v3, v2, v1);
            }

            BuildNewTriangles(mesh, v4, tetraBase.Backward().ToList());
            return mesh;
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

        static double GetComponent(int component, T point, Func<T, double> getX, Func<T, double> getY, Func<T, double> getZ)
        {
            switch (component)
            {
                case 0:
                    return getX(point);
                case 1:
                    return getY(point);
                case 2:
                    return getZ(point);
                default:
                    throw new ArgumentOutOfRangeException(nameof(component));
            }
        }

        static void GetFirstTwoPoints(IList<T> points, Func<T, double> getX, Func<T, double> getY, Func<T, double> getZ, out int p1, out int p2)
        {
            p1 = NO_INDEX;
            p2 = NO_INDEX;

            double minX, minY, minZ, maxX, maxY, maxZ;
            minX = minY = minZ = double.MaxValue;
            maxX = maxY = maxZ = double.MinValue;

            int[] min = new int[3];
            int[] max = new int[3];
            for (int i = 0; i < points.Count; i++)
            {
                T point = points[i];
                double x = getX(point);
                double y = getY(point);
                double z = getZ(point);

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

            double maxDistance = double.MinValue;
            int index = NO_INDEX;
            for (int i = 0; i < 3; i++)
            {
                T minPOint = points[min[i]];
                T maxPoint = points[max[i]];

                double maxComponent = GetComponent(i, points[max[i]], getX, getY, getZ);
                double minComponent = GetComponent(i, points[min[i]], getX, getY, getZ);
                double difference = maxComponent - minComponent;
                if (maxDistance < difference)
                {
                    maxDistance = difference;
                    index = i;
                }
            }

            p1 = min[index];
            p2 = max[index];

            if (p1 == NO_INDEX || p2 == NO_INDEX)
            {
                throw new InvalidOperationException("LOGIC ERROR: could not build initial tetrahedron.");
            }
        }

        public static void GetThirdTetrahedronPoint(IList<T> points, Func<T, double> getX, Func<T, double> getY, Func<T, double> getZ, int p1, int p2, out int p3)
        {
            p3 = NO_INDEX;

            double maxDistance = double.MinValue;

            double x1 = getX(points[p1]);
            double y1 = getY(points[p1]);
            double z1 = getZ(points[p1]);

            double x2 = getX(points[p2]);
            double y2 = getY(points[p2]);
            double z2 = getZ(points[p2]);
            for (int i = 0; i < points.Count; i++)
            {
                if (i != p1 && i != p2)
                {
                    double x = getX(points[i]);
                    double y = getY(points[i]);
                    double z = getZ(points[i]);

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

        public static void GetForthTetrahedronPoint(IList<T> points, Func<T, double> getX, Func<T, double> getY, Func<T, double> getZ, int p1, int p2, int p3, out int p4)
        {
            p4 = NO_INDEX;

            T a = points[p1];
            T b = points[p2];
            T c = points[p3];

            Plane plane = new Plane(
                getX(a), getY(a), getZ(a),
                getX(b), getY(b), getZ(b),
                getX(c), getY(c), getZ(c));

            double maxDistance = double.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                if (i != p1 && i != p2 && i != p3)
                {
                    T point = points[i];
                    double distance = Math.Abs(plane.SignedDistance(getX(point), getY(point), getZ(point)));
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

        public static double SquareDistanceFromPointToLine(
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
    }
}
