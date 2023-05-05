using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Ziden.Traducao;

namespace Server.Engines.Craft
{
    public class CraftGumpItem : Gump
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly CraftItem m_CraftItem;
        private readonly ITool m_Tool;

        private const int LabelHue = 0; // 0x384
        private const int RedLabelHue = 0;

        private const int LabelColor = 0;
        private const int RedLabelColor = 0x6400;

        private const int GreyLabelColor = 0;

        private int m_OtherCount;

        public CraftGumpItem(Mobile from, CraftSystem craftSystem, CraftItem craftItem, ITool tool)
            : base(40, 40)
        {
            this.m_From = from;
            this.m_CraftSystem = craftSystem;
            this.m_CraftItem = craftItem;
            this.m_Tool = tool;

            from.CloseGump(typeof(CraftGump));
            from.CloseGump(typeof(CraftGumpItem));

            AddPage(0);
            AddBackground(0, 0, 530, 417, 5054);

            AddBackground(10, 10, 510, 22, 9350); // Titulo do Menu
            AddBackground(10, 37, 150, 134, 9350); // Foto do Item
            AddBackground(165, 37, 355, 70, 9350); // Item
            // AddBackground(10, 190, 155, 22, 9350);
            // AddBackground(10, 240, 150, 57, 9350);
            AddBackground(165, 112, 355, 60, 9350); // Skill
            // AddBackground(10, 325, 150, 57, 9350);
            AddBackground(165, 177, 355, 120, 9350); // Materiais
            AddBackground(165, 302, 355, 80, 9350);  // Outros
            AddBackground(10, 387, 510, 22, 9350); // Botoes 

            //AddAlphaRegion(10, 10, 510, 399);
            //AddBackground(10, 10, 510, 399, 9350);
            AddHtml(170, 40, 150, 20, "Item", false, false); // ITEM
            AddHtml(10, 180, 150, 22, "Materiais", false, false); // <CENTER>MATERIALS</CENTER>
            AddHtml(10, 302, 150, 22, "Outros", false, false); // <CENTER>OTHER</CENTER>

            if (craftSystem.GumpTitleNumber > 0)
                this.AddHtmlLocalized(10, 12, 510, 20, craftSystem.GumpTitleNumber, LabelColor, false, false);
            else
                this.AddHtml(10, 12, 510, 20, craftSystem.GumpTitleString, false, false);

            bool needsRecipe = (craftItem.Recipe != null && from is PlayerMobile && !((PlayerMobile)from).HasRecipe(craftItem.Recipe));

            if (needsRecipe)
            {
                this.AddButton(405, 387, 4005, 4007, 0, GumpButtonType.Page, 0);
                this.AddHtml(440, 390, 150, 18, "Craftar", false, false); // MAKE NOW
            }
            else
            {
                this.AddButton(405, 387, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtml(445, 390, 150, 18, "Craftar", false, false); // MAKE NOW
            }

            #region Stygian Abyss
            AddButton(265, 387, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtml(300, 390, 150, 18, "Fazer Numero", false, false); //MAKE NUMBER

            AddButton(135, 387, 4005, 4007, 3, GumpButtonType.Reply, 0);
            //AddHtmlLocalized(170, 390, 150, 18, 1112624, LabelColor, false, false); //MAKE MAX
            AddHtml(170, 390, 150, 18, "Fazer Infinito", false, false); //MAKE MAX)
            #endregion

            this.AddButton(15, 387, 4014, 4016, 0, GumpButtonType.Reply, 0);
            this.AddHtml(50, 390, 150, 18, "Voltar", false, false); // BACK

            if (craftItem.NameNumber > 0)
                this.AddHtmlLocalized(330, 40, 180, 18, craftItem.NameNumber, LabelColor, false, false);
            else
                this.AddLabel(330, 40, LabelHue, craftItem.NameString);

            if (craftItem.UseAllRes)
                this.AddHtmlLocalized(170, 302 + (this.m_OtherCount++ * 20), 310, 18, 1048176, LabelColor, false, false); // Makes as many as possible at once

            this.DrawItem();
            this.DrawSkill();
            this.DrawResource();

            /*
            if( craftItem.RequiresSE )
            AddHtmlLocalized( 170, 302 + (m_OtherCount++ * 20), 310, 18, 1063363, LabelColor, false, false ); //* Requires the "Samurai Empire" expansion
            * */

            if (craftItem.RequiredExpansion != Expansion.None)
            {
                bool supportsEx = (from.NetState != null && from.NetState.SupportsExpansion(craftItem.RequiredExpansion));
                TextDefinition.AddHtmlText(this, 170, 302 + (this.m_OtherCount++ * 20), 310, 18, this.RequiredExpansionMessage(craftItem.RequiredExpansion), false, false, supportsEx ? LabelColor : RedLabelColor, supportsEx ? LabelHue : RedLabelHue);
            }

            if (craftItem.RequiredThemePack != ThemePack.None)
            {
                TextDefinition.AddHtmlText(this, 170, 302 + (this.m_OtherCount++ * 20), 310, 18, this.RequiredThemePackMessage(craftItem.RequiredThemePack), false, false, LabelColor, LabelHue);
            }

            if (needsRecipe)
                this.AddHtml(170, 302 + (this.m_OtherCount++ * 20), 310, 18, "!! Voce ainda nao aprendeu esta receita !!", false, false); // You have not learned this recipe.
        }

        private TextDefinition RequiredExpansionMessage(Expansion expansion)
        {
            switch (expansion)
            {
                case Expansion.SE:
                    return 1063363; // * Requires the "Samurai Empire" expansion
                case Expansion.ML:
                    return 1072651; // * Requires the "Mondain's Legacy" expansion
                case Expansion.SA:
                    return 1094732; // * Requires the "Stygian Abyss" expansion
                case Expansion.HS:
                    return 1116296; // * Requires the "High Seas" booster
                case Expansion.TOL:
                    return 1155876; // * Requires the "Time of Legends" expansion.
                default:
                    return String.Format("* Requires the \"{0}\" expansion", ExpansionInfo.GetInfo(expansion).Name);
            }
        }

        private TextDefinition RequiredThemePackMessage(ThemePack pack)
        {
            switch (pack)
            {
                case ThemePack.Kings:
                    return 1154195; // *Requires the "King's Collection" theme pack
                case ThemePack.Rustic:
                    return 1150651; // * Requires the "Rustic" theme pack
                case ThemePack.Gothic:
                    return 1150650; // * Requires the "Gothic" theme pack
                default:
                    return String.Format("Requires the \"{0}\" theme pack.", null);
            }
        }

        private bool m_ShowExceptionalChance;

        public void DrawItem()
        {
            Type type = m_CraftItem.ItemType;
            int id = m_CraftItem.DisplayID;
            if (id == 0) id = CraftItem.ItemIDOf(type);
            Rectangle2D b = ItemBounds.Table[id];
            AddItem(90 - b.Width / 2 - b.X, 110 - b.Height / 2 - b.Y, id, m_CraftItem.ItemHue);

            if (m_CraftItem.IsMarkable(type))
            {
                AddHtml(170, 302 + (m_OtherCount++ * 20), 310, 18, "Este item pode ser assinado", false, false); // This item may hold its maker's mark
                m_ShowExceptionalChance = true;
            }
        }

        public void DrawSkill()
        {
            for (int i = 0; i < this.m_CraftItem.Skills.Count; i++)
            {
                CraftSkill skill = this.m_CraftItem.Skills.GetAt(i);
                double minSkill = skill.MinSkill, maxSkill = skill.MaxSkill;

                if (minSkill < 0)
                    minSkill = 0;

                if (skill.SkillToMake == SkillName.TasteID)
                {
                    this.AddHtml(170, 114 + (i * 20), 200, 18, "Jewelcrafting", LabelColor, false, false);
                }
                else
                {
                    this.AddHtmlLocalized(170, 114 + (i * 20), 200, 18, AosSkillBonuses.GetLabel(skill.SkillToMake), LabelColor, false, false);
                }

                this.AddLabel(430, 114 + (i * 20), LabelHue, String.Format("{0:F1}", minSkill));
            }

            CraftSubResCol res = (this.m_CraftItem.UseSubRes2 ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

            if (context != null)
                resIndex = (this.m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            bool allRequiredSkills = true;
            double chance = this.m_CraftItem.GetSuccessChance(this.m_From, resIndex > -1 ? res.GetAt(resIndex).ItemType : null, this.m_CraftSystem, false, ref allRequiredSkills);
            double excepChance = this.m_CraftItem.GetExceptionalChance(this.m_CraftSystem, chance, this.m_From);

            if (chance < 0.0)
                chance = 0.0;
            else if (chance > 1.0)
                chance = 1.0;

            this.AddHtml(170, 63, 250, 18, "Chance de Acerto:", false, false); // Success Chance:
            this.AddLabel(430, 63, LabelHue, String.Format("{0:F1}%", chance * 100));

            if (this.m_ShowExceptionalChance)
            {
                if (excepChance < 0.0)
                    excepChance = 0.0;
                else if (excepChance > 1.0)
                    excepChance = 1.0;

                this.AddHtml(170, 83, 250, 18, "Chance Excepcional:", false, false); // Exceptional Chance:
                this.AddLabel(430, 83, LabelHue, String.Format("{0:F1}%", excepChance * 100));
            }
        }

        private static readonly Type typeofBlankScroll = typeof(BlankScroll);
        private static readonly Type typeofSpellScroll = typeof(SpellScroll);

        public void DrawResource()
        {
            bool retainedColor = false;

            CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

            CraftSubResCol res = (this.m_CraftItem.UseSubRes2 ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            if (context != null)
                resIndex = (this.m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            bool cropScroll = (this.m_CraftItem.Resources.Count > 1) &&
                              this.m_CraftItem.Resources.GetAt(this.m_CraftItem.Resources.Count - 1).ItemType == typeofBlankScroll &&
                              typeofSpellScroll.IsAssignableFrom(this.m_CraftItem.ItemType);

            for (int i = 0; i < this.m_CraftItem.Resources.Count - (cropScroll ? 1 : 0) && i < 6; i++)
            {
                Type type;
                string nameString;
                int nameNumber;

                CraftRes craftResource = this.m_CraftItem.Resources.GetAt(i);

                type = craftResource.ItemType;
                nameString = craftResource.NameString;
                nameNumber = craftResource.NameNumber;

                // Resource Mutation
                if (type == res.ResType && resIndex > -1)
                {
                    CraftSubRes subResource = res.GetAt(resIndex);

                    type = subResource.ItemType;

                    nameString = subResource.NameString;
                    nameNumber = subResource.GenericNameNumber;

                    if (nameNumber <= 0)
                        nameNumber = subResource.NameNumber;
                }
                // ******************

                if (Shard.DebugEnabled)
                    Shard.Debug("Lendo type de " + type.GetTypeName(true));

                var resres = CraftResources.GetFromType(type);
                if (resres != CraftResource.None)
                {
                    var tipo = CraftResources.GetType(resres);
                    string nome = "Couro ";
                    if (tipo == CraftResourceType.Wood)
                        nome = "Tabuas de ";
                    else if (tipo == CraftResourceType.Metal)
                        nome = "Lingotes de ";
                    else if (tipo == CraftResourceType.Scales)
                        nome = "Escamas ";

                    nameString = nome + resres.ToString();
                    nameNumber = 0;

                }
                else
                {
                    var trad = Trads.GetNome(type);
                    if (trad != null)
                    {
                        if (Shard.DebugEnabled)
                            Shard.Debug("Traduzindo " + nameString + " para " + trad);
                        nameString = trad;
                        nameNumber = 0;
                    }
                }



                if (!retainedColor && this.m_CraftItem.RetainsColorFrom(this.m_CraftSystem, type))
                {
                    retainedColor = true;
                    this.AddHtml(170, 302 + (this.m_OtherCount++ * 20), 310, 18, "Mantem a cor do material", false, false); // * The item retains the color of this material
                    this.AddLabel(500, 180 + (i * 20), LabelHue, "*");
                }

                if (nameNumber > 0)
                    this.AddHtmlLocalized(170, 180 + (i * 20), 310, 18, nameNumber, LabelColor, false, false);
                else
                    this.AddLabel(170, 180 + (i * 20), LabelHue, nameString);

                this.AddLabel(430, 180 + (i * 20), LabelHue, craftResource.Amount.ToString());
            }

            if (this.m_CraftItem.NameNumber == 1041267) // runebook
            {
                this.AddHtmlLocalized(170, 180 + (this.m_CraftItem.Resources.Count * 20), 310, 18, 1044447, LabelColor, false, false);
                this.AddLabel(430, 180 + (this.m_CraftItem.Resources.Count * 20), LabelHue, "1");
            }

            if (cropScroll)
                this.AddHtmlLocalized(170, 302 + (this.m_OtherCount++ * 20), 360, 18, 1044379, LabelColor, false, false); // Inscribing scrolls also requires a blank scroll and mana.
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: // Back Button
                    {
                        CraftGump craftGump = new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, null);
                        this.m_From.SendGump(craftGump);
                        break;
                    }
                case 1: // Make Button
                    {
                        int num = this.m_CraftSystem.CanCraft(this.m_From, this.m_Tool, this.m_CraftItem.ItemType);

                        if (num > 0)
                        {
                            this.m_From.SendGump(new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, num));
                        }
                        else
                        {
                            Type type = null;

                            CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

                            if (context != null)
                            {
                                CraftSubResCol res = (this.m_CraftItem.UseSubRes2 ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);
                                int resIndex = (this.m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

                                if (resIndex > -1)
                                    type = res.GetAt(resIndex).ItemType;
                            }

                            this.m_CraftSystem.CreateItem(this.m_From, this.m_CraftItem.ItemType, type, this.m_Tool, this.m_CraftItem);
                        }
                        break;
                    }
                case 2: //Make Number
                    m_From.Prompt = new MakeNumberCraftPrompt(m_From, m_CraftSystem, m_CraftItem, m_Tool);
                    m_From.SendLocalizedMessage(1112576); //Please type the amount you wish to create(1 - 100): <Escape to cancel>
                    break;
                case 3: //Make Max 
                    AutoCraftTimer.EndTimer(m_From);
                    new AutoCraftTimer(m_From, m_CraftSystem, m_CraftItem, m_Tool, 9999, TimeSpan.FromSeconds(m_CraftSystem.Delay * m_CraftSystem.MaxCraftEffect + 0.5), TimeSpan.FromSeconds(m_CraftSystem.Delay * m_CraftSystem.MaxCraftEffect + 0.5));
                    break;
            }
        }
    }
}
