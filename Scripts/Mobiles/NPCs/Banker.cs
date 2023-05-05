#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Accounting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Misc.Custom;
using Server.Network;

using Acc = Server.Accounting.Account;
#endregion

namespace Server.Mobiles
{
    public class Banker : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public Banker()
            : base("o banqueiro")
        { }

        public Banker(Serial serial)
            : base(serial)
        { }

        public override NpcGuild NpcGuild { get { return NpcGuild.MerchantsGuild; } }

        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public static int GetBalance(Mobile m)
        {
            double balance = 0;

            if (AccountGold.Enabled && m.Account != null)
            {
                int goldStub;
                m.Account.GetGoldBalance(out goldStub, out balance);

                if (balance > Int32.MaxValue)
                {
                    return Int32.MaxValue;
                }
            }

            Container bank = m.Player ? m.BankBox : m.FindBankNoCreate();

            if (bank != null)
            {
                var gold = bank.FindItemsByType<Gold>();
                var checks = bank.FindItemsByType<BankCheck>();

                balance += gold.Aggregate(0.0, (c, t) => c + t.Amount);
                balance += checks.Aggregate(0.0, (c, t) => c + t.Worth);
            }

            return (int)Math.Max(0, Math.Min(Int32.MaxValue, balance));
        }

        public static int GetBalance(Mobile m, out Item[] gold, out Item[] checks)
        {
            double balance = 0;

            if (AccountGold.Enabled && m.Account != null)
            {
                int goldStub;
                m.Account.GetGoldBalance(out goldStub, out balance);

                if (balance > Int32.MaxValue)
                {
                    gold = checks = new Item[0];
                    return Int32.MaxValue;
                }
            }

            Container bank = m.Player ? m.BankBox : m.FindBankNoCreate();

            if (bank != null)
            {
                gold = bank.FindItemsByType(typeof(Gold));
                checks = bank.FindItemsByType(typeof(BankCheck));

                balance += gold.OfType<Gold>().Aggregate(0.0, (c, t) => c + t.Amount);
                balance += checks.OfType<BankCheck>().Aggregate(0.0, (c, t) => c + t.Worth);
            }
            else
            {
                gold = checks = new Item[0];
            }

            return (int)Math.Max(0, Math.Min(Int32.MaxValue, balance));
        }

        public static bool Withdraw(Mobile from, int amount, bool message = false)
        {
            // If for whatever reason the TOL checks fail, we should still try old methods for withdrawing currency.
            if (AccountGold.Enabled && from.Account != null && from.Account.WithdrawGold(amount))
            {
                if (message)
                    from.SendMessage(amount + " moedas foram debitadas de sua conta no banco");
                //from.SendLocalizedMessage(1155856, amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US"))); // ~1_AMOUNT~ gold has been removed from your bank box.

                return true;
            }

            Item[] gold, checks;
            var balance = GetBalance(from, out gold, out checks);

            if (balance < amount)
            {
                return false;
            }

            for (var i = 0; amount > 0 && i < gold.Length; ++i)
            {
                if (gold[i].Amount <= amount)
                {
                    amount -= gold[i].Amount;
                    gold[i].Delete();
                }
                else
                {
                    gold[i].Amount -= amount;
                    amount = 0;
                }
            }

            var a = amount;
            for (var i = 0; amount > 0 && i < checks.Length; ++i)
            {
                var check = (BankCheck)checks[i];

                if (check.Worth <= amount)
                {
                    amount -= check.Worth;
                    check.Delete();
                }
                else
                {
                    check.Worth -= amount;
                    amount = 0;
                }
            }

            if (message)
                from.SendLocalizedMessage(amount + " moedas foram retiradas de seu banco"); // ~1_AMOUNT~ gold has been removed from your bank box.
            else
            {
                from.SendMessage(55, $"[Banco] -{a} ({balance-a})");
            }
            return true;
        }

