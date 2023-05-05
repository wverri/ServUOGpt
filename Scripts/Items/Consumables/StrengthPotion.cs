using System;

namespace Server.Items
{
    public class StrengthPotion : BaseStrengthPotion
    {
		public override string DefaultName
        {
            get { return "Poção de Força"; }            
        }
		
        [Constructable]
        public StrengthPotion()
            : base(PotionEffect.Forca)
        {
        }

        public StrengthPotion(Serial serial)
            : base(serial)
        {
        }

        public override int StrOffset
        {
            get
            {
                return 10;
            }
        }
        public override TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromMinutes(3.0);
            }
        }
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
