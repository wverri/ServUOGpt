using System;
using System.Collections.Generic;
using Server.Fronteira.Talentos;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// Strike your opponent with great force, partially bypassing their armor and inflicting greater damage. Requires either Bushido or Ninjitsu skill
    /// </summary>
    public class ArmorPierce : WeaponAbility
    {
        public static Dictionary<Mobile, Timer> _Table = new Dictionary<Mobile, Timer>();

        public ArmorPierce()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }

        public override double DamageScalar
        {
            get
            {
                return Core.HS ? 1.0 : 1.5;
            }
        }

        public override bool RequiresSE
        {
            get
            {
                return true;
            }
        }

        public override Talento TalentoParaUsar { get { return Talento.Hab_ArmorPierce; } }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage("Voce penetrou a armadura do inimigo"); // You pierce your opponent's armor!

            defender.SendLocalizedMessage("Sua armadura foi penetrada"); // Your armor has been pierced!
                                                                         //defender.SendLocalizedMessage(1063351); // Your attacker pierced your armor!            

            //if (Core.HS)
            //{
            if (_Table.ContainsKey(defender))
            {
                if (attacker.Weapon is BaseRanged)
                    return;

                _Table[defender].Stop();
            }

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.ArmorPierce, 1028860, 1154367, TimeSpan.FromSeconds(3), defender, "10"));
            _Table[defender] = Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(3), RemoveEffects, defender);
            //}

            defender.PlaySound(0x28E);
            defender.FixedParticles(0x3728, 1, 26, 0x26D6, 0, 0, EffectLayer.Waist);
        }

        public static void RemoveEffects(Mobile m)
        {
            if (IsUnderEffects(m))
            {
                m.SendLocalizedMessage("Sua armadura voltou ao normal"); // Your armor has returned to normal.
                _Table.Remove(m);
            }
        }

        public static bool IsUnderEffects(Mobile m)
        {
            if (m == null)
                return false;

            return _Table.ContainsKey(m);
        }
    }
}