        public static bool Deposit(Mobile from, int amount, bool message = false)
        {
            // If for whatever reason the TOL checks fail, we should still try old methods for depositing currency.
            if (AccountGold.Enabled && from.Account != null && from.Account.DepositGold(amount))
            {
                if (message)
                    from.SendMessage("Foram depositadas " + amount + " moedas de ouro em sua conta no banco"); // ~1_AMOUNT~ gold was deposited in your account.

                return true;
            }

            var box = from.Player ? from.BankBox : from.FindBankNoCreate();

            if (box == null)
            {
                return false;
            }

            if (message)
            {
                from.PlaySound(0x2E6);
                from.SendMessage("Foram depositadas " + amount + " moedas em seu banco"); // ~1_AMOUNT~ gold was deposited in your account.
            }

            var items = new List<Item>();

            while (amount > 0)
            {
                Item item;
                if (amount < 5000)
                {
                    item = new Gold(amount);
                    amount = 0;
                }
                else if (amount <= 1000000)
                {
                    item = new BankCheck(amount);
                    amount = 0;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amount -= 1000000;
                }

                if (box.TryDropItem(from, item, false))
                {
                    items.Add(item);
                }
                else
                {
                    item.Delete();
                    foreach (var curItem in items)
                    {
                        curItem.Delete();
                    }

                    return false;
                }
            }

            return true;
        }

        public static int DepositUpTo(Mobile from, int amount, bool message = false)
        {
            // If for whatever reason the TOL checks fail, we should still try old methods for depositing currency.
            if (AccountGold.Enabled && from.Account != null && from.Account.DepositGold(amount))
            {
                if (message)
                    from.SendLocalizedMessage(1042763, amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US"))); // ~1_AMOUNT~ gold was deposited in your account.

                return amount;
            }

            var box = from.Player ? from.BankBox : from.FindBankNoCreate();

            if (box == null)
            {
                return 0;
            }

            var amountLeft = amount;
            while (amountLeft > 0)
            {
                Item item;
                int amountGiven;

                if (amountLeft < 5000)
                {
                    item = new Gold(amountLeft);
                    amountGiven = amountLeft;
                }
                else if (amountLeft <= 1000000)
                {
                    item = new BankCheck(amountLeft);
                    amountGiven = amountLeft;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amountGiven = 1000000;
                }

                if (box.TryDropItem(from, item, false))
                {
                    amountLeft -= amountGiven;
                }
                else
                {
                    item.Delete();
                    break;
                }
            }

            return amount - amountLeft;
        }

        public static void Deposit(Container cont, int amount)
        {
            while (amount > 0)
            {
                Item item;

                if (amount < 5000)
                {
                    item = new Gold(amount);
                    amount = 0;
                }
                else if (amount <= 1000000)
                {
                    item = new BankCheck(amount);
                    amount = 0;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amount -= 1000000;
                }

                cont.DropItem(item);
            }
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBanker());
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(Location, 12))
            {
                return true;
            }

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            HandleSpeech(this, e);

