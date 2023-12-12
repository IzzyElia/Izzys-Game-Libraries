namespace IzzysGameLibrary.MeshGeneration
{
    public interface IMeshGenerator
    {
        public Mesh Generate(params string[] parameters);
    }
}