/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class RedMapleTree2cAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new RedMapleTree2cAddonDeed();
			}
		}

		[ Constructable ]
		public RedMapleTree2cAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 9341 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 9338 );
			AddComponent( ac, 0, 0, 0 );

		}

		public RedMapleTree2cAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class RedMapleTree2cAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new RedMapleTree2cAddon();
			}
		}

		[Constructable]
		public RedMapleTree2cAddonDeed()
		{
			Name = "RedMapleTree2c";
		}

		public RedMapleTree2cAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}