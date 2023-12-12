using Izzy;
using Izzy.DataStructures.HashsetDictionary;
using IzzysGameLibrary.MeshGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IzzysGameLibrary.Geometry
{
    public struct VoronoiDiagram
    {
        public VoronoiCell[] cells { get; private set; }
        public static bool TryMakeVoronoiDiagram(Vector2[] points, out VoronoiDiagram voronoi)
        {
            DirectionComparer directionComparer = new DirectionComparer();
            DelaunayTriangulator triangulator = new DelaunayTriangulator();
            triangulator.Setup(points);
            Triangle2d[] triangles = triangulator.triangles;

            VoronoiCell[] cells = new VoronoiCell[points.Length];

            HashsetDictionary<int, int> cellConnections = new HashsetDictionary<int, int>();

            Dictionary<Vector2, int> cellIndexByVertex = new Dictionary<Vector2, int>();
            for (int i = 0; i < points.Length; i++)
                if (!cellIndexByVertex.TryAdd(points[i], i))
                    throw new ArgumentException("Duplicate cell vectors in points array");

            HashsetDictionary<int, Vector2> circumcentersByCell = new HashsetDictionary<int, Vector2>();
            for (int i = 0; i < points.Length; i++)
                circumcentersByCell.EnsureKey(i);

            for (int i = 0; i < triangles.Length; i++)
            {
                Vector2 circumcenter = triangles[i].circumcenter;
                foreach (Vector2 triangleVertex in triangles[i].Vertices)
                {
                    int iCell = cellIndexByVertex[triangleVertex];
                    circumcentersByCell.Add_CertainOfKey(iCell, circumcenter);
                }
                foreach (Edge2d edge in triangles[i].Edges)
                {
                    cellConnections.Add(cellIndexByVertex[edge.A], cellIndexByVertex[edge.B]);
                    cellConnections.Add(cellIndexByVertex[edge.B], cellIndexByVertex[edge.A]);
                }
            }

            for (int iCell = 0; iCell < points.Length; iCell++)
            {
                directionComparer.SetCenter(points[iCell]);
                Vector2[] circumcenters = circumcentersByCell.Get_CertainOfKey(iCell);
                if (circumcenters.Length == 0)
                {
                    //DynamicLogger.LogWarning($"No circumcenters found for cell at {points[iCell]}");
                    voronoi = new VoronoiDiagram();
                    return false;
                }
                Vector2[] vertices2d = new Vector2[circumcenters.Length + 1];
                vertices2d[0] = points[iCell];
                for (int i = 0; i < circumcenters.Length; i++)
                {
                    vertices2d[i + 1] = circumcenters[i];
                }
                SortedSet<Vector2> clockwiseVertices = new SortedSet<Vector2>(directionComparer);
                for (int i = 1; i < vertices2d.Length; i++)
                {
                    clockwiseVertices.Add(vertices2d[i]);
                }
                clockwiseVertices.CopyTo(vertices2d, 1);

                int[] meshTriangles = new int[circumcenters.Length * 3];
                for (int i = 0; i < circumcenters.Length; i++)
                {
                    int i2 = i * 3;
                    meshTriangles[i2] = 0;
                    meshTriangles[i2 + 1] = i + 1;
                    meshTriangles[i2 + 2] = i + 2;
                }
                meshTriangles[meshTriangles.Length - 1] = 1;
                Vector3[] meshVertices = new Vector3[vertices2d.Length];
                for (int i = 0; i < meshVertices.Length; i++)
                    meshVertices[i] = new Vector3(vertices2d[i].X, vertices2d[i].Y, 0);

                Mesh mesh = new Mesh(meshVertices, meshTriangles);

                int[] connections = cellConnections.Get_CertainOfKey(iCell);
                cells[iCell] = new VoronoiCell(points[iCell], mesh, connections, iCell);
            }
            voronoi = new VoronoiDiagram(cells);
            return true;
        }
        private VoronoiDiagram(VoronoiCell[] cells)
        {
            this.cells = cells;
        }

        public VoronoiCell this[int index]
        {
            get
            {
                return GetCellWithIndex(index);
            }
        }
        public VoronoiCell GetCellWithIndex(int index)
        {
            if (index > cells.Length)
                throw new IndexOutOfRangeException();
            return cells[index];
        }
        public VoronoiCell[] GetConnectedCells(VoronoiCell cell)
        {
            VoronoiCell[] connectedCells = new VoronoiCell[cell.Connections.Length];
            for (int i = 0; i < connectedCells.Length; i++)
            {
                connectedCells[i] = GetCellWithIndex(cell.Connections[i]);
            }
            return connectedCells;
        }

        private class DirectionComparer : IComparer<Vector2>
        {
            Vector2 _center = Vector2.Zero;
            public void SetCenter(Vector2 center) => _center = center;
            public int Compare(Vector2 a, Vector2 b)
            {
                float angleA = Mathfi.Atan2(a.Y - _center.Y, a.X - _center.X);
                float angleB = Mathfi.Atan2(b.Y - _center.Y, b.X - _center.X);
                if (angleA > angleB) return -1;
                else if (angleB > angleA) return 1;
                else
                {
                    if ((a - _center).Length() > (b - _center).Length())
                        return -1;
                    else
                        return 1;
                }
            }
        }
    }
    public struct VoronoiCell
    {
        public Mesh Mesh { get; private set; }
        public Vector2 Position { get; private set; }
        public Triangle2d[] Triangles { get; private set; }
        public int[] Connections { get; private set; }
        public int ID { get; private set; }
        public VoronoiCell(Vector2 position, Mesh mesh, int[] connections, int id)
        {
            this.Mesh = mesh;
            this.Position = position;
            this.Connections = connections;
            this.ID = id;
            Triangles = new Triangle2d[mesh.Triangles.Length / 3];
            for (int i = 0; i < Triangles.Length; i++)
            {
                int t = i * 3;
                Vector2 a = VectorUtils.FlattenToVector2(mesh.Vertices[mesh.Triangles[t]].Position);
                Vector2 b = VectorUtils.FlattenToVector2(mesh.Vertices[mesh.Triangles[t + 1]].Position);
                Vector2 c = VectorUtils.FlattenToVector2(mesh.Vertices[mesh.Triangles[t + 2]].Position);

                Triangles[i] = new Triangle2d(a, b, c);
            }
            
        }
    }
}
