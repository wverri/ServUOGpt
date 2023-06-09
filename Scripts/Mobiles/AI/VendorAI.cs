using Server.Ziden.RecompensaLogin;

namespace Server.Mobiles
{
    public class VendorAI : BaseAI
    {
        public VendorAI(BaseCreature m)
            : base(m)
        { }

        public override bool DoActionWander()
        {
            m_Mobile.DebugSay("I'm fine");

            if (m_Mobile.Combatant != null)
            {
                if (m_Mobile.Debug)
                    m_Mobile.DebugSay("{0} is attacking me", m_Mobile.Combatant.Name);

                if (m_Mobile.CanCallGuards)
                    m_Mobile.Say(Utility.RandomList(1005305, 501603));

                Action = ActionType.Flee;
            }
            else
            {
                if (m_Mobile.FocusMob != null)
                {
                    if (m_Mobile.Debug)
                        m_Mobile.DebugSay("{0} has talked to me", m_Mobile.FocusMob.Name);

                    Action = ActionType.Interact;
                }
                else
                {
                    m_Mobile.Warmode = false;

                    base.DoActionWander();
                }
            }

            return true;
        }

        public override bool DoActionInteract()
        {
            var customer = m_Mobile.FocusMob as Mobile;

            if (m_Mobile.Combatant != null)
            {
                if (m_Mobile.Debug)
                    m_Mobile.DebugSay("{0} is attacking me", m_Mobile.Combatant.Name);

                if (m_Mobile.CanCallGuards)
                    m_Mobile.Say(Utility.RandomList(1005305, 501603));

                Action = ActionType.Flee;

                return true;
            }

            if (customer == null || customer.Deleted || customer.Map != m_Mobile.Map)
            {
                m_Mobile.DebugSay("My customer have disapeared");
                m_Mobile.FocusMob = null;

                Action = ActionType.Wander;
            }
            else
            {
                if (customer.InRange(m_Mobile, m_Mobile.RangeFight))
                {
                    if (m_Mobile.Debug)
                        m_Mobile.DebugSay("I am with {0}", customer.Name);

                    if (!DirectionLocked)
                        m_Mobile.Direction = m_Mobile.GetDirectionTo(customer);
                }
                else
                {
                    if (m_Mobile.Debug)
                        m_Mobile.DebugSay("{0} is gone", customer.Name);

                    m_Mobile.FocusMob = null;

                    Action = ActionType.Wander;
                }
            }

            return true;
        }

        public override bool DoActionGuard()
        {
            m_Mobile.FocusMob = m_Mobile.Combatant as Mobile;
            return base.DoActionGuard();
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(m_Mobile, 4))
                return true;

            return base.HandlesOnSpeech(from);
        }

        // Temporary 
        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            var from = e.Mobile;

            if (m_Mobile is BaseVendor && from.InRange(m_Mobile, Core.AOS ? 1 : 4) && !e.Handled)
            {
                var vendor = m_Mobile as BaseVendor;
                if (e.HasKeyword(0x14D) || e.Speech.Contains("vender")) // *vendor sell*
                {
                    e.Handled = true;

                    ((BaseVendor)m_Mobile).VendorSell(from);
                    m_Mobile.FocusMob = from;
                }
                else if (e.Speech.Contains("trabalho"))
                {
                    e.Handled = true;
                    if (!vendor.CheckVendorAccess(from))
                    {
                        vendor.SayTo(from, true, "Voce nao pode trabalhar pra mim...");
                    }
                    else
                    {
                        BaseVendor.OfereceBulkOrder(from, vendor);
                    }
                }
                else if (e.Speech.Contains("suborno"))
                {
                    e.Handled = true;
                    BaseVendor.Suborna(from, vendor);
                }
                else if (e.Speech.Contains("recompensa"))
                {
                    if(vendor is Banker)
                    {
                        from.SendGump(new LoginRewardsGump(from, from as PlayerMobile));
                    } else
                    {
                        e.Handled = true;
                        BaseVendor.PegaRecompensa(from, vendor);
                    }
                }
                else if(e.Speech.Contains("treinar") || e.Speech.Contains("train"))
                {
                    if (vendor != null)
                    {
                        vendor.Treinar(e.Mobile);
                        return;
                    }
                }
                else if (e.HasKeyword(0x3C) || e.Speech.Contains("comprar")) // *vendor buy*
                {

                    e.Handled = true;

                    ((BaseVendor)m_Mobile).VendorBuy(from);
                    m_Mobile.FocusMob = from;
                }
                else if (WasNamed(e.Speech))
                {
                    if (e.HasKeyword(0x177) || e.Speech.Contains("vender")) // *sell*
                    {
                        e.Handled = true;

                        ((BaseVendor)m_Mobile).VendorSell(from);
                    }
                    else if (e.HasKeyword(0x171) || e.Speech.Contains("comprar")) // *buy*
                    {
                        e.Handled = true;

                        ((BaseVendor)m_Mobile).VendorBuy(from);
                    }

                    m_Mobile.FocusMob = from;
                }
            }
        }

        public override double TransformMoveDelay(double delay)
        {
            if (m_Mobile is BaseVendor)
            {
                return ((BaseVendor)m_Mobile).GetMoveDelay;
            }

            return base.TransformMoveDelay(delay);
        }
    }
}
