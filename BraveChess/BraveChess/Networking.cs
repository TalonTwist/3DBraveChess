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
        GameState currentGameState = GameState.SignIn;
        GameTime gameTime;

        public GameState CurrentGameState
        {
            get { return currentGameState; }
            set { currentGameState = value; }
        }

        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        SpriteFont font;
        public byte[] Player = new byte[2];

        // networking
        public NetworkSession networkSession;
        public PacketWriter packetWriter = new PacketWriter();
        public PacketReader packetReader = new PacketReader();

        public void NetworkInitialize()
        {
            
        }

        public void Update()
        {
            switch (currentGameState)
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
                    Start(gameTime);//starts game
                    break;
                case GameState.InGame:
                    //inside game
                    break;
                case GameState.GameOver:
                    //game finnished;
                    break;
            }

            if (networkSession != null)
                networkSession.Update();
        }

        protected void Update_SignIn()
        {
            if (Gamer.SignedInGamers.Count < 1)
            {
                    Guide.ShowSignIn(1, false);
                    currentGameState = GameState.FindSession;
               
            }
            else
            {
                currentGameState = GameState.FindSession;
            }
        }

        private void Update_findSession()
        {
            AvailableNetworkSessionCollection sessions =
                NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);
            if (sessions.Count == 0)
            {
                currentGameState = GameState.CreateSession;
            }
            else
            {
                networkSession = NetworkSession.Join(sessions[0]);
                Events();
                currentGameState = GameState.Start;
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
            {
                e.Gamer.Tag = Player[0];
            }
            else
            {
                e.Gamer.Tag = Player[1];
            }
        }

        void networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            networkSession.Dispose();
            networkSession = null;
            currentGameState = GameState.FindSession;
        }
        
        private void CreateSession()
        {
            networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
            networkSession.AllowHostMigration = true;
            networkSession.AllowJoinInProgress = false;

            Events();
            currentGameState = GameState.Start;
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
            currentGameState = GameState.InGame;
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

        public void ReadPacket()
        {
        }

        protected void EndGame()
        {
            currentGameState = GameState.GameOver;
        }//end of method

        private void RejoinLobby()
        {
            currentGameState = GameState.Start;
        }//end method

        private void RestartGame()
        {
            StartGame();
        }

        protected void UpdatePossition(GameTime gametime)
        {
            NetworkGamer otherPlayer = GetOtherPlayer();


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


        public void WritePacketInfo(Vector3 pos, int pieceType, int pieceColor, UInt64 fromSquare, UInt64 toSquare)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {

                packetWriter.Write((int)MessageType.UpdateOtherMove);
                packetWriter.Write(pos);
                packetWriter.Write(pieceType);
                packetWriter.Write(pieceColor);
                packetWriter.Write(fromSquare);
                packetWriter.Write(toSquare);

                gamer.SendData(packetWriter, SendDataOptions.None); 

                
            }
        }

        public void ProcessIncData(GameTime gameTime)
        {
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];

            while (localGamer.IsDataAvailable)
            {
                NetworkGamer sender;
                localGamer.ReceiveData(packetReader, out sender);

                if (!sender.IsLocal)
                {
                    
                }
            }
        }

        

    }
}
