using System;
using Server;
using Server.Mobiles;
using Server.Engines.Points;
using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.ShameRevamped
{
    public class ShameAltar : Item
    {
        public static readonly int CoolDown = 10;
        public static readonly bool AllowParties = true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SummonCost { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleporterLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleporterDestination { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Type GuardianType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Summoner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShameGuardian Guardian { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeadLine { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSummon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShameTeleporter Teleporter { get; set; }

        public Timer DeadLineTimer { get; set; }

        public override int LabelNumber { get { return 1151636; } } // Guardian's Altar
        public override bool ForceShowProperties { get { return true; } }

        public ShameAltar(Type type, Point3D teleLoc, Point3D teleDest, Point3D spawnLoc, int cost, bool active = true)
            : base(13801)
        {
            Name = "Altar do Guardiao";
            Movable = false;
            Hue = 2619;

            GuardianType = type;
            TeleporterLocation = teleLoc;
            TeleporterDestination = teleDest;
            SpawnLocation = spawnLoc;
            SummonCost = cost;

            Timer.DelayCall(TimeSpan.FromSeconds(1), SpawnTeleporter);

            Active = active;
            DeadLine = DateTime.MinValue;
            NextSummon = DateTime.UtcNow;
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Teleporter != null)
                Teleporter.Map = this.Map;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Active && from.InRange(this.Location, 3))
                CheckSummon(from);
        }

        public void CheckSummon(Mobile from)
        {
            var cristais = from.FindItemByType<CristalElemental>();
            if(cristais == null || cristais.Amount < SummonCost)
                from.SendLocalizedMessage("Voce nao tem cristais suficientes para lutar contra um guardiao !"); // You are not yet worthy of challenging the champion.
            else if (Guardian != null)
                from.SendLocalizedMessage("Ja existe um guardiao ativo neste nivel"); // The champion for this dungeon level has already been summoned.
            else if (NextSummon > DateTime.UtcNow)
                from.SendLocalizedMessage("Voce precisa aguardar para poder invocar o guardiao novamente"); // The champion has recently been defeated, and cannot be summoned again for a few minutes.
            else
            {
                cristais.Consume(SummonCost);

                SpawnGuardian();
                Summoner = from;

                if (Teleporter == null || Teleporter.Deleted)
                    SpawnTeleporter();

                from.SendLocalizedMessage("O Guardiao aceita seu desafio ! Voce tem 1h para derrota-lo !"); // The champion accepts your challenge. You have one hour to find and defeat him!
                from.SendLocalizedMessage("Para invocar o guardiao voce usou "+ SummonCost.ToString()+" cristais"); // Summoning the dungeon level's champion costs you ~1_COST~ Crystals of Shame.
                StartDeadlineTimer();
            }
        }

        public void SpawnGuardian()
        {
            Guardian = Activator.CreateInstance(GuardianType) as ShameGuardian;
            Guardian.Altar = this;
            Guardian.MoveToWorld(SpawnLocation, this.Map);

            Guardian.Home = SpawnLocation;
            Guardian.RangeHome = 8;

            DeadLine = DateTime.UtcNow + TimeSpan.FromHours(1);
            PublicOverheadMessage("* tremendo *");
        }

        public void OnGuardianKilled()
        {
            Guardian = null;

            EndDeadLineTimer();
            NextSummon = DateTime.UtcNow + TimeSpan.FromMinutes(CoolDown);
            Summoner = null;
        }

        public void CheckDeadLine()
        {
            if (DeadLine < DateTime.UtcNow)
                EndDeadLineTimer();
        }

        public void StartDeadlineTimer()
		{
			if(DeadLineTimer != null)
				DeadLineTimer.Stop();
		
			DeadLineTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), CheckDeadLine);
			DeadLineTimer.Start();
		}

        public void EndDeadLineTimer()
        {
            if (DeadLineTimer != null)
            {
                DeadLineTimer.Stop();
                DeadLineTimer = null;
            }

            if (Guardian != null && Guardian.Alive) // Failed
            {
                Guardian.Delete();

                if (Summoner != null)
                    Summoner.SendLocalizedMessage("Voce falhou em derrotar o guardiao a tempo !"); // You failed to defeat the champion in time.

                NextSummon = DateTime.UtcNow;
            }
        }

        private void SpawnTeleporter()
        {
            if (Teleporter != null)
                Teleporter.Delete();

            if (this.Map == null || this.Map == Map.Internal)
                return;

            IPooledEnumerable eable = this.Map.GetItemsInRange(TeleporterLocation, 0);
            foreach (Item item in eable)
            {
                if (item is ShameTeleporter)
                    item.Delete();
            }
            eable.Free();

            Teleporter = new ShameTeleporter(TeleporterDestination, this.Map);
            Teleporter.MoveToWorld(TeleporterLocation, this.Map);
        }

        public ShameAltar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Teleporter);
            writer.Write(TeleporterLocation);
            writer.Write(TeleporterDestination);
            writer.Write(SpawnLocation);
            writer.Write(Guardian);
            writer.Write(GuardianType.Name);
            writer.Write(Active);
            writer.Write(DeadLine);
            writer.Write(SummonCost);
            writer.Write(Summoner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Teleporter = reader.ReadItem() as ShameTeleporter;
            TeleporterLocation = reader.ReadPoint3D();
            TeleporterDestination = reader.ReadPoint3D();
            SpawnLocation = reader.ReadPoint3D();
            Guardian = reader.ReadMobile() as ShameGuardian;
            GuardianType = ScriptCompiler.FindTypeByName(reader.ReadString());
            Active = reader.ReadBool();
            DeadLine = reader.ReadDateTime();
            SummonCost = reader.ReadInt();
            Summoner = reader.ReadMobile();

            if (DeadLine > DateTime.UtcNow)
            {
                if (Guardian != null)
                {
                    Guardian.Altar = this;
                    StartDeadlineTimer();
                }
                else
                    DeadLine = DateTime.MinValue;
            }
            else if (Guardian != null)
            {
                Guardian.Delete();
                Guardian = null;

                NextSummon = DateTime.UtcNow;
            }
        }
    }
}
