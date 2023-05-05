using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using Server.Network;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.VvV
{
    public class GuildLeaderboardGump : Gump
    {
        public static int PerPage = 10;

        public PlayerMobile User { get; set; }
        public Filter Filter { get; set; }

        public GuildLeaderboardGump(PlayerMobile pm, Filter filter = Filter.Score) : base(50, 50)
        {
            User = pm;
            Filter = filter;

            AddPage(0);
            AddBackground(0, 0, 560, 320, 9200);
            AddBackground(10, 10, 540, 300, 3000);

            AddHtml(0, 12, 560, 20, "<CENTER>Guerra Infinita</CENTER>", 0, false, false); // <DIV ALIGN=CENTER>Vice Vs Virtue - Guild Rankings</DIV>

            AddHtml(10, 55, 60, 20, "<DIV ALIGN=CENTER>#:</DIV>", 0, false, false); // <DIV ALIGN=CENTER>#:</DIV>
            AddHtml(50, 55, 180, 20, "<DIV ALIGN=CENTER>Guilda:</DIV>", 0, false, false); // <DIV ALIGN=CENTER>Guild:</DIV>
            AddHtml(230, 55, 100, 20, "<DIV ALIGN=RIGHT>Pontos:</DIV>", Filter == Filter.Score ? 78 : 0, false, false); // <DIV ALIGN=RIGHT>Score:</DIV>
            AddHtml(330, 55, 85, 20, "<DIV ALIGN=RIGHT>Kills:</DIV>", Filter == Filter.Kills ? 78 : 0, false, false); // <DIV ALIGN=RIGHT>Kills:</DIV>
            AddHtml(425, 55, 95, 20, "<DIV ALIGN=RIGHT>Sigilos:</DIV>", Filter == Filter.ReturnedSigils ? 78 : 0, false, false); // <DIV ALIGN=RIGHT>Returned Sigil:</DIV>

            if (Filter != Filter.Score)
                AddButton(330, 55, 2437, 2438, 1, GumpButtonType.Reply, 0);
            else
                AddImage(330, 55, 10006);

            if (Filter != Filter.Kills)
                AddButton(415, 55, 2437, 2438, 2, GumpButtonType.Reply, 0);
            else
                AddImage(415, 55, 10006);

            if (Filter != Filter.ReturnedSigils)
                AddButton(520, 55, 2437, 2438, 3, GumpButtonType.Reply, 0);
            else
                AddImage(520, 55, 10006);

            AddButton(280, 290, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddHtml(315, 290, 150, 20, "Ranking Guildas", 0, false, false); // Guild Rankings

            List<VvVGuildStats> list = new List<VvVGuildStats>(ViceVsVirtueSystem.Instance.GuildStats.Values);

            switch (Filter)
            {
                default:
                case Filter.Score: list = list.OrderBy(e => -e.Score).ToList(); break;
                case Filter.Kills: list = list.OrderBy(e => -e.Kills).ToList(); break;
                case Filter.ReturnedSigils: list = list.OrderBy(e => -e.ReturnedSigils).ToList(); break;
            }

            int pages = (int)Math.Ceiling((double)list.Count / PerPage);
            int y = 75;
            int page = 1;
            int pageindex = 0;

            if (pages < 1)
                pages = 1;

            AddPage(page);

            AddHtml(60, 290, 150, 20, $"Pag. {page.ToString()}/{pages.ToString()}", 0, false, false); // Page ~1_CUR~ of ~2_MAX~

            for (int i = 0; i < list.Count; i++)
            {
                VvVGuildStats entry = list[i];

                AddHtml(10, y, 65, 20, CenterGray((i + 1).ToString() + "."), false, false);
                AddHtml(50, y, 180, 20, CenterGray(entry.Guild == null ? "" : entry.Guild.Name), false, false);
                AddHtml(230, y, 100, 20, Filter == Filter.Score ? RightGreen(entry.Score.ToString()) : RightGray(entry.Score.ToString()), false, false);
                AddHtml(330, y, 85, 20, Filter == Filter.Kills ? RightGreen(entry.Kills.ToString()) : RightGray(entry.Kills.ToString()), false, false);
                AddHtml(425, y, 95, 20, Filter == Filter.ReturnedSigils ? RightGreen(entry.ReturnedSigils.ToString()) : RightGray(entry.ReturnedSigils.ToString()), false, false);

                y += 20;
                pageindex++;

                if (pageindex == PerPage)
                {
                    AddHtmlLocalized(60, 290, 150, 20, 1153561, String.Format("{0}\t{1}", page.ToString(), pages.ToString()), 0, false, false); // Page ~1_CUR~ of ~2_MAX~

                    if (i > 0 && i < list.Count - 1)
                    {
                        AddButton(200, 290, 4005, 4007, 0, GumpButtonType.Page, page + 1);

                        page++;
                        y = 75;
                        pageindex = 0;
                        AddPage(page);

                        AddButton(170, 290, 4014, 4016, 0, GumpButtonType.Page, page - 1);
                    }
                }
            }

            list.Clear();
            list.TrimExcess();
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                default: break;
                case 1:
                case 2:
                case 3:
                    Filter f = (Filter)info.ButtonID - 1;
                    User.SendGump(new GuildLeaderboardGump(User, f));
                    break;
                case 4:
                    User.SendGump(new ViceVsVirtueLeaderboardGump(User));
                    break;
            }
        }

        private string CenterGray(string format)
        {
            return String.Format("<basefont color=#000000><DIV ALIGN=CENTER>{0}</DIV>", format);
        }

        private string RightGray(string format)
        {
            return String.Format("<basefont color=#000000><DIV ALIGN=RIGHT>{0}</DIV>", format);
        }

        private string LeftGray(string format)
        {
            return String.Format("<basefont color=#000000><DIV ALIGN=LEFT>{0}</DIV>", format);
        }

        private string RightGreen(string format)
        {
            return String.Format("<basefont color=#000000><DIV ALIGN=RIGHT>{0}</DIV>", format);
        }
    }
}
