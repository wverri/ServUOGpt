using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Items.Crops
{
    public class CottonSeed : HerdingBaseCrop
    {
        public override bool CanGrowGarden { get { return true; } }

        [Constructable]
        public CottonSeed() : this(1) { }

        [Constructable]
        public CottonSeed(int amount)
            : base(0xF27)
        {
            Stackable = true;
            Weight = .1;
            Hue = 0x5E2;
            Movable = true;
            Amount = amount;
            Name = "Cotton Seed";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Mounted && !CropHelper.CanWorkMounted)
            {
                from.SendMessage("Voce nao pode plantar estando montado.");
                return;
            }

            Point3D m_pnt = from.Location;
            Map m_map = from.Map;
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010);
                return;
            }
            else if (!CropHelper.CheckCanGrow(this, m_map, m_pnt.X, m_pnt.Y))
            {
                from.SendMessage("Esta semente nao vai crescer aqui. Talvez esta semente precise ser plantada em um jardim em casa, rancho ou fazenda..");
                return;

            }

            ArrayList cropshere = CropHelper.CheckCrop(m_pnt, m_map, 0);
            if (cropshere.Count > 0)
            {
                from.SendMessage("Ja existe uma plantacao aqui.");
                return;
            }

            ArrayList cropsnear = CropHelper.CheckCrop(m_pnt, m_map, 1);
            if ((cropsnear.Count > 2))
            {
                from.SendMessage("Muitas plantacoes perto.");
                return;
            }

            if (this.BumpZ) ++m_pnt.Z;

            if (!from.Mounted) from.Animate(32, 5, 1, true, false, 0);
            {
                from.SendMessage("Voce plantou a semente.");
                this.Consume();
                Item item = new CottonSeedling(from);
                item.Location = m_pnt;
                item.Map = m_map;
            }
        }

        public CottonSeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }

        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }
}
