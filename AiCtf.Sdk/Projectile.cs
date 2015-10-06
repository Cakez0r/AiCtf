using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCtf.Sdk
{
    public class Projectile : GameEntity
    {
        public Guid FiredBy { get; internal set; }

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