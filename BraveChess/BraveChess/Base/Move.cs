using System;
using System.Text;
using BraveChess.Helpers;
using BraveChess.Objects;
using Microsoft.Xna.Framework;

namespace BraveChess.Base
{
    public class Move
    {
        private readonly GameEngine _engine;
        private Board _board;
        public string Algebraic{ get {return ToAlgebraic();}}

        public bool IsValidMove { get; set; }
        public Square FromSquare { get; set; }
        public Square ToSquare { get; set; }
        public Piece PieceMoved { get; set; }
        public Piece PieceCaptured { get; set; }
        public Piece.PieceType PiecePromoted { get; set; }
        public bool HasPromoted{get{return PiecePromoted != Piece.PieceType.None;}}
        public bool IsEnpassant { get; set; }
        public bool HasCaptured { get; set; }
        public bool IsCastling{get{return IsLongCastling || IsShortCastling;}}
        public bool IsLongCastling
        {
            get
            {
                if (PieceMoved.Piece_Type == Piece.PieceType.King)
                {
                    if (FromSquare.File - ToSquare.File == 2)
                        return true;
                }
                return false;
            }
        }
        public bool IsShortCastling
        {
            get
            {
                if (PieceMoved.Piece_Type == Piece.PieceType.King)
                {
                    if (FromSquare.File - ToSquare.File == -2)
                        return true;
                }
                return false;
            }
        }
        public bool IsCheck { get; set; }
        public bool IsCheckMate { get; set; }
        public bool IsDoublePawnPush
        {
            get
            {
                if (PieceMoved.Piece_Type == Piece.PieceType.Pawn)
                    if (!HasCaptured)
                        if ((BitboardHelper.GetBitboardFromSquare(ToSquare) << 16) == BitboardHelper.GetBitboardFromSquare(FromSquare))
                            return true;
                        else if ((BitboardHelper.GetBitboardFromSquare(ToSquare) >> 16) == BitboardHelper.GetBitboardFromSquare(FromSquare))
                             return true;
                return false;
            }
        }
        public Piece.Colour SideMove
        {
            get
            {
                return PieceMoved.ColorType;
            }
        }

        public Move(GameEngine engine, Board board, Square fromSquare, Square toSquare, Piece pieceMoved, bool isCapture, Piece pieceCaptured, bool isPacketMove, Piece.PieceType piecePromoted = Piece.PieceType.None)
        {
            _engine = engine;
            _board = board;
            FromSquare = fromSquare;
            ToSquare = toSquare;
            PieceMoved = pieceMoved;
            PiecePromoted = piecePromoted;
            HasCaptured = isCapture;
            PieceCaptured = pieceCaptured; 

            ProcessMove(isPacketMove);
        }

        private void ProcessMove(bool isPacketMove)
        {
            if (isPacketMove || TestMove())
            {
                if (HasCaptured)
                {
                    _engine.Audio.PlayEffect("CapturePiece");
                    CapturePiece(); //Remove the captured piece
                }
                MovePiece(isPacketMove);
                IsValidMove = true;
            }
            else
                IsValidMove = false;
            
        }

        private bool TestMove()
        {
            UInt64 bbFrom = BitboardHelper.GetBitboardFromSquare(FromSquare);
            UInt64 bbTo = BitboardHelper.GetBitboardFromSquare(ToSquare);

            _board.UpdateRelevantbb(PieceMoved.Piece_Type, PieceMoved.ColorType, bbFrom, bbTo);

            //if (TestForCheck())
            //{
            //    _board.UpdateRelevantbb(PieceMoved.Piece_Type, PieceMoved.ColorType, bbTo, bbFrom);
            //    return false;
            //}
            return true;
        } 

        public void MovePiece(bool isPacketMove)
        {
            if(isPacketMove)
                _board.UpdateRelevantbb(PieceMoved.Piece_Type, PieceMoved.ColorType, BitboardHelper.GetBitboardFromSquare(FromSquare),
                    BitboardHelper.GetBitboardFromSquare(ToSquare));

            PieceMoved.UpdateWorld(GetNewPos(ToSquare)); //update world position of model
        }

        private void CapturePiece() //Remove the Piece and update bitboards
        {
            _board.UpdateRelevantbb(PieceCaptured.Piece_Type, PieceCaptured.ColorType, BitboardHelper.GetBitboardFromSquare(ToSquare), 0);
            _engine.ActiveScene.Pieces.Remove(PieceCaptured);
            PieceCaptured.Destroy();
        }

        private bool TestForCheck()
        {
            Square kingPos;

            if (PieceMoved.ColorType == Piece.Colour.White)
            {
                kingPos = GetSquareFromBB(_board.WhiteKings);

                //if all pieces attacking the kings position minus pieces of his own colour != 0, then the king is in check
                if ((FindAttacksToSquare(kingPos) & ~_board.WhitePieces) != 0)
                    return true;
            }
            else if (PieceMoved.ColorType == Piece.Colour.Black)
            {
                kingPos = GetSquareFromBB(_board.BlackKings);
                if ((FindAttacksToSquare(kingPos) & ~_board.BlackPieces) != 0)
                    return true;
            }
            return false;
        }

        /****Unfinished***
         * Add pawn and king attacks test */
        private UInt64 FindAttacksToSquare(Square s) // returns bitboard with all pieces attacking the specified Square
        {
            int sqIndex = BitboardHelper.GetIndexFromSquare(s);

            ulong attackersBB = (BitboardHelper.KnightAttacks[sqIndex] & _board.WhiteKnights & _board.BlackKnights);
            attackersBB |= (MoveGen.GenerateBishopMoves(s, Piece.Colour.None) & _board.BlackBishops & _board.WhiteBishops & _board.BlackQueens & _board.WhiteQueens);
            attackersBB |= (MoveGen.GenerateRookMoves(s, Piece.Colour.None) & _board.BlackRooks & _board.WhiteRooks & _board.BlackQueens & _board.WhiteQueens);
            //add pawn and king attacks

            return attackersBB;
        }

        private Square GetSquareFromBB(ulong bb)
        {
            var v = BitboardHelper.GetSquareFromBitboard(bb);

            return _engine.ActiveScene.Squares[v.Item2, v.Item1];
        }

        private Vector3 GetNewPos(Square destination)
        {
            return destination.World.Translation + new Vector3(0, 2, 0);
        }

        public string ToAlgebraic()
        {
            var algebraic = new StringBuilder();

            if (IsCastling)
            {
                algebraic.Append(IsShortCastling ? "O-O" : "O-O-O");
            }
            else
            {
                if (PieceMoved.Piece_Type != Piece.PieceType.Pawn) //If not a pawn, add Piece Initial
                    algebraic.Append(GetInitial(PieceMoved.Piece_Type));

                algebraic.Append(FromSquare.ToAlgebraic());

                algebraic.Append(HasCaptured ? "x" : "-");

                algebraic.Append(ToSquare.ToAlgebraic());
            }

            if (HasPromoted)
                algebraic.Append("=" + GetInitial(PiecePromoted));

            if (IsCheck)
                algebraic.Append(IsCheckMate ? "#" : "+");

            return algebraic.ToString();
        }

        static private string GetInitial(Piece.PieceType type)
        {
            switch (type)
            {
                case Piece.PieceType.Bishop:
                    return "B";
                case Piece.PieceType.Knight:
                    return "N";
                case Piece.PieceType.Queen:
                    return "Q";
                case Piece.PieceType.Rook:
                    return "R";
                    case Piece.PieceType.King:
                    return "K";
                default:
                    return "initial";
            }
        }

        
    }
}
