using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCtf.Sdk
{
    public class Flag : GameEntity
    {
        public Guid? HeldBy { get; set; }

        public Flag Clone()
        {
            return new Flag()
            {
                AngularVelocity = AngularVelocity,
                Bounds = Bounds,
                HeldBy = HeldBy,
                Id = Id,
                Owner = Owner,
                Position = Position,
                Rotation = Rotation,
                Velocity = Velocity
            };
        }
    }
}