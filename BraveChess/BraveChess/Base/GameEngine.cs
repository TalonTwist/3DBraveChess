using System;
using System.Collections.Generic;
using BraveChess.Helpers;
using Microsoft.Xna.Framework;
using BraveChess.Engines;
using BraveChess.Scenes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using BraveChess.Objects;
using Microsoft.Xna.Framework.Media;

namespace BraveChess.Base
{
    public class GameEngine : DrawableGameComponent
    {
        public enum State
        {
            MainMenu,
            NonNetworkGame,
            NetworkGame,
            TimedNetworkGame,
            PlayingNormal,
            PlayingNetworked,
            PlayingTimedNetworkGame,
            GameOverWhiteWins,
            GameOverBlackWins,
            GameOverDraw,
            ExitGame
        }

        public State GameState;


        Button _btnStandart, _btnNetwork, _btnExit,
            _btnNewGame,_btnUndoMove,_btnStandardPieces,
            _btnAnimatedPieces,_btnBack,_btnMainMenu,_btnTimedGame, 
            _queenBtn, _bishopBtn, _knightBtn, _rookBtn;

        private List<Button> promoteButtons;
        
        Texture2D _background,_draw,_whiteWins,_blackWins;

        SpriteBatch _batch;
        SpriteFont _font, _fontTimer;

        public InputEngine Input { get; set; }
        public CameraEngine Cameras { get; set; }
        public AudioEngine Audio { get; set; }
        public DebugEngine Debug { get; set; }
        public FrameRateCounter FpsCounter { get; set; }
        public NotificationEngine NoteEngine;
        public NetworkEngine Network { get; set; }

        public Scene ActiveScene { get; set; }

        public GameEngine(Game game)
            : base(game)
        {
            game.Components.Add(this);

            Input = new InputEngine(game);
            Cameras = new CameraEngine(game);
            Audio = new AudioEngine(game);
            
            Debug = new DebugEngine();
            NoteEngine = new NotificationEngine(game, 3);
          //  GameNetwork = new Networking(_game, this);
        }

        public override void Initialize()
        {
            Debug.Initialize();
            
            #region Sound
            //loading songs//efects
            Audio.LoadSong("BackgroundSong");
            MediaPlayer.IsRepeating = true;
            Audio.LoadEffect("MovePiece");
            Audio.LoadEffect("CapturePiece");
            Audio.LoadEffect("MenuHover");
            #endregion

            base.Initialize();
        }//End of Override

        protected override void LoadContent()
        {
            _batch = new SpriteBatch(GraphicsDevice);
            _font = Game.Content.Load<SpriteFont>("Fonts\\debug");
            _fontTimer = Game.Content.Load<SpriteFont>("Fonts\\timer");

            FpsCounter = new FrameRateCounter(Game, new Vector2(800,10));

            #region Load Buttons and other stuff
            _background = Game.Content.Load<Texture2D>("Buttons\\Background");
            _blackWins = Game.Content.Load<Texture2D>("Buttons\\BlackWins");
            _whiteWins = Game.Content.Load<Texture2D>("Buttons\\WhiteWins");
            _draw = Game.Content.Load<Texture2D>("Buttons\\Draw");

            _btnStandart = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\StandardButton"),200);

            _btnNetwork = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\NetworkButton"), 250);

            _btnTimedGame = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\TimedGameButton"), 300);

            _btnExit = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\ExitGameButton"), 350);

            _btnNewGame = new Button(Buttontype.Ui, Game.Content.Load<Texture2D>("Buttons\\NewGameButton"),
                    new Vector2(20,400));
            _btnUndoMove = new Button(Buttontype.Ui, Game.Content.Load<Texture2D>("Buttons\\UndoMoveButton"),
                    new Vector2(20,450));
            _btnStandardPieces = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\StandardPiecesButton"), 200);

            _btnAnimatedPieces = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\AnimatedPiecesButton"), 250);

            _btnBack = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\BackButton"), 300);

            _btnMainMenu = new Button(Buttontype.Menu, Game.Content.Load<Texture2D>("Buttons\\MainMenuButton"), 450);

            _knightBtn = new Button(Buttontype.Promotion, Game.Content.Load<Texture2D>("Buttons\\KnightButton"),
                Helper.GraphicsDevice.Viewport.Width/2);
            _bishopBtn = new Button(Buttontype.Promotion, Game.Content.Load<Texture2D>("Buttons\\BishopButton"),
                Helper.GraphicsDevice.Viewport.Width/2 + 100);
            _rookBtn = new Button(Buttontype.Promotion, Game.Content.Load<Texture2D>("Buttons\\RookButton"),
                Helper.GraphicsDevice.Viewport.Width/2 + 200);
            _queenBtn = new Button(Buttontype.Promotion, Game.Content.Load<Texture2D>("Buttons\\QueenButton"),
                Helper.GraphicsDevice.Viewport.Width/2 + 300);

            promoteButtons = new List<Button>
            {
                _queenBtn,
                _knightBtn,
                _bishopBtn,
                _rookBtn
            };

            #endregion

            Debug.LoadContent(Game.Content);
            base.LoadContent();
        }//End of Override

