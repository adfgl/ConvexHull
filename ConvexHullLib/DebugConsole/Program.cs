using ConvexHullLib;

namespace DebugConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HalfEdgeMesh ds = new HalfEdgeMesh();
            Vertex a = ds.Add(0, 0, 0);
            Vertex b = ds.Add(5, 5, 0);
            Vertex c = ds.Add(10, 0, 0);

            Face face = ds.Add(a, b, c, true);

            ds.RemoveFaceAt(0);
        }

      
    }
}
