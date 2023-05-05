using System;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Misc.Custom
{
    public class ElementalBall : Item, ICraftable
    {
        public int Cargas = 0;

        [Constructable]
        public ElementalBall(int cargas = 1000)
            : base(3630)
        {
            this.Hue = 386;
            Weight = 10.0;
            this.Name = "Bola de Cristal Elemental";
            this.LootType = LootType.Blessed;
            this.Cargas = cargas;
        }

        [Constructable]
        public ElementalBall()
            : base(3630)
        {
            Cargas = 1000;
            this.Hue = 386;
            Weight = 10.0;
            this.Name = "Bola de Cristal Elemental";
            this.LootType = LootType.Blessed;
        }

        public ElementalBall(Serial serial) : base(serial)
        {

        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add("Permite lancar magias sem gastar reagentes");
            list.Add("Magias: " + Cargas);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("Bola de Cristal Elemental");
        }

        public static bool UseElementalBall(Mobile from)
        {
            var spellStone = from.Backpack.FindItemByType(typeof(ElementalBall));
            if (spellStone != null)
            {
                if (spellStone.BoundTo != null && spellStone.BoundTo != from.RawName)
                    return false;

                var stone = (ElementalBall)spellStone;
                stone.Cargas--;
                if (stone.Cargas == 0)
                {
                    from.SendMessage(38, "Sua bola elemental se desfez.");
                    stone.Consume();
                }
                else
                {
                    stone.InvalidateProperties();
                }
                return true;
            }
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(Cargas);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    {
                        Cargas = reader.ReadInt();
                        break;
                    }
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            this.Name += " feita por "+from.Name;
            this.LootType = LootType.Blessed;
            this.Cargas = 2000;
            return quality;
        }
    }


}
