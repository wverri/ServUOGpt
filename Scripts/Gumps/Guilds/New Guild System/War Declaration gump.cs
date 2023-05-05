using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Guilds
{
    public class WarDeclarationGump : BaseGuildGump
    {
        private readonly Guild m_Other;
        public WarDeclarationGump(PlayerMobile pm, Guild g, Guild otherGuild)
            : base(pm, g)
        {
            this.m_Other = otherGuild;
            WarDeclaration war = g.FindPendingWar(otherGuild);

            this.AddPage(0);

            this.AddBackground(0, 0, 500, 340, 0x24AE);
            this.AddBackground(65, 50, 370, 30, 0x2486);
            this.AddHtml(75, 55, 370, 26, "<div align=center><i>Declarar Guerra</i></div>", 0x3C00, false, false); // <div align=center><i>Declaration of War</i></div>
            this.AddImage(410, 45, 0x232C);
            this.AddHtml(65, 95, 200, 20, "Duracao", 0x14AF, false, false); // <i>Duration of War</i>
            this.AddHtml(65, 120, 400, 20, "Digite a duracao em horas da guerra",  false, false); // Enter the number of hours the war will last.
            this.AddBackground(65, 150, 40, 30, 0x2486);
            this.AddTextEntry(70, 154, 50, 30, 0x481, 10, (war != null) ? war.WarLength.Hours.ToString() : "0");
            this.AddHtml(65, 195, 200, 20, "Condicoes de Vitoria", 0x14AF, false, false); // <i>Victory Condition</i>
            this.AddHtml(65, 220, 400, 20, "Digite a quantidade de kills",  false, false); // Enter the winning number of kills.
            this.AddBackground(65, 250, 40, 30, 0x2486);
            this.AddTextEntry(70, 254, 50, 30, 0x481, 11, (war != null) ? war.MaxKills.ToString() : "0");
            this.AddBackground(190, 270, 130, 26, 0x2486);
            this.AddButton(195, 275, 0x845, 0x846, 0, GumpButtonType.Reply, 0);
            this.AddHtml(220, 273, 90, 26, "Cancelar",  false, false); // Cancel
            this.AddBackground(330, 270, 130, 26, 0x2486);
            this.AddButton(335, 275, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
            this.AddHtml(360, 273, 90, 26, "Declarar guerra", 0x5000, false, false); // Declare War!
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (!IsMember(pm, this.guild))
                return;

            RankDefinition playerRank = pm.GuildRank;

            switch( info.ButtonID )
            {
                case 1:
                    {
                        AllianceInfo alliance = this.guild.Alliance;
                        AllianceInfo otherAlliance = this.m_Other.Alliance;	

                        if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                        {
                            pm.SendMessage("Sem permissao"); // You don't have permission to negotiate wars.
                        }
                        else if (alliance != null && alliance.Leader != this.guild)
                        {
                            pm.SendMessage(String.Format("{0} nao eh o lider de \t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                            pm.SendMessage("Negocie com "+alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                        }
                        else if (otherAlliance != null && otherAlliance.Leader != this.m_Other)
                        {
                            pm.SendMessage(String.Format("{0} nao eh o lider de \t{1}", this.m_Other.Name, otherAlliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                            pm.SendMessage("Negocie com "+otherAlliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                        }
                        else
                        {
                            WarDeclaration activeWar = this.guild.FindActiveWar(this.m_Other);

                            if (activeWar == null)
                            {
                                WarDeclaration war = this.guild.FindPendingWar(this.m_Other);
                                WarDeclaration otherWar = this.m_Other.FindPendingWar(this.guild);

                                //Note: OSI differs from what it says on website.  unlimited war = 0 kills/ 0 hrs.  Not > 999.  (sidenote: they both cap at 65535, 7.5 years, but, still.)
                                TextRelay tKills = info.GetTextEntry(11);
                                TextRelay tWarLength = info.GetTextEntry(10);

                                int maxKills = (tKills == null) ? 0 : Math.Max(Math.Min(Utility.ToInt32(info.GetTextEntry(11).Text), 0xFFFF), 0);
                                TimeSpan warLength = TimeSpan.FromHours((tWarLength == null) ? 0 : Math.Max(Math.Min(Utility.ToInt32(info.GetTextEntry(10).Text), 0xFFFF), 0));

                                if (war != null)
                                {
                                    war.MaxKills = maxKills;
                                    war.WarLength = warLength;
                                    war.WarRequester = true;
                                }
                                else
                                {
                                    this.guild.PendingWars.Add(new WarDeclaration(this.guild, this.m_Other, maxKills, warLength, true));
                                }

                                if (otherWar != null)
                                {
                                    otherWar.MaxKills = maxKills;
                                    otherWar.WarLength = warLength;
                                    otherWar.WarRequester = false;
                                }
                                else
                                {
                                    this.m_Other.PendingWars.Add(new WarDeclaration(this.m_Other, this.guild, maxKills, warLength, false));
                                }

                                if (war != null)
                                {
                                    pm.SendMessage("Atualizado"); // The proposal has been updated.
                                    //m_Other.GuildMessage( 1070782 ); // ~1_val~ has responded to your proposal.
                                }
                                else
                                    this.m_Other.GuildTextMessage(((this.guild.Alliance != null) ? this.guild.Alliance.Name : this.guild.Name) +" propos uma GUERRA !!!!"); // ~1_val~ has proposed a war.

                                pm.SendMessage("Proposta de guerra enviada a "+((this.m_Other.Alliance != null) ? this.m_Other.Alliance.Name : this.m_Other.Name)); // War proposal has been sent to ~1_val~.
                            }
                        }
                        break;
                    }
                default:
                    {
                        pm.SendGump(new OtherGuildInfo(pm, this.guild, this.m_Other));
                        break;
                    }
            }
        }
    }
}
