using System;
using Server.Fronteira.Talentos;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// Available on some crossbows, this special move allows archers to fire while on the move.
    /// This shot is somewhat less accurate than normal, but the ability to fire while running is a clear advantage.
    /// </summary>
    public class MovingShot : WeaponAbility
    {
        public MovingShot()
        {
        }

        public override int BaseMana
        {
            get
            {
                return Shard.RP ? 60 : 40;
            }
        }
        public override int AccuracyBonus
        {
            get
            {
                return Core.TOL ? -35 : -25;
            }
        }
        public override bool ValidatesDuringHit
        {
            get
            {
                return false;
            }
        }

        public override Talento TalentoParaUsar { get { return Talento.Hab_MovingSHot; } }


        public override bool OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            return (this.Validate(attacker) && this.CheckMana(attacker, true));
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            //Validates in OnSwing for accuracy scalar
            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage("Voce errou ao usar ataque em movimento"); // You fail to execute your special move
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            //Validates in OnSwing for accuracy scalar
            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage("Voce acertou ataque em movimento"); // Your shot was successful
        }
    }
}
