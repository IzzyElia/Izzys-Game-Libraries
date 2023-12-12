using Microsoft.Xna.Framework;

namespace IzzysGameLibrary.MeshGeneration
{
	class HexGenerator : IMeshGenerator
	{
		static float outerRadiusConstant = 0.5f;
		static float innerRadiusConstant = outerRadiusConstant * 0.866025404f;
		static Vector3[] hexVerticies = new Vector3[7]
		{
            Vector3.Zero,
			new Vector3(0f, outerRadiusConstant, 0f),
			new Vector3(outerRadiusConstant, outerRadiusConstant * 0.5f, 0f),
			new Vector3(outerRadiusConstant, outerRadiusConstant * -0.5f, 0f),
			new Vector3(0f, -outerRadiusConstant, 0f),
			new Vector3(-outerRadiusConstant, outerRadiusConstant * -0.5f, 0f),
			new Vector3(-outerRadiusConstant, outerRadiusConstant * 0.5f, 0f)
		};
		public Mesh Generate(params string[] parameters)
		{
			int[] triangles = new int[6 * 3];
			for (int i = 0; i < 6; i++)
			{
                int b = 3 * i;
				triangles[b] = 0;
				triangles[b + 1] = i % 6 + 1;
				triangles[b + 2] = (i + 1) % 6 + 1;
			}

            Mesh mesh = new Mesh(hexVerticies, triangles);
			return mesh;
		}
	}
}