using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IzzysGameLibrary.MeshGeneration
{
	public class QuadGenerator : IMeshGenerator
	{
		static Vector3[] quadVerticies = new Vector3[4]
			{
			new Vector3(-0.5f, -0.5f, 0),
			new Vector3(0.5f, -0.5f, 0),
			new Vector3(-0.5f, 0.5f, 0),
			new Vector3(0.5f, 0.5f, 0)
			};
		static int[] quadTriangles = new int[6]
			{
				0,2,1,
				2,3,1
			};
		static Vector2[] quadUV = new Vector2[]
			{
				new Vector2(0,0),
				new Vector2(1,0),
				new Vector2(0,1),
				new Vector2(1,1)
			};
		public Mesh Generate(params string[] parameters)
		{
			return new Mesh(quadVerticies, quadTriangles, quadUV);
		}
	}
}