using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Services;

namespace Server.Items
{
    public class PoolOfAcid : Item
    {
        private readonly TimeSpan m_Duration;
        private readonly int m_MinDamage;
        private readonly int m_MaxDamage;
        private readonly DateTime m_Created;
        private readonly Timer m_Timer;
        private bool m_Drying;
        [Constructable]
        public PoolOfAcid()
            : this(TimeSpan.FromSeconds(10.0), 2, 5)
        {
        }

        [Constructable]
        public PoolOfAcid(TimeSpan duration, int minDamage, int maxDamage)
            : base(0x122A)
        {
            this.Hue = 0x3F;
            this.Movable = false;

            if (minDamage < 1)
                minDamage = 1;
            this.m_MinDamage = minDamage;
            this.m_MaxDamage = maxDamage;
            this.m_Created = DateTime.UtcNow;
            this.m_Duration = duration;

            this.m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerCallback(OnTick));
        }

        public PoolOfAcid(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "acido";
            }
        }
        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();
        }

        public override bool OnMoveOver(Mobile m)
        {
            this.Damage(m);
            return true;
        }

        public void Damage(Mobile m)
        {
            if(m.IsCooldown("danoacido"))
            {
                return;
            }
            m.SetCooldown("danoacido", TimeSpan.FromMilliseconds(500));

            if(!m.IsCooldown("dicaacido"))
            {
                m.SetCooldown("dicaacido", TimeSpan.FromSeconds(60));
                m.SendMessage(78, "Evite ficar em cima de vomitos e acidos verdes, elas podem lhe causar dano !");
            }

            if(m is BaseCreature)
            {
                var bc = (BaseCreature)m;
                if(bc.Tribe == TribeType.MortoVivo)
                {
                    return;
                }
            }
            var dmg = Utility.RandomMinMax(this.m_MinDamage, this.m_MaxDamage);
            m.Damage(dmg);
            DamageNumbers.ShowDamage(dmg, null, m, 78);
        }

        public override void Serialize(GenericWriter writer)
        {
            //Don't serialize these
        }

        public override void Deserialize(GenericReader reader)
        {
        }

        private void OnTick()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan age = now - this.m_Created;

            if (age > this.m_Duration)
            {
                this.Delete();
            }
            else
            {
                if (!this.m_Drying && age > (this.m_Duration - age))
                {
                    this.m_Drying = true;
                    this.ItemID = 0x122B;
                }

                List<Mobile> toDamage = new List<Mobile>();
                IPooledEnumerable eable = GetMobilesInRange(0);

                foreach (Mobile m in eable)
                {
                    BaseCreature bc = m as BaseCreature;

                    if (m.Alive && !m.IsDeadBondedPet && (bc == null || bc.Controlled || bc.Summoned))
                    {
                        toDamage.Add(m);
                    }
                }
                eable.Free();

                for (int i = 0; i < toDamage.Count; i++)
                    this.Damage(toDamage[i]);
            }
        }
    }
}
