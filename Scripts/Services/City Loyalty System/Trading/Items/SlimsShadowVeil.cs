using System;
using Server;
using Server.Mobiles;
using Server.Engines.CityLoyalty;

namespace Server.Items
{
    public class SlimsShadowVeil : LeatherMempo
    {
        public override int LabelNumber { get { return 1154906; } } // Slim's Shadow Veil

        [Constructable]
        public SlimsShadowVeil()
        {
            Hue = 1932;
            Attributes.Resistence = 1;
        }

        public SlimsShadowVeil(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}