        public override void Update(GameTime gameTime)
        {
            if (ActiveScene != null)
            {
                ActiveScene.Update(gameTime);

                if (ActiveScene.SelectState == Scene.SelectionState.Promote)
                {
                    if (_queenBtn.IsClicked)
                        ActiveScene.SetPromotion(Piece.PieceType.Queen);
                    if (_bishopBtn.IsClicked)
                        ActiveScene.SetPromotion(Piece.PieceType.Bishop);
                    if (_knightBtn.IsClicked)
                        ActiveScene.SetPromotion(Piece.PieceType.Knight);
                    if (_rookBtn.IsClicked)
                        ActiveScene.SetPromotion(Piece.PieceType.Rook);

                    foreach (var b in promoteButtons)
                    {
                        b.Update(this);
                    }
                }

            }
            
              
            
            ScreenStates(gameTime);

            base.Update(gameTime);
        }//End of Override

        public override void Draw(GameTime gameTime)
        {
            if (GameState == State.MainMenu || GameState == State.NetworkGame || GameState == State.NonNetworkGame || GameState == State.TimedNetworkGame)
            {
                _batch.Begin();
                _batch.Draw(_background, GraphicsDevice.Viewport.Bounds, Color.White);
                _batch.End();
            }
            ScreenStatesDraw();

            Draw3D();

            if (ActiveScene != null)
            {
                ActiveScene.DrawMoves(_batch, _font, GraphicsDevice);

                if (ActiveScene.SelectState == Scene.SelectionState.Promote)
                {
                    foreach (var b in promoteButtons)
                        b.Draw(_batch);
                }
            }
                

            if (GameState == State.PlayingNetworked || GameState == State.PlayingTimedNetworkGame)
                Draw2D();
          
            base.Draw(gameTime);
        }

        public void Draw2D() 
        {
            switch (Network.CurrentNetworkState)
            {
                case NetworkState.SignIn:
                case NetworkState.FindSession:
                case NetworkState.CreateSession:
                    GraphicsDevice.Clear(Color.DarkGray);
                    break;
                case NetworkState.Start:
                    DrawReadyToStartScreen();
                    break;
                case NetworkState.InGame:
                    ActiveScene.DrawTime(_batch,_fontTimer);
                    if(ActiveScene.TimeWhite <= TimeSpan.Zero && ActiveScene.TimeWhite2 <= TimeSpan.Zero)
                        Network.CurrentNetworkState = NetworkState.GameOverBlackWins;
                    if (ActiveScene.TimeBlack <= TimeSpan.Zero && ActiveScene.TimeBlack2 <= TimeSpan.Zero)
                        Network.CurrentNetworkState = NetworkState.GameOverWhiteWins;
                    break;
                case NetworkState.GameOverWhiteWins:
                    _batch.Begin();
                    _batch.Draw(_whiteWins,GraphicsDevice.Viewport.Bounds,Color.White);
                    _batch.End();
                    _btnMainMenu.Update(this);
                    if (_btnMainMenu.IsClicked)
                    {
                        ActiveScene = null;
                        Network.NetworkSession = null;
                        GameState = State.MainMenu;
                    }
                    _btnMainMenu.Draw(_batch);
                    break;
                case NetworkState.GameOverBlackWins:
                    _batch.Begin();
                    _batch.Draw(_blackWins,GraphicsDevice.Viewport.Bounds,Color.White);
                    _batch.End();
                    _btnMainMenu.Update(this);
                    if (_btnMainMenu.IsClicked)
                    {
                        ActiveScene = null;
                        Network.NetworkSession = null;
                        GameState = State.MainMenu;
                    }
                    _btnMainMenu.Draw(_batch);
                    break;
                case NetworkState.GameOverDraw:
                    break;
            }
        }

