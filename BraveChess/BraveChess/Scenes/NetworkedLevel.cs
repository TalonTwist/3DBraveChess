using System;
using System.Collections.Generic;
using System.Linq;
using BraveChess.Engines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using BraveChess.Base;
using BraveChess.Objects;
using BraveChess.Helpers;

namespace BraveChess.Scenes
{
    class NetworkedLevel : Scene
    {
        public NetworkedLevel(GameEngine engine, bool isAnimated)
            : base("NetworkLevel", engine, isAnimated) { }

        public override void Initialize()
        {  
            #region Sound
            //loading songs//efects
            Engine.Audio.LoadSong("BackgroundSong");
            Engine.Audio.PlaySong("BackgroundSong");
            MediaPlayer.IsRepeating = true;
            Engine.Audio.LoadEffect("move");
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
            if((bool)Engine.Network.NetworkSession.LocalGamers[0].Tag)
                HandleInput();

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

                        if (p != null && ((int)PieceToMove.ColorType) == (int)Turn) //Replace selection with this piece
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
                         //Send packet 
                         Engine.Network.WriteMovePacket(PieceToMove.World.Translation, (int)PieceToMove.Piece_Type,
                             (int)PieceToMove.ColorType, BitboardHelper.GetBitboardFromSquare(FromSquare), BitboardHelper.GetBitboardFromSquare(ToSquare));
                         if(Turn == TurnState.Black)
                            BlackMoves.Add(m.ToAlgebraic());
                         else if(Turn == TurnState.White)
                             WhiteMoves.Add(m.ToAlgebraic());
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
            Vector3 pos = Engine.Network.PacketReader.ReadVector3();
            int pieceType = Engine.Network.PacketReader.ReadInt32();
            int pieceColor = Engine.Network.PacketReader.ReadInt32();
            UInt64 fromSq = Engine.Network.PacketReader.ReadUInt64();
            UInt64 toSq = Engine.Network.PacketReader.ReadUInt64();

            MoveOtherPiece(pos, (Piece.PieceType)pieceType, (Piece.Colour)pieceColor, fromSq, toSq);
        }

        private void MoveOtherPiece(Vector3 pos, Piece.PieceType type, Piece.Colour color, UInt64 bbFrom, UInt64 bbTo)
        {
            GameBoard.UpdateRelevantbb(type, color, bbFrom, bbTo); //update bitboards with new piece position

            Square s = GetSquareFromBB(bbTo);
            Vector3 newPos = GetNewPos(s);

            Piece capturedPiece = GameBoard.GetPiece(s.World.Translation + new Vector3(0, 2, 0));
            if (capturedPiece != null)
                capturedPiece.Destroy();

            GameBoard.GetPiece(pos).UpdateWorld(newPos);
        }

        private void SwitchTurn(bool recieved)
        {
            Turn = Turn == TurnState.White ? TurnState.Black : TurnState.White;

            if(!recieved)
                Engine.Network.TurnSwitch();
        }

        protected override void HandleInput()
        {
            base.HandleInput();
        }

        private Vector3 GetNewPos(Square destination)
        {
            return destination.World.Translation + new Vector3(0, 2, 0);
        }

        private Square GetSquareFromBB(ulong bb)
        {
            var v = BitboardHelper.GetSquareFromBitboard(bb);

            return GameBoard.Squares[v.Item2, v.Item1];
        }

        private List<Square> GetSquareListFromBB(ulong bb)
        {
            List<Square> s = new List<Square>();
            var sList = BitboardHelper.GetSquareListFromBB(bb);

            if (sList == null) return null;
            s.AddRange(sList.Select(t => GameBoard.Squares[t.Item2, t.Item1]));

            return s;
        }

        private List<Square> GenerateMoves(Piece p, Square s)
        {
            //Call method based on Type of Piece passed in
            switch (p.Piece_Type)
            {
                case Piece.PieceType.King:
                    return GetSquareListFromBB(MoveGen.GenerateKingMoves(s, p.ColorType));

                case Piece.PieceType.Pawn:
                    return GetSquareListFromBB(MoveGen.GeneratePawnMoves(s, p.ColorType));

                case Piece.PieceType.Knight:
                    return GetSquareListFromBB(MoveGen.GenerateKnightMoves(s, p.ColorType));

                case Piece.PieceType.Bishop:
                    return GetSquareListFromBB(MoveGen.GenerateBishopMoves(s, p.ColorType));

                case Piece.PieceType.Rook:
                    return GetSquareListFromBB(MoveGen.GenerateRookMoves(s, p.ColorType));

                case Piece.PieceType.Queen:
                    return GetSquareListFromBB(MoveGen.GenerateQueenMoves(s, p.ColorType));
                default:
                    return null;
            }
        }     
       
    }//End of Class
}//End of Namespace
