using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Globalization;
using Server.Network;
using Server.Gumps;
using Server.SkillHandlers;

namespace Server.Engines.VendorSearching
{
    public class VendorSearchGump : BaseGump
    {
        public SearchCriteria Criteria { get; set; }
        public int ChosenCategory { get; set; }
        public Map SetMap { get; set; }

        public bool NoFind { get; private set; }
        public bool NoCrit { get; private set; }        

        public static int LabelColor = 0x4BBD;
        public static int CriteriaColor = 0x6B55;
        public static int TextColor = 0x9C2;

        public VendorSearchGump(PlayerMobile pm, int cat = -1) 
            : base(pm, 10, 10)
        {
            Criteria = VendorSearch.GetContext(pm);
            ChosenCategory = cat;

            if (Criteria == null)
                Criteria = VendorSearch.AddNewContext(pm);
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 780, 570, 30546);
            AddBackground(10, 50, 246, 22, 9350);

            AddHtml(10, 25, 150, 20, "Nome do Item", LabelColor, false, false); // Item Name
            AddHtml(10, 10, 760, 18, "Pesquisa", LabelColor, false, false); // Vendor Search Query
            AddHtml(522, 30, 246, 18, "Filtros", false, false); // Selected Search Criteria

            AddTextEntry(12, 52, 242, 18, TextColor, 0, Criteria.SearchName, 25);

            for (int i = 0; i < VendorSearch.Categories.Count; i++)
            {
                if (i == 0)
                    AddHtmlLocalized(50, 75 + (i * 22), 215, 20, VendorSearch.Categories[i].Label, String.Format("{0}\t{1}", Criteria.MinPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Criteria.MaxPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), LabelColor, false, false);
                else
                    AddHtmlLocalized(50, 75 + (i * 22), 215, 20, VendorSearch.Categories[i].Label, LabelColor, false, false);

                AddButton(10, 75 + (i * 22), 30533, 30533, i + 100, GumpButtonType.Reply, 0);
            }

            if (ChosenCategory >= 0 && ChosenCategory < VendorSearch.Categories.Count)
            {
                if (ChosenCategory == 0)
                {
                    AddHtml(266, 30, 246, 18, "Preco Minimo", LabelColor, false, false); // Minimum Price
                    AddBackground(266, 50, 246, 22, 9350);
                    AddTextEntry(268, 52, 242, 18, TextColor, 1, Criteria.MinPrice.ToString(), 10);

                    AddHtml(266, 74, 246, 18, "Preco Maximo", LabelColor, false, false); // Maximum Price
                    AddBackground(266, 94, 246, 22, 9350);
                    AddTextEntry(268, 96, 242, 18, TextColor, 2, Criteria.MaxPrice.ToString(), 10);

                    AddButton(266, 118, 4011, 4012, 1, GumpButtonType.Reply, 0);
                    AddHtml(306, 118, 100, 272, "Adicionar Filtro", LabelColor, false, false); // Add Search Criteria
                }
                else if (ChosenCategory == VendorSearch.Categories.Count - 1)
                {
                    AddHtmlLocalized(270, 25, 150, 20, VendorSearch.Categories[ChosenCategory].Label, LabelColor, false, false); // Sort Results

                    AddHtml(310, 45, 150, 20, "Preco Crescente", LabelColor, false, false); // Price: Low to High
                    AddButton(270, 45, 30533, 30533, 2, GumpButtonType.Reply, 0);

                    AddHtml(310, 67, 150, 20, "Preco Decrescente", LabelColor, false, false); // Price: High to Low
                    AddButton(270, 67, 30533, 30533, 3, GumpButtonType.Reply, 0);
                }
                else if (ChosenCategory >= 0 && ChosenCategory < VendorSearch.Categories.Count)
                {
                    var cat = VendorSearch.Categories[ChosenCategory];
                    
                    AddHtml(266, 30, 150, 20, VendorSearch.Categories[ChosenCategory].Category.ToString(), LabelColor, false, false);

                    for (int index = 0; index < VendorSearch.Categories[ChosenCategory].Objects.Count; index++)
                    {
                        Tuple<object, int> data = VendorSearch.Categories[ChosenCategory].Objects[index];

                        AddHtmlLocalized(306, 50 + (index * 22), 215, 20, (int)data.Item2, LabelColor, false, false);
                        AddButton(266, 50 + (index * 22), 30533, 30533, 200 + index, GumpButtonType.Reply, 0);

                        if (VendorSearch.HasValue(data.Item1, VendorSearch.Categories[ChosenCategory]))
                        {
                            int value = Criteria.GetValueForDetails(data.Item1);

                            AddBackground(482, 50 + (index * 22), 30, 20, 9350);
                            AddTextEntry(484, 50 + (index * 22), 26, 16, TextColor, 400 + index, value > 0 ? value.ToString() : "", 3);
                        }
                    }
                }
            }

