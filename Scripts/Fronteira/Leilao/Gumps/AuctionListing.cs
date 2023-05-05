#region AuthorHeader
//
//	Auction version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;

using Server;
using Server.Gumps;
using Leilaum.Utilities;

namespace Server.Leilaum
{
    /// <summary>
    /// Lists auction items
    /// </summary>
    public class AuctionListing : Gump
    {
        private bool m_EnableSearch;
        private int m_Page;
        private ArrayList m_List;
        private bool m_ReturnToAuction;
        private bool playerShop = false;

        public AuctionListing(Mobile m, ArrayList items, bool searchEnabled, bool returnToAuction, int page, bool playerShop = false) : base(50, 50)
        {
            m.CloseGump(typeof(AuctionListing));
            m_EnableSearch = searchEnabled;
            m_Page = page;
            m_List = new ArrayList(items);
            var l = new ArrayList(items);

            this.playerShop = playerShop;
            m_ReturnToAuction = returnToAuction;
            MakeGump();
        }

        public AuctionListing(Mobile m, ArrayList items, bool searchEnabled, bool returnToAuction, bool playerShop = false) : this(m, items, searchEnabled, returnToAuction, 0, playerShop)
        {

        }

        private void MakeGump()
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(29, 68, 735, 362, 9200);

            if (m_List.Count > 0)
            {
                for (int i = 0; i < 10 && (m_Page * 10 + i) < m_List.Count; i++)
                {
                    AuctionItem item = m_List[m_Page * 10 + i] as AuctionItem;

                    var coluna = i % 2; // 0 ou 1
                    var linha = Math.Ceiling((i + 1) / 2d) - 1;

                    var modX = (int)(coluna * 350);
                    var modY = (int)(linha * 50);

                    this.AddBackground(49+ modX, 99+modY, 50, 50, 3000);
                    this.AddLabel(104 + modX, 102 + modY, 0, $"{item.ItemName} - {item.Owner.Name}");

                    Rectangle2D r = ItemBounds.Table[item.Item.ItemID];
                    if(r.Width < 100 && r.Height < 100)
                    {
                        NewAuctionGump.AddItemCentered(49 + modX, 99 + modY, 50, 50, item.Item.ItemID, item.Item.Hue, this);
                        AddItemProperty(item.Item);
                    }
                
                    this.AddLabel(105 + modX, 122 + modY, 0, $"{item.BuyNow} moedas");
                    this.AddButton(36 + modX, 117 + modY, 2224, 2224, 10 + i, GumpButtonType.Reply, 0);
                }
            }

            /*
            this.AddBackground(49, 99, 50, 50, 3000);
            this.AddLabel(104, 102, 0, @"2x Katana - Kataninha - Ziden");
            this.AddLabel(105, 122, 0, @"2000");
            this.AddButton(36, 117, 2224, 2224, (int)Buttons.Button1, GumpButtonType.Reply, 0);

            this.AddBackground(49, 151, 50, 50, 3000);
            this.AddLabel(104, 154, 0, @"2x Katana - Kataninha - Ziden");
            this.AddLabel(105, 174, 0, @"2000");
            this.AddButton(36, 169, 2224, 2224, (int)Buttons.CopyofButton1, GumpButtonType.Reply, 0);


            this.AddBackground(417, 97, 50, 50, 3000);
            this.AddLabel(472, 100, 0, @"2x Katana - Kataninha - Ziden");
            this.AddLabel(473, 120, 0, @"2000");
            this.AddButton(404, 115, 2224, 2224, (int)Buttons.CopyofButton1, GumpButtonType.Reply, 0);


            this.AddBackground(417, 149, 50, 50, 3000);
            this.AddLabel(472, 152, 0, @"2x Katana - Kataninha - Ziden");
            this.AddLabel(473, 172, 0, @"2000");
            this.AddButton(404, 167, 2224, 2224, (int)Buttons.CopyofCopyofButton1, GumpButtonType.Reply, 0);
            */

            this.AddLabel(661, 76, 0, @"Procurar");
            this.AddButton(630, 75, 4014, 248, 1, GumpButtonType.Reply, 0);

            this.AddButton(487, 74, 4014, 248, 2, GumpButtonType.Reply, 0);
            this.AddLabel(519, 75, 0, @"Ordenar");

            if ((m_Page + 1) * 10 < m_List.Count)
            {
                this.AddLabel(641, 406, 0, @"Proximo");
                this.AddButton(703, 405, 4007, 4007, 3, GumpButtonType.Reply, 0);
            }

            if (m_Page > 0)
            {
                this.AddButton(62, 406, 4014, 4014, 4, GumpButtonType.Reply, 0);
                this.AddLabel(102, 407, 0, @"Anterior");
            }


            this.AddImage(26, 67, 10460);
            this.AddImage(734, 67, 10460);
            this.AddImage(28, 401, 10460);
            this.AddImage(736, 401, 10460);
            this.AddImage(309, 20, 10452);
            this.AddImage(377, 16, 3823);


