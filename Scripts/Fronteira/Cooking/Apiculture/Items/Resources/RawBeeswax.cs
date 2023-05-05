using System;

namespace Server.Items
{
	public class RawBeeswax : Item
	{
		[Constructable]
		public RawBeeswax() : this( 1 )
		{
		}

		[Constructable]
		public RawBeeswax( int amount ) : base( 0x1422 )
		{
            Name = "Cera de Abelha Crua";
			Weight = 1.0;
			Stackable = true;
			Amount = amount;
			Hue = 1126;
		}

		public RawBeeswax( Serial serial ) : base( serial )
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Talvez voce possa usar isto para transformar em cera de abelha");
            base.OnDoubleClick(from);
        }

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
