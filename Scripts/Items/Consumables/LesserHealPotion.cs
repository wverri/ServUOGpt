using System;

namespace Server.Items
{
    public class LesserHealPotion : BaseHealPotion
    {
		public override string DefaultName
        {
            get { return "Poção de Vida Fraca"; }            
        }
		
        [Constructable]
        public LesserHealPotion()
            : base(PotionEffect.VidaFraca)
        {
        }

        public LesserHealPotion(Serial serial)
            : base(serial)
        {
        }

        public override int MinHeal
        {
            get
            {
                return (Core.AOS ? 6 : 10);
            }
        }
        public override int MaxHeal
        {
            get
            {
                return (Core.AOS ? 8 : 20);
            }
        }
        public override double Delay
        {
            get
            {
                return (Core.AOS ? 3.0 : 10.0);
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