            int y = 50;            

            if (!String.IsNullOrEmpty(Criteria.SearchName))
            {
                AddButton(522, y, 4017, 4019, 7, GumpButtonType.Reply, 0);
                AddTooltip("Remover Filtro"); // Remove Selected Search Criteria

                AddHtmlLocalized(562, y, 215, 20, 1154510, CriteriaColor, false, false);

                y += 22;
            }

            if (Criteria.EntryPrice)
            {
                AddButton(522, y, 4017, 4019, 8, GumpButtonType.Reply, 0);
                AddTooltip("Remover Filtro"); // Remove Selected Search Criteria

                AddHtmlLocalized(562, y, 215, 20, 1154512, String.Format("{0}\t{1}", Criteria.MinPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Criteria.MaxPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), CriteriaColor, false, false);
                
                y += 22;
            }

            for (int i = 0; i < Criteria.Details.Count; i++)
            {
                AddButton(522, y + (i * 22), 4017, 4019, 300 + i, GumpButtonType.Reply, 0);
                AddTooltip("Remover filtros"); // Remove Selected Search Criteria

               /// Criteria.Details.
                AddHtmlLocalized(562, y + (i * 22), 215, 20, Criteria.Details[i].Label, CriteriaColor, false, false);

                if (Criteria.Details[i].Value > 0)
                {
                    AddHtml(720, y + (i * 22), 60, 20, String.Format("<basefont color=#F5DEB3>{0}", FormatValue(Criteria.Details[i].Attribute, Criteria.Details[i].Value)), false, false);
                }
            }
                        
