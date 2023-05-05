#region AuthorHeader
//
//	Auction version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Globalization;
using Server;
using Server.Gumps;
using Leilaum.Utilities;
using Server.Items;

namespace Server.Leilaum
{
	/// <summary>
	/// This gump displays the general information about an auction
	/// </summary>
	public class AuctionViewGump : Gump
	{
		private const int kHueExampleID = 7107;
		private const int kBeigeBorderOuter = 2524;
		private const int kBeigeBorderInner = 2624;

		private Mobile m_User;
		private AuctionItem m_Auction;
		private int m_Page = 0;
		private AuctionGumpCallback m_Callback;
        private bool playerShop = false;

		public AuctionViewGump( Mobile user, AuctionItem auction, bool playerShop = false ) : this( user, auction, null, playerShop)
		{
		}

		public AuctionViewGump( Mobile user, AuctionItem auction, AuctionGumpCallback callback, bool playerShop = false ) : this( user, auction, callback, 0, playerShop)
		{
		}

		public AuctionViewGump( Mobile user, AuctionItem auction, AuctionGumpCallback callback, int page , bool playerShop = false) : base( 50, 50 )
		{
            this.playerShop = false;
			m_Page = page;
			m_User = user;
			m_Auction = auction;
			m_Callback = callback;

			MakeGump();
		}

		/// <summary>
		/// Gets the item hue
		/// </summary>
		/// <param name="item">The item to get the hue of</param>
		/// <returns>A positive hue value</returns>
		private int GetItemHue( Item item )
		{
			if ( null == item )
				return 0;

			int hue = item.Hue == 1 ? AuctionConfig.BlackHue : item.Hue;

			hue &= 0x7FFF;	// Some hues are | 0x8000 for some reason, but it leads to the same hue

			// Validate in case the hue was shifted by some other value

			return ( hue < 0 || hue >= 3000 ) ? 0 : hue;
		}