            /*
            AddImageTiled(49, 39, 402, 352, 3004);
            AddImageTiled(50, 40, 400, 350, 2624);
            AddAlphaRegion(50, 40, 400, 350);
            AddImage(165, 65, 10452);
            AddImage(0, 20, 10400);
            AddImage(0, 330, 10402);
            AddImage(35, 20, 10420);
            AddImage(421, 20, 10410);
            AddImage(410, 20, 10430);
            AddImageTiled(90, 32, 323, 16, 10254);
            AddLabel(160, 45, LUtils.kGreenHue, AuctionSystem.ST[8]);
            AddImage(420, 330, 10412);
            AddImage(420, 175, 10411);
            AddImage(0, 175, 10401);

            // Search: BUTTON 1
            if (m_EnableSearch)
            {
                AddLabel(305, 120, LUtils.kLabelHue, AuctionSystem.ST[16]);
                AddButton(270, 120, 4005, 4006, 1, GumpButtonType.Reply, 0);
            }

            // Sort: BUTTON 2
            AddLabel(395, 120, LUtils.kLabelHue, AuctionSystem.ST[17]);
            AddButton(360, 120, 4005, 4006, 2, GumpButtonType.Reply, 0);

            while (m_Page * 10 >= m_List.Count)
                m_Page--;

            if (m_List.Count > 0)
            {
                // Display the page number
                AddLabel(360, 95, LUtils.kRedHue, string.Format(AuctionSystem.ST[18], m_Page + 1, (m_List.Count - 1) / 10 + 1));
                AddLabel(70, 120, LUtils.kRedHue, string.Format(AuctionSystem.ST[19], m_List.Count));
            }
            else
                AddLabel(70, 120, LUtils.kRedHue, AuctionSystem.ST[20]);

            // Display items: BUTTONS 10 + i

            int lower = m_Page * 10;

            if (m_List.Count > 0)
            {
                for (int i = 0; i < 10 && (m_Page * 10 + i) < m_List.Count; i++)
                {
                    AuctionItem item = m_List[m_Page * 10 + i] as AuctionItem;

                    

                    AddButton(115, 153 + i * 20, 5601, 5605, 10 + i, GumpButtonType.Reply, 0);
                    AddLabelCropped(140, 150 + i * 20, 260, 20, LUtils.kLabelHue, item.ItemName);
                }
            }

            // Next page: BUTTON 3
            if ((m_Page + 1) * 10 < m_List.Count)
            {
                AddLabel(355, 360, LUtils.kLabelHue, AuctionSystem.ST[22]);
                AddButton(315, 360, 4005, 4006, 3, GumpButtonType.Reply, 0);
            }

            // Previous page: BUTTON 4
            if (m_Page > 0)
            {
                AddLabel(180, 360, LUtils.kLabelHue, AuctionSystem.ST[21]);
                AddButton(280, 360, 4014, 4015, 4, GumpButtonType.Reply, 0);
            }

            // Close: BUTTON 0
            AddLabel(115, 360, LUtils.kLabelHue, AuctionSystem.ST[7]);
            AddButton(75, 360, 4017, 4018, 0, GumpButtonType.Reply, 0);
            */
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (!AuctionSystem.Running)
            {
                sender.Mobile.SendMessage(AuctionConfig.MessageHue, AuctionSystem.ST[15]);
                return;
            }

            switch (info.ButtonID)
            {
                case 0: // Exit
                    if (playerShop)
                    {
                        return;
                    }
                    if (m_ReturnToAuction)
                        sender.Mobile.SendGump(new AuctionGump(sender.Mobile));
                    else
                        sender.Mobile.SendGump(new MyAuctionGump(sender.Mobile, null));
                    break;

                case 1: // Search
                    sender.Mobile.SendGump(new AuctionSearchGump(sender.Mobile, m_List, m_ReturnToAuction));
                    break;

                case 2: // Sort
                    sender.Mobile.SendGump(new AuctionSortGump(sender.Mobile, m_List, m_ReturnToAuction, m_EnableSearch));
                    break;

                case 3: // Next page
                    sender.Mobile.SendGump(new AuctionListing(sender.Mobile, m_List, m_EnableSearch, m_ReturnToAuction, m_Page + 1, this.playerShop));
                    break;

                case 4: // Previous page
                    sender.Mobile.SendGump(new AuctionListing(sender.Mobile, m_List, m_EnableSearch, m_ReturnToAuction, m_Page - 1, this.playerShop));
                    break;

                default:
                    int index = m_Page * 10 + info.ButtonID - 10;

                    if (index < 0 || index >= m_List.Count)
                    {
                        // Apparently in some cases this can go out of bounds, investigating.

                        sender.Mobile.SendMessage(AuctionConfig.MessageHue, AuctionSystem.ST[23]);

                        if (m_ReturnToAuction)
                            sender.Mobile.SendGump(new AuctionGump(sender.Mobile));
                        else
                            sender.Mobile.SendGump(new MyAuctionGump(sender.Mobile, null));

                        return;
                    }

                    AuctionItem item = m_List[index] as AuctionItem;

                    if (item != null)
                    {
                        if ((!item.Expired || item.Pending) && (AuctionSystem.Auctions.Contains(item) || AuctionSystem.Pending.Contains(item)))
                        {
                            sender.Mobile.SendGump(new AuctionViewGump(sender.Mobile, item, new AuctionGumpCallback(ViewCallback), this.playerShop));
                        }
                        else
                        {
                            sender.Mobile.SendMessage(AuctionConfig.MessageHue, AuctionSystem.ST[24]);
                            sender.Mobile.SendGump(new AuctionListing(sender.Mobile, m_List, m_EnableSearch, m_ReturnToAuction, m_Page, this.playerShop));
                        }
                    }
                    break;
            }
        }

        private void ViewCallback(Mobile user)
        {
            user.SendGump(new AuctionListing(user, m_List, m_EnableSearch, m_ReturnToAuction, m_Page, playerShop));
        }
    }
}
