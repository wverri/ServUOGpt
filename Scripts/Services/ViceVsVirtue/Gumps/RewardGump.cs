using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using Server.Network;
using Server.Engines.Points;
using System.Collections.Generic;
using Server.Ziden.RecompensaLogin;

namespace Server.Engines.VvV
{
    public class VvVRewardGump : BaseRewardGump
    {
        public VvVRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, VvVRewards.Rewards, "Guerra Infinita")
        {
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.ViceVsVirtue.GetPoints(m);
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            Item item;

            if (citem.Type == typeof(VvVPotionKeg))
            {
                PotionType type;

                switch (index)
                {
                    default:
                    case 0: type = PotionType.GreaterStamina; break;
                    case 1: type = PotionType.Supernova; break;
                    case 2: type = PotionType.StatLossRemoval; break;
                    case 3: type = PotionType.AntiParalysis; break;
                }

                item = new VvVPotionKeg(type);
            }
            else if (citem.Type == typeof(VvVSteedStatuette))
            {
                SteedType type = index == 5 || index == 6 ? SteedType.WarHorse : SteedType.Ostard;

                item = new VvVSteedStatuette(type, citem.Hue);
            }
            else if (citem.Type == typeof(VvVTrapKit))
            {
                VvVTrapType type;

                switch (index - 11)
                {
                    default:
                    case 0: type = VvVTrapType.Poison; break;
                    case 1: type = VvVTrapType.Cold; break;
                    case 2: type = VvVTrapType.Energy; break;
                    case 3: type = VvVTrapType.Blade; break;
                    case 4: type = VvVTrapType.Explosion; break;
                }
                item = new VvVTrapKit(type);
            }
            else if (citem.Type == typeof(VvVRobe) || citem.Type == typeof(VvVHairDye))
            {
                item = Activator.CreateInstance(citem.Type, citem.Hue) as Item;
            }
            else if (citem.Type == typeof(ScrollOfTranscendence))
            {
                item = ScrollOfTranscendence.CreateRandom(10, 10);
            }
            else
                item = Activator.CreateInstance(citem.Type) as Item;

            if (item != null)
            {
                VvVRewards.OnRewardItemCreated(User, item);

                if (User.Backpack == null || !User.Backpack.TryDropItem(User, item, false))
                {
                    User.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                    item.Delete();
                }
                else
                {
                    if (User.AccessLevel <= AccessLevel.VIP)
                        PointsSystem.ViceVsVirtue.DeductPoints(User, citem.Points);

                    User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                    User.PlaySound(0x5A7);
                }
            }
        }
    }

    public class ArenaRewardsGump : BaseRewardGump
    {
        public ArenaRewardsGump(Mobile owner, PlayerMobile user)
            : base(owner, user, RecompensasArena.Rewards, "Arena PvP")
        {
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.PontosArena.GetPoints(m);
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            PointsSystem.PontosArena.DeductPoints(User, citem.Points);
            User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
            User.PlaySound(0x5A7);
        }
    }
}
