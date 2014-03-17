using System;
using BraveChess.Base;
using BraveChess.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace BraveChess.Engines
{
    public enum NetworkState
    {
        SignIn,
        FindSession,
        CreateSession,
        Start,
        InGame,
        GameOverWhiteWins,
        GameOverBlackWins,
        GameOverDraw
    }

    public enum MessageType
    {
        StartGame,
        EndGame,
        RestartGame,
        RejoinLobby,
        UpdateOtherMove,
        ChangeTurnState,
        Null
    }

    public class NetworkEngine 
    {
        public NetworkState CurrentNetworkState
        {
            get { return _currentNetworkState; }
            set { _currentNetworkState = value; }
        }
      
        public byte[] Player = new byte[2];
        public bool[] Turn = { false, true };

        // networking variables
        public NetworkSession NetworkSession;
        public PacketWriter PacketWriter = new PacketWriter();
        public PacketReader PacketReader = new PacketReader();

        NetworkState _currentNetworkState = NetworkState.SignIn;
        readonly Game _game;
        readonly GameEngine _engine;

        public NetworkEngine(Game g, GameEngine e)
        {
            _game = g;
            _engine = e;
        }

        public void Update(GameTime gameTime)
        {
            if (_game.IsActive)
            {
                switch (_currentNetworkState)
                {
                    case NetworkState.SignIn:
                        Update_SignIn();//sign in
                        break;
                    case NetworkState.FindSession:
                        Update_findSession();//finds session
                        break;
                    case NetworkState.CreateSession:
                        CreateSession();//creates session
                        break;
                    case NetworkState.Start:
                        Start(gameTime);//starts game
                        break;
                    case NetworkState.InGame:
                        //inside game
                        break;
                    case NetworkState.GameOverWhiteWins:
                        _engine.GameState = GameEngine.State.GameOverWhiteWins;
                        break;
                    case NetworkState.GameOverBlackWins:
                        _engine.GameState = GameEngine.State.GameOverBlackWins;
                        break;
                    case NetworkState.GameOverDraw:
                        _engine.GameState = GameEngine.State.GameOverDraw;
                        break;
                }
            }

            if (NetworkSession != null)
                NetworkSession.Update();
        }

        protected void Update_SignIn()
        {
            if (Gamer.SignedInGamers.Count < 1)
            {
                    Guide.ShowSignIn(1, false);
                    _currentNetworkState = NetworkState.FindSession;
            }
            else
                _currentNetworkState = NetworkState.FindSession;
            
        }

        private void Update_findSession()
        {
            AvailableNetworkSessionCollection sessions = NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);

            if (sessions.Count == 0)
            {
                _currentNetworkState = NetworkState.CreateSession;
            }
            else
            {
                NetworkSession = NetworkSession.Join(sessions[0]);
                Events();
                _currentNetworkState = NetworkState.Start;
            }
        }

        protected void Events()
        {
            NetworkSession.GamerJoined += networkSession_GamerJoined;
            NetworkSession.GamerLeft += networkSession_GamerLeft;
        }

        void networkSession_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            //using .Tag property to store whos turn it is (Turn[1] is true)
            //Host gets White pieces and moves first

            e.Gamer.Tag = e.Gamer.IsHost ? Turn[1] : Turn[0];
        }

        void networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            NetworkSession.Dispose();
            NetworkSession = null;
            _currentNetworkState = NetworkState.FindSession;
        }
        
        private void CreateSession()
        {
            NetworkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
            NetworkSession.AllowHostMigration = true;
            NetworkSession.AllowJoinInProgress = false;

            Events();
            _currentNetworkState = NetworkState.Start;
        }

        private void Start(GameTime gameTime)
        {
            //local gamer
            LocalNetworkGamer localGamer = NetworkSession.LocalGamers[0];

            //if 2 players check for start or button press
            if (NetworkSession.AllGamers.Count == 2)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    //sends message to other player that we starting
                    PacketWriter.Write((int)MessageType.StartGame);
                    localGamer.SendData(PacketWriter, SendDataOptions.Reliable);

                    StartGame();
                }
            }
            ProcessIncomingData(gameTime);
        }

        protected void StartGame()
        {
            if (_engine.GameState == GameEngine.State.PlayingNetworked)
            {
                _engine.LoadScene(new NetworkedLevel(_engine, false));
                _currentNetworkState = NetworkState.InGame;
            }
            if (_engine.GameState == GameEngine.State.PlayingTimedNetworkGame)
            {
                _engine.LoadScene(new TimedNetworkGame(_engine, false));
                _currentNetworkState = NetworkState.InGame;
            }
        }

        public MessageType ProcessIncomingData(GameTime gameTime)
        {
            LocalNetworkGamer localGamer = NetworkSession.LocalGamers[0];
            MessageType msgType = MessageType.Null;

            while (localGamer.IsDataAvailable)
            {
                NetworkGamer sender;

                localGamer.ReceiveData(PacketReader, out sender);//get packet

                if (!sender.IsLocal)
                {
                    msgType = (MessageType)PacketReader.ReadInt32();
                    switch (msgType)
                    {
                        case MessageType.EndGame:
                            //ends game
                            EndGame();
                            break;
                        case MessageType.StartGame:
                            //starts game
                            StartGame();
                            break;
                        case MessageType.RejoinLobby:
                            //rejoins game lobby
                            RejoinLobby();
                            break;
                        case MessageType.RestartGame:
                            //restarts game
                            RestartGame();
                            break;
                        case MessageType.UpdateOtherMove:
                            TurnSwitch();
                            break;
                        //case MessageType.ChangeTurnState:
                        //    TurnSwitch();
                        //    break;
                    }
                }
            }
            return msgType;
        }//end of method

        protected void EndGame()
        {
            _currentNetworkState = NetworkState.GameOverWhiteWins;
        }//end of method

        private void RejoinLobby()
        {
            _currentNetworkState = NetworkState.Start;
        }//end method

        private void RestartGame()
        {
            StartGame();
        }

        protected NetworkGamer GetOtherGamer()
        {
            foreach (NetworkGamer gamer in NetworkSession.AllGamers)
            {
                if (!gamer.IsLocal)
                    return gamer;
            }
            return null;
        }

        protected NetworkGamer GetLocalGamer()
        {
            foreach (NetworkGamer gamer in NetworkSession.AllGamers)
            {
                if (gamer.IsLocal)
                    return gamer;
            }
            return null;
        }

        public void WriteMovePacket(UInt64 fromSquare, UInt64 toSquare)
        {
                PacketWriter.Write((int)MessageType.UpdateOtherMove);
                PacketWriter.Write(fromSquare);
                PacketWriter.Write(toSquare);

                NetworkSession.LocalGamers[0].SendData(PacketWriter, SendDataOptions.Reliable);
        }

        public void WriteTimePacket(double time)
        {
            PacketWriter.Write(time);

            NetworkSession.LocalGamers[0].SendData(PacketWriter,SendDataOptions.Reliable);
        }

        public void WriteTurnPacket()
        {
                PacketWriter.Write((int)MessageType.ChangeTurnState);

                NetworkSession.LocalGamers[0].SendData(PacketWriter, SendDataOptions.Reliable);

                TurnSwitch();
        }

        public void TurnSwitch()
        {
            //Changes each gamers .Tag property to reflect the turn change
            foreach(NetworkGamer g in NetworkSession.AllGamers)
            {
                g.Tag = (bool)g.Tag == Turn[0] ? Turn[1] : Turn[0];
            }
        }

        

    } // end class
} // end namespace
