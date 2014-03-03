using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using BraveChess.Engines;
using BraveChess.Scenes;
using BraveChess.Objects;
using BraveChess.Base;

namespace BraveChess
{
    public enum GameState
    {
        SignIn,
        FindSession,
        CreateSession,
        Start,
        InGame,
        GameOver
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

    public class Networking 
    {
        public GameState CurrentGameState
        {
            get { return _currentGameState; }
            set { _currentGameState = value; }
        }
      
        public byte[] Player = new byte[2];
        public bool[] Turn = new bool[2] { false, true };

        // networking variables
        public NetworkSession networkSession;
        public PacketWriter packetWriter = new PacketWriter();
        public PacketReader packetReader = new PacketReader();

        GameState _currentGameState = GameState.SignIn;
        Game _game;
        GameEngine _engine;

        public Networking(Game g, GameEngine e)
        {
            _game = g;
            _engine = e;
        }

        public void Update(GameTime _gameTime)
        {
            if (_game.IsActive)
            {
                switch (_currentGameState)
                {
                    case GameState.SignIn:
                        Update_SignIn();//sign in
                        break;
                    case GameState.FindSession:
                        Update_findSession();//finds session
                        break;
                    case GameState.CreateSession:
                        CreateSession();//creates session
                        break;
                    case GameState.Start:
                        Start(_gameTime);//starts game
                        break;
                    case GameState.InGame:
                        //inside game
                        break;
                    case GameState.GameOver:
                        //game finished;
                        break;
                }
            }

            if (networkSession != null)
                networkSession.Update();
        }

        protected void Update_SignIn()
        {
            if (Gamer.SignedInGamers.Count < 1)
            {
                    Guide.ShowSignIn(1, false);
                    _currentGameState = GameState.FindSession;
            }
            else
                _currentGameState = GameState.FindSession;
            
        }

        private void Update_findSession()
        {
            AvailableNetworkSessionCollection sessions = NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);

            if (sessions.Count == 0)
            {
                _currentGameState = GameState.CreateSession;
            }
            else
            {
                networkSession = NetworkSession.Join(sessions[0]);
                Events();
                _currentGameState = GameState.Start;
            }
        }

        protected void Events()
        {
            networkSession.GamerJoined += new EventHandler<GamerJoinedEventArgs>(networkSession_GamerJoined);
            networkSession.GamerLeft += new EventHandler<GamerLeftEventArgs>(networkSession_GamerLeft);
        }

        void networkSession_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            //using .Tag property to store whos turn it is (Turn[1] is true)
            //Host gets White pieces and moves first

            if (e.Gamer.IsHost)
                e.Gamer.Tag = Turn[1];
            else
                e.Gamer.Tag = Turn[0];
        }

        void networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            networkSession.Dispose();
            networkSession = null;
            _currentGameState = GameState.FindSession;
        }
        
        private void CreateSession()
        {
            networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
            networkSession.AllowHostMigration = true;
            networkSession.AllowJoinInProgress = false;

            Events();
            _currentGameState = GameState.Start;
        }

        private void Start(GameTime gameTime)
        {
            //local gamer
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];

            //if 2 players check for start or button press
            if (networkSession.AllGamers.Count == 2)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    //sends message to other player that we starting
                    packetWriter.Write((int)MessageType.StartGame);
                    localGamer.SendData(packetWriter, SendDataOptions.Reliable);

                    StartGame();
                }
            }
            ProcessIncomingData(gameTime);
        }

        protected void StartGame()
        {
            _engine.LoadScene(new Level0(_engine));
            _currentGameState = GameState.InGame;
        }

        public MessageType ProcessIncomingData(GameTime gameTime)
        {
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];
            MessageType msgType = MessageType.Null;

            while (localGamer.IsDataAvailable)
            {
                NetworkGamer sender;

                localGamer.ReceiveData(packetReader, out sender);//get packet

                if (!sender.IsLocal)
                {
                    msgType = (MessageType)packetReader.ReadInt32();
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
            _currentGameState = GameState.GameOver;
        }//end of method

        private void RejoinLobby()
        {
            _currentGameState = GameState.Start;
        }//end method

        private void RestartGame()
        {
            StartGame();
        }

        protected NetworkGamer GetOtherGamer()
        {
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                if (!gamer.IsLocal)
                    return gamer;
            }
            return null;
        }

        protected NetworkGamer GetLocalGamer()
        {
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                if (gamer.IsLocal)
                    return gamer;
            }
            return null;
        }

        public void WriteMovePacket(Vector3 _pos, int _pieceType, int _pieceColor, UInt64 _fromSquare, UInt64 _toSquare)
        {
                packetWriter.Write((int)MessageType.UpdateOtherMove);
                packetWriter.Write(_pos);
                packetWriter.Write(_pieceType);
                packetWriter.Write(_pieceColor);
                packetWriter.Write(_fromSquare);
                packetWriter.Write(_toSquare);

                networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
        }

        public void WriteTurnPacket()
        {
                packetWriter.Write((int)MessageType.ChangeTurnState);

                networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);

                TurnSwitch();
        }

        public void TurnSwitch()
        {
            //Changes each gamers .Tag property to reflect the turn change
            foreach(NetworkGamer g in networkSession.AllGamers)
            {
                g.Tag = (bool)g.Tag == Turn[0] ? Turn[1] : Turn[0];
            }
        }

        

    } // end class
} // end namespace
