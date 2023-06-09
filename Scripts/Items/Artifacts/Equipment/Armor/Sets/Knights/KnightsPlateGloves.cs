using System;
using Server.Items;

namespace Server.Items
{
	public class KnightsPlateGloves : PlateGloves
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1080161; } } // Knight's Gloves

        public override SetItem SetID{ get{ return SetItem.Knights; } }
		public override int Pieces{ get{ return 6; } }
		
		public override int BasePhysicalResistance{ get{ return 7; } }
		public override int BaseFireResistance{ get{ return 7; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 7; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public KnightsPlateGloves() : base()
		{
            this.Hue = 1150;
            this.Weight = 2;

            this.SetAttributes.BonusDex = 10;

            this.SetHue = 1150;
            this.SetPhysicalBonus = 28;
            this.SetFireBonus = 28;
            this.SetColdBonus = 28;
            this.SetPoisonBonus = 28;
            this.SetEnergyBonus = 28;
		}

		public KnightsPlateGloves( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}
