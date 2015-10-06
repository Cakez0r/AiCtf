using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCtf.Sdk
{
    public class GameEntity
    {
        public Guid Id { get; internal set; }
        public Guid Owner { get; internal set; }
        public BoundingCircle Bounds { get; internal set; }
        public Vector2 Velocity { get; internal set; }
        public float Rotation { get; internal set; }
        public float AngularVelocity { get; internal set; }

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