            AddButton(522, y + (Criteria.Details.Count * 22), 4017, 4019, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(562, y + (Criteria.Details.Count * 22), 215, 20, Criteria.SortBy == SortBy.LowToHigh ? 1154696 : 1154697, CriteriaColor, false, false);
            
            AddHtml(630, 520, 100, 20, "Limpar Busca", LabelColor, false, false); // Clear Search Criteria
            AddButton(740, 520, 30533, 30533, 5, GumpButtonType.Reply, 0);

            AddHtml(630, 540, 100, 20, "Pesquisar", LabelColor, false, false); // Search
            AddButton(740, 540, 30534, 30534, 6, GumpButtonType.Reply, 0);

            AddHtml(50, 540, 50, 20, "Cancelar", LabelColor, false, false); // Cancel
            AddButton(10, 540, 0x7747, 0x7747, 0, GumpButtonType.Reply, 0);

            if (NoFind)
            {
                AddHtml(125, 540, 400, 16, "Nenhum item encontrado", C32216(0xFF0000), false, false); // No items matched your search.
            }
            else if (NoCrit)
            {
                AddHtml(125, 540, 400, 16, "Escolhe um filtro para pesquisar", C32216(0xFF0000), false, false); // Please select some criteria to search for.
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            NoCrit = false;
            NoFind = false;

            if (info.ButtonID != 0)
            {
                if (!VendorSearch.CanSearch(User))
                {
                    User.SendLocalizedMessage("Voce precisa estar em uma cidade ou em sua casa para isto"); //Before using vendor search, you must be in a justice region or a safe log-out location (such as an inn or a house which has you on its Owner, Co-owner, or Friends list). 
                    return;
                }

                TextRelay searchname = info.GetTextEntry(0);

                if (searchname != null && !String.IsNullOrEmpty(searchname.Text))
                {
                    string text = searchname.Text.Trim();

                    if (Criteria.SearchName == null || text.ToLower() != Criteria.SearchName.ToLower())
                        Criteria.SearchName = searchname.Text;
                }
            }

            switch (info.ButtonID)
            {
                case 0: break;
                case 1: // Set Min/Max price
                    TextRelay tr1 = info.GetTextEntry(1);
                    TextRelay tr2 = info.GetTextEntry(2);

                    if (tr1 != null && tr1.Text != null)
                    {
                        string text = tr1.Text.Trim();
                        int min = 0;

                        if (int.TryParse(text, out min))
                        {
                            Criteria.MinPrice = min;
                        }
                    }

                    if (tr2 != null && tr2.Text != null)
                    {
                        string text = tr2.Text.Trim();
                        int max = 0;

                        if (int.TryParse(text, out max))
                        {
                            Criteria.MaxPrice = max;
                        }
                    }

                    Criteria.EntryPrice = true;
                    Refresh();
                    break;
                case 2: // Low to High
                    Criteria.SortBy = SortBy.LowToHigh;
                    Refresh();
                    break;
                case 3: // High to Low
                    Criteria.SortBy = SortBy.HighToLow;
                    Refresh();
                    break;
                case 4: // Nothing, resend gump                    
                    Refresh();
                    break;
                case 5: // Clear Criteria
                    Criteria.Reset();
                    Refresh();
                    break;
                case 6: // Search
                    User.CloseGump(typeof(SearchResultsGump));

                    if (Criteria.IsEmpty)
                    {
                        NoCrit = true;
                        Refresh();
                    }
                    else
                    {
                        List<VendorItem> list = VendorSearch.DoSearch(User, Criteria);

                        if (list == null || list.Count == 0)
                        {
                            NoFind = true;
                            Refresh();
                        }
                        else
                        {
                            Refresh();
                            BaseGump.SendGump(new SearchResultsGump(User, list));
                        }
                    }
                    break;
                case 7: // remove item name
                    Criteria.SearchName = null;
                    Refresh();
                    break;
                case 8: // remove price entry
                    Criteria.EntryPrice = false;
                    Refresh();
                    break;
                default:
                    if (info.ButtonID >= 100 && info.ButtonID < 200)
                    {
                        ChosenCategory = info.ButtonID - 100;
                        Refresh();
                    }
                    else if (info.ButtonID >= 200 && info.ButtonID < 300 && ChosenCategory >= 0 && ChosenCategory < VendorSearch.Categories.Count)
                    {
                        int index = info.ButtonID - 200;

                        SearchCategory category = VendorSearch.Categories[ChosenCategory];
                        Tuple<object, int> data = category.Objects[index];
                        object o = data.Item1;
                        int value = 0;

                        TextRelay valuetext = info.GetTextEntry(index + 400);

                        if (valuetext != null)
                            value = Math.Max(o is AosAttribute && (AosAttribute)o == AosAttribute.Resistence ? -1 : 0, Utility.ToInt32(valuetext.Text));

                        Criteria.TryAddDetails(o, data.Item2, value, category.Category);
                        Refresh();
                    }
                    else if (info.ButtonID >= 300 && info.ButtonID - 300 >= 0 && info.ButtonID - 300 < Criteria.Details.Count)
                    {
                        SearchDetail toRemove = Criteria.Details[info.ButtonID - 300];

                        if (toRemove.Category == Category.Equipamento)
                            Criteria.SearchType = Layer.Invalid;

                        Criteria.Details.Remove(toRemove);
                        Refresh();
                    }
                    break;
            }
        }

        private string FormatValue(object attribute, int value)
        {
            int mod = Imbuing.GetMod(attribute);

            if (!Imbuing.Table.ContainsKey(mod))
                return value.ToString();

            if (mod == 41)
                return String.Format("-{0}", value.ToString());
            else if (Imbuing.Table[mod].MaxIntensity <= 8 || mod == 21 || mod == 17)
                return value.ToString();

            return String.Format("{0}%", value.ToString());
        }
    }

