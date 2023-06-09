using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;
using Server.Fronteira.Talentos;
using Server.Mobiles;

namespace Server.Items
{
    public class ForceArrow : WeaponAbility
    {
        public ForceArrow()
        {
        }

        public override int BaseMana { get { return 20; } }


        public override Talento TalentoParaUsar { get { return Talento.Hab_ForceArrow; } }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage("Voce atirou uma flecha de energia"); // You fire an arrow of pure force.
            defender.SendLocalizedMessage("Voce foi atingido por uma flecha de energia"); // You are struck by a force arrow!

            if (.4 > Utility.RandomDouble())
            {
                defender.Combatant = null;
                defender.Warmode = false;
            }

            ForceArrowInfo info = GetInfo(attacker, defender);

            if (info == null)
                BeginForceArrow(attacker, defender);
            else
            {
                if (info.Timer != null && info.Timer.Running)
                {
                    info.Timer.IncreaseExpiration();

                    BuffInfo.RemoveBuff(defender, BuffIcon.ForceArrow);
                    BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.ForceArrow, 1151285, 1151286, info.DefenseChanceMalus.ToString()));
                }
            }

            Spell spell = defender.Spell as Spell;

            if (spell != null && spell.IsCasting)
                spell.Disturb(DisturbType.Dano, false, true);
        }

        private static Dictionary<Mobile, List<ForceArrowInfo>> m_Table = new Dictionary<Mobile, List<ForceArrowInfo>>();

        public static void BeginForceArrow(Mobile attacker, Mobile defender)
        {
            ForceArrowInfo info = new ForceArrowInfo(attacker, defender);
            info.Timer = new ForceArrowTimer(info);

            if (!m_Table.ContainsKey(attacker))
                m_Table[attacker] = new List<ForceArrowInfo>();

            m_Table[attacker].Add(info);

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.ForceArrow, 1151285, 1151286, info.DefenseChanceMalus.ToString()));
        }

        public static void EndForceArrow(ForceArrowInfo info)
        {
            if (info == null)
                return;

            Mobile attacker = info.Attacker;

            if (m_Table.ContainsKey(attacker) && m_Table[attacker].Contains(info))
            {
                m_Table[attacker].Remove(info);

                if (m_Table[attacker].Count == 0)
                    m_Table.Remove(attacker);
            }

            BuffInfo.RemoveBuff(info.Defender, BuffIcon.ForceArrow);
        }

        public static bool HasForceArrow(Mobile attacker, Mobile defender)
        {
            if (!m_Table.ContainsKey(attacker))
                return false;

            foreach (ForceArrowInfo info in m_Table[attacker])
            {
                if (info.Defender == defender)
                    return true;
            }

            return false;
        }

        public static ForceArrowInfo GetInfo(Mobile attacker, Mobile defender)
        {
            if (!m_Table.ContainsKey(attacker))
                return null;

            foreach (ForceArrowInfo info in m_Table[attacker])
            {
                if (info.Defender == defender)
                    return info;
            }

            return null;
        }

        public class ForceArrowInfo
        {
            private Mobile m_Attacker;
            private Mobile m_Defender;
            private ForceArrowTimer m_Timer;
            private int m_DefenseChanceMalus;

            public Mobile Attacker { get { return m_Attacker; } }
            public Mobile Defender { get { return m_Defender; } }
            public ForceArrowTimer Timer { get { return m_Timer; } set { m_Timer = value; } }
            public int DefenseChanceMalus { get { return m_DefenseChanceMalus; } set { m_DefenseChanceMalus = value; } }

            public ForceArrowInfo(Mobile attacker, Mobile defender)
            {
                m_Attacker = attacker;
                m_Defender = defender;
                m_DefenseChanceMalus = 10;
            }
        }

        public class ForceArrowTimer : Timer
        {
            private ForceArrowInfo m_Info;
            private DateTime m_Expires;

            public ForceArrowTimer(ForceArrowInfo info)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1))
            {
                m_Info = info;
                Priority = TimerPriority.OneSecond;

                m_Expires = DateTime.UtcNow + TimeSpan.FromSeconds(10);

                Start();
            }

            protected override void OnTick()
            {
                if (m_Expires < DateTime.UtcNow)
                {
                    Stop();
                    EndForceArrow(m_Info);
                }
            }

            public void IncreaseExpiration()
            {
                m_Expires = m_Expires + TimeSpan.FromSeconds(2);

                m_Info.DefenseChanceMalus += 5;
            }
        }
    }
}