		private void MakeGump()
		{
			AuctionItem.ItemInfo item = m_Auction[ m_Page ];

			if ( item == null )
				return;

			int itemHue = GetItemHue( item.Item );

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			// The page and background
			AddBackground( 0, -2, 502, 370, 9200 );
			//AddImageTiled( 4, 4, 492, 362, kBeigeBorderOuter );
			//AddImageTiled( 5, 5, 490, 360, kBeigeBorderInner );
			//AddAlphaRegion( 5, 5, 490, 360);

			//
			// The item display area
			//
			//AddImageTiled( 4, 4, 156, 170, kBeigeBorderOuter );
			//AddImageTiled( 5, 5, 154, 168, kBeigeBorderInner );
			//AddAlphaRegion( 5, 5, 154, 168);
            AddBackground(5, 5, 155, 140, 3500);

            // Item image goes here
            if ( item.Item != null )
			{
                NewAuctionGump.AddItemCentered(5, 5, 155, 140, item.Item.ItemID, item.Item.Hue, this);
				AddItemProperty(item.Item);
			}
			// Hue preview image goes here if the item has a hue
			if ( item.Item != null && 0 != itemHue )
			{
                //AddImageTiled( 30, 140, 107, 24, 3004 );
                //AddImageTiled( 31, 141, 105, 22, kBeigeBorderInner );
                AddBackground( 31, 141, 105, 22, 3000);
				AddLabel( 37, 142, LUtils.kLabelHue, AuctionSystem.ST[ 82 ]  );
				AddItem( 90, 141, kHueExampleID, itemHue );
			}

            //
            // The Auction info area
            //
            //AddImageTiled( 4, 169, 156, 196, kBeigeBorderOuter );
            //AddImageTiled( 5, 170, 154, 195, kBeigeBorderInner );
            AddBackground( 5, 170, 154, 195, 3000);

			// Reserve and bids
            /*
            if(!this.playerShop)
            {
                AddLabel(10, 175, LUtils.kLabelHue, AuctionSystem.ST[68]);
                AddLabel(45, 190, LUtils.kGreenHue, m_Auction.MinBid.ToString("#,0"));

                AddLabel(10, 280, LUtils.kLabelHue, AuctionSystem.ST[69]);
                AddLabel(45, 295, m_Auction.ReserveMet ? LUtils.kGreenHue : LUtils.kRedHue, m_Auction.ReserveMet ? "Met" : "Not Met");

                AddLabel(10, 210, LUtils.kLabelHue, AuctionSystem.ST[70]);

                if (m_Auction.HasBids)
                    AddLabel(45, 225, m_Auction.ReserveMet ? LUtils.kGreenHue : LUtils.kRedHue, m_Auction.HighestBid.Amount.ToString("#,0"));
                else
                    AddLabel(45, 225, LUtils.kRedHue, AuctionSystem.ST[71]);
            }
            */

			// Time remaining
			string timeleft = null;

			AddLabel( 10, 245, LUtils.kLabelHue, AuctionSystem.ST[ 56 ] );

			if ( ! m_Auction.Expired )
			{
				if ( m_Auction.TimeLeft >= TimeSpan.FromDays( 1 ) )
					timeleft = string.Format( AuctionSystem.ST[ 73 ] , m_Auction.TimeLeft.Days, m_Auction.TimeLeft.Hours );
				else if ( m_Auction.TimeLeft >= TimeSpan.FromMinutes( 60 ) )
					timeleft = string.Format( AuctionSystem.ST[ 74 ] , m_Auction.TimeLeft.Hours );
				else if ( m_Auction.TimeLeft >= TimeSpan.FromSeconds( 60 ) )
					timeleft = string.Format( AuctionSystem.ST[ 75 ] , m_Auction.TimeLeft.Minutes );
				else
					timeleft = string.Format( AuctionSystem.ST[ 76 ] , m_Auction.TimeLeft.Seconds );
			}
			else if ( m_Auction.Pending )
			{
				timeleft = AuctionSystem.ST[ 77 ] ;
			}
			else
			{
				timeleft = AuctionSystem.ST[ 78 ] ;
			}
			AddLabel( 45, 260, LUtils.kGreenHue, timeleft );

            /*
            if(!this.playerShop)
            {
                // Bidding
                if (m_Auction.CanBid(m_User) && !m_Auction.Expired)
                {
                    AddLabel(10, 318, LUtils.kLabelHue, AuctionSystem.ST[79]);
                    AddImageTiled(9, 338, 112, 22, kBeigeBorderOuter);
                    AddImageTiled(10, 339, 110, 20, kBeigeBorderInner);
                    AddAlphaRegion(10, 339, 110, 20);

                    // Bid text: 0
                    AddTextEntry(10, 339, 110, 20, LUtils.kGreenHue, 0, @"");

                    // Bid button: 4
                    AddButton(125, 338, 4011, 4012, 4, GumpButtonType.Reply, 0);
                }
                else if (m_Auction.IsOwner(m_User))
                {
                    // View bids: button 5
                    AddLabel(10, 338, LUtils.kLabelHue, AuctionSystem.ST[80]);
                    AddButton(125, 338, 4011, 4012, 5, GumpButtonType.Reply, 0);
                }
            }
            */
			
			//
			// Item properties area
			//
			//AddImageTiled( 169, 29, 317, 142, kBeigeBorderOuter );
			//( 170, 30, 315, 140, kBeigeBorderInner );
			AddBackground( 170, 30, 315, 140, 3000 );

			// If it is a container make room for the arrows to navigate to each of the items
			if ( m_Auction.ItemCount > 1 )
			{
				AddLabel( 170, 10, LUtils.kGreenHue, string.Format( AuctionSystem.ST[ 231 ] , m_Auction.ItemName ));

                //AddImageTiled( 169, 29, 317, 27, kBeigeBorderOuter );
                //AddImageTiled( 170, 30, 315, 25, kBeigeBorderInner );
                AddBackground( 170, 30, 315, 25, 3000 );
				AddLabel( 185, 35, LUtils.kGreenHue, string.Format( AuctionSystem.ST[ 67 ] , m_Page + 1, m_Auction.ItemCount ) );

				// Prev Item button: 1
				if ( m_Page > 0 )
				{
					AddButton( 415, 31, 4014, 4015, 1, GumpButtonType.Reply, 0 );
				}

				// Next Item button: 2
				if ( m_Page < m_Auction.ItemCount - 1 )
				{
					AddButton( 450, 31, 4005, 4006, 2, GumpButtonType.Reply, 0 );
				}

				//AddHtml( 173, 56, 312, 114, m_Auction[ m_Page ].Properties, (bool)false, (bool)true );
				AddHtml( 173, 56, 312, 114, Gump.Cor("Deixe o mouse em cima do item para ver suas propriedades.", "black"), (bool)false, (bool)true );
			}
			else
			{
                var qtd = m_Auction.Item.Amount;
                var nome = m_Auction.Item.Name;
                if (nome == null)
                    nome = m_Auction.Item.ItemData.Name;
                if (nome == null)
                    nome = "Item";

                var str = qtd + "x " + nome;
                AddLabel( 170, 10, LUtils.kGreenHue, m_Auction.ItemName );
				//AddHtml( 173, 30, 312, 140, m_Auction[ m_Page ].Properties, (bool)false, (bool)true );
				AddHtml( 173, 30, 312, 140, str+"<br>Deixe o mouse em cima do item para ver suas propriedades.", (bool)false, (bool)true );
			}
			
			//
			// Owner description area
			//
			//AddImageTiled( 169, 194, 317, 112, kBeigeBorderOuter );
			AddLabel( 170, 175, LUtils.kLabelHue, AuctionSystem.ST[ 81 ] );
            //AddImageTiled( 170, 195, 315, 110, kBeigeBorderInner );
            AddBackground( 170, 195, 315, 110, 3000);
			AddHtml( 173, 195, 312, 110, string.Format( "<basefont>{0}", m_Auction.Description ), (bool)false, (bool)true);
			
			// Web link button: 3
			if ( m_Auction.WebLink != null && m_Auction.WebLink.Length > 0 )
			{
				AddLabel( 350, 175, LUtils.kLabelHue, AuctionSystem.ST[ 72 ] );
				AddButton( 415, 177, 5601, 5605, 3, GumpButtonType.Reply, 0 );
			}

			//
			// Auction controls
			//

			// Buy now : Button 8
			if ( m_Auction.AllowBuyNow && m_Auction.CanBid( m_User ) && !m_Auction.Expired )
			{
				AddButton( 170, 310, 4029, 4030, 8, GumpButtonType.Reply, 0 );
                var buyOut = (double)m_Auction.BuyNow;
                if(playerShop)
                {
                   // buyOut = buyOut * 0.9; // 10% desconto comprar em loja
                }
                AddLabel( 205, 312, LUtils.kGreenHue, string.Format( AuctionSystem.ST[ 210 ], ((int)buyOut).ToString( "#,0" )));
			}

            // Button 6 : Admin Auction Panel
            if (m_User.AccessLevel >= AuctionConfig.AuctionAdminAcessLevel)
            {
                AddButton(170, 338, 4011, 4012, 6, GumpButtonType.Reply, 0);
                AddLabel(205, 340, LUtils.kLabelHue, AuctionSystem.ST[227]);
            } else if (m_Auction.IsOwner(m_User))
            {
                AddButton(170, 338, 4011, 4012, 666, GumpButtonType.Reply, 0);
                AddLabel(205, 340, LUtils.kLabelHue, "Pegar Item");
            }

			// Close button: 0
			AddButton( 455, 338, 4017, 4018, 0, GumpButtonType.Reply, 0 );
			AddLabel( 415, 340, LUtils.kLabelHue, AuctionSystem.ST[ 7 ] );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( ! AuctionSystem.Running )
			{
				sender.Mobile.SendMessage( AuctionConfig.MessageHue, AuctionSystem.ST[ 15 ] );
				return;
			}

			if ( !AuctionSystem.Auctions.Contains( m_Auction ) )
			{
				sender.Mobile.SendMessage( AuctionConfig.MessageHue, AuctionSystem.ST[ 207 ] );
				
				if ( m_Callback != null )
				{
					try { m_Callback.DynamicInvoke( new object[] { m_User } ) ; }
					catch {}
				}

				return;
			}

			switch ( info.ButtonID )
			{
				case 0: // Close

					if ( m_Callback != null )
					{
						try { m_Callback.DynamicInvoke( new object[] { m_User } ); }
						catch {}
					}
					break;

				case 1: // Prev item

					m_User.SendGump( new AuctionViewGump( m_User, m_Auction, m_Callback, m_Page - 1, this.playerShop ) );
					break;

				case 2: // Next item

					m_User.SendGump( new AuctionViewGump( m_User, m_Auction, m_Callback, m_Page + 1, this.playerShop) );
					break;

				case 3: // Web link

					m_User.SendGump( new AuctionViewGump( m_User, m_Auction, m_Callback, m_Page, this.playerShop) );
					m_Auction.SendLinkTo( m_User );
					break;

				case 4: // Bid

					uint bid = 0;

					try { bid = uint.Parse( info.TextEntries[ 0 ].Text, NumberStyles.AllowThousands ); }
					catch {}

					if ( m_Auction.Expired )
					{
						m_User.SendMessage( AuctionConfig.MessageHue, AuctionSystem.ST[ 84 ] );
					}
					else if ( bid == 0 )
					{
						m_User.SendMessage( AuctionConfig.MessageHue, AuctionSystem.ST[ 85 ]  );
					}
					else
					{
                        m_Auction.PlaceBid(m_User, (int)bid);
                        /*
						if ( m_Auction.AllowBuyNow && bid >= m_Auction.BuyNow )
						{
							// Do buy now instead
							goto case 8;
						}
						else
						{
							m_Auction.PlaceBid( m_User, (int) bid );
						}
                        */
                    }

					m_User.SendGump( new AuctionViewGump( m_User, m_Auction, m_Callback, m_Page, this.playerShop) );
					break;

				case 5: // View bids

					m_User.SendGump( new BidViewGump( m_User, m_Auction.Bids, new AuctionGumpCallback( BidViewCallback ) ) );
					break;

				case 6: // Staff Panel

					m_User.SendGump( new AuctionControlGump( m_User, m_Auction, this ) );
					break;

				case 8: // Buy Now

					if ( m_Auction.DoBuyNow( sender.Mobile, playerShop ) )
					{
						goto case 0; // Close the gump
					}
					else
					{
						sender.Mobile.SendGump( new AuctionViewGump( sender.Mobile, m_Auction, m_Callback, m_Page, this.playerShop) );
					}
					break;
                case 666: // Pegar Item
                    m_Auction.ForceEnd();
                    break;
            }
		}

		private void BidViewCallback( Mobile m )
		{
			m.SendGump( new AuctionViewGump( m, m_Auction, m_Callback, m_Page, this.playerShop) );
		}
	}
}
