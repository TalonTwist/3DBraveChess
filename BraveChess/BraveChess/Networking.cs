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
        UpdatePlayerPos,
    }

    public class Networking 
    {
        //current game state
        GameState currentGameState = GameState.SignIn;

        public GameState CurrentGameState
        {
            get { return currentGameState; }
            set { currentGameState = value; }
        }

        // networking
        NetworkSession networkSession;
        PacketWriter packetWriter = new PacketWriter();
        PacketReader packetReader = new PacketReader();

        public void NetworkInitialize()
        {
            
        }

        public void Update()
        {
            switch (currentGameState)
            {
                case GameState.SignIn:
                    //sign in
                    break;
                case GameState.FindSession:
                    //finds session
                    break;
                case GameState.CreateSession:
                    //creates session
                    break;
                case GameState.Start:
                    //starts game
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
            //not done
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

        protected void ProcessIncomingData(GameTime gameTime)
        {
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
                        case MessageType.UpdatePlayerPos:
                            //updates possition
                            break;
                    }
                }
            }
        }//end of method

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

        public void WritePacketInfo(int pieceType, int pieceColor, ulong fromSquare, ulong toSquare)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                packetWriter.Write(pieceType);
                packetWriter.Write(pieceColor);
                packetWriter.Write(fromSquare);
                packetWriter.Write(toSquare);

                gamer.SendData(packetWriter, SendDataOptions.None); ;
            }
        }

    }
}
