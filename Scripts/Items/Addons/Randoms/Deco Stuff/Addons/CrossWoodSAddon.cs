// Automatically generated by the
// AddonGenerator script by Arya
// Generator edited 10.Mar.07 by Papler
using System;
using Server;
using Server.Items;
namespace Server.Items
{
	public class CrossWoodSAddon : BaseAddon {
		public override BaseAddonDeed Deed{get{return new CrossWoodSAddonDeed();}}
		[ Constructable ]
		public CrossWoodSAddon()
		{
			CrossAddonComponent ac = null;
			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, -2, 0, 35 );

			ac = new CrossAddonComponent( 1864 );
			AddComponent( ac, -2, -2, 0 );

			ac = new CrossAddonComponent( 1862 );
			AddComponent( ac, -2, 2, 0 );

			ac = new CrossAddonComponent( 1852 );
			AddComponent( ac, -2, 1, 0 );

			ac = new CrossAddonComponent( 1852 );
			AddComponent( ac, -2, 0, 0 );

			ac = new CrossAddonComponent( 1852 );
			AddComponent( ac, -2, -1, 0 );

			ac = new CrossAddonComponent( 3223 );
			AddComponent( ac, 0, -1, 5 );

			ac = new CrossAddonComponent( 3223 );
			AddComponent( ac, -1, 0, 5 );

			ac = new CrossAddonComponent( 3223 );
			AddComponent( ac, 1, 0, 5 );

			ac = new CrossAddonComponent( 3223 );
			AddComponent( ac, 0, 1, 5 );

			ac = new CrossAddonComponent( 3203 );
			AddComponent( ac, 1, -1, 5 );

			ac = new CrossAddonComponent( 3203 );
			AddComponent( ac, 1, 1, 5 );

			ac = new CrossAddonComponent( 3203 );
			AddComponent( ac, -1, 1, 5 );

			ac = new CrossAddonComponent( 11516 );
			AddComponent( ac, 0, 1, 5 );

			ac = new CrossAddonComponent( 11516 );
			AddComponent( ac, 0, 1, 15 );

			ac = new CrossAddonComponent( 4845 );
			AddComponent( ac, 2, 1, 20 );

			ac = new CrossAddonComponent( 4845 );
			AddComponent( ac, 0, 1, 20 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 45 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 40 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 35 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, -1, 0, 35 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 1, 0, 35 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 2, 0, 35 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 30 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 25 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 20 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 15 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 10 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 5 );

			ac = new CrossAddonComponent( 1863 );
			AddComponent( ac, 2, -2, 0 );

			ac = new CrossAddonComponent( 1861 );
			AddComponent( ac, 2, 2, 0 );

			ac = new CrossAddonComponent( 1851 );
			AddComponent( ac, 1, -2, 0 );

			ac = new CrossAddonComponent( 1851 );
			AddComponent( ac, 0, -2, 0 );

			ac = new CrossAddonComponent( 1851 );
			AddComponent( ac, -1, -2, 0 );

			ac = new CrossAddonComponent( 1850 );
			AddComponent( ac, 2, 1, 0 );

			ac = new CrossAddonComponent( 1850 );
			AddComponent( ac, 2, 0, 0 );

			ac = new CrossAddonComponent( 1850 );
			AddComponent( ac, 2, -1, 0 );

			ac = new CrossAddonComponent( 1849 );
			AddComponent( ac, 1, 2, 0 );

			ac = new CrossAddonComponent( 1849 );
			AddComponent( ac, 0, 2, 0 );

			ac = new CrossAddonComponent( 1849 );
			AddComponent( ac, -1, 2, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 1, 1, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 1, 0, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 1, -1, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 1, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, 0, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, 0, -1, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, -1, 1, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, -1, 0, 0 );

			ac = new CrossAddonComponent( 1848 );
			AddComponent( ac, -1, -1, 0 );


		}
		public CrossWoodSAddon( Serial serial ) : base( serial ){}
		public override void Serialize( GenericWriter writer ){base.Serialize( writer );writer.Write( 0 );}
		public override void Deserialize( GenericReader reader ){base.Deserialize( reader );reader.ReadInt();}
	}

	public class CrossWoodSAddonDeed : BaseAddonDeed {
		public override BaseAddon Addon{get{return new CrossWoodSAddon();}}
		[Constructable]
		public CrossWoodSAddonDeed(){Name = "CrossWoodS";}
		public CrossWoodSAddonDeed( Serial serial ) : base( serial ){}
		public override void Serialize( GenericWriter writer ){	base.Serialize( writer );writer.Write( 0 );}
		public override void	Deserialize( GenericReader reader )	{base.Deserialize( reader );reader.ReadInt();}
	}
}