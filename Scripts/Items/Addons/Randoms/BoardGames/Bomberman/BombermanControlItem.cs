#region References

using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Solaris.BoardGames;

#endregion

namespace Server.Items
{
    //a test of the boardgame system
    public class BombermanControlItem : BoardGameControlItem
    {
        public override string GameName { get { return "Bomberman"; } }

        public override string GameDescription
        {
            get
            {
                return
                    "Exploda paredes e jogadores com bombas. Colete upgrades para melhorar seu número de bombas, tamanho da explosão, etc..";
            }
        }

        public override string GameRules
        {
            get
            {
                return
                    "Cada jogo pode ter até oito jogadores e todos começam em um canto ou na borda da arena." +
                    "Uma bolsa de bomba é colocada nas mochilas dos jogadores. Os jogadores usam essa bolsa para colocar bombas a seus pés." +
                    "Uma bomba detonará após algum tempo e tem um tamanho de explosão limitado. O número de bombas que qualquer jogador pode colocar a qualquer momento é limitado. <BR> <BR>" +
                    "Os jogadores devem explodir nas paredes quebráveis ​​para navegar na arena." +
                    "Ao explodir, podem ser encontradas atualizações que melhoram o tamanho da explosão ou o número de bombas que um jogador pode colocar de uma vez." +
                    "Há também uma atualização de detonador que permite que um jogador escolha quando quer que suas bombas explodam." +
                    "Cuidado com as explosões de outros jogadores! Uma bomba pode acionar outra bomba, criando reações em cadeia interessantes! <BR> <BR>" +
                    "O jogo termina quando resta apenas um jogador em pé.";
            }
        }

        public override bool CanCastSpells { get { return false; } }
        public override bool CanUseSkills { get { return false; } }
        public override bool CanUsePets { get { return false; } }

        public override TimeSpan WinDelay { get { return TimeSpan.FromSeconds(5); } }

        //bomberman main controller must be accessed from ground
        public override bool UseFromBackpack { get { return false; } }

        //only 1 to 8 players allowed in a bomberman game
        public override int MinPlayers { get { return 2; } }
        public override int MaxPlayers { get { return 8; } }

        protected BombermanStyle _Style;

        [CommandProperty(AccessLevel.GameMaster)]
        public BombermanStyle Style
        {
            get { return _Style; }
            set
            {
                if ((int) _State <= (int) BoardGameState.Recruiting)
                {
                    _Style = value;
                    ResetBoard();
                }
            }
        }

        //reference to be bomb bags that are handed out for the game
        protected List<BombBag> _BombBags;

