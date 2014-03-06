 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BraveChess.Objects;
using BraveChess.Engines;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

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

        public List<Piece> Pieces { get { return _pieces; } }
        static BitboardHelper BBHelper = new BitboardHelper();
        public string ID { get; set; }
        public TurnState Turn { get; set; }
        public SelectionState SelectState { get; set; }
        public List<GameObject3D> Objects { get { return _sceneObjects; } }

        protected Camera _camWhite, _camBlack;
        protected int _currentI, _currentJ;
        protected bool  IsFight = false;
        protected Piece PieceToCapture, PieceToMove;
        protected Square _currentSquare, _previousSquare, _goFromSquare, _goToSquare;
        protected List<Square> Moves;
        protected Square[,] Squares;
        protected List<Piece> _pieces = new List<Piece>();
        protected List<GameObject3D> _sceneObjects = new List<GameObject3D>();
        protected GameEngine Engine;

        float aspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

        #region Bitboards

        /* white pieces */
        protected UInt64 white_pawns = 0xFF00LU;
        protected UInt64 white_knights = 0x42LU;
        protected UInt64 white_bishops = 0X24LU;
        protected UInt64 white_rooks = 0x81LU;
        protected UInt64 white_queens = 0x8LU;
        protected UInt64 white_kings = 0x10LU;
        /* black pieces */
        protected UInt64 black_pawns = 0xFF000000000000LU;
        protected UInt64 black_knights = 0x4200000000000000LU;
        protected UInt64 black_bishops = 0x2400000000000000LU;
        protected UInt64 black_rooks = 0x8100000000000000LU;
        protected UInt64 black_queens = 0x800000000000000LU;
        protected UInt64 black_kings = 0x1000000000000000LU;

        public UInt64 WhitePieces { get { return white_bishops | white_kings | white_knights | white_pawns | white_queens | white_rooks; } }
        public UInt64 BlackPieces { get { return black_bishops | black_kings | black_knights | black_pawns | black_queens | black_rooks; } }
        public UInt64 AllPieces { get { return WhitePieces | BlackPieces; } }

        #endregion

        #endregion

        public Scene(string id, GameEngine engine)
        {
            ID = id;
            Engine = engine;
            Turn = TurnState.White;
        }//End of Constructor

        public virtual void Initialize()
        {
            #region Cameras
            _camWhite = new Camera("camWhite",
                new Vector3(1, 60, 60),
                new Vector3(1, 5, 4),
                aspectRatio);

            _camBlack = new Camera("camBlack",
                new Vector3(0, 60, -61),
                new Vector3(0, 5, -5),
                aspectRatio);

            Engine.Cameras.AddCamera(_camWhite);
            Engine.Cameras.AddCamera(_camBlack);

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


            Squares[_currentI, _currentJ].IsHover = true;
            #endregion

            for (int i = 0; i < _sceneObjects.Count; i++)
                _sceneObjects[i].Initialise();
        }//End of Method

        public virtual void Update(GameTime gametime)
        {
            for (int i = 0; i < _sceneObjects.Count; i++)
                _sceneObjects[i].Update(gametime);
        }//End of Method

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

            Squares[_currentI, _currentJ].IsHover = false;

            if (InputEngine.IsKeyPressed(Keys.Right))
            {
                if (Turn == TurnState.White)
                {
                    _currentI++;
                    if (_currentI > 7)
                        _currentI = 0;
                }
                else
                {
                    _currentI--;
                    if (_currentI < 0)
                        _currentI = 7;
                }
            }

            if (InputEngine.IsKeyPressed(Keys.Left))
            {
                if (Turn == TurnState.White)
                {
                    _currentI--;
                    if (_currentI < 0)
                        _currentI = 7;
                }
                else
                {
                    _currentI++;
                    if (_currentI > 7)
                        _currentI = 0;
                }
            }

            if (InputEngine.IsKeyPressed(Keys.Up))
            {
                if (Turn == TurnState.White)
                {
                    _currentJ++;
                    if (_currentJ > 7)
                        _currentJ = 0;
                }
                else
                {
                    _currentJ--;
                    if (_currentI < 0)
                        _currentI = 7;
                }
            }

            if (InputEngine.IsKeyPressed(Keys.Down))
            {
                if (Turn == TurnState.White)
                {
                    _currentJ--;
                    if (_currentJ < 0)
                        _currentJ = 7;
                }
                else
                {
                    _currentJ++;
                    if (_currentI > 7)
                        _currentI = 0;
                }
            }
            Squares[_currentI, _currentJ].IsHover = true;

            if (InputEngine.IsKeyPressed(Keys.Enter))
            {
                if (_previousSquare != null)
                    _previousSquare.IsSelected = false;

                _currentSquare = Squares[_currentI, _currentJ];

                if (SelectState == SelectionState.SelectPiece) //if a piece hasnt been selected, highlight
                {
                    _currentSquare.IsSelected = true;
                }
                else
                    _previousSquare = null;

                _previousSquare = _currentSquare;
            }
            #endregion 
        }

        public void AddObject(GameObject3D _newObject)
        {
            _newObject.Destroying += new GameObjectEventHandler(_newObject_Destroying);
            _sceneObjects.Add(_newObject);
        }//End of method

        void _newObject_Destroying(string id)
        {
            RemoveObject(id);
        }//End of Method

        public GameObject3D GetObject(string id)
        {
            for (int i = 0; i < _sceneObjects.Count; i++)
                if (_sceneObjects[i].ID == id)
                    return _sceneObjects[i];

            return null;
        }

        public void RemoveObject(string id)
        {
            for (int i = 0; i < _sceneObjects.Count; i++)
                if (_sceneObjects[i].ID == id)
                    _sceneObjects.RemoveAt(i);
        }

        protected void ResetMoves()
        {
            if (Moves != null)
            {
                foreach (Square s in Moves)
                    s.IsMoveOption = s.IsMoveOption == true ? false : false;
            }
            Moves = null;
        }
    }
}
