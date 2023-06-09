using System;
using System.Collections.Generic;
using System.Linq;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Services.Virtues;

namespace Server.Engines.Quests
{
    public class BaseEscort : MondainQuester
    {
        private static readonly TimeSpan m_EscortDelay = TimeSpan.FromMinutes(5.0);
        private static readonly Dictionary<Mobile, Mobile> m_EscortTable = new Dictionary<Mobile, Mobile>();
        private Timer m_DeleteTimer;
        private bool m_Checked;

        public BaseQuest Quest { get; set; }
        public DateTime LastSeenEscorter { get; set; }

        public BaseEscort()
            : base()
        {
            AI = AIType.AI_Melee;
            FightMode = FightMode.Aggressor;
            RangePerception = 22;
            RangeFight = 1;
            ActiveSpeed = 0.2;
            PassiveSpeed = 1.0;

            ControlSlots = 0;
        }

        public BaseEscort(Serial serial)
            : base(serial)
        {
        }

        public override bool OwnerCanRename { get { return false; } }
        public override bool InitialInnocent { get { return true; } }
        public override bool IsInvulnerable { get { return false; } }
        public override bool Commandable { get { return false; } }

        public override Type[] Quests { get { return null; } }

        public override bool CanAutoStable { get { return false; } }
        public override bool CanDetectHidden { get { return false; } }

        public override void OnTalk(PlayerMobile player)
        {
            if (AcceptEscorter(player))
                base.OnTalk(player);
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return (from.AccessLevel >= AccessLevel.GameMaster);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && from == ControlMaster)
                list.Add(new AbandonEscortEntry(this));

            base.AddCustomContextEntries(from, list);
        }

        public override void OnAfterDelete()
        {
            if (Quest != null)
            {
                Quest.RemoveQuest();

                if (Quest.Owner != null)
                    m_EscortTable.Remove(Quest.Owner);
            }

            base.OnAfterDelete();
        }

        public override void OnThink()
        {
            base.OnThink();

            CheckAtDestination();
        }

        public override bool CanBeDamaged()
        {
            return true;
        }

        public override void InitBody()
        {
            SetStr(90, 100);
            SetDex(90, 100);
            SetInt(15, 25);

            Hue = Utility.RandomSkinHue();
            Female = Utility.RandomBool();
            Name = NameList.RandomName(Female ? "female" : "male");
            Race = Race.Human;

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_DeleteTimer != null);

