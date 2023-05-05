namespace Server.Mobiles
{
	[CorpseName( "a Shenron corpse" )]
	public class Shenron : BaseMount
	{
		[Constructable]
		public Shenron() : this( "Shenron" )
		{
			Hue = 2265;
		}

		[Constructable]
		public Shenron( string name ) : base( name, 0x59A , 0x3ECE , AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0xA3;

	//stats are high for my server adjust at will

			SetStr( 210, 300 );
			SetDex( 75, 120 );
			SetInt( 20, 40 );

			SetHits( 400, 470 );
			SetMana( 0 );

			SetDamage( 20, 50 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Cold, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.MagicResist, 15.1, 20.0 );
			SetSkill( SkillName.Tactics, 19.2, 29.0 );
			SetSkill( SkillName.Wrestling, 19.2, 29.0 );

			Fame = 300;
			Karma = 0;

	//might wanna adjust these for your server

			Tamable = true;
			//ControlSlots = 1;
			MinTameSkill = 100.0;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Shenron( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}