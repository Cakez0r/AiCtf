using System;

namespace AiCtf.Sdk
{
    public class Projectile : GameEntity
    {
        public Guid FiredBy { get; set; }

        public Projectile Clone()
        {
            return new Projectile()
            {
                AngularVelocity = AngularVelocity,
                Bounds = Bounds,
                FiredBy = FiredBy,
                Id = Id,
                Owner = Owner,
                Position = Position,
                Rotation = Rotation,
                Velocity = Velocity
            };
        }
    }
}