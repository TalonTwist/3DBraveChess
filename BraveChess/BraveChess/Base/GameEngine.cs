using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using BraveChess.Engines;
using BraveChess.Scenes;
using BraveChess.Screens;
using BraveChess.Objects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Ruminate.Utils;
<<<<<<< HEAD
=======
using Microsoft.Xna.Framework.Media;

>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08

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

<<<<<<< HEAD
        public Dictionary<State, Screen> Screens = new Dictionary<State, Screen>();
        public State CurrentState;
        public Screen CurrentScreen;
        public State GameState;
        
=======
        public Dictionary<State, Screen> _screens = new Dictionary<State, Screen>();
        public State _currentState;
        public Screen _currentScreen;
        public State _currentScreenState = State.MainMenu;
        
        Button btnStandart,btnNetworked,btnExit;
       
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08
        public SpriteFont GreySpriteFont;
        public Texture2D GreyImageMap;
        public string GreyMap;

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

        public void StateChange(State newState)
        {
            if (Screens.ContainsKey(newState))
            {
                CurrentScreen = Screens[newState];

                if (CurrentScreen != null)
                    CurrentScreen.Init(this);
                
            }
            else
                CurrentScreen = null;
        }

        public override void Initialize()
        {
            Debug.Initialize();

<<<<<<< HEAD
            Screens.Add(State.MainMenu, new MainMenu());
=======
            #region Sound
            //loading songs//efects
            Audio.LoadSong("BackgroundSong");
            MediaPlayer.IsRepeating = true;
            Audio.LoadEffect("MovePiece");
            Audio.LoadEffect("CapturePiece");
            #endregion

            _screens.Add(State.MainMenu, new MainMenu());
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08

            base.Initialize();
        }//End of Override

        protected override void LoadContent()
        {
<<<<<<< HEAD
            _batch = new SpriteBatch(GraphicsDevice);
            _font = Game.Content.Load<SpriteFont>("Fonts\\debug");
=======
            batch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("Fonts\\debug");
            Audio.LoadEffect("MenuHover");

            btnStandart = new Button(Game.Content.Load<Texture2D>("Buttons\\StandartGameButton"),
                new Vector2(50,150), GraphicsDevice);
            btnNetworked = new Button(Game.Content.Load<Texture2D>("Buttons\\NetworkButton"), 
                new Vector2(50, 200), GraphicsDevice);
            btnExit = new Button(Game.Content.Load<Texture2D>("Buttons\\ExitGameButton"), 
                new Vector2(50, 250), GraphicsDevice);
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08

            //GreyImageMap = Game.Content.Load<Texture2D>(@"Skin\ImageMap");
            //GreyMap = File.OpenText(@"Content\Skin\Map.txt").ReadToEnd();
            //GreySpriteFont = Game.Content.Load<SpriteFont>(@"Skin\Texture");

            //DebugUtils.Init(GraphicsDevice, GreySpriteFont);

            //StateChange(State.MainMenu);
            //LoadScene(new StandardLevel(this));

            Debug.LoadContent(Game.Content);
            base.LoadContent();
        }//End of Override

        public override void Update(GameTime gameTime)
        {
            if (ActiveScene != null)
                ActiveScene.Update(gameTime);

            if (CurrentScreen != null)
                CurrentScreen.Update();

            ScreenStates(gameTime);

            base.Update(gameTime);
        }//End of Override

        public override void Draw(GameTime gameTime)
        {
<<<<<<< HEAD
            if (CurrentScreen != null)
                CurrentScreen.Draw();

            Draw3D();

            if (GameState == State.PlayingNetworked)
=======
            
            ScreenStatesDraw();
            

            if (_currentScreen != null)
                _currentScreen.Draw();

            Draw3D();

            if (_currentScreenState == State.PlayingNetworked)
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08
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
            _batch.DrawString(_font, text, new Vector2((GraphicsDevice.Viewport.Width / 2) - (_font.MeasureString(text).X /2), 50),
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
                new Vector2((GraphicsDevice.Viewport.Width / 2) - txtWidth, 100),
                Color.SaddleBrown);

            _batch.End();
        }

        private void ScreenStates(GameTime gameTime)
        {
<<<<<<< HEAD
            switch (GameState)
=======
            switch (_currentScreenState)
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08
            {
                case State.MainMenu:
                    if (btnStandart.isClicked)
                        _currentScreenState = State.NonNetworkGame;
                    else if (btnNetworked.isClicked)
                        _currentScreenState = State.NetworkGame;
                    else if (btnExit.isClicked)
                        _currentScreenState = State.ExitGame;
                    btnStandart.Update(this);
                    btnNetworked.Update(this);
                    btnExit.Update(this);
                    break;
                case State.NetworkGame:
<<<<<<< HEAD
                    Network = new NetworkEngine(Game, this);
                    GameState = State.PlayingNetworked;
=======
                    Network = new NetworkEngine(this.Game, this);
                    _currentScreenState = State.PlayingNetworked;
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08
                    break;
                case State.NonNetworkGame:
                    StateChange(State.NonNetworkGame);
                    LoadScene(new StandardLevel(this));
<<<<<<< HEAD
                    GameState = State.PlayingNormal;
=======
                    _currentScreenState = State.PlayingNormal;
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08
                    break;
                case State.PlayingNormal:
                    break;
                case State.PlayingNetworked:
                    Network.Update(gameTime);
                    break;
                case State.ExitGame:
                    break;
            }
        }

        private void ScreenStatesDraw()
        {
            switch (_currentScreenState)
            {
                case State.MainMenu:
                    btnStandart.Draw(batch);
                    btnNetworked.Draw(batch);
                    btnExit.Draw(batch);
                    break;
                case State.NetworkGame:
                    break;
                case State.NonNetworkGame:
                    break;
                case State.PlayingNormal:
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
