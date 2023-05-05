using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using System.Collections.Generic;
using Server.Engines.Points;

namespace Server.Engines.VvV
{
    public class SilverTrader : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }
        public override void InitSBInfo() { }

        [Constructable]
        public SilverTrader() : base("o comerciante de Pratinhas")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            Robe robe = new Robe();
            robe.ItemID = 0x2684;
            robe.Name = "Sobretudo Lindao";

            SetWearable(robe, 1109);

            Timer.DelayCall(TimeSpan.FromSeconds(10), StockInventory);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Vendedor de Pratinhas"); // Vice vs Virtue Reward Vendor
        }

        private DateTime _NextSpeak;

        public override void OnDoubleClick(Mobile m)
        {
            if (ViceVsVirtueSystem.Enabled && m is PlayerMobile && InRange(m.Location, 3))
            {
                m.SendMessage(78, "Para conseguir pratinhas participe da Guerra Infinita, um sistema de guerra de guildas para o dominio de cidades. Para ver mais veja nossa Wiki.");

                m.SendGump(new VvVRewardGump(this, (PlayerMobile)m));
                //SayTo(m, "Voce nao tem pratinhas. Participe da guerra infinita para conseguir !"); // You have no silver to trade with. Join Vice vs Virtue and return to me.

            }
        }

        public void StockInventory()
        {
            if (Backpack == null)
                AddItem(new Backpack());

            foreach (CollectionItem item in VvVRewards.Rewards)
            {
                if (item.Tooltip == 0 && item.TooltipStr == null)
                {
                    if (Backpack.GetAmount(item.Type) > 0)
                    {
                        Item itm = Backpack.FindItemByType(item.Type);

                        if (itm is IVvVItem)
                            ((IVvVItem)itm).IsVvVItem = true;

                        continue;
                    }

                    Item i = Activator.CreateInstance(item.Type) as Item;

                    if (i != null)
                    {
                        if (i is IOwnerRestricted)
                            ((IOwnerRestricted)i).OwnerName = "Nome do Jogador";

                        if (i is IVvVItem)
                            ((IVvVItem)i).IsVvVItem = true;

                        NegativeAttributes neg = RunicReforging.GetNegativeAttributes(i);

                        if (neg != null)
                        {
                            neg.Antique = 1;

                            if (i is IDurability && ((IDurability)i).MaxHitPoints == 0)
                            {
                                ((IDurability)i).MaxHitPoints = 255;
                                ((IDurability)i).HitPoints = 255;
                            }
                        }

                        ViceVsVirtueSystem.Instance.AddVvVItem(i, true);

                        Backpack.DropItem(i);
                    }
                }
            }
        }

        private Type[][] _Table =
        {
            new Type[] { typeof(CrimsonCincture), typeof(GargishCrimsonCincture) },
            new Type[] { typeof(MaceAndShieldGlasses), typeof(GargishMaceAndShieldGlasses) },
            new Type[] { typeof(WizardsCrystalGlasses), typeof(GargishWizardsCrystalGlasses) },
            new Type[] { typeof(FoldedSteelGlasses), typeof(GargishFoldedSteelGlasses) },
        };

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (ViceVsVirtueSystem.IsVvV(from))
            {
                if (!(dropped is IOwnerRestricted) || ((IOwnerRestricted)dropped).Owner == from)
                {
                    if (dropped is IVvVItem && from.Race == Race.Gargoyle)
                    {
                        foreach (var t in _Table)
                        {
                            if (dropped.GetType() == t[0])
                            {
                                IDurability dur = dropped as IDurability;

                                if (dur != null && dur.MaxHitPoints == 255 && dur.HitPoints == 255)
                                {
                                    var item = Loot.Construct(t[1]);

                                    if (item != null)
                                    {
                                        VvVRewards.OnRewardItemCreated(from, item);

                                        if (item is GargishCrimsonCincture)
                                        {
                                            ((GargishCrimsonCincture)item).Attributes.BonusDex = 10;
                                        }

                                        if (item is GargishMaceAndShieldGlasses)
                                        {
                                            ((GargishMaceAndShieldGlasses)item).Attributes.WeaponDamage = 10;
                                        }

                                        if (item is GargishFoldedSteelGlasses)
                                        {
                                            ((GargishFoldedSteelGlasses)item).Attributes.DefendChance = 25;
                                        }

                                        if (item is GargishWizardsCrystalGlasses)
                                        {
                                            ((GargishWizardsCrystalGlasses)item).PhysicalBonus = 5;
                                            ((GargishWizardsCrystalGlasses)item).FireBonus = 5;
                                            ((GargishWizardsCrystalGlasses)item).ColdBonus = 5;
                                            ((GargishWizardsCrystalGlasses)item).PoisonBonus = 5;
                                            ((GargishWizardsCrystalGlasses)item).EnergyBonus = 5;
                                        }

                                        from.AddToBackpack(item);
                                        dropped.Delete();

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            SayTo(from, 1157365); // I'm sorry, I cannot accept this item.
            return false;
        }

        public SilverTrader(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(() =>
                    {
                        ColUtility.SafeDelete<Item>(Backpack.Items, null);
                    });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(5), StockInventory);
        }
    }

    public class ArenaTrader : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }
        public override void InitSBInfo() { }

        [Constructable]
        public ArenaTrader() : base("o comerciante da Arena PvP")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            SetWearable(new DoubleAxe() { Resource = CraftResource.Adamantium });
            SetWearable(new ChainChest() { Resource = CraftResource.Vibranium });
            SetWearable(new ChainLegs() { Resource = CraftResource.Vibranium });
            SetWearable(new ChainCoif() { Resource = CraftResource.Vibranium });
            SetWearable(new ChainGloves() { Resource = CraftResource.Vibranium });
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Comerciante da Arena"); // Vice vs Virtue Reward Vendor
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (ViceVsVirtueSystem.Enabled && m is PlayerMobile && InRange(m.Location, 3))
            {
                m.SendMessage(78, "Para conseguir pontos de PvP, jogue PvP na arena pvp usando o comando .pvp ! Nao perde items !");

                m.SendGump(new ArenaRewardsGump(this, (PlayerMobile)m));
            }
        }

        public ArenaTrader(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