            if (m_DeleteTimer != null)
                writer.WriteDeltaTime(m_DeleteTimer.Next);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (reader.ReadBool())
            {
                DateTime deleteTime = reader.ReadDeltaTime();
                m_DeleteTimer = Timer.DelayCall(deleteTime - DateTime.UtcNow, new TimerCallback(Delete));
            }
        }

        public void AddHash(PlayerMobile player)
        {
            m_EscortTable[player] = this;
        }

        public virtual void StartFollow()
        {
            StartFollow(ControlMaster);
        }

        public virtual void StartFollow(Mobile escorter)
        {
            ActiveSpeed = 0.1;
            PassiveSpeed = 0.2;

            ControlOrder = OrderType.Follow;
            ControlTarget = escorter;

            CantWalk = false;
            Frozen = false;
            IsPrisoner = false;
            RangeHome = 0;
            Home = Point3D.Zero;

            CurrentSpeed = 0.1;
        }

        public virtual void StopFollow()
        {
            ActiveSpeed = 0.2;
            PassiveSpeed = 1.0;

            ControlOrder = OrderType.None;
            ControlTarget = null;

            CurrentSpeed = 1.0;

            SetControlMaster(null);
        }

        public virtual void BeginDelete(Mobile m)
        {
            StopFollow();

            if (m != null)
                m_EscortTable.Remove(m);

            m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(45.0), new TimerCallback(Delete));
        }        

        public virtual bool AcceptEscorter(Mobile m)
        {
            if (!m.Alive)
            {
                return false;
            }
            else if (m_DeleteTimer != null)
            {
                Say("Eu to de boinha aqui..."); // I am sorry, but I do not wish to go anywhere.
                return false;
            }
            else if (Controlled)
            {
                if (m == ControlMaster)
                    m.SendGump(new MondainQuestGump(Quest, MondainQuestGump.Section.InProgress, false));
                else
                    Say("Ja estou tendo um escudeiro obrigado"); // I am already being led!

                return false;
            }
            else if (!m.InRange(Location, 5))
            {
                Say(500348); // I am too far away to do that.
                return false;
            }
            else if (m_EscortTable.ContainsKey(m))
            {
                Say("Vejo que ja esta acompanhando alguem"); // I see you already have an escort.
                return false;
            }
            else if (m.AccessLevel <= AccessLevel.VIP && m is PlayerMobile && (((PlayerMobile)m).LastEscortTime + m_EscortDelay) >= DateTime.UtcNow)
            {
                int minutes = (int)Math.Ceiling(((((PlayerMobile)m).LastEscortTime + m_EscortDelay) - DateTime.UtcNow).TotalMinutes);

                Say("Aguarde {0} minuto{1} para embarcar em outra jornada.", minutes, minutes == 1 ? "" : "s");
                return false;
            }

            return true;
        }

        public virtual EscortObjective GetObjective()
        {
            if (Quest != null)
            {
                for (int i = 0; i < Quest.Objectives.Count; i++)
                {
                    EscortObjective escort = Quest.Objectives[i] as EscortObjective;

                    if (escort != null && !escort.Completed && !escort.Failed)
                        return escort;
                }
            }

            return null;
        }

        public virtual Mobile GetEscorter()
        {
            Mobile master = ControlMaster;

            if (master == null || !Controlled)
            {
                return master;
            }
            else if (master.Map != Map || !master.InRange(Location, 30) || !master.Alive)
            {
                TimeSpan lastSeenDelay = DateTime.UtcNow - LastSeenEscorter;

                if (lastSeenDelay >= TimeSpan.FromMinutes(2.0))
                {
                    EscortObjective escort = GetObjective();

                    if (escort != null)
                    {
                        master.SendLocalizedMessage("Voce falhou em levar a pessoa..."); // You have failed your escort quest…
                        master.PlaySound(0x5B3);
                        escort.Fail();
                    }

                    master.SendLocalizedMessage("Voce se perdeu da pessoa que estava levando..."); // You have lost the person you were escorting.
                    Say("Hmm acho que me perdi..."); // Hmmm.  I seem to have lost my master.

                    StopFollow();
                    m_EscortTable.Remove(master);
                    m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));

                    return null;
                }
                else
                {
                    ControlOrder = OrderType.Stay;
                }
            }
            else
            {
                if (ControlOrder != OrderType.Follow)
                    StartFollow(master);

                LastSeenEscorter = DateTime.UtcNow;
            }

            return master;
        }

        public virtual Region GetDestination()
        {
            return null;
        }

        public virtual bool CheckAtDestination()
        {
            if (Quest != null)
            {
                EscortObjective escort = GetObjective();

                if (escort == null)
                    return false;

                Mobile escorter = GetEscorter();

                if (escorter == null)
                    return false;

                if (escort.Region != null && escort.Region.Contains(Location))
                {
                    Say("Ah chegamos ! Muito obrigado "+ escorter.Name); // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.

                    escort.Complete();

                    if (Quest.Completed)
                    {
                        escorter.SendMessage("Voce completou a missao !");		

                        if (QuestHelper.AnyRewards(Quest))
                            escorter.SendGump(new MondainQuestGump(Quest, MondainQuestGump.Section.Rewards, false, true));
                        else
                            Quest.GiveRewards();

                        escorter.PlaySound(Quest.CompleteSound);

                        StopFollow();
                        m_EscortTable.Remove(escorter);
                        m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));

                        // fame
                        Misc.Titles.AwardFame(escorter, escort.Fame, true);

                        // compassion
                        bool gainedPath = false;

                        PlayerMobile pm = escorter as PlayerMobile;

                        if (pm != null)
                        {
                            if (pm.CompassionGains > 0 && DateTime.UtcNow > pm.NextCompassionDay)
                            {
                                pm.NextCompassionDay = DateTime.MinValue;
                                pm.CompassionGains = 0;
                            }

                            if (pm.CompassionGains >= 5) // have already gained 5 times in one day, can gain no more
                            {
                                //pm.SendLocalizedMessage("Voce precisa aguardar um dia para ganhar mais compaixao"); // You must wait about a day before you can gain in compassion again.
                            }
                            else if (VirtueHelper.Award(pm, VirtueName.Compassion, escort.Compassion, ref gainedPath))
                            {
                                pm.SendLocalizedMessage("Voce demonstrou compaixao");  // You have demonstrated your compassion!  Your kind actions have been noted.

                                if (gainedPath)
                                    pm.SendLocalizedMessage("Voce esta no caminho da compaixao"); // You have achieved a path in compassion!
                                //else
                                //    pm.SendLocalizedMessage(1053002); // You have gained in compassion.

                                pm.NextCompassionDay = DateTime.UtcNow + TimeSpan.FromDays(1.0); // in one day CompassionGains gets reset to 0
                                ++pm.CompassionGains;
                            }
                            else
                            {
                                pm.SendLocalizedMessage("Voce chegou ao maximo do caminho da compaixao"); // You have achieved the highest path of compassion and can no longer gain any further.
                            }
                        }
                    }
                    else
                    {
                        escorter.PlaySound(Quest.UpdateSound);
                    }
                    return true;
                }
            }
            else if (!m_Checked)
            {
                Region region = GetDestination();

                if (region != null && region.Contains(Location))
                {
                    m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));
                    m_Checked = true;
                }
            }

            return false;
        }

        private class AbandonEscortEntry : ContextMenuEntry
        {
            private readonly BaseEscort m_Mobile;

            public AbandonEscortEntry(BaseEscort m)
                : base(6102, 3)
            {
                m_Mobile = m;
            }

            public override void OnClick()
            {
                Owner.From.SendLocalizedMessage(1071194); // You have failed your escort quest…
                Owner.From.PlaySound(0x5B3);
                m_Mobile.Delete();
            }
        }

        public static void DeleteEscort(Mobile owner)
        {
            PlayerMobile pm = owner as PlayerMobile;

            foreach (var escortquest in pm.Quests.Where(x => x.Quester is BaseEscort))
            {
                BaseEscort escort = (BaseEscort)escortquest.Quester;

                Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerCallback(
                delegate
                {
                    escort.Say("AAhhn socorro !!"); // Ack!  My escort has come to haunt me!
                    owner.SendLocalizedMessage("Voce falhou em sua missao..."); // You have failed your escort quest…
                    owner.PlaySound(0x5B3);
                    escort.Delete();
                }));
            }            
        }
    }
}