        public void Draw3D()
        {
            if (Cameras.ActiveCamera != null && ActiveScene != null)
                foreach (GameObject3D t in ActiveScene.Objects)
                    t.Draw(Cameras.ActiveCamera);

            Debug.Draw(Cameras.ActiveCamera);
        }//End of Method

        public void LoadScene(Scene scene)
        {
            if (scene!= null)
            {
                ActiveScene = scene;
                ActiveScene.Initialize();

                foreach (GameObject3D t in ActiveScene.Objects)
                {
                    t.LoadContent(Game.Content);
                }
            }
        }//End of Method

        private void DrawReadyToStartScreen()
        {
            // Clear screen
            GraphicsDevice.Clear(Color.AliceBlue);

            // Draw text for intro splash screen
            _batch.Begin();
            
            // Draw instructions
            string text = Network.NetworkSession.Host.Gamertag + " is the HOST";
            _batch.DrawString(_font, text, new Vector2(GraphicsDevice.Viewport.Width / 2 - (_font.MeasureString(text).X /2), 50),
                Color.SaddleBrown);
            
            // If both gamers are there, tell gamers to press space bar or Start to begin
            if (Network.NetworkSession.AllGamers.Count == 2)
            {
                text = "(Game is ready. Press Spacebar or Start button to begin)";
                _batch.DrawString(_font, text,
                    new Vector2((GraphicsDevice.Viewport.Width / 2) - (_font.MeasureString(text).X / 2), GraphicsDevice.Viewport.Height - 100),
                    Color.SaddleBrown);
            }
            // If only one player is there, tell gamer you're waiting for players
            else
            {
                text = "(Waiting for players)";
                _batch.DrawString(_font, text,
                    new Vector2((GraphicsDevice.Viewport.Width / 2) - (_font.MeasureString(text).X / 2), GraphicsDevice.Viewport.Height - 100),
                    Color.SaddleBrown);
            }

            // Loop through all gamers and get their gamertags,
            // then draw list of all gamers currently in the game
            text = "\n\nCurrent Player(s):";
            float txtWidth = _font.MeasureString(text).X / 2;
            foreach (Gamer gamer in Network.NetworkSession.AllGamers)
            {
                text += "\n" + gamer.Gamertag;
            }
            _batch.DrawString(_font, text,
                new Vector2(GraphicsDevice.Viewport.Width / 2 - txtWidth, 100),
                Color.SaddleBrown);

            _batch.End();
        }

