namespace AiCtf.Sdk
{
    /// <summary>
    /// A circle collision area
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary>
        /// The center position of the bounding circle
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        /// The radius of the bounding circle
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Initialise a bounding circle
        /// </summary>
        /// <param name="center">The center position of the bounding circle</param>
        /// <param name="radius">The radius of the bounding circle</param>
        public BoundingCircle(Vector2 center, float radius)
            : this()
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Determine whether another bounding circle intersects with this one
        /// </summary>
        /// <param name="other">The other bounding circle to test against this bounding circle</param>
        public bool Intersects(BoundingCircle other)
        {
            return Vector2.Distance(Center, other.Center) < Radius + other.Radius;
        }

        /// <summary>
        /// Determine whether a point lies within this bounding circle
        /// </summary>
        /// <param name="point">The point to test against this bounding circle</param>
        public bool ContainsPoint(Vector2 point)
        {
            return Vector2.Distance(Center, point) < Radius;
        }
    }
}