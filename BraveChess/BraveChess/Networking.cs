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
    }

    public class Networking 
    {
        //current game state
        GameState _currentGameState = GameState.SignIn;
        GameTime _gameTime;
        Game _game;

        SpriteFont _font;

        public GameState CurrentGameState
        {
            get { return _currentGameState; }
            set { _currentGameState = value; }
        }
      
        public byte[] Player = new byte[2];

        // networking variables
        public NetworkSession networkSession;
        public PacketWriter packetWriter = new PacketWriter();
        public PacketReader packetReader = new PacketReader();

        public Networking(Game g)
        {
            _game = g;
        }

        public void Update()
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
            if (e.Gamer.IsHost)
                e.Gamer.Tag = Player[0];
            else
                e.Gamer.Tag = Player[1];
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
            _currentGameState = GameState.InGame;
        }

        public bool ProcessIncomingData(GameTime gameTime)
        {
            bool IsMove = false;
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];
            
            while (localGamer.IsDataAvailable)
            {
                NetworkGamer sender;

                localGamer.ReceiveData(packetReader, out sender);//get packet

                if (!sender.IsLocal)
                {
                    MessageType msgType = (MessageType)packetReader.ReadInt32();
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
                            IsMove = true;
                            break;
                    }
                }
            }
            return IsMove;
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

        protected NetworkGamer GetOtherPlayer()
        {
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                if (!gamer.IsLocal)
                    return gamer;
            }

            return null;
        }


        public void WritePacketInfo(Vector3 _pos, int _pieceType, int _pieceColor, UInt64 _fromSquare, UInt64 _toSquare)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
               // packetWriter.Write(_turn);
                packetWriter.Write((int)MessageType.UpdateOtherMove);
                packetWriter.Write(_pos);
                packetWriter.Write(_pieceType);
                packetWriter.Write(_pieceColor);
                packetWriter.Write(_fromSquare);
                packetWriter.Write(_toSquare);

                gamer.SendData(packetWriter, SendDataOptions.None);  
            }
        }

        //public void TurnCheck()
        //{
        //    //NetworkGamer gamer = new NetworkGamer(); ;

        //    if (CurrentGameState == GameState.InGame)
        //    {
        //        if (networkSession.IsHost)
        //            gamer.Tag = Player[0];
        //        else
        //            gamer.Tag = Player[1];

        //    }
       // }

        

    } // end class
} // end namespace