            base.OnSpeech(e);
        }

        public static void HandleSpeech(Mobile vendor, SpeechEventArgs e)
        {
            if (!e.Handled && e.Mobile.InRange(vendor, 12))
            {
                foreach (var keyword in e.Speech.ToLower().Split(' '))
                {
                    switch (keyword)
                    {
                        case "withdraw":
                        case "sacar": // *withdraw*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    if (Banker.Withdraw(e.Mobile, 50) || e.Mobile.Backpack.ConsumeTotal(typeof(Gold), 50))
                                    {
                                        vendor.Say("Peguei 50 moedas pelos servicos 'clandestinos' ...");
                                    }
                                    else
                                    {
                                        vendor.Say("Se voce tivesse umas 50 moedas poderia deixar um criminoso usar o banco...");
                                        return;
                                    }
                                }

                                var split = e.Speech.Split(' ');

                                if (split.Length >= 2)
                                {
                                    int amount;

                                    var pack = e.Mobile.Backpack;

                                    if (!int.TryParse(split[1], out amount))
                                    {
                                        break;
                                    }

                                    if ((!Core.ML && amount > 5000) || (Core.ML && amount > 60000))
                                    {
                                        // Thou canst not withdraw so much at one time!
                                        vendor.Say("Voce nao pode sacar tanto assim vai falir meu banco");
                                    }
                                    else if (pack == null || pack.Deleted || !(pack.TotalWeight < pack.MaxWeight) ||
                                             !(pack.TotalItems < pack.MaxItems))
                                    {
                                        // Your backpack can't hold anything else.
                                        vendor.Say("Voce nao aguenta isso tudo de moedas");
                                    }
                                    else if (amount > 0)
                                    {
                                        var box = e.Mobile.Player ? e.Mobile.BankBox : e.Mobile.FindBankNoCreate();

                                        if (box == null || !Withdraw(e.Mobile, amount))
                                        {
                                            // Ah, art thou trying to fool me? Thou hast not so much gold!
                                            vendor.Say("Voce nao tem dinheiro suficiente senhor(a)");
                                        }
                                        else
                                        {
                                            pack.DropItem(new Gold(amount));

                                            // Thou hast withdrawn gold from thy account.
                                            vendor.Say("Aqui estao suas moedas senhor(a)");
                                        }
                                    }
                                }
                            }
                            break;
                        case "aprimorar":
                            e.Handled = true;
                            if (e.Mobile.Criminal)
                            {
                                // I will not do business with a criminal!
                                vendor.Say("Xo criminoso");
                                break;
                            }

                            var nivelMax = BankLevels.Niveis.Count - 1;
                            var nivelAtual = ((PlayerMobile)e.Mobile).NivelBanco;

                            if (nivelAtual >= nivelMax)
                            {
                                vendor.Say("Voce ja eh nosso maior cliente VIP. Para aprimorar mais ainda seu banco aguarde construirmos cofres maiores.");
                                break;
                            }
                            e.Mobile.SendGump(new UpgradeBankGump(e.Mobile));
                            break;
                        case "balance":
                        case "saldo": // *balance*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    // I will not do business with a criminal!
                                    vendor.Say("Xo criminoso");
                                    break;
                                }

                                if (AccountGold.Enabled && e.Mobile.Account is Account)
                                {
                                    vendor.Say(1155855, String.Format("{0:#,0}\t{1:#,0}",
                                        e.Mobile.Account.TotalPlat,
                                        e.Mobile.Account.TotalGold), 0x3BC);

                                    vendor.Say(1155848, String.Format("{0:#,0}", ((Account)e.Mobile.Account).GetSecureAccountAmount(e.Mobile)), 0x3BC);
                                }
                                else
                                {
                                    // Thy current bank balance is ~1_AMOUNT~ gold.
                                    vendor.Say(1042759, GetBalance(e.Mobile).ToString("#,0"));
                                }
                            }
                            break;
                        case "banco": // *bank*
                        case "bank": // *bank*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    if (Banker.Withdraw(e.Mobile, 50) || e.Mobile.Backpack.ConsumeTotal(typeof(Gold), 50))
                                    {
                                        vendor.Say("Peguei 50 moedas pelos servicos 'clandestinos' ...");
                                    }
                                    else
                                    {
                                        vendor.Say("Se voce tivesse umas 50 moedas poderia deixar um criminoso usar o banco...");
                                        return;
                                    }
                                }

                                e.Mobile.SendMessage("Banco aberto. Para transformar dinheiro em cheques, diga 'cheque <valor>' para o NPC");
                                BankLevels.OpenBank(e.Mobile);
                            }
                            break;
                        case "check":
                        case "cheque": // *check*
                            {
                                e.Handled = true;

                                if (e.Mobile.Criminal)
                                {
                                    // I will not do business with a criminal!
                                    vendor.Say("Saaaai seu fih di capeta");
                                    break;
                                }

                                if (AccountGold.Enabled && e.Mobile.Account != null)
                                {
                                    vendor.Say("Nao estamos oferecendo este servico por hora.");
                                    break;
                                }

                                var split = e.Speech.Split(' ');

                                if (split.Length >= 2)
                                {
                                    int amount;

                                    if (!int.TryParse(split[1], out amount))
                                    {
                                        break;
                                    }

                                    if (amount < 2000)
                                    {
                                        // We cannot create checks for such a paltry amount of gold!
                                        vendor.Say("Nao posso criar um cheque para uma quantidade tao pequena");
                                    }
                                    else if (amount > int.MaxValue)
                                    {
                                        // Our policies prevent us from creating checks worth that much!
                                        vendor.Say("Nossa politica nao permitie criar cheques tao grandes");
                                    }
                                    else
                                    {
                                        var check = new BankCheck(amount);

                                        var box = e.Mobile.BankBox;

                                        if (!box.TryDropItem(e.Mobile, check, false))
                                        {
                                            // There's not enough room in your bankbox for the check!
                                            vendor.Say("Nao tem espaco no seu banco para o cheque");
                                            check.Delete();
                                        }
                                        else if (!box.ConsumeTotal(typeof(Gold), amount))
                                        {
                                            // Ah, art thou trying to fool me? Thou hast not so much gold!
                                            vendor.Say("Oras nao tens tal dinheiro, va trabalhar !");
                                            check.Delete();
                                        }
                                        else
                                        {
                                            // Into your bank box I have placed a check in the amount of:
                                            vendor.Say("Coloquei o cheque em seu banco senhor(a)");
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                list.Add(new OpenBankEntry(this));
            }

            base.AddCustomContextEntries(from, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
