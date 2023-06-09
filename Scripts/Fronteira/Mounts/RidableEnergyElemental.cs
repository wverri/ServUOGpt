using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a energy elemental corpse" )]
	public class RidableEnergyElemental : BaseMount
	{
		[Constructable]
		public RidableEnergyElemental() : this( "a ridable energy elemental" )
		{
		}

        [Constructable]
        public RidableEnergyElemental(string name)
            : base(name, 0xD, 0x3EA6, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 655;
            Hue = 1378;

            SetStr(415, 425);
            SetDex(190, 215);
            SetInt(315, 390);

            SetHits(433, 456);

            SetDamage(17, 25);

            SetDamageType(ResistanceType.Energy, 100);

            SetResistance(ResistanceType.Physical, 50, 70);
            SetResistance(ResistanceType.Fire, 20, 40);
            SetResistance(ResistanceType.Cold, 20, 40);
            SetResistance(ResistanceType.Poison, 20, 40);
            SetResistance(ResistanceType.Energy, 100);

            SetSkill(SkillName.EvalInt, 99.1, 100.0);
            SetSkill(SkillName.Magery, 99.1, 100.0);
            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 18000;
            Karma = -18000;

            VirtualArmor = 64;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 97.1;
        }

            //int totalstats = this.Str + this.Dex + this.Int + this.HitsMax + this.StamMax + this.ManaMax + this.PhysicalResistance + this.FireResistance + this.ColdResistance + this.EnergyResistance + this.PoisonResistance + this.DamageMin + this.DamageMax + this.VirtualArmor;
            //int nextlevel = totalstats * 10;

        //    this.NextLevel = nextlevel;
        //}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.LV4 );
			AddLoot( LootPack.LowScrolls );
			AddLoot( LootPack.Potions );
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public RidableEnergyElemental( Serial serial ) : base( serial )
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