using Microsoft.Xna.Framework;

namespace IzzysGameLibrary.MeshGeneration
{
    public interface ITriangulator
    {
        public void Setup(Vector3[] vertices);
        public int[] GetTriangulation();
    }
}
