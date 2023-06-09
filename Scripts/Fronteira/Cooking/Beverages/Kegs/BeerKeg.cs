using System;
using System.Collections;
using Server.Multis;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{

public class BeerKeg : BaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x3E; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 33, 36, 109, 112 ); }
		}

		[Constructable]
		public BeerKeg() : base( 0xE7F )
		{
			Name = "tonel de cerveja";
			Weight = 25.0;
		}

		public BeerKeg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
