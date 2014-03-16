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
         public StandardLevel(GameEngine engine, bool isAnimated)
            : base("StandardLevel", engine, isAnimated) { }

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

            // AddObject(new SimpleModel("Terrain","Enviroment",new Vector3(0,0,0)));

             //add Objects
             foreach (Square s in GameBoard.Squares)
                 AddObject(s);

             foreach (Piece p in GameBoard.Pieces)
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
     

    }
}
