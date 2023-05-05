#region References

using Server.Items;

#endregion

namespace Server.Gumps
{
    public class BoardGameLostGump : BoardGameGump
    {
        public override int Height { get { return 140; } }
        public override int Width { get { return 300; } }

        public BoardGameLostGump(Mobile owner, BoardGameControlItem controlitem) : base(owner, controlitem)
        {
            AddLabel(40, 20, 1152, "Jogo:");

            AddLabel(140, 20, 1172, _ControlItem.GameName);

            AddLabel(40, 50, 1152, "Voce perdeu o jogo!");

            //TODO: add info about points earned?

            AddButton(100, 80, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0);
        }
    }
}
