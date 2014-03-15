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

        public Board GameBoard;
        
        public string Id { get; set; }
        public int MovePossitionY { get; set; }      
        public TurnState Turn { get; set; }
        public SelectionState SelectState { get; set; }
        public List<GameObject3D> Objects { get { return SceneObjects; } }
        public List<string> BlackMoves = new List<string>();
        public List<string> WhiteMoves = new List<string>();

        protected Camera CamWhite, CamBlack;
        protected int CurrentI, CurrentJ;
        protected bool  IsFight = false;
        private bool _isMouseClick;
        protected Piece PieceToCapture, PieceToMove;
        protected Square CurrentSquare, PreviousSquare, FromSquare, ToSquare;
        protected List<Square> MovesAvailable;

        private bool _IsAnimated;
        
        protected List<GameObject3D> SceneObjects = new List<GameObject3D>();
        protected GameEngine Engine;

        readonly float _aspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

        #endregion

        public Scene(string id, GameEngine engine, bool isAnimated)
        {
            Id = id;
            Engine = engine;
            Turn = TurnState.White;
            _IsAnimated = isAnimated;
        }//End of Constructor

        public virtual void Initialize()
        {
            GameBoard = new Board(_IsAnimated);

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


            GameBoard.Squares[CurrentI, CurrentJ].IsHover = true;
            foreach (GameObject3D t in SceneObjects)
                t.Initialise();

            MoveGen.Init(GameBoard);
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

            GameBoard.Squares[CurrentI, CurrentJ].IsHover = false;

            if (InputEngine.IsKeyPressed(Keys.Right))
            {
                _isMouseClick = false;
                if (PreviousSquare != null)
                    PreviousSquare.IsSelected = false;

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
                _isMouseClick = false;
                if (PreviousSquare != null)
                    PreviousSquare.IsSelected = false;

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
                _isMouseClick = false;
                if (PreviousSquare != null)
                    PreviousSquare.IsSelected = false;

                if (Turn == TurnState.White)
                {
                    CurrentJ++;
                    if (CurrentJ > 7)
                        CurrentJ = 0;
                }
                else
                {
                    CurrentJ--;
                    if (CurrentJ < 0)
                        CurrentJ = 7;
                }
            }

            if (InputEngine.IsKeyPressed(Keys.Down))
            {
                _isMouseClick = false;
                if (PreviousSquare != null)
                    PreviousSquare.IsSelected = false;

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
            if (!_isMouseClick)
            {
                GameBoard.Squares[CurrentI, CurrentJ].IsHover = true;
            }

            if (InputEngine.IsKeyPressed(Keys.Enter))
            {
                if (PreviousSquare != null)
                    PreviousSquare.IsSelected = false;

                CurrentSquare = GameBoard.Squares[CurrentI, CurrentJ];

                if (SelectState == SelectionState.SelectPiece) //if a piece hasnt been selected, highlight
                {
                    CurrentSquare.IsSelected = true;
                }
                else
                    PreviousSquare = null;

                PreviousSquare = CurrentSquare;
            }

            if (InputEngine.IsMouseLeftClick())
            {
                if (PreviousSquare != null)
                    PreviousSquare.IsSelected = false;

                _isMouseClick = true;

                CurrentSquare = SquareSelectWithMouse();

                if (SelectState == SelectionState.SelectPiece && CurrentSquare != null) //if a piece hasnt been selected, highlight
                {
                        CurrentSquare.IsSelected = true;
                }
                else
                    PreviousSquare = null;
                

                PreviousSquare = CurrentSquare;
            }
            
            #endregion 
        }

        public Square SquareSelectWithMouse()
        {
                //Square square;
                Ray ray = RayCast();

                foreach (Square sq in GameBoard.Squares)
                {
                    float? result = ray.Intersects(sq.AABB);

                    if (result.HasValue)
                    {
                        return sq;
                    }
                }
            return null;
        }

        public void DrawMoves(SpriteBatch batch, SpriteFont font,GraphicsDevice graphics)
        {
            batch.Begin();
            for (int i = 0; i < WhiteMoves.Count; i++)
            {
                batch.DrawString(font, String.Format("{0}.", i), new Vector2(10, 20 * i), Color.Black);
                batch.DrawString(font, WhiteMoves[i],
                    new Vector2(graphics.Viewport.Width / 30, 20 * i), Color.Black);
            }
            for (int i = 0; i < BlackMoves.Count; i++)
                batch.DrawString(font, BlackMoves[i],
                    new Vector2(graphics.Viewport.Width / 10, 20 * i), Color.Black);
            batch.End();

        }

        protected Piece GetPiece(Vector3 pos)
        {
            return GameBoard.Pieces.FirstOrDefault(t => t.World.Translation == pos);
        }

        public Ray RayCast()//makes a ray
        {
            int mouseX = InputEngine.CurrentMouseState.X;
            int mouseY = InputEngine.CurrentMouseState.Y;

            Vector3 nearSource = new Vector3(mouseX, mouseY, 0);
            Vector3 farSource = new Vector3(mouseX, mouseY, 1);

            Vector3 nearPoint = Engine.GraphicsDevice.Viewport.Unproject(nearSource, 
                Engine.Cameras.ActiveCamera.Projection, 
                Engine.Cameras.ActiveCamera.View, 
                Matrix.Identity);
            Vector3 farPoint = Engine.GraphicsDevice.Viewport.Unproject(farSource, 
                Engine.Cameras.ActiveCamera.Projection, 
                Engine.Cameras.ActiveCamera.View, 
                Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

            return pickRay;
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

        protected void ResetMovesAvailable()
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
