using System;

using Server.Gumps;
using Server.Misc.Custom;
using Server.Mobiles;

namespace Server.ContextMenus
{
    public class OpenBankEntry : ContextMenuEntry
    {
        private readonly Mobile m_Banker;

        public OpenBankEntry(Mobile banker)
            : base(6105, 12)
        {
            m_Banker = banker;
        }

        public override void OnClick()
        {
            if (!Owner.From.CheckAlive())
                return;

            if (Owner.From.Criminal)
            {
                m_Banker.Say(500378); // Thou art a criminal and cannot access thy bank box.
            }
            else
            {
                var m = Owner.From;

                BankLevels.OpenBank(Owner.From);

                if (Core.TOL && Owner.From is PlayerMobile)
                    Owner.From.SendGump(new BankerGump((PlayerMobile)Owner.From));
            }
        }
    }
}
