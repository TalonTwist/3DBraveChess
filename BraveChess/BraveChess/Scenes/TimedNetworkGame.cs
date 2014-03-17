﻿using System;
using System.Collections.Generic;
using BraveChess.Engines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using BraveChess.Base;
using BraveChess.Objects;

namespace BraveChess.Scenes
{
    class TimedNetworkGame : Scene
    {
        public TimedNetworkGame(GameEngine engine, bool isAnimated)
            : base("TimedNetworkLevel", engine, isAnimated) { }

        public override void Initialize()
        {
            #region Sound
            //loading songs//efects
            Engine.Audio.LoadSong("BackgroundSong");
            Engine.Audio.PlaySong("BackgroundSong");
            MediaPlayer.IsRepeating = true;
            Engine.Audio.LoadEffect("MovePiece");
            #endregion
            
            base.Initialize(); 

            //add Objects
            foreach (Square s in GameBoard.Squares)
                AddObject(s);

            foreach (Piece p in GameBoard.Pieces)
                AddObject(p);

            Engine.Cameras.SetActiveCamera(Engine.Network.NetworkSession.IsHost ? "camWhite" : "camBlack");

            MovesAvailable = new List<Square>();         
            
        }//End of Method

        private void CheckIncomingPackets(GameTime gametime)
        {
            if (Engine.Network.NetworkSession != null ) 
            {
                MessageType msg = Engine.Network.ProcessIncomingData(gametime);

                if (msg == MessageType.UpdateOtherMove)
                {
                    ReadMovePacket(); // reads incoming packet and process 
                    SwitchTurn(true); // changes Turn
                }

            }
        } //end method

        public override void Update(GameTime gametime)
        {
            CheckIncomingPackets(gametime);

            //Check LocalGamers .Tag property, if true, then it is their move and we must accept Input
            if (Engine.Network.NetworkSession != null)
                if ((bool) Engine.Network.NetworkSession.LocalGamers[0].Tag)
                         HandleInput();
                
            UpdateTimers();

            UpdateSelection();
            
            base.Update(gametime);
        }//End of Method

        private void UpdateSelection()
        {
            switch (SelectState)
            {
                case SelectionState.SelectPiece:
                    if (CurrentSquare != null)
                    {
                        PieceToMove = GameBoard.GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

                        if (PieceToMove != null && ((int)PieceToMove.ColorType) == (int)Turn)
                        {
                            FromSquare = CurrentSquare;
                            SelectState = SelectionState.PieceSelected;
                        }

                        CurrentSquare = null;
                    }
                    break;

                case SelectionState.PieceSelected:
                    MovesAvailable = GenerateMoves(PieceToMove, FromSquare);
                    if (MovesAvailable != null)
                    {
                        foreach (Square s in MovesAvailable)
                            s.IsMoveOption = true;
                        SelectState = SelectionState.SelectMove;
                    }
                    else
                    {
                        ResetMovesAvailable();
                        SelectState = SelectionState.SelectPiece;
                    }
                    break;

                case SelectionState.SelectMove:
                    if (CurrentSquare != null && MovesAvailable.Contains(CurrentSquare))
                    {
                        PieceToCapture = GameBoard.GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

                        if (PieceToCapture != null)
                            IsFight = true;

                        ToSquare = CurrentSquare;

                        SelectState = SelectionState.MoveSelected;
                    }
                    else if (CurrentSquare != null)
                    {
                        Piece p = GameBoard.GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

                        if (p != null && ((int)p.ColorType) == (int)Turn) //Replace selection with this piece
                        {
                            PieceToMove = p;
                            FromSquare = CurrentSquare;
                            SelectState = SelectionState.PieceSelected;
                        }
                        else
                            SelectState = SelectionState.SelectPiece;
                        ResetMovesAvailable();
                    }

                    CurrentSquare = null;
                    break;

                case SelectionState.MoveSelected:
                    ResetMovesAvailable();

                    //check for pawn queening

                    var m = new Move(Engine, GameBoard, FromSquare, ToSquare, PieceToMove, IsFight, PieceToCapture, false); //add new Move to list AllMoves
                     if (m.IsValidMove)
                     {
                         AllMoves.Add(m);
                         Engine.Audio.PlayEffect("MovePiece");
                         SelectState = SelectionState.SelectPiece;
                         IsFight = false;
                         SwitchTurn(false);
                     }
                     else
                         NotificationEngine.AddNotification(new Notification("Sorry that would leave you in check!!", 4000));
                    break;

            } // end switch
        }

        public void ReadMovePacket()
        {
            UInt64 fromSq = Engine.Network.PacketReader.ReadUInt64();
            UInt64 toSq = Engine.Network.PacketReader.ReadUInt64();

            MoveOtherPiece(fromSq, toSq);
        }

        private void MoveOtherPiece(UInt64 bbFrom, UInt64 bbTo)
        {
            bool isCapture = false;
            Square s = GameBoard.GetSquareFromBB(bbTo);
            Square sqFrom = GameBoard.GetSquareFromBB(bbFrom);

            Piece capturedPiece = GameBoard.GetPiece(s);
            Piece movedPiece = GameBoard.GetPiece(sqFrom);

            if (capturedPiece != null)
                isCapture = true;

            Move m = new Move(Engine, GameBoard, GameBoard.GetSquareFromBB(bbFrom), s, movedPiece, isCapture, capturedPiece,true);

            AllMoves.Add(m);
        }

        private void UpdateTimers()
        {
            if (Turn == TurnState.White)
            {
                TimeWhite -= new TimeSpan(0, 0, 1);
                TimeWhite2 -= new TimeSpan(0, 0, 1);
            }
            if (Turn == TurnState.Black)
            {
                TimeBlack -= new TimeSpan(0, 0, 1);
                TimeBlack2 -= new TimeSpan(0, 0, 1);
            }
        }

        private void SwitchTurn(bool recieved)
        {
            Turn = Turn == TurnState.White ? TurnState.Black : TurnState.White;

            if (!recieved)
                Engine.Network.TurnSwitch();
        }

    }//End of Class
}//End of Namespace