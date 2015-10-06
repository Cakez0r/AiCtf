using System;

namespace AiCtf.Sdk
{
    public struct BoundingCircle
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public BoundingCircle(Vector2 center, float radius)
            : this()
        {
            Center = center;
            Radius = radius;
        }

        public bool Intersects(BoundingCircle other)
        {
            return Vector2.Distance(Center, other.Center) < Radius + other.Radius;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return Vector2.Distance(Center, point) < Radius;
        }
    }
}