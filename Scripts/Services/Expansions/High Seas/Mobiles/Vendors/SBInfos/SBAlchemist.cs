using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBDocksAlchemist : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBDocksAlchemist()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo("1116302", typeof(Saltpeter), 2867, 20, 16954, 1150));

                Add(new GenericBuyInfo(typeof(RefreshPotion), 50, 10, 0xF0B, 0));
                Add(new GenericBuyInfo(typeof(AgilityPotion), 15, 10, 0xF08, 0));
                Add(new GenericBuyInfo(typeof(NightSightPotion), 15, 10, 0xF06, 0));
                Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 10, 0xF0C, 0));
                Add(new GenericBuyInfo(typeof(StrengthPotion), 15, 10, 0xF09, 0));
                Add(new GenericBuyInfo(typeof(LesserPoisonPotion), 15, 10, 0xF0A, 0));
                Add(new GenericBuyInfo(typeof(LesserCurePotion), 15, 10, 0xF07, 0));
                Add(new GenericBuyInfo(typeof(LesserExplosionPotion), 21, 10, 0xF0D, 0));

                SBMage.BuyReagents(this);

                Add(new GenericBuyInfo(typeof(Bottle), 10, 100, 0xF0E, 0));


                Add(new GenericBuyInfo(typeof(MortarPestle), 8, 10, 0xE9B, 0));

                Add(new GenericBuyInfo(typeof(HeatingStand), 2, 100, 0x1849, 0));

                Add(new GenericBuyInfo("1041060", typeof(HairDye), 55, 10, 0xEFF, 0));

                Add(new GenericBuyInfo("Manual de Craft Vidros (Alchemy)", typeof(GlassblowingBook), 20000, 30, 0xFF4, 0));
                Add(new GenericBuyInfo("Manual Minerar Areia (Mining)", typeof(SandMiningBook), 20000, 30, 0xFF4, 0));
                Add(new GenericBuyInfo(typeof(TransformationDust), 5000, 100, 0x5745, 0, true));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Saltpeter), 10);
                Add(typeof(HairDye), 19);
                Add(typeof(MortarPestle), 4);

                Add(typeof(NightSightPotion), 7);
                Add(typeof(AgilityPotion), 7);
                Add(typeof(StrengthPotion), 7);
                Add(typeof(RefreshPotion), 7);
                Add(typeof(LesserCurePotion), 7);
                Add(typeof(LesserHealPotion), 7);
                Add(typeof(LesserPoisonPotion), 7);
                Add(typeof(LesserExplosionPotion), 10);
                /*
                Add(typeof(BlackPearl), 3);
                Add(typeof(Bloodmoss), 3);
                Add(typeof(MandrakeRoot), 2);
                Add(typeof(Garlic), 2);
                Add(typeof(Ginseng), 2);
                Add(typeof(Nightshade), 2);
                Add(typeof(SpidersSilk), 2);
                Add(typeof(SulfurousAsh), 2);
                Add(typeof(Bottle), 3);
              

           
                */
            }
        }
    }
}
