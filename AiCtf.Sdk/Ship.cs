namespace AiCtf.Sdk
{
    public class Ship : GameEntity
    {
        public string Label { get; set; }
        public float Thrust { get; set; }
        public float Torque { get; set; }
        public bool IsFiring { get; set; }

        public Ship Clone()
        {
            return new Ship()
            {
                AngularVelocity = AngularVelocity,
                Bounds = Bounds,
                Id = Id,
                Label = Label,
                Owner = Owner,
                Position = Position,
                Rotation = Rotation,
                Thrust = Thrust,
                Torque = Torque,
                Velocity = Velocity
            };
        }

        public void SetLabel(string label)
        {
            Label = label ?? string.Empty;
            if (Label.Length > 128)
            {
                Label = Label.Substring(0, 128);
            }
        }

        public void SetThrust(float thrust)
        {
            Thrust = MathHelper.Clamp(thrust, -1f, 1f);
        }

        public void SetTorque(float torque)
        {
            Torque = MathHelper.Clamp(torque, -1f, 1f);
        }

        public void Fire()
        {
            IsFiring = true;
        }

        public void StopFiring()
        {
            IsFiring = false;
        }
    }
}