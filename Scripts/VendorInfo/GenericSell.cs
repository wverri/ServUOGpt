using System;
using System.Collections.Generic;
using Server.Items;
using System.Linq;

namespace Server.Mobiles
{
    public class GenericSellInfo : IShopSellInfo
    {
        private readonly Dictionary<Type, int> m_Table = new Dictionary<Type, int>();
        private Type[] m_Types;
        public GenericSellInfo()
        {
        }

        public Type[] Types
        {
            get
            {
                if (m_Types == null)
                {
                    m_Types = new Type[m_Table.Keys.Count];
                    m_Table.Keys.CopyTo(m_Types, 0);
                }

                return m_Types;
            }
        }
        public void Add(Type type, int price)
        {
            m_Table[type] = price;
            m_Types = null;
        }

        public int GetSellPriceFor(Item item)
        {
            return GetSellPriceFor(item, null);
        }

        public int GetSellPriceFor(Item item, BaseVendor vendor)
        {
            int price = 0;
            m_Table.TryGetValue(item.GetType(), out price);

            if (Shard.DebugEnabled)
                Shard.Debug("Vendendo item " + item.GetType().Name + " Usaneo economia vendor ? " + BaseVendor.UseVendorEconomy);

            if (vendor != null && BaseVendor.UseVendorEconomy)
            {
                IBuyItemInfo buyInfo = vendor.GetBuyInfo().OfType<GenericBuyInfo>().FirstOrDefault(info => info.EconomyItem && info.Type == item.GetType());

                if (buyInfo != null)
                {
                    if(price == 0)
                        return 1;
                    int sold = buyInfo.TotalSold;
                    price = (int)((double)buyInfo.Price * .5);
                    if (Shard.DebugEnabled)
                        Shard.Debug("Preco de venda " + price + " item " + item.GetType().Name);
                    return Math.Max(1, price);
                }
            }

            Shard.Debug("Nao tem Buyinfo");

            if (item is BaseArmor)
            {
                BaseArmor armor = (BaseArmor)item;

                if (armor.Quality == ItemQuality.Low)
                    price = (int)(price * 0.60);
                else if (armor.Quality == ItemQuality.Exceptional)
                    price = (int)(price * 1.25);

                price += 100 * (int)armor.Durability;

                price += 100 * (int)armor.ProtectionLevel;

                if (price < 1)
                    price = 1;
            }
            else if (item is BaseClothing)
            {
                BaseClothing armor = (BaseClothing)item;

                if (armor.Quality == ItemQuality.Low)
                    price = (int)(price * 0.60);
                else if (armor.Quality == ItemQuality.Exceptional)
                    price = (int)(price * 1.1);

                if (price < 1)
                    price = 1;
            }
            else if (item is BaseWeapon)
            {
                BaseWeapon weapon = (BaseWeapon)item;

                if (weapon.Quality == ItemQuality.Low)
                    price = (int)(price * 0.60);
                else if (weapon.Quality == ItemQuality.Exceptional)
                    price = (int)(price * 1.1);

                price += 100 * (int)weapon.DurabilityLevel;

                price += 100 * (int)weapon.DamageLevel;

                if (price < 1)
                    price = 1;
            }
            else if (item is BaseBeverage)
            {
                int price1 = price, price2 = price;

                if (item is Pitcher)
                {
                    price1 = 3;
                    price2 = 5;
                }
                else if (item is BeverageBottle)
                {
                    price1 = 3;
                    price2 = 3;
                }
                else if (item is Jug)
                {
                    price1 = 6;
                    price2 = 6;
                }

                BaseBeverage bev = (BaseBeverage)item;

                if (bev.IsEmpty || bev.Content == BeverageType.Milk)
                    price = price1;
                else
                    price = price2;
            }

            var p = (int)(price * 0.6);
            if (p < 1)
                p = 1;
            return p;
        }

        public int GetBuyPriceFor(Item item)
        {
            return GetBuyPriceFor(item, null);
        }

        public int GetBuyPriceFor(Item item, BaseVendor vendor)
        {
            return (int)(1.90 * GetSellPriceFor(item, vendor));
        }

        public string GetNameFor(Item item)
        {
            if (item.Name != null)
                return item.Name;
            else
                return item.LabelNumber.ToString();
        }

        public bool IsSellable(Item item)
        {
            if (item.QuestItem)
                return false;

            //if ( item.Hue != 0 )
            //return false;

            return IsInList(item.GetType());
        }

        public bool IsResellable(Item item)
        {
            if (item.QuestItem)
                return false;

            //if ( item.Hue != 0 )
            //return false;

            return IsInList(item.GetType());
        }

        public bool IsInList(Type type)
        {
            return m_Table.ContainsKey(type);
        }
    }
}
