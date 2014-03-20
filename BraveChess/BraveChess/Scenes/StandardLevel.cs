using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

using BraveChess.Engines;
using BraveChess.Base;
using BraveChess.Objects;

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
                     MovesAvailable = MoveGen.GenerateMoves(PieceToMove, FromSquare);
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

                     //check for pawn promotion
                     if (GameBoard.TestForPromotion(PieceToMove.ColorType, ToSquare.Rank) &
                         PromoteTo == Piece.PieceType.None)
                     {
                         SelectState = SelectionState.Promote;
                         break;
                     }


                     Move m = new Move(Engine, GameBoard, FromSquare, ToSquare, PieceToMove, PieceToCapture, false, PromoteTo); //add new Move to list AllMoves
                     if (m.IsValidMove)
                     {
                         GameBoard.AllMoves.Add(m);
                         Engine.Audio.PlayEffect("MovePiece");
                         SelectState = SelectionState.SelectPiece;
                         SwitchTurn();
                     }
                     else
                     {
                         NotificationEngine.AddNotification(new Notification("Sorry that would leave you in check!!", 4000));
                         SelectState = SelectionState.SelectPiece;
                     }
                         PromoteTo = Piece.PieceType.None;
                     
                     break;

             } // end switch
         }
     
        public void SwitchTurn()
        {
            Turn = Turn == TurnState.White ? TurnState.Black : TurnState.White;

            Engine.Cameras.SetActiveCamera(Engine.Cameras.ActiveCamera.Id == "camWhite" ? "camBlack" : "camWhite");
        }
       
    }
}
