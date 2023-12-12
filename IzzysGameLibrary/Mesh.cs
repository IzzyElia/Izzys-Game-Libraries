using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Izzy.Geometry;
using System;
using IzzysGameLibrary.MeshGeneration;
using IzzysGameLibrary.Geometry;

namespace IzzysGameLibrary
{
    public struct Mesh
    {
        private int[] _triangles;
        public int[] Triangles => _triangles;
        private VertexPositionColor[] _vertices;
        public VertexPositionColor[] Vertices => _vertices;
        private Vector2[] _uv;
        public Vector2[] UV => _uv;
        public Mesh(Vector3[] vertices, int[] triangles, Vector2[] uv)
        {
            if (triangles.Length % 3 != 0) throw new ArgumentException("Triangle index array must be divisible by 3");
            this._triangles = triangles;
            this._vertices = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                this._vertices[i] = new VertexPositionColor(vertices[i], Color.White);
            }
            this._uv = MeshUtils.AutoUV(vertices, _triangles);
            this._uv = uv;
        }
        public Mesh(Vector3[] vertices, int[] triangles, Color[] colors)
        {
            if (triangles.Length % 3 != 0) throw new ArgumentException("Triangle index array must be divisible by 3");
            if (colors.Length != vertices.Length) throw new ArgumentException("Vertex color array must be the length of the vertex array");
            this._triangles = triangles;
            this._vertices = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                this._vertices[i] = new VertexPositionColor(vertices[i], colors[i]);
            }
            this._uv = MeshUtils.AutoUV(vertices, _triangles);
        }
        public Mesh(Vector3[] vertices, int[] triangles)
        {
            if (triangles.Length % 3 != 0) throw new ArgumentException("Triangle index array must be divisible by 3");
            this._triangles = triangles;
            this._vertices = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                this._vertices[i] = new VertexPositionColor(vertices[i], Color.White);
            }
            this._uv = MeshUtils.AutoUV(vertices, _triangles);
        }
        public Mesh(Vector3[] vertices, ITriangulator triangulator)
        {
            this._vertices = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Color color = Color.Lerp(Color.IndianRed, Color.CornflowerBlue, (float)i / (float)vertices.Length);
                this._vertices[i] = new VertexPositionColor(vertices[i], color);
            }
            triangulator.Setup(vertices);
            this._triangles = triangulator.GetTriangulation();
            this._uv = MeshUtils.AutoUV(vertices, _triangles);
        }
        public Triangle2d[] GetTrianglesAsFlattenedOntoTheXYPlane ()
        {
            Triangle2d[] triangles = new Triangle2d[_triangles.Length / 3];
            for (int i = 0; i < triangles.Length; i++)
            {
                Vector2 v0 = VectorUtils.FlattenToVector2(_vertices[_triangles[i * 3]].Position);
                Vector2 v1 = VectorUtils.FlattenToVector2(_vertices[_triangles[i * 3 + 1]].Position);
                Vector2 v2 = VectorUtils.FlattenToVector2(_vertices[_triangles[i * 3 + 2]].Position);
                triangles[i] = new Triangle2d(v0, v1, v2);
            }
            return triangles;
        }
    }
}
