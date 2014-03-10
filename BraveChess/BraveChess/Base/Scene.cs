 using System;
using System.Collections.Generic;
 using System.Linq;
 using Microsoft.Xna.Framework;
using BraveChess.Objects;
using BraveChess.Engines;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using BraveChess.Helpers;

namespace BraveChess.Base
{
    public class Scene
    {
        public enum SelectionState
        {
            SelectPiece,
            PieceSelected,
            SelectMove,
            MoveSelected
        }
        public enum TurnState
        {
            Black,
            White
        }

        #region public, protected and private variables

        
        public string Id { get; set; }
        public TurnState Turn { get; set; }
        public SelectionState SelectState { get; set; }
        public List<GameObject3D> Objects { get { return SceneObjects; } }
        protected List<Move> AllMoves = new List<Move>();

        protected Camera CamWhite, CamBlack;
        protected int CurrentI, CurrentJ;
        protected bool  IsFight = false;
        protected Piece PieceToCapture, PieceToMove;
        protected Square CurrentSquare, PreviousSquare, GoFromSquare, GoToSquare;
        protected List<Square> MovesAvailable;
        protected Square[,] Squares;
        
        protected List<Piece> Pieces = new List<Piece>();
        protected List<GameObject3D> SceneObjects = new List<GameObject3D>();
        protected GameEngine Engine;

        readonly float _aspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

        #region Bitboards

        /* white pieces */
        protected UInt64 WhitePawns = 0xFF00LU;
        protected UInt64 WhiteKnights = 0x42LU;
        protected UInt64 WhiteBishops = 0X24LU;
        protected UInt64 WhiteRooks = 0x81LU;
        protected UInt64 WhiteQueens = 0x8LU;
        protected UInt64 WhiteKings = 0x10LU;
        /* black pieces */
        protected UInt64 BlackPawns = 0xFF000000000000LU;
        protected UInt64 BlackKnights = 0x4200000000000000LU;
        protected UInt64 BlackBishops = 0x2400000000000000LU;
        protected UInt64 BlackRooks = 0x8100000000000000LU;
        protected UInt64 BlackQueens = 0x800000000000000LU;
        protected UInt64 BlackKings = 0x1000000000000000LU;

        public UInt64 WhitePieces { get { return WhiteBishops | WhiteKings | WhiteKnights | WhitePawns | WhiteQueens | WhiteRooks; } }
        public UInt64 BlackPieces { get { return BlackBishops | BlackKings | BlackKnights | BlackPawns | BlackQueens | BlackRooks; } }
        public UInt64 AllPieces { get { return WhitePieces | BlackPieces; } }

        #endregion

        #endregion

        public Scene(string id, GameEngine engine)
        {
            Id = id;
            Engine = engine;
            Turn = TurnState.White;
        }//End of Constructor

        public virtual void Initialize()
        {
            #region Cameras
            CamWhite = new Camera("camWhite",
                new Vector3(1, 60, 60),
                new Vector3(1, 5, 4),
                _aspectRatio);

            CamBlack = new Camera("camBlack",
                new Vector3(0, 60, -61),
                new Vector3(0, 5, -5),
                _aspectRatio);

            Engine.Cameras.AddCamera(CamWhite);
            Engine.Cameras.AddCamera(CamBlack);

            #endregion

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


            Squares[CurrentI, CurrentJ].IsHover = true;
            #endregion

            foreach (GameObject3D t in SceneObjects)
                t.Initialise();

            BitboardHelper.Initialize();
        }//End of Method

        public virtual void Update(GameTime gametime)
        {
            foreach (GameObject3D t in SceneObjects)
                t.Update(gametime);
        } //End of Method

        protected virtual void HandleInput() 
        {
            if (InputEngine.IsKeyPressed(Keys.N))
            {
                MediaPlayer.Pause();
            }

            if (InputEngine.IsKeyPressed(Keys.M))
            {
                MediaPlayer.Resume();
            }

            #region Arrow move/selection 

            Squares[CurrentI, CurrentJ].IsHover = false;

            if (InputEngine.IsKeyPressed(Keys.Right))
            {
                if (Turn == TurnState.White)
                {
                    CurrentI++;
                    if (CurrentI > 7)
                        CurrentI = 0;
                }
                else
                {
                    CurrentI--;
                    if (CurrentI < 0)
                        CurrentI = 7;
                }
            }

            if (InputEngine.IsKeyPressed(Keys.Left))
            {
                if (Turn == TurnState.White)
                {
                    CurrentI--;
                    if (CurrentI < 0)
                        CurrentI = 7;
                }
                else
                {
                    CurrentI++;
                    if (CurrentI > 7)
                        CurrentI = 0;
                }
            }

            if (InputEngine.IsKeyPressed(Keys.Up))
            {
                if (Turn == TurnState.White)
                {
                    CurrentJ++;
                    if (CurrentJ > 7)
                        CurrentJ = 0;
                }
                else
                {
                    CurrentJ--;
                    if (CurrentI < 0)
                        CurrentI = 7;
                }
            }

            if (InputEngine.IsKeyPressed(Keys.Down))
            {
                if (Turn == TurnState.White)
                {
                    CurrentJ--;
                    if (CurrentJ < 0)
                        CurrentJ = 7;
                }
                else
                {
                    CurrentJ++;
                    if (CurrentJ > 7)
                        CurrentJ = 0;
                }
            }
            Squares[CurrentI, CurrentJ].IsHover = true;

            if (InputEngine.IsKeyPressed(Keys.Enter))
            {
                if (PreviousSquare != null)
                    PreviousSquare.IsSelected = false;

                CurrentSquare = Squares[CurrentI, CurrentJ];

                if (SelectState == SelectionState.SelectPiece) //if a piece hasnt been selected, highlight
                {
                    CurrentSquare.IsSelected = true;
                }
                else
                    PreviousSquare = null;

                PreviousSquare = CurrentSquare;
            }

            #endregion 
        }

        public void AddObject(GameObject3D newObject)
        {
            newObject.Destroying += _newObject_Destroying;
            SceneObjects.Add(newObject);
        }//End of method

        void _newObject_Destroying(string id)
        {
            RemoveObject(id);
        }//End of Method

        public GameObject3D GetObject(string id)
        {
            return SceneObjects.FirstOrDefault(t => t.Id == id);
        }

        public void RemoveObject(string id)
        {
            for (int i = 0; i < SceneObjects.Count; i++)
                if (SceneObjects[i].Id == id)
                    SceneObjects.RemoveAt(i);
        }

        protected void ResetMoves()
        {
            if (MovesAvailable != null)
            {
                foreach (Square s in MovesAvailable)
                    s.IsMoveOption = false;
            }
            MovesAvailable = null;
        }
    }
}
