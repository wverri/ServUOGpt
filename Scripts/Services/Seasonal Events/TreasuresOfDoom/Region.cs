using System;

using Server;
using Server.Spells;

namespace Server.Regions
{
    public class MonestaryRegion : BaseRegion
    {
        public static void Initialize()
        {
            new MonestaryRegion();
        }

        public MonestaryRegion()
            : base("Doom Monestary", Map.Malas, Region.DefaultPriority, new Rectangle2D(64, 204, 99, 37))
        {
            Register();
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            if (traveller.AccessLevel > AccessLevel.VIP)
            {
                return true;
            }

            return type == TravelCheckType.TeleportTo || type == TravelCheckType.TeleportFrom;
        } 
    }
}
