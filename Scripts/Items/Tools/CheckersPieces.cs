using System;

namespace Server.Items
{
    public class PieceWhiteChecker : BasePiece
    {
        public PieceWhiteChecker(BaseBoard board)
            : base(0x3584, board)
        {
        }

        public PieceWhiteChecker(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "peca branca";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PieceBlackChecker : BasePiece
    {
        public PieceBlackChecker(BaseBoard board)
            : base(0x358B, board)
        {
        }

        public PieceBlackChecker(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "peca preta";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