    public class SearchResultsGump : BaseGump
    {
        public int PerPage = 5;
        public int TextColor { get { return C32216(0xF5DEB3); } }

        public List<VendorItem> Items { get; set; }
        public int Index { get; set; }

        public SearchResultsGump(PlayerMobile pm, List<VendorItem> items) 
            : base(pm, 50, 50)
        {
            Items = items;
            Index = 0;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 500, 550, 30536);

            AddHtml(0, 40, 500, 20, "Resultados", TextColor, false, false); // Vendor Search Results

            AddHtml(200, 70, 90, 20, "Preco", TextColor, false, false); // Price
            AddHtml(290, 70, 100, 20, "Mapa", TextColor, false, false); // Facet
            AddHtml(360, 70, 100, 20, "Ir", TextColor, false, false); // Create Map

            if (Index < 0) Index = Items.Count - 1;
            if(Index >= Items.Count) Index = 0;

            int start = Index;
            int index = 0;

            for (int i = start; i < start + PerPage && i < Items.Count; i++)
            {
                VendorItem item = Items[i];
                Rectangle2D bounds = ItemBounds.Table[item.Item.ItemID];
                int y = 100 + (index * 75);
                Map map = item.Item.Map;

                if (map == null && item.Item.RootParentEntity is Mobile)
                    map = ((Mobile)item.Item.RootParentEntity).Map;

                AddImage(50, y, 2328);
                AddItem(90 - bounds.Width / 2 - bounds.X, (30 - bounds.Height / 2 - bounds.Y) + y, item.Item.ItemID, item.Item.Hue);

                AddImage(50, y, 2328);
                AddItemProperty(item.Item);

                AddItem(90 - bounds.Width / 2 - bounds.X, (30 - bounds.Height / 2 - bounds.Y) + y, item.Item.ItemID, item.Item.Hue);

                AddHtml(200, y + 5, 88, 20, String.Format("<basefont color=#F5DEB3>{0}", item.Price == -1 ? "0" : item.FormattedPrice), false, false);
                
                if(map != null)
                    AddHtml(290, y + 5, 70, 20, String.Format("<basefont color=#F5DEB3>{0}", map.ToString()), false, false);

                AddButton(370, y + 5, 30533, 30533, 100 + i, GumpButtonType.Reply, 0);

                index++;
            }

            if (Index + PerPage < Items.Count)
            {
                AddButton(430, 480, 30533, 30533, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(345, 480, 100, 20, 1044045, VendorSearchGump.C32216(VendorSearchGump.LabelColor), false, false); // NEXT PAGE
            }

            if (Index >= PerPage)
            {
                AddButton(50, 480, 30533, 30533, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 480, 100, 20, 1044044, VendorSearchGump.C32216(VendorSearchGump.LabelColor), false, false); // PREV PAGE
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                default: // Buy Map
                    VendorItem item = Items[info.ButtonID - 100];
                    PlayerVendor vendor = item.Item.RootParentEntity as PlayerVendor;
                    if(vendor != null && vendor.Map != null && vendor.Map != Map.Internal)
                    {
                        User.QuestArrow = new QuestArrow(User, vendor.Location);
                        User.QuestArrow.Update();
                        User.SendMessage("Apontando para loja...");
                        /*
                        if (_GivenTo == null)
                            _GivenTo = new Dictionary<VendorItem, List<PlayerMobile>>();

                        if (!_GivenTo.ContainsKey(item))
                            _GivenTo[item] = new List<PlayerMobile>();

                        if (!_GivenTo[item].Contains(User))
                        {
                            VendorSearchMap map = new VendorSearchMap(vendor, item.Item);

                            if (User.Backpack == null || !User.Backpack.TryDropItem(User, map, false))
                                map.Delete();
                            else
                            {
                                User.SendLocalizedMessage(1154690); // The vendor map has been placed in your backpack.
                                _GivenTo[item].Add(User);
                            }
                        }
                        */
                    }
                    else
                        User.SendLocalizedMessage("O item nao esta mais a venda"); // Item no longer for sale.
                    break;
                case 2: // Next Page
                    Index += PerPage;
                    Refresh();
                    break;
                case 3: // Prev Page
                    Index -= PerPage;
                    Refresh();
                    break;
            }
        }

