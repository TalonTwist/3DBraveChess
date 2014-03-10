using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BraveChess.Objects;
using BraveChess.Helpers;

namespace BraveChess.Base
{
    public class MoveGen
    {
        static private GameEngine _engine;

        static public void Init(GameEngine engine)
        {
            _engine = engine;
        }

        static public UInt64 GenerateKingMoves(Square s, Piece.Color c)
        {
            UInt64 myPieceBB = BitboardHelper.GetBitboardFromSquare(s);

            UInt64 myPieceBB_H_Clip = (myPieceBB & BitboardHelper.ClearFile[7]);
            UInt64 myPieceBB_A_Clip = (myPieceBB & BitboardHelper.ClearFile[0]);

            UInt64 validMovesBB = (myPieceBB_A_Clip << 7) | (myPieceBB << 8) | (myPieceBB_H_Clip << 9) | (myPieceBB_H_Clip << 1) | (myPieceBB_H_Clip >> 7) | (myPieceBB >> 8) | (myPieceBB_A_Clip >> 9) | (myPieceBB_A_Clip >> 1);


            if (c == Piece.Color.White)
                validMovesBB = validMovesBB ^ (validMovesBB & _engine.ActiveScene.WhitePieces);
            else
                validMovesBB = validMovesBB ^ (validMovesBB & _engine.ActiveScene.BlackPieces);

            return validMovesBB;
        }

        static public UInt64 GeneratePawnMoves(Square s, Piece.Color c)
        {
            UInt64 validMovesBB;
            UInt64 myPieceBB = BitboardHelper.GetBitboardFromSquare(s); //bitboard representation of the pawns position
            int index = BitboardHelper.GetIndexFromSquare(s);

            if (c == Piece.Color.White)
            {
                validMovesBB = (myPieceBB << 7 | myPieceBB << 9) & _engine.ActiveScene.BlackPieces;

                if (((myPieceBB << 8) & _engine.ActiveScene.AllPieces) == 0)
                {
                    validMovesBB = validMovesBB | (myPieceBB << 8);

                    if (((myPieceBB & BitboardHelper.MaskRank[(int)Ranks.Two]) != 0) && ((myPieceBB << 16) & _engine.ActiveScene.AllPieces) == 0)
                        validMovesBB = validMovesBB | (myPieceBB << 16);
                }
            }
            else
            {
                validMovesBB = (myPieceBB >> 7 | myPieceBB >> 9) & _engine.ActiveScene.WhitePieces;

                if (((myPieceBB >> 8) & _engine.ActiveScene.AllPieces) == 0)
                {
                    validMovesBB = validMovesBB | (myPieceBB >> 8);

                    if (((myPieceBB & BitboardHelper.MaskRank[(int)Ranks.Seven]) != 0) && ((myPieceBB >> 16) & _engine.ActiveScene.AllPieces) == 0)
                        validMovesBB = validMovesBB | myPieceBB >> 16;
                }
            }

            return validMovesBB;
        }

        static public UInt64 GenerateKnightMoves(Square s, Piece.Color c)
        {
            UInt64 validMovesBB;
            int sqIndex = BitboardHelper.GetIndexFromSquare(s);

            if (c == Piece.Color.White)
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & _engine.ActiveScene.WhitePieces;

            else if (c == Piece.Color.Black)
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & _engine.ActiveScene.BlackPieces;

            else
                validMovesBB = BitboardHelper.KnightAttacks[sqIndex];

            return validMovesBB;
        }

        static public UInt64 GenerateBishopMoves(Square s, Piece.Color c)
        {
            UInt64 validSquares;
            int sqIndex = BitboardHelper.GetIndexFromSquare(s);

            UInt64 bbBlockers = _engine.ActiveScene.AllPieces & BitboardHelper.OccupancyMaskBishop[sqIndex];

            int databaseIndex = (int)((bbBlockers * BitboardHelper.MagicNumberBishop[sqIndex]) >> BitboardHelper.MagicNumberShiftsBishop[sqIndex]);

            if (c == Piece.Color.White)
                validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex] & ~_engine.ActiveScene.WhitePieces;
            else if (c == Piece.Color.Black)
                validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex] & ~_engine.ActiveScene.BlackPieces;
            else
                validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex];

            return validSquares;
        }

        static public UInt64 GenerateRookMoves(Square s, Piece.Color c)
        {
            UInt64 validSquares;
            int sqIndex = BitboardHelper.GetIndexFromSquare(s);

            UInt64 bbBlockers = _engine.ActiveScene.AllPieces & BitboardHelper.OccupancyMaskRook[sqIndex];

            int databaseIndex = (int)((bbBlockers * BitboardHelper.MagicNumberRook[sqIndex]) >> BitboardHelper.MagicNumberShiftsRook[sqIndex]);

            if (c == Piece.Color.White)
                validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex] & ~_engine.ActiveScene.WhitePieces;
            else if (c == Piece.Color.Black)
                validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex] & ~_engine.ActiveScene.BlackPieces;
            else
                validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex];

            return validSquares;
        }

        static public UInt64 GenerateQueenMoves(Square s, Piece.Color c)
        {
            //first calulate Rook movements for queen
            ulong validSquares = GenerateRookMoves(s, c);

            //then calculate Bishop moves for queen and OR with rook movements
            validSquares |= GenerateBishopMoves(s, c);

            return validSquares;
        }


    }
}
