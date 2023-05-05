using System;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using System.Linq;
using Server.Ziden.Traducao;

namespace Server.Engines.BulkOrders
{
    public class LargeBODGump : Gump
    {
        private readonly LargeBOD m_Deed;
        private readonly Mobile m_From;
        public LargeBODGump(Mobile from, LargeBOD deed)
            : base(25, 25)
        {
            m_From = from;
            m_Deed = deed;

            m_From.CloseGump(typeof(LargeBODGump));
            m_From.CloseGump(typeof(SmallBODGump));

            LargeBulkEntry[] entries = deed.Entries;

            AddPage(0);

            int height = 0;

            if (BulkOrderSystem.NewSystemEnabled)
            {
                if (deed.RequireExceptional || deed.Material != BulkMaterialType.None)
                    height += 24;

                if (deed.RequireExceptional)
                    height += 24;

                if (deed.Material != BulkMaterialType.None)
                    height += 24;
            }

            AddBackground(50, 10, 455, 218 + height + (entries.Length * 24), 1579);

            //AddBackground(58, 20, 438, 200 + height + (entries.Length * 24), 9350);
            AddAlphaRegion(58, 20, 438, 200 + height + (entries.Length * 24));

            AddImage(45, 5, 10460);
            AddImage(480, 5, 10460);
            AddImage(45, 203 + height + (entries.Length * 24), 10460);
            AddImage(480, 203 + height + (entries.Length * 24), 10460);

            AddHtmlLocalized(225, 25, 120, 20, 1045134, 1, false, false); // A large bulk order

            AddHtmlLocalized(75, 48, 250, 20, 1045138, 1, false, false); // Amount to make:
            AddLabel(275, 48, 1152, deed.AmountMax.ToString());

            AddHtmlLocalized(75, 72, 120, 20, 1045137, 1, false, false); // Items requested:
            AddHtmlLocalized(275, 76, 200, 20, 1045153, 1, false, false); // Amount finished:

            int y = 96;

            for (int i = 0; i < entries.Length; ++i)
            {
                LargeBulkEntry entry = entries[i];
                SmallBulkEntry details = entry.Details;

                var nome = Trads.GetNome(details.Type);
                if(nome != null)
                {
                    AddHtml(75, y, 210, 20, nome, 1, false, false);
                } else
                {
                    AddHtmlLocalized(75, y, 210, 20, details.Number, 1, false, false);
                }
                AddLabel(275, y, 0x480, entry.Amount.ToString()+"/"+deed.AmountMax);

                if(entry.Amount >= deed.AmountMax)
                {
                    AddLabel(325, y, 78, "[Completo]");
                }

                y += 24;
            }

            if (deed.RequireExceptional || deed.Material != BulkMaterialType.None)
            {
                AddHtmlLocalized(75, y, 200, 20, 1045140, 1, false, false); // Special requirements to meet:
                y += 24;
            }

            if (deed.RequireExceptional)
            {
                AddHtmlLocalized(75, y, 300, 20, 1045141, 1, false, false); // All items must be exceptional.
                y += 24;
            }

            if (deed.Material != BulkMaterialType.None)
            {
                AddHtml(75, y, 300, 20,"Feitos de "+deed.Material, 1, false, false); // All items must be made with x material.
                y += 24;
            }

            if (BulkOrderSystem.NewSystemEnabled)
            {
                BODContext c = BulkOrderSystem.GetContext((PlayerMobile)from);

                int points = 0;
                double banked = 0.0;

                BulkOrderSystem.ComputePoints(deed, out points, out banked);

                AddHtml(75, y, 500, 20, String.Format("Vale {0} pontos", banked.ToString("0.00")), 1, false, false); // Worth ~1_POINTS~ turn in points and ~2_POINTS~ bank points.
                y += 24;

                /*
                AddButton(125, y, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtml(160, y, 300, 20, c.PointsMode == PointsMode.Enabled ? "Pontos Habilitados" : c.PointsMode == PointsMode.Disabled ? "Pontos Desabilitados" : "Pontos Automaticos", 1, false, false); // Banking Points Enabled/Disabled/Automatic
                y += 24;
                */

                AddButton(125, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, y, 300, 20, 1045154, 1, false, false); // Combine this deed with the item requested.
                y += 24;

                AddButton(125, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, y, 300, 20, 1157304, 1, false, false); // Combine this deed with contained items.
                y += 24;

                AddButton(125, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, y, 120, 20, 1011441, 1, false, false); // EXIT
            }
            else
            {
                AddButton(125, 168 + (entries.Length * 24), 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, 168 + (entries.Length * 24), 300, 20, 1045155, 1, false, false); // Combine this deed with another deed.

                AddButton(125, 192 + (entries.Length * 24), 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, 192 + (entries.Length * 24), 120, 20, 1011441, 1, false, false); // EXIT
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Deed.Deleted || !m_Deed.IsChildOf(m_From.Backpack))
                return;

            if (info.ButtonID == 2) // Combine
            {
                m_From.SendGump(new LargeBODGump(m_From, m_Deed));
                m_Deed.BeginCombine(m_From);
            }
            else if (info.ButtonID == 3) // bank button
            {
                BODContext c = BulkOrderSystem.GetContext(m_From);

                if (c != null)
                {
                    switch (c.PointsMode)
                    {
                        case PointsMode.Enabled: c.PointsMode = PointsMode.Disabled; break;
                        case PointsMode.Disabled: c.PointsMode = PointsMode.Automatic; break;
                        case PointsMode.Automatic: c.PointsMode = PointsMode.Enabled; break;
                    }
                }

                m_From.SendGump(new LargeBODGump(m_From, m_Deed));
            }
            else if (info.ButtonID == 4) // combine from container
            {
                m_From.SendMessage("Selecione um container ou mochila para pegar os items");
                m_From.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (m, targeted) =>
                {
                    if (!m_Deed.Deleted && targeted is Container)
                    {
                        Item [] list = ((Container)targeted).FindItemsByType(typeof(Item), false);
                        foreach (var item in list)
                        {
                            m_Deed.EndCombine(m_From, item, false);
                        }

                        list.Clear();
                    }
                });
            }
        }
    }
}