        public static void Initialize()
        {
            Timer t = Timer.DelayCall(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30), () =>
                {
                    if (_GivenTo != null)
                    {
                        _GivenTo.Clear();
                        _GivenTo = null;
                    }
                });

            t.Priority = TimerPriority.OneMinute;
        }

        public static Dictionary<VendorItem, List<PlayerMobile>> _GivenTo;
    }

    public class ConfirmTeleportGump : BaseGump
    {
        public VendorSearchMap VendorMap { get; set; }

        public ConfirmTeleportGump(VendorSearchMap map, PlayerMobile pm)
            : base(pm, 10, 10)
        {
            VendorMap = map;            
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 414, 214, 0x7752);

            if (VendorMap.Vendor != null && VendorMap.SetLocation != Point3D.Zero)
                AddHtmlLocalized(27, 47, 380, 80, 1154637, String.Format("{0}\t{1}", VendorMap.GetCoords(), VendorMap.Vendor.Map.ToString()), 0xFFFF, false, false); // Please select 'Accept' if you would like to return to ~1_loc~ (~2_facet~).  This map will be deleted after use.
            else
                AddHtmlLocalized(27, 47, 380, 80, 1156475, String.Format("@{0}@{1}@{2}", VendorMap.TeleportCost.ToString(), VendorMap.Vendor.Name, VendorMap.DeleteDelayMinutes.ToString()), 0xFFFF, false, false); // Please select 'Accept' if you would like to pay ~1_cost~ gold to teleport to auction house ~2_name~. For this price you will also be able to teleport back to this location within the next ~3_minutes~ minutes.

            AddButton(7, 167, 0x7747, 0x7747, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(47, 167, 100, 40, 1150300, 0x4E73, false, false); // CANCEL

            AddButton(377, 167, 0x7746, 0x7746, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(267, 167, 100, 40, 1114514, "#1150299", 0xFFFF, false, false); // // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                default: break;
                case 1:
                    {
                        if (Banker.GetBalance(User) < VendorMap.TeleportCost)
                        {
                            User.SendLocalizedMessage(1154672); // You cannot afford to teleport to the vendor.
                        }
                        else if (!VendorMap.CheckVendor())
                        {
                            User.SendLocalizedMessage(1154643); // That item is no longer for sale.
                        }
                        else if (VendorMap.SetLocation == Point3D.Zero && !VendorSearch.CanSearch(User))
                        {
                            User.SendLocalizedMessage(1154680); //Before using vendor search, you must be in a justice region or a safe log-out location (such as an inn or a house which has you on its Owner, Co-owner, or Friends list). 
                        }
                        else if (VendorMap.SetLocation == Point3D.Zero && !VendorSearch.CanSearch(User))
                        {
                            User.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
                        }
                        else if (VendorMap.SetLocation != Point3D.Zero && (!Utility.InRange(VendorMap.SetLocation, User.Location, 100) || VendorMap.SetMap != User.Map))
                        {
                            User.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
                        }
                        else
                        {
                            new Server.Spells.Fourth.RecallSpell(User, VendorMap, VendorMap).Cast();
                        }

                        break;
                    }
            }
        }
    }
}
