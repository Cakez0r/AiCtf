using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCtf.Sdk
{
    public class Team
    {
        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public Flag Flag { get; internal set; }
        public IList<Ship> Ships { get; internal set; }
        public IList<Projectile> Projectiles { get; internal set; }
        public int FlagCaptures { get; internal set; }
        public int Kills { get; internal set; }

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