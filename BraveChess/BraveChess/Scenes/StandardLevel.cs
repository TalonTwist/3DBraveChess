using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

using BraveChess.Engines;
using BraveChess.Base;
using BraveChess.Objects;
using BraveChess.Helpers;

namespace BraveChess.Scenes
{
    class StandardLevel : Scene
    {
         public StandardLevel(GameEngine engine)
            : base("StandardLevel", engine) { }

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

             AddObject(new SimpleModel("Terrain","Enviroment",new Vector3(0,0,0)));

             //add Objects
             foreach (Square s in Squares)
                 AddObject(s);

             foreach (Piece p in Pieces)
                 AddObject(p);

             Engine.Cameras.SetActiveCamera("camWhite");

             MovesAvailable = new List<Square>();

         }//End of Method
       
         public override void Update(GameTime gametime)
         {
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
                         PieceToMove = GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

                         if (PieceToMove != null && ((int)PieceToMove.ColorType) == (int)Turn)
                         {
                             GoFromSquare = CurrentSquare;
                             SelectState = SelectionState.PieceSelected;
                         }

                         CurrentSquare = null;
                     }
                     break;

                 case SelectionState.PieceSelected:
                     MovesAvailable = GenerateMoves(PieceToMove, GoFromSquare);
                     if (MovesAvailable != null)
                     {
                         foreach (Square s in MovesAvailable)
                             s.IsMoveOption = true;
                         SelectState = SelectionState.SelectMove;
                     }
                     else
                     {
                         ResetMoves();
                         SelectState = SelectionState.SelectPiece;
                     }
                     break;

                 case SelectionState.SelectMove:
                     if (CurrentSquare != null && MovesAvailable.Contains(CurrentSquare))
                     {
                         PieceToCapture = GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

                         if (PieceToCapture != null)
                             IsFight = true;

                         GoToSquare = CurrentSquare;

                         SelectState = SelectionState.MoveSelected;
                     }
                     else if (CurrentSquare != null)
                     {
                         Piece p = GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

                         if (p != null && ((int)PieceToMove.ColorType) == (int)Turn) //Replace selection with this piece
                         {
                             PieceToMove = p;
                             GoFromSquare = CurrentSquare;
                             SelectState = SelectionState.PieceSelected;
                         }
                         else
                             SelectState = SelectionState.SelectPiece;
                         ResetMoves();
                     }

                     CurrentSquare = null;
                     break;

                 case SelectionState.MoveSelected:
                     ResetMoves();

                     //check for pawn queening

                     //Make sure the move doesnt leave king in check(test the move)
                     if (TestMove(PieceToMove, GoFromSquare, GoToSquare))
                     {
                         if (IsFight)
                         {
                             Engine.Audio.PlayEffect("CapturePiece");
                             CapturePiece(); //Remove the captured piece
                         }
                         AllMoves.Add(new Move(GoFromSquare, GoToSquare, PieceToMove, IsFight)); //add new Move to list AllMoves
                         NotificationEngine.AddNotification(new Notification(AllMoves.Last().ToAlgebraic(), 3000));
                         
                         MovePiece(PieceToMove, GoToSquare); //make the move

                         SelectState = SelectionState.SelectPiece; //reset
                         IsFight = false;

                         Engine.Audio.PlayEffect("MovePiece");

                         SwitchTurn(); //Changes turnState 
                     }
                     else
                         NotificationEngine.AddNotification(new Notification("Sorry that would leave you in check!!", 3000));

