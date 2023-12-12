using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace IzzysGameLibrary.Geometry
{
    public struct Edge2d
    {
        public Vector2 A { get; private set; }
        public Vector2 B { get; private set; }
        public int _debugHash { get => GetHashCode(); }
        public Vector2 Midpoint => new Vector2((A.X + B.X) / 2, (A.Y + B.Y) / 2);
        public Edge2d PerpendicularBisector
        {
            get
            {
                Vector2 midpoint = Midpoint;

                Vector2 normalizedA = A - midpoint;
                Vector2 normalizedB = B - midpoint;

                Vector2 bisectorA = new Vector2
                    (
                    normalizedA.Y,
                    -normalizedA.X
                    );
                Vector2 bisectorB = new Vector2
                    (
                    normalizedB.Y,
                    -normalizedB.X
                    );

                return new Edge2d(bisectorA + midpoint, bisectorB + midpoint);
            }
        }
        public Edge2d(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }
        public static bool operator ==(Edge2d a, Edge2d b) => a.GetHashCode() == b.GetHashCode();
        public static bool operator !=(Edge2d a, Edge2d b) => a.GetHashCode() != b.GetHashCode();
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return GetHashCode() == obj.GetHashCode() && obj is Edge2d;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return A.GetHashCode() + B.GetHashCode();
            }
        }
        public override string ToString()
        {
            return $"[{A},{B}]";
        }
    }
}
