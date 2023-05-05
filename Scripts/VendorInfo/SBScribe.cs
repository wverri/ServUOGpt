using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBScribe : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo;
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBScribe(Mobile m)
        {
            if (m != null)
            {
                m_BuyInfo = new InternalBuyInfo(m);
            }
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(Mobile m)
            {
                Add(new GenericBuyInfo(typeof(ScribesPen), 8, 20, 0xFBF, 0));
                Add(new GenericBuyInfo(typeof(BlankScroll), 5, 999, 0x0E34, 0));
                Add(new GenericBuyInfo(typeof(PenAndInk), 8, 20, 0xFC0, 0));
                Add(new GenericBuyInfo(typeof(BrownBook), 15, 10, 0xFEF, 0));
                Add(new GenericBuyInfo(typeof(TanBook), 15, 10, 0xFF0, 0));
                Add(new GenericBuyInfo(typeof(BlueBook), 15, 10, 0xFF2, 0));
                Add(new GenericBuyInfo(typeof(Runebook), 40000, 10, 0xEFA, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(ScribesPen), 4);
                Add(typeof(PenAndInk), 4);
                Add(typeof(BrownBook), 7);
                Add(typeof(TanBook), 7);
                Add(typeof(BlueBook), 7);
                Add(typeof(BlankScroll), 3);

                Type[] types = Loot.RegularScrollTypes;

                for (int i = 0; i < types.Length; ++i)
                    Add(types[i], ((i / 8) * 2 + 2));
            }
        }
    }
}
