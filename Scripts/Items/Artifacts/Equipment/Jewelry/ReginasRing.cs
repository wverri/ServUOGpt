using System;

namespace Server.Items
{
    public class ReginasRing : SilverRing
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReginasRing()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public ReginasRing(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add("Adornado");
            list.Add("Lindo");
            list.Add("Maravilhoso");
        }

        public override int LabelNumber
        {
            get
            {
                return 1075305;
            }
        }// Regina's Ring
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
