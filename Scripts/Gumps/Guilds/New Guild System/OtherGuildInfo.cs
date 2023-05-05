using System;
using Fronteira.Discord;
using Server.Commands;
using Server.Factions;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Guilds
{
    public class OtherGuildInfo : BaseGuildGump
    {
        private readonly Guild m_Other;
        public OtherGuildInfo(PlayerMobile pm, Guild g, Guild otherGuild)
            : base(pm, g, 10, 40)
        {
            this.m_Other = otherGuild;

            g.CheckExpiredWars();
			
            this.PopulateGump();
        }

        public void AddButtonAndBackground(int x, int y, int buttonID, string locNum)
        {
            this.AddBackground(x, y, 225, 26, 0x2486);
            this.AddButton(x + 5, y + 5, 0x845, 0x846, buttonID, GumpButtonType.Reply, 0);
            this.AddHtml(x + 30, y + 3, 185, 26, locNum,  false, false);
        }

        public override void PopulateGump()
        {
            Guild g = Guild.GetAllianceLeader(this.guild);
            Guild other = Guild.GetAllianceLeader(this.m_Other);
			
            WarDeclaration war = g.FindPendingWar(other);
            WarDeclaration activeWar = g.FindActiveWar(other);

            AllianceInfo alliance = this.guild.Alliance;
            AllianceInfo otherAlliance = this.m_Other.Alliance;
            //NOTE TO SELF: Only only alliance leader can see pending guild alliance statuses

            bool PendingWar = (war != null);
            bool ActiveWar = (activeWar != null);
            this.AddPage(0);

            this.AddBackground(0, 0, 520, 335, 0x242C);
            this.AddHtml(20, 15, 480, 26, "<div align=center><i>Relacoes</i></div>",  false, false); // <div align=center><i>Guild Relationship</i></div>
            this.AddImageTiled(20, 40, 480, 2, 0x2711);
            this.AddHtml(20, 50, 120, 26, "Nome",  true, false); // <i>Guild Name</i>
            this.AddHtml(150, 53, 360, 26, this.m_Other.Name, false, false);

            this.AddHtml(20, 80, 120, 26, "Alianca",  true, false); // <i>Alliance</i>

            if (otherAlliance != null)
            {
                if (otherAlliance.IsMember(this.m_Other))
                {
                    this.AddHtml(150, 83, 360, 26, otherAlliance.Name, false, false);
                }
                //else if( otherAlliance.Leader == guild && ( otherAlliance.IsPendingMember( m_Other ) || otherAlliance.IsPendingMember( guild ) ) )
                /*		else if( (otherAlliance.Leader == guild && otherAlliance.IsPendingMember( m_Other ) ) || ( otherAlliance.Leader == m_Other && otherAlliance.IsPendingMember( guild ) ) )
                {
                AddHtml( 150, 83, 360, 26, Color( alliance.Name, 0xF), false, false );
                }
                //AddHtml( 150, 83, 360, 26, ( alliance.PendingMembers.Contains( guild ) || alliance.PendingMembers.Contains( m_Other ) ) ? String.Format( "<basefont color=#blue>{0}</basefont>", alliance.Name ) : alliance.Name, false, false );
                //AddHtml( 150, 83, 360, 26, ( otherAlliance == alliance &&  otherAlliance.PendingMembers.Contains( guild ) || otherAlliance.PendingMembers.Contains( m_Other ) ) ? String.Format( "<basefont color=#blue>{0}</basefont>", otherAlliance.Name ) : otherAlliance.Name, false, false );
                */
            }

            this.AddHtml(20, 110, 120, 26, "Tag",  true, false); // <i>Abbreviation</i>
            this.AddHtml(150, 113, 120, 26, this.m_Other.Abbreviation, false, false);

            string kills = "0/0";
            string time = "00:00";
            string otherKills = "0/0";
			
            WarDeclaration otherWar;

            if (ActiveWar)
            {
                kills = String.Format("{0}/{1}", activeWar.Kills, activeWar.MaxKills);

                TimeSpan timeRemaining = TimeSpan.Zero;

                if (activeWar.WarLength != TimeSpan.Zero && (activeWar.WarBeginning + activeWar.WarLength) > DateTime.UtcNow)
                    timeRemaining = (activeWar.WarBeginning + activeWar.WarLength) - DateTime.UtcNow;

                //time = String.Format( "{0:D2}:{1:D2}", timeRemaining.Hours.ToString(), timeRemaining.Subtract( TimeSpan.FromHours( timeRemaining.Hours ) ).Minutes );	//Is there a formatter for htis? it's 2AM and I'm tired and can't find it
                time = String.Format("{0:D2}:{1:mm}", timeRemaining.Hours, DateTime.MinValue + timeRemaining);

                otherWar = this.m_Other.FindActiveWar(this.guild);
                if (otherWar != null)
                    otherKills = String.Format("{0}/{1}", otherWar.Kills, otherWar.MaxKills);
            }
            else if (PendingWar)
            {
                kills = Color(String.Format("{0}/{1}", war.Kills, war.MaxKills), 0x990000);
                //time = Color( String.Format( "{0}:{1}", war.WarLength.Hours, ((TimeSpan)(war.WarLength - TimeSpan.FromHours( war.WarLength.Hours ))).Minutes ), 0xFF0000 );
                time = Color(war.WarLength.TotalHours+" horas", 0x990000);

                otherWar = this.m_Other.FindPendingWar(this.guild);
                if (otherWar != null)
                    otherKills = Color(String.Format("{0}/{1}", otherWar.Kills, otherWar.MaxKills), 0x990000);
            }

            this.AddHtml(280, 110, 120, 26, "Seus Kills",  true, false); // <i>Your Kills</i>
            this.AddHtml(410, 113, 120, 26, kills, false, false);

            this.AddHtml(20, 140, 120, 26, "Tempo Restante:",  true, false); // <i>Time Remaining</i>
            this.AddHtml(150, 143, 120, 26, time, false, false);

            this.AddHtml(280, 140, 120, 26, "Kills Deles:",  true, false); // <i>Their Kills</i>
            this.AddHtml(410, 143, 120, 26, otherKills, false, false);

            this.AddImageTiled(20, 172, 480, 2, 0x2711);

            string number = "Voce esta em paz com eles";// <div align=center>You are at peace with this guild.</div>
			
            if (PendingWar)
            {
                if (war.WarRequester)
                {
                    number = "Chamou para guerra"; // <div align=center>You have challenged this guild to war!</div>
                }
                else
                {
                    number = "Esta guilda te chamou para guerra"; // <div align=center>This guild has challenged you to war!</div>

                    this.AddButtonAndBackground(20, 260, 5, "Aceitar"); // Accept Challenge
                    this.AddButtonAndBackground(275, 260, 6, "Mudar termos"); //Modify Terms
                }

                this.AddButtonAndBackground(20, 290, 7, "Negar"); // Dismiss Challenge
            }
            else if (ActiveWar)
            {
                number = "Voce esta em guerra com eles"; // <div align=center>You are at war with this guild!</div>
                this.AddButtonAndBackground(20, 290, 8, "Render-se"); // Surrender
            }
            else if (alliance != null && alliance == otherAlliance) //alliance, Same Alliance
            {
                if (alliance.IsMember(this.guild) && alliance.IsMember(this.m_Other))	//Both in Same alliance, full members
                {
                    number = "Voce eh aliado deles"; // <div align=center>You are allied with this guild.</div>

                    if (alliance.Leader == this.guild)
                    {
                        this.AddButtonAndBackground(20, 260, 12, "Remover guilda da alianca"); // Remove Guild from Alliance
                        this.AddButtonAndBackground(275, 260, 13, "Promover a lider"); // Promote to Alliance Leader	//Note: No 'confirmation' like the other leader guild promotion things
                        //Remove guild from alliance	//Promote to Alliance Leader
                    }

                    //Show roster, Centered, up
                    this.AddButtonAndBackground(148, 215, 10, "Mostrar Membros"); //Show Alliance Roster
                    //Leave Alliance
                    this.AddButtonAndBackground(20, 290, 11, "Sair da Alianca"); // Leave Alliance
                }
                else if (alliance.Leader == this.guild && alliance.IsPendingMember(this.m_Other))
                {
                    number = "Voce pediu alianca com eles"; // <div align=center>You have requested an alliance with this guild.</div>

                    //Show Alliance Roster, Centered, down.
                    this.AddButtonAndBackground(148, 245, 10, "Mostrar Membros da Alianca"); //Show Alliance Roster
                    //Withdraw Request
                    this.AddButtonAndBackground(20, 290, 14, "Retirar pedido"); // Withdraw Request

                    this.AddHtml(150, 83, 360, 26, Color(alliance.Name, 0x99), false, false);
                }
                else if (alliance.Leader == this.m_Other && alliance.IsPendingMember(this.guild))
                {
                    number = "Esta guilda pediu alianca com voce"; // <div align=center>This guild has requested an alliance.</div>

                    //Show alliance Roster, top
                    this.AddButtonAndBackground(148, 215, 10, "Mostrar membros da alianca"); //Show Alliance Roster
                    //Deny Request
                    //Accept Request
                    this.AddButtonAndBackground(20, 260, 15, "Negar"); // Deny Request
                    this.AddButtonAndBackground(20, 290, 16, "Aceitar"); // Accept Request

                    this.AddHtml(150, 83, 360, 26, Color(alliance.Name, 0x99), false, false);
                }
            }
            else
            {
                this.AddButtonAndBackground(20, 260, 2, "Pedir Alianca"); // Request Alliance 
                this.AddButtonAndBackground(20, 290, 1, "Declarar Guerra"); // Declare War!
            }

            this.AddButtonAndBackground(275, 290, 0, "Cancelar"); //Cancel

            this.AddHtml(20, 180, 480, 30, number,  true, false);
            this.AddImageTiled(20, 245, 480, 2, 0x2711);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (!IsMember(pm, this.guild))
                return;

            RankDefinition playerRank = pm.GuildRank;
			
            Guild guildLeader = Guild.GetAllianceLeader(this.guild);
            Guild otherGuild = Guild.GetAllianceLeader(this.m_Other);

            WarDeclaration war = guildLeader.FindPendingWar(otherGuild);
            WarDeclaration activeWar = guildLeader.FindActiveWar(otherGuild);
            WarDeclaration otherWar = otherGuild.FindPendingWar(guildLeader);

            AllianceInfo alliance = this.guild.Alliance;
            AllianceInfo otherAlliance = otherGuild.Alliance;

            switch( info.ButtonID )
            {
                #region War
                case 5:	//Accept the war
                    {
                        if (war != null && !war.WarRequester && activeWar == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != this.guild)
                            {
                                pm.SendLocalizedMessage(1063239, String.Format("{0}\t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            {
                                //Accept the war
                                this.guild.PendingWars.Remove(war);
                                war.WarBeginning = DateTime.UtcNow;
                                this.guild.AcceptedWars.Add(war);
                                Guild.Ativas.Add(war);
                                var m = $"Uma guerra se inicia, {war.Guild.Abbreviation} vs {war.Opponent.Abbreviation}";
                                Anuncio.Anuncia(m, false);
                                DiscordBot.SendMessage($":crossed_swords:Uma guerra se inicia, {war.Guild.Name} vs {war.Opponent.Name}");

                                if (alliance != null && alliance.IsMember(this.guild))
                                {
                                    alliance.AllianceMessage(1070769, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    alliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    this.guild.GuildMessage(1070769, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    this.guild.InvalidateMemberProperties();
                                }
                                //Technically  SHOULD say Your guild is now at war w/out any info, intentional diff.

                                otherGuild.PendingWars.Remove(otherWar);
                                otherWar.WarBeginning = DateTime.UtcNow;
                                otherGuild.AcceptedWars.Add(otherWar);
                                Guild.Ativas.Add(otherWar);

                                if (otherAlliance != null && this.m_Other.Alliance.IsMember(this.m_Other))
                                {
                                    otherAlliance.AllianceMessage(1070769, ((alliance != null) ? alliance.Name : this.guild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    otherAlliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    otherGuild.GuildMessage(1070769, ((alliance != null) ? alliance.Name : this.guild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    otherGuild.InvalidateMemberProperties();
                                }
                            }
                        }

                        break;
                    }
                case 6:	//Modify war terms
                    {
                        if (war != null && !war.WarRequester && activeWar == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != this.guild)
                            {
                                pm.SendLocalizedMessage(1063239, String.Format("{0}\t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            {
                                pm.SendGump(new WarDeclarationGump(pm, this.guild, otherGuild));
                            }
                        }
                        break;
                    }
                case 7:	//Dismiss war
                    {
                        if (war != null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != this.guild)
                            {
                                pm.SendLocalizedMessage(1063239, String.Format("{0}\t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            { 
                                //Dismiss the war
                                this.guild.PendingWars.Remove(war);
                                otherGuild.PendingWars.Remove(otherWar);
                                pm.SendLocalizedMessage(1070752); // The proposal has been updated.
                                //Messages to opposing guild? (Testing on OSI says no)
                            }
                        }
                        break;
                    }
                case 8:	//Surrender
                    {
                        if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                        {
                            pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                        }
                        else if (alliance != null && alliance.Leader != this.guild)
                        {
                            pm.SendLocalizedMessage(1063239, String.Format("{0}\t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                            pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                        }
                        else
                        {
                            if (activeWar != null)
                            {
                                if (alliance != null && alliance.IsMember(this.guild))
                                {
                                    alliance.AllianceMessage(1070740, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name));// You have lost the war with ~1_val~.
                                    alliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    this.guild.GuildMessage(1070740, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name));// You have lost the war with ~1_val~.
                                    this.guild.InvalidateMemberProperties();
                                }
	
                                this.guild.AcceptedWars.Remove(activeWar);
                                Guild.History.Add(activeWar);

                                var m = $"Fim de guerra, {this.guild.Abbreviation} se rendeu a {(activeWar.Opponent == null ? "?" : activeWar.Opponent.Abbreviation)}";
                                Anuncio.Anuncia(m, false);
                                DiscordBot.SendMessage($":crossed_swords: Fim de guerra, {this.guild.Name} se rendeu a {(activeWar.Opponent == null ? "?" : activeWar.Opponent.Name)}");

                                if (otherAlliance != null && otherAlliance.IsMember(otherGuild))
                                {
                                    otherAlliance.AllianceMessage(1070739, ((this.guild.Alliance != null) ? this.guild.Alliance.Name : this.guild.Name));// You have won the war against ~1_val~!
                                    otherAlliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    otherGuild.GuildMessage(1070739, ((this.guild.Alliance != null) ? this.guild.Alliance.Name : this.guild.Name));// You have won the war against ~1_val~!
                                    otherGuild.InvalidateMemberProperties();
                                }

                                var other = otherGuild.FindActiveWar(this.guild);
                                otherGuild.AcceptedWars.Remove(other);
                                Guild.History.Add(other);
                            }
                        }
                        break;
                    }
                case 1:	//Declare War
                    {
                        if (war == null && activeWar == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != this.guild)
                            {
                                pm.SendLocalizedMessage(1063239, String.Format("{0}\t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else if (otherAlliance != null && otherAlliance.Leader != this.m_Other)
                            {
                                pm.SendLocalizedMessage(1063239, String.Format("{0}\t{1}", this.m_Other.Name, otherAlliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, otherAlliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            {
                                pm.SendGump(new WarDeclarationGump(pm, this.guild, this.m_Other));
                            }
                        }
                        break;
                    }
                    #endregion
                case 2:	//Request Alliance
                    {
                        #region New alliance
                        if (alliance == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.AllianceControl))
                            {
                                pm.SendLocalizedMessage(1070747); // You don't have permission to create an alliance.
                            }
                            else if (Faction.Find(this.guild.Leader) != Faction.Find(this.m_Other.Leader))
                            {
                                pm.SendLocalizedMessage(1070758); // You cannot propose an alliance to a guild with a different faction allegiance.
                            }
                            else if (otherAlliance != null)
                            {
                                if (otherAlliance.IsPendingMember(this.m_Other))
                                    pm.SendLocalizedMessage(1063416, this.m_Other.Name); // ~1_val~ is currently considering another alliance proposal.
                                else
                                    pm.SendLocalizedMessage(1063426, this.m_Other.Name); // ~1_val~ already belongs to an alliance.
                            }
                            else if (this.m_Other.AcceptedWars.Count > 0 || this.m_Other.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, this.m_Other.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else if (this.guild.AcceptedWars.Count > 0 || this.guild.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, this.guild.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else
                            {
                                pm.SendLocalizedMessage(1063439); // Enter a name for the new alliance:
                                pm.BeginPrompt(new PromptCallback(CreateAlliance_Callback));
                            }
                        }
                        #endregion
                        #region Existing Alliance
                        else
                        {
                            if (!playerRank.GetFlag(RankFlags.AllianceControl))
                            {
                                pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                            }
                            else if (alliance.Leader != this.guild)
                            {
                                pm.SendLocalizedMessage(1063239, String.Format("{0}\t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                            }
                            else if (otherAlliance != null)
                            {
                                if (otherAlliance.IsPendingMember(this.m_Other))
                                    pm.SendLocalizedMessage(1063416, this.m_Other.Name); // ~1_val~ is currently considering another alliance proposal.
                                else
                                    pm.SendLocalizedMessage(1063426, this.m_Other.Name); // ~1_val~ already belongs to an alliance.
                            }
                            else if (alliance.IsPendingMember(this.guild))
                            {
                                pm.SendLocalizedMessage(1063416, this.guild.Name); // ~1_val~ is currently considering another alliance proposal.
                            }
                            else if (this.m_Other.AcceptedWars.Count > 0 || this.m_Other.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, this.m_Other.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else if (this.guild.AcceptedWars.Count > 0 || this.guild.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, this.guild.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else if (Faction.Find(this.guild.Leader) != Faction.Find(this.m_Other.Leader))
                            {
                                pm.SendLocalizedMessage(1070758); // You cannot propose an alliance to a guild with a different faction allegiance.
                            }
                            else
                            {
                                pm.SendLocalizedMessage(1070750, this.m_Other.Name); // An invitation to join your alliance has been sent to ~1_val~.

                                this.m_Other.GuildMessage(1070780, this.guild.Name); // ~1_val~ has proposed an alliance.

                                this.m_Other.Alliance = alliance;	//Calls addPendingGuild
                                //alliance.AddPendingGuild( m_Other );
                            }
                        }
                        #endregion
                        break;
                    }
                case 10:	//Show Alliance Roster
                    {
                        if (alliance != null && alliance == otherAlliance)
                            pm.SendGump(new AllianceInfo.AllianceRosterGump(pm, this.guild, alliance));

                        break;
                    }
                case 11:	//Leave Alliance
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendMessage("Sem Permissao"); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.IsMember(this.guild))
                        {
                            this.guild.Alliance = null;	//Calls alliance.Removeguild
                            //						alliance.RemoveGuild( guild );
						
                            this.m_Other.InvalidateWarNotoriety();
						
                            this.guild.InvalidateMemberNotoriety();
                        }
                        break;
                    }
                case 12:	//Remove Guild from alliance
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendMessage("Sem permissao"); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.Leader != this.guild)
                        {
                            pm.SendMessage("Esta pessoa nao eh o lider"); // ~1_val~ is not the leader of the ~2_val~ alliance.
                        }
                        else if (alliance != null && alliance.IsMember(this.guild) && alliance.IsMember(this.m_Other))
                        {
                            this.m_Other.Alliance = null;
						
                            this.m_Other.InvalidateMemberNotoriety();
						
                            this.guild.InvalidateWarNotoriety();
                        }
                        break;
                    }
                case 13:	//Promote to Alliance leader
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendMessage("Sem permissao"); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.Leader != this.guild)
                        {
                            pm.SendMessage(String.Format("{0} nao eh o lider de \t{1}", this.guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                        }
                        else if (alliance != null && alliance.IsMember(this.guild) && alliance.IsMember(this.m_Other))
                        { 
                            pm.SendMessage(String.Format("{0} agora eh o lider de \t{1}", this.m_Other.Name, alliance.Name)); // ~1_val~ is now the leader of ~2_val~.
                            alliance.Leader = this.m_Other;
                        }
                        break;
                    }
                case 14:	//Withdraw Request
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendMessage("Sem permissao"); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.Leader == this.guild && alliance.IsPendingMember(this.m_Other))
                        {
                            this.m_Other.Alliance = null;
                            pm.SendMessage("Atualizado"); // The proposal has been updated.
                        }
                        break;
                    }
                case 15: //Deny Alliance Request
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendMessage("Sem permissao"); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && otherAlliance != null && alliance.Leader == this.m_Other && otherAlliance.IsPendingMember(this.guild))
                        {
                            pm.SendMessage("Atualizado"); // The proposal has been updated.
                            //m_Other.GuildMessage( 1070782 ); // ~1_val~ has responded to your proposal.	//Per OSI commented out.

                            this.guild.Alliance = null;
                        }
                        break;
                    }
                case 16: //Accept Alliance Request
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendMessage("Sem Permissao"); // You don't have permission to negotiate an alliance.
                        }
                        else if (otherAlliance != null && otherAlliance.Leader == this.m_Other && otherAlliance.IsPendingMember(this.guild))
                        {
                            pm.SendMessage("Atualizado"); // The proposal has been updated.

                            otherAlliance.TurnToMember(this.m_Other); //No need to verify it's in the guild or already a member, the function does this

                            otherAlliance.TurnToMember(this.guild);
                        }
                        break;
                    }
            }
        }

        public void CreateAlliance_Callback(Mobile from, string text)
        { 
            PlayerMobile pm = from as PlayerMobile;

            AllianceInfo alliance = this.guild.Alliance;
            AllianceInfo otherAlliance = this.m_Other.Alliance;

            if (!IsMember(from, this.guild) || alliance != null)
                return;

            RankDefinition playerRank = pm.GuildRank;

            if (!playerRank.GetFlag(RankFlags.AllianceControl))
            {
                pm.SendMessage("Sem permissao"); // You don't have permission to create an alliance.
            }
            else if (Faction.Find(this.guild.Leader) != Faction.Find(this.m_Other.Leader))
            {
                //Notes about this: OSI only cares/checks when proposing, you can change your faction all you want later.  
                pm.SendMessage("Voce nao pode aliar a alguem de outra faction"); // You cannot propose an alliance to a guild with a different faction allegiance.
            }
            else if (otherAlliance != null)
            {
                if (otherAlliance.IsPendingMember(this.m_Other))
                    pm.SendMessage(this.m_Other.Name+" esta considerando outra alianca"); // ~1_val~ is currently considering another alliance proposal.
                else
                    pm.SendMessage("Ja percence a uma alianca"); // ~1_val~ already belongs to an alliance.
            }
            else if (this.m_Other.AcceptedWars.Count > 0 || this.m_Other.PendingWars.Count > 0)
            {
                pm.SendMessage("Esta envolvido em uma guerra"); // ~1_val~ is currently involved in a guild war.
            }
            else if (this.guild.AcceptedWars.Count > 0 || this.guild.PendingWars.Count > 0)
            {
                pm.SendMessage("Esta envolvido em uma guerra"); // ~1_val~ is currently involved in a guild war.
            }
            else
            {
                string name = Utility.FixHtml(text.Trim());

                if (!BaseGuildGump.CheckProfanity(name))
                    pm.SendMessage("Nome negado"); // That alliance name is not allowed.
                else if (name.Length > Guild.NameLimit)
                    pm.SendMessage("Nome muito grande"); // An alliance name cannot exceed ~1_val~ characters in length.
                else if (AllianceInfo.Alliances.ContainsKey(name.ToLower()))
                    pm.SendMessage("Nome indisponivel"); // That alliance name is not available.
                else
                {
                    pm.SendMessage("Convite enviado"); // An invitation to join your alliance has been sent to ~1_val~.

                    this.m_Other.GuildTextMessage(this.guild.Name+ " propos uma Alianca !"); // ~1_val~ has proposed an alliance.
					
                    new AllianceInfo(this.guild, name, this.m_Other);
                }
            }
        }
    }
}
