using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Items.Crops
{
	public class WheatSeedling : HerdingBaseCrop
	{
		private static Mobile m_sower;
		public Timer thisTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Sower
        { 
            get
            { 
                return m_sower;
            } 
            set
            { 
                m_sower = value;
            }
        }

		[Constructable]
		public WheatSeedling( Mobile sower ) : base( Utility.RandomList ( 0xDAE, 0xDAF ) )
		{
            Name = "Wheat Seedling";
			Movable = false;
			m_sower = sower;
			init( this );
		}

		public static void init( WheatSeedling plant )
		{
			plant.thisTimer = new CropHelper.GrowTimer( plant, typeof(WheatCrop), plant.Sower );
			plant.thisTimer.Start();
		}

		public override void OnDoubleClick( Mobile from )
		{
            if (from.Mounted && !CropHelper.CanWorkMounted)
            {
                from.SendMessage("Esta planta eh muito pequena para ser colhida montado.");
                return;
            }
            else
            {
                from.SendMessage("Esta plantacao ainda e muito jovem.");
            }
		}
		public WheatSeedling( Serial serial ) : base( serial ) 
        {
        }

		public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
            writer.Write( m_sower );
        }

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_sower = reader.ReadMobile();
			init( this );
		}
	}
}