        private void ScreenStates(GameTime gameTime)
        {
            switch (GameState)
            {
                case State.MainMenu:
                    if (_btnStandart.IsClicked)
                       GameState = State.NonNetworkGame;
                    else if (_btnNetwork.IsClicked)
                        GameState = State.NetworkGame;
                    else if (_btnExit.IsClicked)
                        GameState = State.ExitGame;
                    else if(_btnTimedGame.IsClicked)
                        GameState = State.TimedNetworkGame;
                    _btnStandart.Update(this);
                    _btnNetwork.Update(this);
                    _btnTimedGame.Update(this);
                    _btnExit.Update(this);
                    break;
                case State.NetworkGame:
                    _btnStandardPieces.Update(this);
                    _btnAnimatedPieces.Update(this);
                    _btnBack.Update(this);
                    if (_btnBack.IsClicked)
                        GameState = State.MainMenu;
                    if (_btnStandardPieces.IsClicked)
                    {
                        Network = new NetworkEngine(Game,this);
                        GameState = State.PlayingNetworked;
                    }
                    if (_btnAnimatedPieces.IsClicked)
                    {
                        Network = new NetworkEngine(Game, this);
                        GameState = State.PlayingNetworked;
                    }
                    break;
                case State.NonNetworkGame:
                    _btnStandardPieces.Update(this);
                    _btnAnimatedPieces.Update(this);
                    _btnBack.Update(this);
                    if(_btnBack.IsClicked)
                        GameState = State.MainMenu;
                    if (_btnStandardPieces.IsClicked)
                    {
                        LoadScene(new StandardLevel(this, false));
                        GameState = State.PlayingNormal;
                    }
                    if (_btnAnimatedPieces.IsClicked)
                    {
                        LoadScene(new StandardLevel(this, false));
                        GameState = State.PlayingNormal;
                    }
                    break;
                 case State.TimedNetworkGame:
                    _btnStandardPieces.Update(this);
                    _btnAnimatedPieces.Update(this);
                    _btnBack.Update(this);
                    if(_btnBack.IsClicked)
                        GameState = State.MainMenu;
                    if (_btnStandardPieces.IsClicked)
                    {
                        Network = new NetworkEngine(Game, this);
                        GameState = State.PlayingTimedNetworkGame;
                    }
                    if (_btnAnimatedPieces.IsClicked)
                    {
                        Network = new NetworkEngine(Game, this);
                        GameState = State.PlayingTimedNetworkGame;
                    }
                    break;
                case State.PlayingTimedNetworkGame:
                    Network.Update(gameTime);
                    break;
                case State.PlayingNormal:
                    if(_btnNewGame.IsClicked)
                        LoadScene(new StandardLevel(this, false));
                    _btnNewGame.Update(this);
                    _btnUndoMove.Update(this);
                    break;
                case State.PlayingNetworked:
                    Network.Update(gameTime);
                    break;
                    case State.GameOverWhiteWins:
                    ActiveScene = null;
                    //draw
                    break;
                    case State.GameOverBlackWins:
                    ActiveScene = null;
                    //draw
                    break;
                    case State.GameOverDraw:
                    ActiveScene = null;
                    //draw
                    break;
            }
        }

        private void ScreenStatesDraw()
        {
            switch (GameState)
            {
                case State.MainMenu:
                    _btnStandart.Draw(_batch);
                    _btnNetwork.Draw(_batch);
                    _btnTimedGame.Draw(_batch);
                    _btnExit.Draw(_batch);
                    break;
                case State.NetworkGame:
                    _btnAnimatedPieces.Draw(_batch);
                    _btnStandardPieces.Draw(_batch);
                    _btnBack.Draw(_batch);
                    break;
                case State.NonNetworkGame:
                    _btnAnimatedPieces.Draw(_batch);
                    _btnStandardPieces.Draw(_batch);
                    _btnBack.Draw(_batch);
                    break;
                case State.PlayingNormal:
                    _btnNewGame.Draw(_batch);
                    _btnUndoMove.Draw(_batch);
                    break;
                case State.PlayingNetworked:
                     break;
                    case State.TimedNetworkGame:
                    _btnAnimatedPieces.Draw(_batch);
                    _btnStandardPieces.Draw(_batch);
                    _btnBack.Draw(_batch);
                    break;

            }
         }

        public void LoadRuntimeObject(GameObject3D gameObject)
        {
            if (gameObject != null)
            {
                gameObject.LoadContent(Game.Content);

                ActiveScene.Objects.Add(gameObject);

            }
        }
    }//End of Class
}
