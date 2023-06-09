using Server;
using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishBrightblade))]
    public class Brightblade : Katana
    {
        public override int LabelNumber { get { return 1152732; } } // Brightblade
		public override bool IsArtifact { get { return true; } }

        [Constructable]
        public Brightblade()
        {
            WeaponAttributes.HitLeechStam = 100;
            WeaponAttributes.SplinteringWeapon = 20;
            Attributes.RegenStam = 3;
            Attributes.AttackChance = 10;
            Attributes.Resistence = 1;
            Attributes.WeaponSpeed = 40;
            Attributes.WeaponDamage = 50;
            AosElementDamages.Fire = 100;
            Hue = 1756;
        }

        public Brightblade(Serial serial)
            : base(serial)
        {
        }
        
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishBrightblade : GargishKatana
    {
        public override int LabelNumber { get { return 1152732; } } // Brightblade
		public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishBrightblade()
        {
            WeaponAttributes.HitLeechStam = 100;
            WeaponAttributes.SplinteringWeapon = 20;
            Attributes.RegenStam = 3;
            Attributes.AttackChance = 10;
            Attributes.Resistence = 1;
            Attributes.WeaponSpeed = 40;
            Attributes.WeaponDamage = 50;
            AosElementDamages.Fire = 100;
            Hue = 1756;
        }

        public GargishBrightblade(Serial serial) : base(serial)
        {
        }
        
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
