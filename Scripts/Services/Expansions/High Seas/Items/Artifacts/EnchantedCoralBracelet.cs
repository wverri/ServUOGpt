﻿using Server;
using System;

namespace Server.Items
{
    public class EnchantedCoralBracelet : SilverBracelet
    {
        public override int LabelNumber { get { return 1116624; } }

        [Constructable]
        public EnchantedCoralBracelet()
        {
            Hue = 1548;
            Attributes.BonusHits = 5;
            Attributes.RegenMana = 1;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 15;
            Attributes.Resistence = 1;
            Attributes.CastRecovery = 3;
            Attributes.SpellDamage = 10;
        }

        public EnchantedCoralBracelet(Serial serial)
            : base(serial)
        {
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