using System;

namespace AiCtf.Sdk
{
    public class GameEntity
    {
        public Guid Id { get; set; }
        public Guid Owner { get; set; }
        public BoundingCircle Bounds { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }

        public Vector2 Position
        {
            get { return Bounds.Center; }
            internal set { Bounds = new BoundingCircle(value, Bounds.Radius); }
        }

        public GameEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}