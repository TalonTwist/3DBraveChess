using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BraveChess.Objects;

namespace BraveChess.Base
{
    public class Board
    {
        /* white pieces */
        public UInt64 WhitePawns = 0xFF00LU;
        public UInt64 WhiteKnights = 0x42LU;
        public UInt64 WhiteBishops = 0X24LU;
        public UInt64 WhiteRooks = 0x81LU;
        public UInt64 WhiteQueens = 0x8LU;
        public UInt64 WhiteKings = 0x10LU;
        /* black pieces */
        public UInt64 BlackPawns = 0xFF000000000000LU;
        public UInt64 BlackKnights = 0x4200000000000000LU;
        public UInt64 BlackBishops = 0x2400000000000000LU;
        public UInt64 BlackRooks = 0x8100000000000000LU;
        public UInt64 BlackQueens = 0x800000000000000LU;
        public UInt64 BlackKings = 0x1000000000000000LU;

        public UInt64 WhitePieces { get { return WhiteBishops | WhiteKings | WhiteKnights | WhitePawns | WhiteQueens | WhiteRooks; } }
        public UInt64 BlackPieces { get { return BlackBishops | BlackKings | BlackKnights | BlackPawns | BlackQueens | BlackRooks; } }
        public UInt64 AllPieces { get { return WhitePieces | BlackPieces; } }

        public Board()
        {
                
        }

        public void UpdateRelevantbb(Piece.PieceType type, Piece.Colour c, ulong bbFrom, ulong bbTo)
        {
            if (c == Piece.Colour.White)
            {
                switch (type)
                {
                    case Piece.PieceType.Bishop:
                        WhiteBishops ^= bbFrom;
                        WhiteBishops ^= bbTo;
                        break;
                    case Piece.PieceType.King:
                        WhiteKings ^= bbFrom;
                        WhiteKings ^= bbTo;
                        break;
                    case Piece.PieceType.Knight:
                        WhiteKnights ^= bbFrom;
                        WhiteKnights ^= bbTo;
                        break;
                    case Piece.PieceType.Queen:
                        WhiteQueens ^= bbFrom;
                        WhiteQueens ^= bbTo;
                        break;
                    case Piece.PieceType.Rook:
                        WhiteRooks ^= bbFrom;
                        WhiteRooks ^= bbTo;
                        break;
                    case Piece.PieceType.Pawn:
                        WhitePawns ^= bbFrom;
                        WhitePawns ^= bbTo;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case Piece.PieceType.Bishop:
                        BlackBishops ^= bbFrom;
                        BlackBishops ^= bbTo;
                        break;
                    case Piece.PieceType.King:
                        BlackKings ^= bbFrom;
                        BlackKings ^= bbTo;
                        break;
                    case Piece.PieceType.Knight:
                        BlackKnights ^= bbFrom;
                        BlackKnights ^= bbTo;
                        break;
                    case Piece.PieceType.Queen:
                        BlackQueens ^= bbFrom;
                        BlackQueens ^= bbTo;
                        break;
                    case Piece.PieceType.Rook:
                        BlackRooks ^= bbFrom;
                        BlackRooks ^= bbTo;
                        break;
                    case Piece.PieceType.Pawn:
                        BlackPawns ^= bbFrom;
                        BlackPawns ^= bbTo;
                        break;
                }
            }
        }
    }
}
