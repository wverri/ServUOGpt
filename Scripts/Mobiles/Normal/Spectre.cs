using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Second;
using Server.Spells.Seventh;
using Server.Spells.Third;

namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Spectre : BaseCreature
    {
        [Constructable]
        public Spectre()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "spectro";
            this.Body = 26;
            this.Hue = 0x4001;
            this.BaseSoundID = 0x482;

            this.SetStr(76, 100);
            this.SetDex(76, 95);
            this.SetInt(36, 60);

            this.SetHits(46, 60);

            this.SetDamage(3, 5);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 10, 20);

            this.SetSkill(SkillName.EvalInt, 55.1, 70.0);
            this.SetSkill(SkillName.Magery, 100, 100);
            this.SetSkill(SkillName.MagicResist, 55.1, 70.0);
            this.SetSkill(SkillName.Tactics, 45.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 55.0);

            this.Fame = 4000;
            this.Karma = -4000;

            this.VirtualArmor = 28;

            this.PackReg(10);
        }

        public Spectre(Serial serial)
            : base(serial)
        {
        }

        public override Spell ChooseSpell()
        {
            var alvo = Combatant as Mobile;
            if (alvo != null)
            {
                if(alvo is BaseCreature)
                {
                    if(!alvo.Poisoned)
                        return new PoisonSpell(this, null);
                    return new FlameStrikeSpell(this, null);
                }
                if(alvo.Mana > 10)
                    return new ManaDrainSpell(this, null);
            }
            return new HarmSpell(this, null);
        }

        public class FreezeTimer : Timer
        {
            private BaseCreature mob;
            private int ct = 0;
            private Mobile from;

            int dMin = 0;
            int dMax = 0;
            int hue = 0;

            public FreezeTimer(BaseCreature defender, Mobile from)
                : base(TimeSpan.FromSeconds(1))
            {
                mob = defender;
                this.from = from;
            }

            protected override void OnTick()
            {
                from.PlaySound(0x204);
                from.Freeze(TimeSpan.FromSeconds(6));
                mob.MovingParticles(from, 0x376A, 9, 0, false, false, 9502, 0x376A, 0x204);
            }
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }

        public override TribeType Tribe { get { return TribeType.MortoVivo; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.LV2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
