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
             #region Init Sound
             //loading songs//efects
             Engine.Audio.LoadSong("BackgroundSong");
             Engine.Audio.PlaySong("BackgroundSong");
             MediaPlayer.IsRepeating = true;
             Engine.Audio.LoadEffect("MovePiece");
             #endregion

             base.Initialize();

             #region White Piece Init
             //White Pawn Set
             Pieces.Add(new Piece("WPawnA", "White Pawn", GetStartPos("a2"), 1, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("WPawnB", "White Pawn", GetStartPos("b2"), 1, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("WPawnC", "White Pawn", GetStartPos("c2"), 1, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("WPawnD", "White Pawn", GetStartPos("d2"), 1, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("WPawnE", "White Pawn", GetStartPos("e2"), 1, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("WPawnF", "White Pawn", GetStartPos("f2"), 1, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("WPawnG", "White Pawn", GetStartPos("g2"), 1, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("WPawnH", "White Pawn", GetStartPos("h2"), 1, Piece.PieceType.Pawn));

             Pieces.Add(new Piece("WRookA", "White Rook", GetStartPos("a1"), 1, Piece.PieceType.Rook));
             Pieces.Add(new Piece("WRookH", "White Rook", GetStartPos("h1"), 1, Piece.PieceType.Rook));

             Pieces.Add(new Piece("WKing", "White King", GetStartPos("e1"), 1, Piece.PieceType.King));

             Pieces.Add(new Piece("WQueen", "White Queen", GetStartPos("d1"), 1, Piece.PieceType.Queen));

             Pieces.Add(new Piece("WKnightB", "White Knight", GetStartPos("b1"), 1, Piece.PieceType.Knight));
             Pieces.Add(new Piece("WKnightG", "White Knight", GetStartPos("g1"), 1, Piece.PieceType.Knight));

             Pieces.Add(new Piece("WBishopC", "White Bishop", GetStartPos("c1"), 1, Piece.PieceType.Bishop));
             Pieces.Add(new Piece("WBishopF", "White Bishop", GetStartPos("f1"), 1, Piece.PieceType.Bishop));
             #endregion

             #region Black Piece Init
             //White Pawn Set
             Pieces.Add(new Piece("BPawnA", "Black Pawn", GetStartPos("a7"), 0, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("BPawnB", "Black Pawn", GetStartPos("b7"), 0, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("BPawnC", "Black Pawn", GetStartPos("c7"), 0, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("BPawnD", "Black Pawn", GetStartPos("d7"), 0, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("BPawnE", "Black Pawn", GetStartPos("e7"), 0, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("BPawnF", "Black Pawn", GetStartPos("f7"), 0, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("BPawnG", "Black Pawn", GetStartPos("g7"), 0, Piece.PieceType.Pawn));
             Pieces.Add(new Piece("BPawnH", "Black Pawn", GetStartPos("h7"), 0, Piece.PieceType.Pawn));


             Pieces.Add(new Piece("rookA", "Black Rook", GetStartPos("a8"), 0, Piece.PieceType.Rook));
             Pieces.Add(new Piece("rookH", "Black Rook", GetStartPos("h8"), 0, Piece.PieceType.Rook));

             Pieces.Add(new Piece("BKing", "Black King", GetStartPos("e8"), 0, Piece.PieceType.King));

             Pieces.Add(new Piece("BQueen", "Black Queen", GetStartPos("d8"), 0, Piece.PieceType.Queen));

             Pieces.Add(new Piece("BKnightB", "Black Knight", GetStartPos("b8"), 0, Piece.PieceType.Knight));
             Pieces.Add(new Piece("BKnightG", "Black Knight", GetStartPos("g8"), 0, Piece.PieceType.Knight));

             Pieces.Add(new Piece("BBishopC", "Black Bishop", GetStartPos("c8"), 0, Piece.PieceType.Bishop));
             Pieces.Add(new Piece("BBishopF", "Black Bishop", GetStartPos("f8"), 0, Piece.PieceType.Bishop));
             #endregion

            // AddObject(new SimpleModel("Terrain","Enviroment",new Vector3(0,0,0)));

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
                         PieceToCapture = GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

                         if (PieceToCapture != null)
                             IsFight = true;

                         ToSquare = CurrentSquare;

                         SelectState = SelectionState.MoveSelected;
                     }
                     else if (CurrentSquare != null)
                     {
                         Piece p = GetPiece(CurrentSquare.World.Translation + new Vector3(0, 2, 0));

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

                     Move m = new Move(Engine, GameBoard, FromSquare, ToSquare, PieceToMove, IsFight, PieceToCapture, false); //add new Move to list AllMoves
                     if (m.IsValidMove)
                     {
                         if (Turn == TurnState.Black)
                         {
                             BlackMoves.Add(m.ToAlgebraic());
                             
                         }
                         else if (Turn == TurnState.White)
                         {
                             WhiteMoves.Add(m.ToAlgebraic());
                         }
                         Engine.Audio.PlayEffect("MovePiece");
                         SelectState = SelectionState.SelectPiece;
                         IsFight = false;
                         SwitchTurn();
                     }
                     else
                     {
                         NotificationEngine.AddNotification(new Notification("Sorry that would leave you in check!!", 4000));
                         SelectState = SelectionState.SelectPiece;
                         IsFight = false;
                     }
                         
                     break;

             } // end switch
         }
     
        private void SwitchTurn()
        {
            Turn = Turn == TurnState.White ? TurnState.Black : TurnState.White;

            Engine.Cameras.SetActiveCamera(Engine.Cameras.ActiveCamera.Id == "camWhite" ? "camBlack" : "camWhite");
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
     

    }
}
