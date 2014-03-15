using System;
using BraveChess.Objects;
using BraveChess.Helpers;

namespace BraveChess.Base
{
    public class MoveGen
    {
        static public bool BlackCanCastleShort { get { return !(HasBlackKingMoved & HasBlackRookHMoved); } }
        static public bool BlackCanCastleLong { get { return !(HasBlackKingMoved & HasBlackRookAMoved); } }
        static public bool WhiteCanCastleShort { get { return !(HasWhiteKingMoved & HasWhiteRookHMoved); } }
        static public bool WhiteCanCastleLong { get { return !(HasWhiteKingMoved & HasWhiteRookAMoved); } }

        static public bool HasBlackRookAMoved { get; set; }
        static public bool HasBlackRookHMoved { get; set; }
        static public bool HasWhiteRookAMoved { get; set; }
        static public bool HasWhiteRookHMoved { get; set; }
        static public bool HasBlackKingMoved { get; set; }
        static public bool HasWhiteKingMoved { get; set; }

        static private Board _board;

        public static void Init( Board board)
        {
            _board = board;
        }

        static public UInt64 GenerateKingMoves(Square s, Piece.Colour c)
        {
            UInt64 myPieceBB = BitboardHelper.GetBitboardFromSquare(s);

            UInt64 myPieceBB_H_Clip = (myPieceBB & BitboardHelper.ClearFile[7]);
            UInt64 myPieceBB_A_Clip = (myPieceBB & BitboardHelper.ClearFile[0]);

            UInt64 validMovesBB = (myPieceBB_A_Clip << 7) | (myPieceBB << 8) | (myPieceBB_H_Clip << 9) | (myPieceBB_H_Clip << 1) | (myPieceBB_H_Clip >> 7) | (myPieceBB >> 8) | (myPieceBB_A_Clip >> 9) | (myPieceBB_A_Clip >> 1);

            if (c == Piece.Colour.White)
            {
                //remove move options occupied by own side
                validMovesBB = validMovesBB ^ (validMovesBB & _board.WhitePieces);

                //Add Castling moves if available
                if (WhiteCanCastleShort && ((BitboardHelper.MaskWhiteCastleShort & _board.AllPieces) == 0))
                    validMovesBB |= myPieceBB << 2;
                if (WhiteCanCastleLong && ((BitboardHelper.MaskWhiteCastleLong & _board.AllPieces) == 0))
                    validMovesBB |= myPieceBB >> 2;
            }
            else
            {
                //remove move options occupied by own side
                validMovesBB = validMovesBB ^ (validMovesBB & _board.BlackPieces);

                //Add Castling moves if available
                if (BlackCanCastleShort && ((BitboardHelper.MaskBlackCastleShort & _board.AllPieces) == 0))
                    validMovesBB |= myPieceBB << 2;
                if (BlackCanCastleLong && ((BitboardHelper.MaskBlackCastleLong & _board.AllPieces) == 0))
                    validMovesBB |= myPieceBB >> 2;
            }

            return validMovesBB;
        }

        static public UInt64 GeneratePawnMoves(Square s, Piece.Colour c)
        {
            UInt64 validMovesBB;
            UInt64 myPieceBB = BitboardHelper.GetBitboardFromSquare(s); //bitboard representation of the pawns position
            UInt64 myPieceBB_H_Clip = (myPieceBB & BitboardHelper.ClearFile[7]);
            UInt64 myPieceBB_A_Clip = (myPieceBB & BitboardHelper.ClearFile[0]);

            if (c == Piece.Colour.White)
            {
                validMovesBB = (myPieceBB_A_Clip << 7 | myPieceBB_H_Clip << 9) & _board.BlackPieces;

                if (((myPieceBB << 8) & _board.AllPieces) == 0)
                {
                    validMovesBB = validMovesBB | (myPieceBB << 8);

                    if (((myPieceBB & BitboardHelper.MaskRank[(int)Ranks.Two]) != 0) && ((myPieceBB << 16) & _board.AllPieces) == 0)
                        validMovesBB = validMovesBB | (myPieceBB << 16);
                }
            }
            else
            {
                validMovesBB = (myPieceBB_H_Clip >> 7 | myPieceBB_A_Clip >> 9) & _board.WhitePieces;

                if (((myPieceBB >> 8) & _board.AllPieces) == 0)
                {
                    validMovesBB = validMovesBB | (myPieceBB >> 8);

                    if (((myPieceBB & BitboardHelper.MaskRank[(int)Ranks.Seven]) != 0) && ((myPieceBB >> 16) & _board.AllPieces) == 0)
                        validMovesBB = validMovesBB | myPieceBB >> 16;
                }
            }

            return validMovesBB;
        }

        static public UInt64 GenerateKnightMoves(Square s, Piece.Colour c)
        {
            UInt64 validMovesBB;
            int sqIndex = BitboardHelper.GetIndexFromSquare(s);

            if (c == Piece.Colour.White)
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & _board.WhitePieces;

            else if (c == Piece.Colour.Black)
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & _board.BlackPieces;

            else
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex];

            return validMovesBB;
        }

        static public UInt64 GenerateBishopMoves(Square s, Piece.Colour c)
        {
            UInt64 validSquares;
            int sqIndex = BitboardHelper.GetIndexFromSquare(s);
     

            UInt64 bbBlockers = _board.AllPieces & BitboardHelper.OccupancyMaskBishop[sqIndex];

            int databaseIndex = (int)((bbBlockers * BitboardHelper.MagicNumberBishop[sqIndex]) >> BitboardHelper.MagicNumberShiftsBishop[sqIndex]);

            if (c == Piece.Colour.White)
                validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex] & ~_board.WhitePieces;
            else if (c == Piece.Colour.Black)
                validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex] & ~_board.BlackPieces;
            else
                validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex];

            return validSquares;
        }

        static public UInt64 GenerateRookMoves(Square s, Piece.Colour c)
        {
            UInt64 validSquares;
            int sqIndex = BitboardHelper.GetIndexFromSquare(s);

            UInt64 bbBlockers = _board.AllPieces & BitboardHelper.OccupancyMaskRook[sqIndex];

            int databaseIndex = (int)((bbBlockers * BitboardHelper.MagicNumberRook[sqIndex]) >> BitboardHelper.MagicNumberShiftsRook[sqIndex]);

            if (c == Piece.Colour.White)
                validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex] & ~_board.WhitePieces;
            else if (c == Piece.Colour.Black)
                validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex] & ~_board.BlackPieces;
            else
                validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex];

            return validSquares;
        }

        static public UInt64 GenerateQueenMoves(Square s, Piece.Colour c)
        {
            //first calulate Rook movements for queen
            ulong validSquares = GenerateRookMoves(s, c);

            //then calculate Bishop moves for queen and OR with rook movements
            validSquares |= GenerateBishopMoves(s, c);

            return validSquares;
        }


    }
}
