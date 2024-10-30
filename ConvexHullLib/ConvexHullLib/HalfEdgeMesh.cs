namespace ConvexHullLib
{
    public class HalfEdgeMesh
    {
        readonly List<Vertex> _vertices;
        readonly List<Face> _faces;
        readonly List<HalfEdge> _edges;

        public HalfEdgeMesh(int verticesCapacity = 4)
        {
            _vertices = new List<Vertex>(verticesCapacity);

            int estimatedFaceCount = verticesCapacity / 2;
            int estimatedEdgeCount = 3 * estimatedFaceCount / 2; 

            _faces = new List<Face>(estimatedFaceCount / 2);
            _edges = new List<HalfEdge>(estimatedEdgeCount);
        }

        public HalfEdgeMesh TrimExcess()
        {
            _vertices.TrimExcess();
            _faces.TrimExcess();
            _edges.TrimExcess();
            return this;
        }

        public IReadOnlyList<Vertex> Vertices => _vertices;
        public IReadOnlyList<Face> Faces => _faces;
        public IReadOnlyList<HalfEdge> Edges => _edges;

        public Vertex Add(double x, double y, double z)
        {
            Vertex vertex = new Vertex(_vertices.Count, x, y, z);
            _vertices.Add(vertex);
            return vertex;
        }

        public Face Add(Vertex a, Vertex b, Vertex c, bool searchTwins = false)
        {
            Face face = new Face(_faces.Count);

            HalfEdge ab = new HalfEdge(_edges.Count, a) { Face = face };
            HalfEdge bc = new HalfEdge(_edges.Count + 1, b) { Face = face };
            HalfEdge ca = new HalfEdge(_edges.Count + 2, c) { Face = face };

            face.Edge = ab;

            ab.Next = bc; ab.Prev = ca;
            bc.Next = ca; bc.Prev = ab;
            ca.Next = ab; ca.Prev = bc;

            if (searchTwins)
            {
                FindTwin(ab);
                FindTwin(bc);
                FindTwin(ca);
            }

            _faces.Add(face);
            _edges.Add(ab);
            _edges.Add(bc);
            _edges.Add(ca);
            return face;
        }

        public void RemoveFaceAt(int index)
        {
            List<int> edgesToRemove = new List<int>();
            Face face = _faces[index];
            foreach (HalfEdge he in face.Forward())
            {
                he.Face = null;
                if (he.Twin is not null)
                {
                    he.Twin.Twin = null;
                    he.Twin = null;
                }

                if (he.Origin.Edge == he)
                {
                    he.Origin.Edge = null;
                }

                InsertDescending(edgesToRemove, he.Index);
            }

            _faces.RemoveAt(index);
            foreach (int heIndex in edgesToRemove)
            {
                _edges.RemoveAt(heIndex);
            }
        }

        static void InsertDescending(List<int> list, int item)
        {
            bool inserted = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < item)
                {
                    list.Insert(i, item);
                    inserted = true;
                    break;
                }
            }

            if (false == inserted)
            {
                list.Add(item);
            }
        }

        bool FindTwin(HalfEdge edge)
        {
            Vertex start = edge.Origin;
            Vertex end = edge.Next.Origin;

            Vertex heStart, heEnd;
            foreach (Face f in _faces)
            {
                foreach (HalfEdge he in f.Forward())
                {
                    heStart = he.Origin;
                    heEnd = he.Next.Origin;
                    if (heStart == end && heEnd == start)
                    {
                        edge.Twin = he;
                        he.Twin = edge;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
