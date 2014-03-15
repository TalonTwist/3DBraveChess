using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BraveChess.Objects;
using Microsoft.Xna.Framework;

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

        public Square[,] Squares;
        public List<Piece> Pieces = new List<Piece>();

        public Board(bool isAnimated)
        {
            #region Init Squares

            Squares = new Square[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            // The dark coloured.. pass false for black
                            Squares[i, j] = new Square("square" + i + j, new Vector3(-24 + (7 * i), 0, 24 - (7 * j)), i, j, false);
                        }
                        else if (j % 2 == 1)
                        {
                            // ..and the light coloured.. pass true for white
                            Squares[i, j] = new Square("square" + i + j, new Vector3(-24 + (7 * i), 0, 24 - (7 * j)), i, j, true);
                        }
                    }
                    else if (i % 2 == 1)
                    {
                        if (j % 2 == 1)
                        {
                            Squares[i, j] = new Square("square" + i + j, new Vector3(-24 + (7 * i), 0, 24 - (7 * j)), i, j, false);
                        }
                        else if (j % 2 == 0)
                        {
                            Squares[i, j] = new Square("square" + i + j, new Vector3(-24 + (7 * i), 0, 24 - (7 * j)), i, j, true);
                        }
                    }
                }
            }
            #endregion

            #region White Piece Init
            //White Pawn Set
            Pieces.Add(new Piece("Pawn1", "White Pawn", GetStartPos("a2"), 1, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("Pawn2", "White Pawn", GetStartPos("b2"), 1, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("Pawn3", "White Pawn", GetStartPos("c2"), 1, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("Pawn4", "White Pawn", GetStartPos("d2"), 1, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("Pawn5", "White Pawn", GetStartPos("e2"), 1, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("Pawn6", "White Pawn", GetStartPos("f2"), 1, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("Pawn7", "White Pawn", GetStartPos("g2"), 1, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("Pawn8", "White Pawn", GetStartPos("h2"), 1, Piece.PieceType.Pawn));

            Pieces.Add(new Piece("Rook1", "White Rook", GetStartPos("a1"), 1, Piece.PieceType.Rook));
            Pieces.Add(new Piece("Rook2", "White Rook", GetStartPos("h1"), 1, Piece.PieceType.Rook));

            Pieces.Add(new Piece("King1", "White King", GetStartPos("e1"), 1, Piece.PieceType.King));

            Pieces.Add(new Piece("Queen1", "White Queen", GetStartPos("d1"), 1, Piece.PieceType.Queen));

            Pieces.Add(new Piece("Knight1", "White Knight", GetStartPos("b1"), 1, Piece.PieceType.Knight));
            Pieces.Add(new Piece("Knight1", "White Knight", GetStartPos("g1"), 1, Piece.PieceType.Knight));

            Pieces.Add(new Piece("Bishop1", "Untextured\\Bishop Piece", GetStartPos("c1"), 1, Piece.PieceType.Bishop));
            Pieces.Add(new Piece("Bishop2", "Untextured\\Bishop Piece", GetStartPos("f1"), 1, Piece.PieceType.Bishop));
            #endregion

            #region Black Piece Init
            //White Pawn Set
            Pieces.Add(new Piece("pawn1", "Black Pawn", GetStartPos("a7"), 0, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("pawn2", "Black Pawn", GetStartPos("b7"), 0, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("pawn3", "Black Pawn", GetStartPos("c7"), 0, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("pawn4", "Black Pawn", GetStartPos("d7"), 0, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("pawn5", "Black Pawn", GetStartPos("e7"), 0, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("pawn6", "Black Pawn", GetStartPos("f7"), 0, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("pawn7", "Black Pawn", GetStartPos("g7"), 0, Piece.PieceType.Pawn));
            Pieces.Add(new Piece("pawn8", "Black Pawn", GetStartPos("h7"), 0, Piece.PieceType.Pawn));


            Pieces.Add(new Piece("rook1", "Black Rook", GetStartPos("a8"), 0, Piece.PieceType.Rook));
            Pieces.Add(new Piece("rook2", "Black Rook", GetStartPos("h8"), 0, Piece.PieceType.Rook));

            Pieces.Add(new Piece("king1", "Black King", GetStartPos("e8"), 0, Piece.PieceType.King));

            Pieces.Add(new Piece("queen1", "Black Queen", GetStartPos("d8"), 0, Piece.PieceType.Queen));

            Pieces.Add(new Piece("knight1", "Black Knight", GetStartPos("b8"), 0, Piece.PieceType.Knight));
            Pieces.Add(new Piece("knight1", "Black Knight", GetStartPos("g8"), 0, Piece.PieceType.Knight));

            Pieces.Add(new Piece("bishop1", "Untextured\\Bishop Piece", GetStartPos("c8"), 0, Piece.PieceType.Bishop));
            Pieces.Add(new Piece("bishop2", "Untextured\\Bishop Piece", GetStartPos("f8"), 0, Piece.PieceType.Bishop));
            #endregion
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

        private Vector3 GetStartPos(string pos)
        {
            char[] c = pos.ToCharArray();

            int file = c[0] - 97;
            int rank = c[1] - 49;

            return Squares[file, rank].World.Translation + new Vector3(0, 2, 0);
        }

    }//end class
}
