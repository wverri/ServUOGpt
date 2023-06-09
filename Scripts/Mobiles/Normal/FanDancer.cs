using System;
using System.Collections;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a fan dancer corpse")]
    public class FanDancer : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public FanDancer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "dancarina do leque";
            Body = 247;
            BaseSoundID = 0x372;

            SetStr(101, 175);
            SetDex(201, 255);
            SetInt(35, 50);

            SetHits(200, 350);

            SetDamage(1, 15);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Fire, 10);
            SetDamageType(ResistanceType.Cold, 10);
            SetDamageType(ResistanceType.Poison, 10);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 50, 70);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.MagicResist, 100.1, 110.0);
            SetSkill(SkillName.Tactics, 85.1, 95.0);
            SetSkill(SkillName.Wrestling, 85.1, 95.0);
            SetSkill(SkillName.Anatomy, 85.1, 95.0);
            SetSkill(SkillName.Magery, 40.1, 60.0);
            SetSkill(SkillName.EvalInt, 100.1, 130.0);

            Fame = 9000;
            Karma = -9000;
			
            if (Utility.RandomDouble() < .01)
                PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
				
            AddItem(new Tessen());
			
            if (0.02 >= Utility.RandomDouble())
                PackItem(new OrigamiPaper());

            SetSpecialAbility(SpecialAbility.ManaDrain);
        }

        public FanDancer(Serial serial)
            : base(serial)
        {
        }

        public override bool Uncalmable
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.LV4);
            AddLoot(LootPack.Gems, 2);
        }

        /* TODO: Repel Magic
        * 10% chance of repelling a melee attack (why did they call it repel magic anyway?)
        * Cliloc: 1070844
        * Effect: damage is dealt to the attacker, no damage is taken by the fan dancer
        */
        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            if (0.8 > Utility.RandomDouble() && !attacker.InRange(this, 1))
            {
                /* Fan Throw
                * Effect: - To: "0x57D4F5B" - ItemId: "0x27A3" - ItemIdName: "Tessen" - FromLocation: "(992 299, 24)" - ToLocation: "(992 308, 22)" - Speed: "10" - Duration: "0" - FixedDirection: "False" - Explode: "False" - Hue: "0x0" - Render: "0x0"
                * Damage: 50-65
                */
                Effects.SendPacket(attacker, attacker.Map, new HuedEffect(EffectType.Moving, Serial.Zero, Serial.Zero, 0x27A3, Location, attacker.Location, 10, 0, false, false, 0, 0));
                AOS.Damage(attacker, this, Utility.RandomMinMax(50, 65), 100, 0, 0, 0, 0);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.8 > Utility.RandomDouble() && !attacker.InRange(this, 1))
            {
                /* Fan Throw
                * Effect: - To: "0x57D4F5B" - ItemId: "0x27A3" - ItemIdName: "Tessen" - FromLocation: "(992 299, 24)" - ToLocation: "(992 308, 22)" - Speed: "10" - Duration: "0" - FixedDirection: "False" - Explode: "False" - Hue: "0x0" - Render: "0x0"
                * Damage: 50-65
                */
                Effects.SendPacket(attacker, attacker.Map, new HuedEffect(EffectType.Moving, Serial.Zero, Serial.Zero, 0x27A3, Location, attacker.Location, 10, 0, false, false, 0, 0));
                AOS.Damage(attacker, this, Utility.RandomMinMax(50, 65), 100, 0, 0, 0, 0);
            }
        }

        
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (!IsFanned(defender) && 0.05 > Utility.RandomDouble())
            {

                defender.SendLocalizedMessage(1070833); // The creature fans you with fire, reducing your resistance to fire attacks.

                int effect = -(defender.FireResistance / 10);

                ResistanceMod rmod = new ResistanceMod(ResistanceType.Fire, effect);
                StatMod mod = new StatMod(StatType.Int, "Fan-Int", -20, TimeSpan.FromSeconds(20));
                defender.AddStatMod(mod);

                defender.FixedParticles(0x37B9, 10, 30, 0x34, EffectLayer.RightFoot);
                defender.PlaySound(0x208);

                // This should be done in place of the normal attack damage.
                AOS.Damage( defender, this, Utility.RandomMinMax( 5, 15 ), 0, 100, 0, 0, 0 );

                //defender.AddResistanceMod(mod);
		
                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.FanDancerFanFire, 1153787, 1153817, TimeSpan.FromSeconds(10.0), defender, effect));
            }
        }
        

        public bool IsFanned(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;
            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(1070834); // Your resistance to fire attacks has returned.
                m_Mobile.RemoveResistanceMod(m_Mod);
                Stop();
                m_Table.Remove(m_Mobile);
            }
        }
    }
}
