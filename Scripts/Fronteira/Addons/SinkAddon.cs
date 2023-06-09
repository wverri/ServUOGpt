using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class SinkAddon : BaseAddon
    {
        [Constructable]
        public SinkAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(41653, 1024104), 0, -1, 0);
                    AddComponent(new LocalizedAddonComponent(41652, 1024104), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(41651, 1024104), 0, 1, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(41644, 1024104), -1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(41645, 1024104), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(41646, 1024104), 1, 0, 0);
                    break;
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if ((from.InRange(c.Location, 3)))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && (house.IsOwner(from) || (house.LockDowns.ContainsKey(this) && house.LockDowns[this] == from)))
                {
                    Components.ForEach(x =>
                    {
                        switch (x.ItemID)
                        {
                            case 41645:
                                {
                                    x.ItemID = 41647;
                                    break;
                                }
                            case 41647:
                                {
                                    x.ItemID = 41645;
                                    break;
                                }
                            case 41652:
                                {
                                    x.ItemID = 41654;
                                    break;
                                }
                            case 41654:
                                {
                                    x.ItemID = 41652;
                                    break;
                                }
                        }
                    });
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public SinkAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new SinkAddonDeed(); } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SinkAddonDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber { get { return 1024104;  } } // Sink

        public override BaseAddon Addon { get { return new SinkAddon(_Direction); } }

        private DirectionType _Direction;

        [Constructable]
        public SinkAddonDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public SinkAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
