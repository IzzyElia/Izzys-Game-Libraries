using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace IzzysGameLibrary
{
    public class MeshRenderer : IDisposable
    {
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private GraphicsDevice _graphicsDevice;
        private Matrix _cachedWorldMatrix = Matrix.Identity;
        private int _cachedPositionHashCode = 0;
        public GraphicsDevice GraphicsDevice
        {
            get => _graphicsDevice;
            set
            {
                _graphicsDevice = value;
                RecalculateBuffers();
            }
        }
        private Mesh _mesh;

        public Mesh Mesh
        {
            get => _mesh;
            set
            {
                _mesh = value;
                RecalculateBuffers();
            }
        }
        public MeshRenderer(Mesh mesh, GraphicsDevice graphicsDevice)
        {
            this._mesh = mesh;
            this._graphicsDevice = graphicsDevice;
            RecalculateBuffers();
        }
        public void Render(Vector3 position, Effect shader, Camera camera)
        {
            int hash = position.GetHashCode();
            if (hash != _cachedPositionHashCode)
            {
                _cachedPositionHashCode = hash;
                _cachedWorldMatrix = Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up);
            }
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            Matrix worldView = Matrix.Multiply(_cachedWorldMatrix, camera.view);
            Matrix worldViewProjection = Matrix.Multiply(worldView, camera.projection);
            shader.Parameters["WorldViewProj"].SetValue(worldViewProjection);
            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GameCore.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, triangles.Length / 3);
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _mesh.Triangles.Length / 3);
            }
        }
        private void RecalculateBuffers()
        {
            _vertexBuffer = new VertexBuffer(_graphicsDevice, VertexPositionColor.VertexDeclaration, _mesh.Vertices.Length, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, _mesh.Triangles.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(_mesh.Vertices);
            _indexBuffer.SetData(_mesh.Triangles);
        }


        // Disposal
        private bool disposedValue;
        protected virtual void Dispose(bool calledManually)
        {
            if (!disposedValue)
            {
                if (calledManually)
                {
                    _indexBuffer.Dispose();
                    _vertexBuffer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        ~MeshRenderer()
        {
            Dispose(calledManually: false);
        }

        public void Dispose()
        {
            Dispose(calledManually: true);
            GC.SuppressFinalize(this);
        }
    }
}
