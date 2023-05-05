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
	public class CrystalCluster01Addon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new CrystalCluster01AddonDeed();
			}
		}

		[ Constructable ]
		public CrystalCluster01Addon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 8743 );
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 12253 );
			AddComponent( ac, -1, -1, 0 );
			ac = new AddonComponent( 8738 );
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 8770 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 8769 );
			AddComponent( ac, -1, -2, 0 );
			ac = new AddonComponent( 8768 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 8767 );
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 8766 );
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 8765 );
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 8764 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 8736 );
			AddComponent( ac, 0, -2, 0 );

		}

		public CrystalCluster01Addon( Serial serial ) : base( serial )
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

	public class CrystalCluster01AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new CrystalCluster01Addon();
			}
		}

		[Constructable]
		public CrystalCluster01AddonDeed()
		{
			Name = "CrystalCluster01";
		}

		public CrystalCluster01AddonDeed( Serial serial ) : base( serial )
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