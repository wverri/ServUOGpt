using System;

namespace Server.Items
{
    public class MagesRuneBlade : RuneBlade
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MagesRuneBlade()
        {
            Attributes.Resistence = 1;
        }

        public MagesRuneBlade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073538;
            }
        }// mage's rune blade
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}