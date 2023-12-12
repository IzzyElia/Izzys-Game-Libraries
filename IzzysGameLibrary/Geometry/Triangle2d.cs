using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace IzzysGameLibrary.Geometry
{
    public struct Triangle2d
    {
        public Vector2 A { get; private set; }
        public Vector2 B { get; private set; }
        public Vector2 C { get; private set; }
        public Edge2d[] Edges
        {
            get
            {
                return new Edge2d[3]
                {
                    new Edge2d(A, B),
                    new Edge2d(B, C),
                    new Edge2d(C, A)
                };
            }
        }
        public Vector2[] Vertices
        {
            get
            {
                return new Vector2[3]
                {
                    A,
                    B,
                    C
                };
            }
        }
        public Vector2 circumcenter { get; private set; }
        public float circumradius { get; private set; }
        public TriangleDirection direction
        {
            get
            {
                float determinant = (A.X - C.X) * (B.Y - C.Y) - (B.X - C.X) * (A.Y - C.Y);
                if (determinant > 0)
                    return TriangleDirection.Clockwise;
                else if (determinant < 0)
                    return TriangleDirection.CounterClockwise;
                else
                    return TriangleDirection.Line;
            }
        }
        public Triangle2d Flipped
        {
            get
            {
                return new Triangle2d(C, B, A);
            }
        }
        public Triangle2d(Vector2 a, Vector2 b, Vector2 c)
        {
            A = a;
            B = b;
            C = c;
            circumcenter = CalculateCircumcenter(a, b, c);
            circumradius = Vector2.Distance(A, circumcenter);
        }
        public Triangle2d(Vector2 a, int aRef, Vector2 b, int bRef, Vector2 c, int cRef)
        {
            A = a;
            B = b;
            C = c;
            circumcenter = CalculateCircumcenter(a, b, c);
            circumradius = Vector2.Distance(A, circumcenter);
        }
        static Vector2 CalculateCircumcenter(Vector2 A, Vector2 B, Vector2 C)
        {
            float d = (A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) * 2;

            float x = ((Sq(A.X) + Sq(A.Y)) * (B.Y - C.Y) + (Sq(B.X) + Sq(B.Y)) * (C.Y - A.Y) + (Sq(C.X) + Sq(C.Y)) * (A.Y - B.Y)) / d;
            float y = ((Sq(A.X) + Sq(A.Y)) * (C.X - B.X) + (Sq(B.X) + Sq(B.Y)) * (A.X - C.X) + (Sq(C.X) + Sq(C.Y)) * (B.X - A.X)) / d;
            Vector2 circumcenter = new Vector2
                (
                x, y
                );
            return circumcenter;
        }
        public bool ContainsPoint(Vector2 P)
        {
            double s1 = C.Y - A.Y;
            double s2 = C.X - A.X;
            double s3 = B.Y - A.Y;
            double s4 = P.Y - A.Y;

            double w1 = (A.X * s1 + s4 * s2 - P.X * s1) / (s3 * s2 - (B.X - A.X) * s1);
            double w2 = (s4 - w1 * s3) / s1;
            return w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;
        }

        static float Sq(float x) => x * x;
        public override string ToString()
        {
            return $"[{A}, {B}, {C}]";
        }
        public static bool operator ==(Triangle2d a, Triangle2d b) => a.GetHashCode() == b.GetHashCode();
        public static bool operator !=(Triangle2d a, Triangle2d b) => a.GetHashCode() != b.GetHashCode();
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return GetHashCode() == obj.GetHashCode() && obj is Triangle2d;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return A.GetHashCode() + B.GetHashCode() + C.GetHashCode();
            }
        }
    }
    public enum TriangleDirection
    {
        Clockwise,
        CounterClockwise,
        /// <summary>
        /// All 3 vertices are in a line
        /// </summary>
        Line
    }
}
