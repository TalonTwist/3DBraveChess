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
            PlayingNormal,
            PlayingNetworked,
            ExitGame
        }

        public State GameState;

        Button _btnStandart, _btnNetwork, _btnExit, _btnNewGame,_btnUndoMove;
        Texture2D _background;

        SpriteBatch _batch;
        SpriteFont _font;

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
            FpsCounter = new FrameRateCounter(game, new Vector2(10, 10));
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

            _background = Game.Content.Load<Texture2D>("Buttons\\Background");

            _btnStandart = new Button(Game.Content.Load<Texture2D>("Buttons\\StandardButton"),200);

            _btnNetwork = new Button(Game.Content.Load<Texture2D>("Buttons\\NetworkButton"),250);

            _btnExit = new Button(Game.Content.Load<Texture2D>("Buttons\\ExitGameButton"),300);

            _btnNewGame = new Button(Game.Content.Load<Texture2D>("Buttons\\NewGameButton"),
                    new Vector2(20,100));
            _btnUndoMove = new Button(Game.Content.Load<Texture2D>("Buttons\\UndoMoveButton"),
                    new Vector2(20,150));
            

            Debug.LoadContent(Game.Content);
            base.LoadContent();
        }//End of Override

        public override void Update(GameTime gameTime)
        {
            if (ActiveScene != null)
                ActiveScene.Update(gameTime);

            ScreenStates(gameTime);

            base.Update(gameTime);
        }//End of Override

        public override void Draw(GameTime gameTime)
        {
            if (GameState == State.MainMenu)
            {
                _batch.Begin();
                _batch.Draw(_background, GraphicsDevice.Viewport.Bounds, Color.White);
                _batch.End();
            }
            ScreenStatesDraw();

            Draw3D();

            if (GameState == State.PlayingNetworked)
                Draw2D();
          
            base.Draw(gameTime);
        }

        public void Draw2D() 
        {
            switch (Network.CurrentGameState)
            {
                case NetworkState.SignIn:
                case NetworkState.FindSession:
                case NetworkState.CreateSession:
                    GraphicsDevice.Clear(Color.DarkGray);
                    break;
                case NetworkState.Start:
                    DrawReadyToStartScreen();
                    break;
            }
        }

        public void Draw3D()
        {
            if (Cameras.ActiveCamera != null)
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
                    _btnStandart.Update(this);
                    _btnNetwork.Update(this);
                    _btnExit.Update(this);
                    break;
                case State.NetworkGame:
                    Network = new NetworkEngine(Game, this);
                    GameState = State.PlayingNetworked;
                    break;
                case State.NonNetworkGame:
                    LoadScene(new StandardLevel(this));
                    GameState = State.PlayingNormal;
                    break;
                case State.PlayingNormal:
                    if(_btnNewGame.IsClicked)
                        LoadScene(new StandardLevel(this));
                    _btnNewGame.Update(this);
                    _btnUndoMove.Update(this);
                    break;
                case State.PlayingNetworked:
                    Network.Update(gameTime);
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
                    _btnExit.Draw(_batch);
                    break;
                case State.NetworkGame:
                    break;
                case State.NonNetworkGame:
                    break;
                case State.PlayingNormal:
                    _btnNewGame.Draw(_batch);
                    _btnUndoMove.Draw(_batch);
                    break;
                case State.PlayingNetworked:
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
