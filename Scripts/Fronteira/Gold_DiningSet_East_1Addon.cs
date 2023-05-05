
////////////////////////////////////////
//                                    //
//   Generated by CEO's YAAAG - V1.2  //
// (Yet Another Arya Addon Generator) //
//                                    //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Gold_DiningSet_East_1Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {11758, 3, 0, 1}, {9037, 0, 0, 11}, {3210, 0, 0, 9}// 1	2	3	
			, {9431, 0, 0, 7}, {11757, -3, 0, 1}, {11756, 2, -2, 1}// 4	22	23	
			, {11756, 1, -2, 1}, {11756, 0, -2, 1}, {11756, -1, -2, 0}// 24	25	26	
			, {11756, -2, -2, 1}, {11755, 2, 2, 1}, {11755, 1, 2, 1}// 27	28	29	
			, {11755, 0, 2, 1}, {11755, -1, 2, 1}, {11755, -2, 2, 1}// 30	31	32	
			, {2458, 1, 0, 14}// 36	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new Gold_DiningSet_East_1AddonDeed();
			}
		}

		[ Constructable ]
		public Gold_DiningSet_East_1Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 2845, 1, 0, 11, 0, 0, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 2845, -1, 0, 11, 0, 0, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 4611, -1, 0, 1, 1022, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 4610, 0, -1, 1, 1022, -1, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 4610, -1, -1, 0, 1022, -1, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 4610, 2, -1, 1, 1022, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 4610, 1, -1, 1, 1022, -1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 4610, -2, -1, 1, 1022, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 4611, 2, 0, 1, 1022, -1, "", 1);// 13
			AddComplexComponent( (BaseAddon) this, 4611, 1, 0, 1, 1022, -1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 4611, 0, 0, 1, 1022, -1, "", 1);// 15
			AddComplexComponent( (BaseAddon) this, 4611, -2, 0, 1, 1022, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 4609, 2, 1, 1, 1022, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 4609, 1, 1, 1, 1022, -1, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 4609, 0, 1, 1, 1022, -1, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 4609, -1, 1, 1, 1022, -1, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 4609, -2, 1, 1, 1022, -1, "", 1);// 21
			AddComplexComponent( (BaseAddon) this, 2458, -1, 1, 10, 2721, -1, "", 1);// 33
			AddComplexComponent( (BaseAddon) this, 2458, 2, 0, 11, 2721, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 2458, 2, 0, 14, 2721, -1, "", 1);// 35
			AddComplexComponent( (BaseAddon) this, 2458, -1, 0, 14, 2721, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 2458, -1, 1, 15, 2721, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 2458, -2, 1, 10, 2721, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 2458, 0, 1, 10, 2721, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 2458, 1, 1, 10, 2721, -1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 2458, 2, 1, 10, 2721, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 2493, -2, 0, 8, 2721, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 2494, 2, -1, 8, 2721, -1, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 2494, 1, -1, 8, 2721, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 2494, 0, -1, 8, 2721, -1, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 2494, -1, -1, 8, 2721, -1, "", 1);// 47
			AddComplexComponent( (BaseAddon) this, 2494, -2, -1, 8, 2721, -1, "", 1);// 48
			AddComplexComponent( (BaseAddon) this, 2517, 2, 0, 8, 2721, -1, "", 1);// 49
			AddComplexComponent( (BaseAddon) this, 2516, -2, 1, 8, 2721, -1, "", 1);// 50
			AddComplexComponent( (BaseAddon) this, 2516, -1, 1, 8, 2721, -1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 2516, 0, 1, 8, 2721, -1, "", 1);// 52
			AddComplexComponent( (BaseAddon) this, 2516, 1, 1, 8, 2721, -1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 2516, 2, 1, 7, 2721, -1, "", 1);// 54
			AddComplexComponent( (BaseAddon) this, 2519, -2, 1, 7, 2037, -1, "Fine China", 1);// 55
			AddComplexComponent( (BaseAddon) this, 2519, -1, 1, 7, 2037, -1, "Fine China", 1);// 56
			AddComplexComponent( (BaseAddon) this, 2519, 0, 1, 7, 1153, -1, "", 1);// 57
			AddComplexComponent( (BaseAddon) this, 2519, 0, 1, 7, 2037, -1, "Fine China", 1);// 58
			AddComplexComponent( (BaseAddon) this, 2519, 1, 1, 7, 2037, -1, "Fine China", 1);// 59
			AddComplexComponent( (BaseAddon) this, 2519, 2, 1, 7, 2037, -1, "Fine China", 1);// 60
			AddComplexComponent( (BaseAddon) this, 2519, 2, 0, 7, 2037, -1, "Fine China", 1);// 61
			AddComplexComponent( (BaseAddon) this, 2519, 2, -1, 7, 2037, -1, "Fine China", 1);// 62
			AddComplexComponent( (BaseAddon) this, 2519, 1, -1, 7, 2037, -1, "Fine China", 1);// 63
			AddComplexComponent( (BaseAddon) this, 2519, 0, -1, 7, 2037, -1, "Fine China", 1);// 64
			AddComplexComponent( (BaseAddon) this, 2519, -1, -1, 7, 2037, -1, "Fine China", 1);// 65
			AddComplexComponent( (BaseAddon) this, 2519, -2, -1, 7, 2037, -1, "Fine China", 1);// 66
			AddComplexComponent( (BaseAddon) this, 2519, -2, 0, 7, 2037, -1, "Fine China", 1);// 67

		}

		public Gold_DiningSet_East_1Addon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
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

	public class Gold_DiningSet_East_1AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new Gold_DiningSet_East_1Addon();
			}
		}

		[Constructable]
		public Gold_DiningSet_East_1AddonDeed()
		{
			Name = "Mesa de Luxo";
		}

		public Gold_DiningSet_East_1AddonDeed( Serial serial ) : base( serial )
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
