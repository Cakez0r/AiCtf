using System;
using System.Collections.Generic;
using System.Linq;

namespace AiCtf.Sdk
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Flag Flag { get; set; }
        public IList<Ship> Ships { get; set; }
        public IList<Projectile> Projectiles { get; set; }
        public int FlagCaptures { get; set; }
        public int Kills { get; set; }

        public Team()
        {
            Id = Guid.NewGuid();
        }

        public Team Clone()
        {
            return new Team()
            {
                Id = Id,
                Name = Name,
                Flag = Flag.Clone(),
                Ships = Ships.Select(s => s.Clone()).ToList(),
                Projectiles = Projectiles.Select(p => p.Clone()).ToList(),
                FlagCaptures = FlagCaptures,
                Kills = Kills
            };
        }
    }
}