                     break;

             } // end switch
         }



         private void CapturePiece() //Remove the Piece and update bitboards
         {
             UpdateRelevantbb(PieceToCapture.Piece_Type, PieceToCapture.ColorType, BitboardHelper.GetBitboardFromSquare(GoToSquare), 0);
             Pieces.Remove(PieceToCapture);
             PieceToCapture.Destroy();
         }

         private void MovePiece(Piece piece, Square to)
         {
             piece.UpdateWorld(GetNewPos(to)); //update world position of model
         }

         private bool TestMove(Piece piece, Square from, Square to)
         {
             UInt64 bbFrom = BitboardHelper.GetBitboardFromSquare(from);
             UInt64 bbTo = BitboardHelper.GetBitboardFromSquare(to);

             UpdateRelevantbb(piece.Piece_Type, piece.ColorType, bbFrom, bbTo);

             if (TestForCheck(piece.ColorType))
             {
                 UpdateRelevantbb(piece.Piece_Type, piece.ColorType, bbTo, bbFrom);
                 return false;
             }

            
             return true;
         } //sorry that would leave you in check

        private void SwitchTurn()
        {
            Turn = Turn == TurnState.White ? TurnState.Black : TurnState.White;

            Engine.Cameras.SetActiveCamera(Engine.Cameras.ActiveCamera.Id == "camWhite" ? "camBlack" : "camWhite");
        }

        protected override void HandleInput()
         {
            base.HandleInput();
         }

         private Piece GetPiece(Vector3 pos)
         {
             return Pieces.FirstOrDefault(t => t.World.Translation == pos);
         }

        private Vector3 GetStartPos(string pos)
         {
             char[] c = pos.ToCharArray();

             int file = c[0] - 97;
             int rank = c[1] - 49;

             return Squares[file, rank].World.Translation + new Vector3(0, 2, 0);
         }

         private Vector3 GetNewPos(Square destination)
         {
             return destination.World.Translation + new Vector3(0, 2, 0);
         }

         private Square GetSquareFromBB(ulong bb)
         {
             var v = BitboardHelper.GetSquareFromBitboard(bb);

             return Squares[v.Item2, v.Item1];
         }

         private List<Square> GetSquareListFromBB(ulong bb)
         {
             List<Square> s = new List<Square>();
             var sList = BitboardHelper.GetSquareListFromBB(bb);

             if (sList == null) return null;
             s.AddRange(sList.Select(t => Squares[t.Item2, t.Item1]));

             return s;
         }

         private List<Square> GenerateMoves(Piece p, Square s)
         {
             //Call method based on Type of Piece passed in
             switch (p.Piece_Type)
             {
                 case Piece.PieceType.King:
                     return GetSquareListFromBB(GenerateKingMoves(s, p.ColorType));

                 case Piece.PieceType.Pawn:
                     return GetSquareListFromBB(GeneratePawnMoves(s, p.ColorType));

                 case Piece.PieceType.Knight:
                     return GetSquareListFromBB(GenerateKnightMoves(s, p.ColorType));

                 case Piece.PieceType.Bishop:
                     return GetSquareListFromBB(GenerateBishopMoves(s, p.ColorType));

                 case Piece.PieceType.Rook:
                     return GetSquareListFromBB(GenerateRookMoves(s, p.ColorType));

                 case Piece.PieceType.Queen:
                     return GetSquareListFromBB(GenerateQueenMoves(s, p.ColorType));
                 default:
                     return null;
             }
         }

         private UInt64 GenerateKingMoves(Square s, Piece.Color c)
         {
             UInt64 myPieceBB = BitboardHelper.GetBitboardFromSquare(s);

             UInt64 myPieceBB_H_Clip = (myPieceBB & BitboardHelper.ClearFile[7]);
             UInt64 myPieceBB_A_Clip = (myPieceBB & BitboardHelper.ClearFile[0]);

             UInt64 validMovesBB = (myPieceBB_A_Clip << 7) | (myPieceBB << 8) | (myPieceBB_H_Clip << 9) | (myPieceBB_H_Clip << 1) | (myPieceBB_H_Clip >> 7) | (myPieceBB >> 8) | (myPieceBB_A_Clip >> 9) | (myPieceBB_A_Clip >> 1);


             if (c == Piece.Color.White)
                 validMovesBB = validMovesBB ^ (validMovesBB & WhitePieces);
             else
                 validMovesBB = validMovesBB ^ (validMovesBB & BlackPieces);

             return validMovesBB;
         }

         private UInt64 GeneratePawnMoves(Square s, Piece.Color c)
         {
             UInt64 validMovesBB;
             UInt64 myPieceBB = BitboardHelper.GetBitboardFromSquare(s); //bitboard representation of the pawns position

             if (c == Piece.Color.White)
             {
                 validMovesBB = (myPieceBB << 7 | myPieceBB << 9) & BlackPieces;

                 if (((myPieceBB << 8) & AllPieces) == 0)
                 {
                     validMovesBB = validMovesBB | (myPieceBB << 8);

                     if (((myPieceBB & BitboardHelper.MaskRank[(int)Ranks.Two]) != 0) && ((myPieceBB << 16) & AllPieces) == 0)
                         validMovesBB = validMovesBB | (myPieceBB << 16);
                 }
             }
             else
             {
                 validMovesBB = (myPieceBB >> 7 | myPieceBB >> 9) & WhitePieces;

                 if (((myPieceBB >> 8) & AllPieces) == 0)
                 {
                     validMovesBB = validMovesBB | (myPieceBB >> 8);

                     if (((myPieceBB & BitboardHelper.MaskRank[(int)Ranks.Seven]) != 0) && ((myPieceBB >> 16) & AllPieces) == 0)
                         validMovesBB = validMovesBB | myPieceBB >> 16;
                 }
             }

             return validMovesBB;
         }

         private UInt64 GenerateKnightMoves(Square s, Piece.Color c)
         {
             UInt64 validMovesBB;
             int sqIndex = BitboardHelper.GetIndexFromSquare(s);

             if (c == Piece.Color.White)
                 validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & WhitePieces;

             else if (c == Piece.Color.Black)
                 validMovesBB = BitboardHelper.KnightAttacks[sqIndex] ^ (BitboardHelper.KnightAttacks[sqIndex]) & BlackPieces;

             else
                 validMovesBB = BitboardHelper.KnightAttacks[sqIndex];

             return validMovesBB;
         }

         private UInt64 GenerateBishopMoves(Square s, Piece.Color c)
         {
             UInt64 validSquares;
             int sqIndex = BitboardHelper.GetIndexFromSquare(s);

             UInt64 bbBlockers = AllPieces & BitboardHelper.OccupancyMaskBishop[sqIndex];

             int databaseIndex = (int)((bbBlockers * BitboardHelper.MagicNumberBishop[sqIndex]) >> BitboardHelper.MagicNumberShiftsBishop[sqIndex]);

             if (c == Piece.Color.White)
                 validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex] & ~WhitePieces;
             else if (c == Piece.Color.Black)
                 validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex] & ~BlackPieces;
             else
                 validSquares = BitboardHelper.MagicMovesBishop[sqIndex][databaseIndex];

             return validSquares;
         }

         private UInt64 GenerateRookMoves(Square s, Piece.Color c)
         {
             UInt64 validSquares;
             int sqIndex = BitboardHelper.GetIndexFromSquare(s);

             UInt64 bbBlockers = AllPieces & BitboardHelper.OccupancyMaskRook[sqIndex];

             int databaseIndex = (int)((bbBlockers * BitboardHelper.MagicNumberRook[sqIndex]) >> BitboardHelper.MagicNumberShiftsRook[sqIndex]);

             if (c == Piece.Color.White)
                 validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex] & ~WhitePieces;
             else if (c == Piece.Color.Black)
                 validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex] & ~BlackPieces;
             else
                 validSquares = BitboardHelper.MagicMovesRook[sqIndex][databaseIndex];

             return validSquares;
         }

         private UInt64 GenerateQueenMoves(Square s, Piece.Color c)
         {
             //first calulate Rook movements for queen
             ulong validSquares = GenerateRookMoves(s, c);

             //then calculate Bishop moves for queen and OR with rook movements
             validSquares |= GenerateBishopMoves(s, c);

             return validSquares;
         }

         private UInt64 FindAttacksToSquare(Square s) // returns bitboard with all pieces attacking the specified Square
         {
             int sqIndex = BitboardHelper.GetIndexFromSquare(s);

             ulong attackersBB = (BitboardHelper.KnightAttacks[sqIndex] & WhiteKnights & BlackKnights);
             attackersBB |= (GenerateBishopMoves(s, Piece.Color.None) & BlackBishops & WhiteBishops & BlackQueens & WhiteQueens);
             attackersBB |= (GenerateRookMoves(s, Piece.Color.None) & BlackRooks & WhiteRooks & BlackQueens & WhiteQueens);
             //add pawn and king attacks

             return attackersBB;
         }

         private bool TestForCheck(Piece.Color c)
         {
             Square kingPos;

             if (c == Piece.Color.White)
             {
                 kingPos = GetSquareFromBB(WhiteKings);

                 //if all pieces attacking the kings position minus pieces of his own colour != 0, then the king is in check
                 if ((FindAttacksToSquare(kingPos) & ~WhitePieces) != 0)
                     return true;
             }
             else if (c == Piece.Color.Black)
             {
                 kingPos = GetSquareFromBB(BlackKings);
                 if ((FindAttacksToSquare(kingPos) & ~BlackPieces) != 0)
                     return true;
             }
             return false;
         }

         private void UpdateRelevantbb(Piece.PieceType type, Piece.Color c, ulong bbFrom, ulong bbTo)
         {
             if (c == Piece.Color.White)
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