        public List<BombBag> BombBags
        {
            get { return _BombBags ?? (_BombBags = new List<BombBag>()); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int BoardWidth
        {
            get
            {
                _BoardWidth = Math.Max(BombermanSettings.MIN_BOARD_SIZE,
                    Math.Min(BombermanSettings.MAX_BOARD_SIZE, _BoardWidth));
                return _BoardWidth;
            }
            set
            {
                if ((int) _State <= (int) BoardGameState.Recruiting)
                {
                    _BoardWidth = Math.Max(BombermanSettings.MIN_BOARD_SIZE,
                        Math.Min(BombermanSettings.MAX_BOARD_SIZE, value));

                    if ((_BoardWidth & 1) == 0)
                    {
                        _BoardWidth += 1;
                    }
                    ResetBoard();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int BoardHeight
        {
            get
            {
                _BoardHeight = Math.Max(BombermanSettings.MIN_BOARD_SIZE,
                    Math.Min(BombermanSettings.MAX_BOARD_SIZE, _BoardHeight));
                return _BoardHeight;
            }
            set
            {
                if ((int) _State <= (int) BoardGameState.Recruiting)
                {
                    _BoardHeight = Math.Max(BombermanSettings.MIN_BOARD_SIZE,
                        Math.Min(BombermanSettings.MAX_BOARD_SIZE, value));

                    if ((_BoardHeight & 1) == 0)
                    {
                        _BoardHeight += 1;
                    }
                    ResetBoard();
                }
            }
        }

        protected int _DefaultMaxBombs = 2;
        protected int _DefaultBombStrength = 1;
        protected bool _DefaultDetonatorMode = false;
        protected bool _DefaultBaddaBoom = false;

        [CommandProperty(AccessLevel.GameMaster)]
        public int DefaultMaxBombs { get { return _DefaultMaxBombs; } set { _DefaultMaxBombs = Math.Max(1, value); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DefaultBombStrength
        {
            get { return _DefaultBombStrength; }
            set { _DefaultBombStrength = Math.Max(1, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DefaultDetonatorMode
        {
            get { return _DefaultDetonatorMode; }
            set { _DefaultDetonatorMode = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DefaultBaddaBoom { get { return _DefaultBaddaBoom; } set { _DefaultBaddaBoom = value; } }


        //main constructor

        [Constructable]
        public BombermanControlItem()
        {
            ItemID = 0xED4; //guild gravestone

            Name = "Bomberman :D ";

            BoardWidth = BombermanSettings.DEFAULT_BOARD_SIZE;
            BoardHeight = BombermanSettings.DEFAULT_BOARD_SIZE;

            _State = BoardGameState.Inactive;

            _BoardOffset = new Point3D(2, 2, 0);
        }

        //deserialization constructor
        public BombermanControlItem(Serial serial) : base(serial)
        {}


        //this method initializes the game control and connects it with this item

        public override void UpdatePosition()
        {
            GameZone = new Rectangle3D(new Point3D(X + BoardOffset.X, Y + BoardOffset.X, BoardOffset.Z - 100),
                new Point3D(X + BoardOffset.X + BoardWidth, Y + BoardOffset.Y + BoardHeight, BoardOffset.Z + 100));

            base.UpdatePosition();
        }

        public override void AddPlayer(Mobile m)
        {
            base.AddPlayer(m);

            PublicOverheadMessage(MessageType.Regular, 1153, false, "Adicionando " + m.Name + " ao jogo!");

            //if this is the first player to be added, they can also choose the board style
            if (Players.Count == 1 && _AllowPlayerConfiguration)
            {
                PublicOverheadMessage(MessageType.Regular, 1153, false,
                    Players[0].Name + " Esta no comando do jogo!");
                m.SendGump(new SelectBombermanStyleGump(m, this));
            }

            if (Players.Count < CurrentMaxPlayers)
            {
                int requiredplayers = CurrentMaxPlayers - Players.Count;

                PublicOverheadMessage(MessageType.Regular, 1153, false, requiredplayers + " jogadores a mais!");
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 1153, false, Players[0].Name + " precisam confirmar o estilo!");
            }
        }

        public override void RemovePlayer(Mobile m)
        {
            if (Players.IndexOf(m) > -1)
            {
                PublicOverheadMessage(MessageType.Regular, 1153, false, "Removendo " + m.Name + " do jogo!");
            }

            if (Players.IndexOf(m) == 0 && Players.Count > 1)
            {
                PublicOverheadMessage(MessageType.Regular, 1153, false,
                    Players[1].Name + " esta encarregado do jogo!");
                SettingsReady = false;

                Players[1].SendGump(new SelectBombermanStyleGump(Players[1], this));

                Players[1].SendMessage("Voce esta encarregado de configurar o jogo!");
            }


            base.RemovePlayer(m);

            if (Players.Count > 0)
            {
                int requiredplayers = CurrentMaxPlayers - Players.Count;

                PublicOverheadMessage(MessageType.Regular, 1153, false, requiredplayers + " jogadores a mais!");
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 1153, false, "Nao tem mais jogadores, resetando.");
            }
        }

        public override void BuildBoard()
        {
            UpdatePosition();

            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)
                {
                    //build the ground
                    var groundpiece = new BombermanFloorTile(_Style);
                    groundpiece.RegisterToBoardGameControlItem(this, new Point3D(i, j, 0));

                    BackgroundItems.Add(groundpiece);


                    //build the outer walls and inner grid walls
                    if (i == 0 || i == BoardWidth - 1 || j == 0 || j == BoardHeight - 1 || j % 2 == 0 && i % 2 == 0)
                    {
                        var wallpiece = new IndestructableWall(_Style, true);
                        wallpiece.RegisterToBoardGameControlItem(this, new Point3D(i, j, 0));

                        BackgroundItems.Add(wallpiece);
                    }
                    else
                    {
                        if (_State == BoardGameState.Active) //if a game is active, then build obstacles and such
                        {
                            //don't put obstacles in the player starting positions
                            if (i < 3 && j < 3 || i > BoardWidth - 4 && j < 3 || i < 3 && j > BoardHeight - 4 ||
                                i > BoardWidth - 4 && j > BoardHeight - 4)
                            {
                                continue;
                            }
                            if (j > BoardHeight / 2 - 2 && j < BoardHeight / 2 + 2 && (i < 3 || i > BoardWidth - 4))
                            {
                                continue;
                            }
                            if (i > BoardWidth / 2 - 2 && i < BoardWidth / 2 + 2 &&
                                (j < 3 || j > BoardHeight - 4))
                            {
                                continue;
                            }

                            //obstacles
                            if (Utility.RandomDouble() < BombermanSettings.OBSTACLE_CHANCE)
                            {
                                var wallpiece = new DestructableWall(_Style);
                                wallpiece.RegisterToBoardGameControlItem(this, new Point3D(i, j, 0));

                                BackgroundItems.Add(wallpiece);
                            }
                        }
                    }
                }
            }

            base.BuildBoard();
        }

        protected override void PrimePlayers()
        {
            base.PrimePlayers();

            for (int i = 0; i < Players.Count; i++)
            {
                Mobile player = Players[i];

                Point3D movepoint;
                switch (i)
                {
                    case 0:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + 1, Y + BoardOffset.Y + 1, Z + BoardOffset.Z);
                        break;
                    }
                    case 1:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + BoardWidth - 2, Y + BoardOffset.Y + 1,
                            Z + BoardOffset.Z);
                        break;
                    }
                    case 2:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + 1, Y + BoardOffset.Y + BoardHeight - 2,
                            Z + BoardOffset.Z);
                        break;
                    }
                    case 3:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + + BoardWidth - 2,
                            Y + BoardOffset.Y + + BoardHeight - 2, Z + BoardOffset.Z);
                        break;
                    }
                    case 4:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + BoardWidth / 2, Y + BoardOffset.Y + 1,
                            Z + BoardOffset.Z);
                        break;
                    }
                    case 5:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + BoardWidth - 2, Y + BoardOffset.Y + BoardHeight / 2,
                            Z + BoardOffset.Z);
                        break;
                    }
                    case 6:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + BoardWidth / 2, Y + BoardOffset.Y + BoardHeight - 2,
                            Z + BoardOffset.Z);
                        break;
                    }
                    case 7:
                    default:
                    {
                        movepoint = new Point3D(X + BoardOffset.X + 1, Y + BoardOffset.Y + BoardHeight / 2,
                            Z + BoardOffset.Z);
                        break;
                    }
                }

                player.MoveToWorld(movepoint, BoardMap);


                var bag = new BombBag(this, _DefaultMaxBombs, _DefaultBombStrength);


                BombBags.Add(bag);
                bag.Owner = player;
                player.Backpack.DropItem(bag);


                if (_DefaultDetonatorMode)
                {
                    var detonator = new BombDetonator(bag);

                    bag.Detonator = detonator;
                    player.Backpack.DropItem(detonator);
                }


                bag.BaddaBoom = _DefaultBaddaBoom;
            }
        }

        public void CheckForMobileVictims(Point3D location, Map map, BombBag sourcebag)
        {
            IPooledEnumerable ie = map.GetMobilesInRange(location, 0);

            var tomove = new List<Mobile>();

            foreach (Mobile m in ie)
            {
                if (Players.IndexOf(m) > -1)
                {
                    if (m != sourcebag.Owner)
                    {
                        m.SendMessage("Voce foi explodido por " + sourcebag.Owner.Name + "!");

                        sourcebag.Owner.SendMessage("Voce explodiu " + m.Name + "!");

                        //handle scoring
                        BoardGameData.ChangeScore(GameName, sourcebag.Owner, BombermanSettings.KILL_SCORE);

                        BoardGameData.ChangeScore(GameName, m, BombermanSettings.DEATH_SCORE);

                        PublicOverheadMessage(MessageType.Regular, 1153, false,
                            sourcebag.Owner.Name + " explodiu " + m.Name + "!");
                    }
                    else
                    {
                        m.SendMessage("Voce explodiu voce mesmo :D !!");

                        PublicOverheadMessage(MessageType.Regular, 1153, false, m.Name + " Explodiu a ele mesmo ! LOL!");

                        BoardGameData.ChangeScore(GameName, m, BombermanSettings.SUICIDE_SCORE);
                    }
                    BoardGameData.AddLose(GameName, m);


                    m.PlaySound(m.Female ? 0x32E : 0x549);
                    //0x54A - yelp1 

                    tomove.Add(m);
                }
            }
            ie.Free();

            foreach (Mobile m in tomove)
            {
                m.MoveToWorld(new Point3D(X - 1, Y - 1, Z), Map);
                m.SendGump(new BoardGameLostGump(m, this));

                Players.Remove(m);

                var bag = (BombBag) m.Backpack.FindItemByType(typeof(BombBag));

                if (bag != null)
                {
                    //don't let players run around blowing stuff up outside the game while they wait for others to finish
                    bag.Active = false;
                }

                //start the timer to check for endgame, delay for 1s
            }
            //test big bomb chain!
            StartEndGameTimer(TimeSpan.FromSeconds(1));
        }

        //TODO: move into base group?
        protected override void OnEndGameTimer()
        {
            base.OnEndGameTimer();

            if (Players.Count < 2)
            {
                AnnounceWinner();
            }
        }

        protected override void AnnounceWinner()
        {
            base.AnnounceWinner();
            if (Players.Count == 1)
            {
                Players[0].SendGump(new BoardGameWonGump(Players[0], this));

                BoardGameData.ChangeScore(GameName, Players[0], BombermanSettings.WIN_SCORE);
                BoardGameData.AddWin(GameName, Players[0]);

                var bag = (BombBag) Players[0].Backpack.FindItemByType(typeof(BombBag));

                if (bag != null)
                {
                    //don't let players run around blowing stuff up outside the game while they wait for others to finish
                    bag.Active = false;
                }

                PublicOverheadMessage(MessageType.Regular, 1153, false, Players[0].Name + " ganhou o jogo!");
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 1153, false, "Empate !");
            }
        }

        public override void EndGame()
        {
            base.EndGame();

            if (Map != null)
            {
                IPooledEnumerable ie =
                    Map.GetItemsInBounds(new Rectangle2D(new Point2D(GameZone.Start.X, GameZone.Start.Y),
                        new Point2D(GameZone.End.X, GameZone.End.Y)));

                var todelete = new List<BombermanUpgrade>();

                foreach (Item item in ie)
                {
                    if (item is BombermanUpgrade)
                    {
                        todelete.Add((BombermanUpgrade) item);
                    }
                }

                ie.Free();

                foreach (BombermanUpgrade item in todelete)
                {
                    item.Destroy();
                }


                //there should only be one left.. the winner
                foreach (Mobile player in Players)
                {
                    player.MoveToWorld(new Point3D(X - 1, Y - 1, Z), Map);
                }
            }

            foreach (BombBag bag in BombBags)
            {
                if (bag != null)
                {
                    bag.Delete();
                }
            }
            _BombBags = null;

            //announce winner?
            if (Players.Count == 1)
            {
                Players[0].SendMessage("Voce ganhou o jogo!");
            }

            _Players = null;
            _State = BoardGameState.Inactive;
            InvalidateProperties();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            foreach (BombBag bag in BombBags)
            {
                if (bag != null)
                {
                    bag.Delete();
                }
            }
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int) _Style);

            writer.Write(_DefaultMaxBombs);
            writer.Write(_DefaultBombStrength);
            writer.Write(_DefaultDetonatorMode);
            writer.Write(_DefaultBaddaBoom);

            writer.Write(BombBags.Count);

            foreach (BombBag bag in BombBags)
            {
                writer.Write(bag);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Style = (BombermanStyle) reader.ReadInt();

            _DefaultMaxBombs = reader.ReadInt();
            _DefaultBombStrength = reader.ReadInt();
            _DefaultDetonatorMode = reader.ReadBool();
            _DefaultBaddaBoom = reader.ReadBool();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                BombBags.Add((BombBag) reader.ReadItem());
            }
        }
    }
}
