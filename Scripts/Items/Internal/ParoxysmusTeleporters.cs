using System;

namespace Server.Items
{
    public class ParoxysmusTele : Teleporter
    { 
        [Constructable]
        public ParoxysmusTele()
            : base(new Point3D(6222, 335, 60), Map.Trammel)
        {
        }

        public ParoxysmusTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.NetState == null || !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return true;
            }
            else if (!MondainsLegacy.PalaceOfParoxysmus && (int)m.AccessLevel < (int)AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1042753, "Palace of Paroxysmus"); // ~1_SOMETHING~ has been temporarily disabled.
                return true;
            }
		
            if (m.Backpack != null)
            {
                Item rope = m.Backpack.FindItemByType(typeof(MagicalRope), true);
				
                if (rope == null)
                    rope = m.Backpack.FindItemByType(typeof(AcidProofRope), true);
			
                if (rope != null && !rope.Deleted)
                {
                    if (Utility.RandomDouble() < 0.15)
                    {
                        m.SendLocalizedMessage("A corda se rompeu"); // Your rope is severely damaged by the acidic environment.  You're lucky to have made it safely to the ground.
                        rope.Delete();
                    }
                    else
                        m.SendLocalizedMessage("A corda foi enfraquecida"); // Your rope has been weakened by the acidic environment.
					
                    return base.OnMoveOver(m);					
                } else
                {
                    if(!m.IsCooldown("dicorda"))
                    {
                        m.SetCooldown("dicorda", TimeSpan.FromMinutes(10));
                        m.SendMessage(78, "Voce precisa de uma corda que seja resistente a acidos para entrar aqui");
                    }
                }
            }
            else
                m.SendLocalizedMessage(1074272); // You have no way to lower yourself safely into the enormous sinkhole.
			
            return true;	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class ParoxysmusTeleFel : Teleporter
    {
        [Constructable]
        public ParoxysmusTeleFel()
            : base(new Point3D(6222, 335, 60), Map.Felucca)
        {
        }

        public ParoxysmusTeleFel(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.NetState == null || !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return true;
            }
            else if (!MondainsLegacy.PalaceOfParoxysmus && (int)m.AccessLevel < (int)AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1042753, "Palace of Paroxysmus"); // ~1_SOMETHING~ has been temporarily disabled.
                return true;
            }

            if (m.Backpack != null)
            {
                Item rope = m.Backpack.FindItemByType(typeof(MagicalRope), true);

                if (rope == null)
                    rope = m.Backpack.FindItemByType(typeof(AcidProofRope), true);

                if (rope != null && !rope.Deleted)
                {
                    if (Utility.RandomDouble() < 0.3)
                    {
                        m.SendLocalizedMessage(1075097); // Your rope is severely damaged by the acidic environment.  You're lucky to have made it safely to the ground.
                        rope.Delete();
                    }
                    else
                        m.SendLocalizedMessage(1075098); // Your rope has been weakened by the acidic environment.

                    return base.OnMoveOver(m);
                }
            }
            else
                m.SendLocalizedMessage(1074272); // You have no way to lower yourself safely into the enormous sinkhole.

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
