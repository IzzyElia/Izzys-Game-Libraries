using Izzy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IzzysGameLibrary.Geometry
{
    public static class VectorUtils
    {
        public static Vector2 Rotate(Vector2 vector, float angle)
        {
            return new Vector2(
                x: vector.X * Mathfi.Cos(angle) - vector.Y * Mathfi.Sin(angle),
                y: vector.X * Mathfi.Sin(angle) + vector.Y * Mathfi.Cos(angle)
                );
        }
        public static Vector2 RotateAroundOrigin(Vector2 vector, Vector2 origin, float angle)
        {
            Vector2 normalized = vector - origin;
            return Rotate(normalized, angle) + origin;
        }
        public static Vector2 FlattenToVector2(Vector3 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
        public static Vector2 FlattenToVector2 (Vector4 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
        public static Vector3 FlattenToVector3 (Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

    }
}
