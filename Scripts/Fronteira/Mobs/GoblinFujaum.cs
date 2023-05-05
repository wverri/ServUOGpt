using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class GoblinFujaum : BaseCreature
    {
        public override bool ReduceSpeedWithDamage { get { return false; } }

        [Constructable]
        public GoblinFujaum()
            : base(AIType.AI_Runner, FightMode.Closest, 10, 1, 0.1, 0.1)
        {
            Name = "Goblin Medroso";
            Body = 334;
            BaseSoundID = 0x600;

            SetStr(297, 297);
            SetDex(80, 80);
            SetInt(118, 118);

            SetHits(150, 190);
            SetStam(80, 80);
            SetMana(118, 118);

            SetDamage(10, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 47, 47);
            SetResistance(ResistanceType.Fire, 37, 37);
            SetResistance(ResistanceType.Cold, 29, 29);
            SetResistance(ResistanceType.Poison, 10, 11);
            SetResistance(ResistanceType.Energy, 19, 19);

            SetSkill(SkillName.MagicResist, 30.6, 40);
            SetSkill(SkillName.Tactics, 80.0, 82.8);
            SetSkill(SkillName.Anatomy, 82.0, 84.8);
            SetSkill(SkillName.Wrestling, 99.2, 100.7);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 28;

            // Loot - 30-40gold, magicitem,gem,goblin blood, essence control
            switch (Utility.Random(20))
            {
                case 0:
                    PackItem(new Scimitar());
                    break;
                case 1:
                    PackItem(new Katana());
                    break;
                case 2:
                    PackItem(new WarMace());
                    break;
                case 3:
                    PackItem(new WarHammer());
                    break;
                case 4:
                    PackItem(new Kryss());
                    break;
                case 5:
                    PackItem(new Pitchfork());
                    break;
            }

            PackItem(new ThighBoots());

            switch (Utility.Random(3))
            {
                case 0:
                    PackItem(new Ribs());
                    break;
                case 1:
                    PackItem(new Shaft());
                    break;
                case 2:
                    PackItem(new Candle());
                    break;
            }

            if (0.2 > Utility.RandomDouble())
                PackItem(new BolaBall());
        }

        public static int BANDS_TIME = 6;

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {

            if(!IsCooldown("fala"))
            {
                SetCooldown("fala", TimeSpan.FromSeconds(40));
                Say("Nhaaa mestre Exodo me ajuuuuude !!!");
            }

            if (!IsCooldown("bands"))
            {
                Shard.Debug("KILL?" + willKill + " - " + this.Hits + " < " + this.HitsMax);
                if (!willKill && this.Hits < this.HitsMax)
                {
                    SetCooldown("bands", TimeSpan.FromSeconds(BANDS_TIME + 10));
                    PublicOverheadMessage(Network.MessageType.Regular, 0, false, "* aplicando bandagens *");
                    new HealTimer(this);
                }
            }
            base.OnDamage(amount, from, willKill);
        }

        public class HealTimer : Timer
        {
            private BaseCreature m_Defender;


            public HealTimer(BaseCreature defender)
                : base(TimeSpan.FromSeconds(Utility.Random(BaseOrc.BANDS_TIME) + 4))
            {
                m_Defender = defender;
                Start();
            }

            protected override void OnTick()
            {
                if (this.m_Defender.Poisoned || BleedAttack.IsBleeding(this.m_Defender))
                {
                    this.m_Defender.PublicOverheadMessage(Network.MessageType.Emote, 0, false, "* todo perdido *");
                    return;
                }
                var heal = 60 + Utility.Random(20);
                this.m_Defender.Heal(heal);
                this.m_Defender.PlaySound(0x57);
            }
        }

        public GoblinFujaum(Serial serial)
            : base(serial)
        {
        }

        public override int GetAngerSound() { return 0x600; }
        public override int GetIdleSound() { return 0x600; }
        public override int GetAttackSound() { return 0x5FD; }
        public override int GetHurtSound() { return 0x5FF; }
        public override int GetDeathSound() { return 0x5FE; }

        public override bool CanRummageCorpses { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 1; } }
        public override OppositionGroup OppositionGroup { get { return OppositionGroup.SavagesAndOrcs; } }

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
