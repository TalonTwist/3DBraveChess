using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using BraveChess.Engines;
using BraveChess.Scenes;
using BraveChess.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Ruminate.GUI;
using Ruminate.Utils;
using BraveChess.Engines;

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
            PlayingNetworked
        }

        public Dictionary<State, Screen> _screens = new Dictionary<State, Screen>();
        public State _currentState;
        public Screen _currentScreen;
        public State _state;

        public SpriteFont GreySpriteFont;
        public Texture2D GreyImageMap;
        public string GreyMap;

        SpriteBatch batch;
        SpriteFont font;

        public InputEngine Input { get; set; }
        public CameraEngine Cameras { get; set; }
        public AudioEngine Audio { get; set; }
        public DebugEngine Debug { get; set; }
        public FrameRateCounter FPSCounter { get; set; }
        public NotificationEngine NoteEngine;
        public NetworkEngine Network { get; set; }

        public Scene ActiveScene { get; set; }

        public GameEngine(Game _game)
            : base(_game)
        {
            _game.Components.Add(this);

            Input = new InputEngine(_game);
            Cameras = new CameraEngine(_game);
            Audio = new AudioEngine(_game);
            FPSCounter = new FrameRateCounter(_game, new Vector2(10, 10));
            Debug = new DebugEngine();
            NoteEngine = new NotificationEngine(_game, 3);
          //  GameNetwork = new Networking(_game, this);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (_currentScreen != null)
                _currentScreen.OnResize();
        }

        public void StateChange(State _newState)
        {
            if (_screens.ContainsKey(_newState))
            {
                _currentScreen = _screens[_newState];

                if (_currentScreen != null)
                    _currentScreen.Init(this);
            }
            else
                _currentScreen = null;
        }

        public override void Initialize()
        {
            Debug.Initialize();

            _screens.Add(State.MainMenu, new MainMenu());

            base.Initialize();
        }//End of Override

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("Fonts\\debug");

            GreyImageMap = Game.Content.Load<Texture2D>(@"Skin\ImageMap");
            GreyMap = File.OpenText(@"Content\Skin\Map.txt").ReadToEnd();
            GreySpriteFont = Game.Content.Load<SpriteFont>(@"Skin\Texture");

            DebugUtils.Init(GraphicsDevice, GreySpriteFont);

            StateChange(State.MainMenu);
            //LoadScene(new StandardLevel(this));

            Debug.LoadContent(Game.Content);
            base.LoadContent();
        }//End of Override

        public override void Update(GameTime gameTime)
        {
            if (ActiveScene != null)
                ActiveScene.Update(gameTime);

            if (_currentScreen != null)
                _currentScreen.Update();

            ScreenStates(gameTime);

            base.Update(gameTime);
        }//End of Override

        public override void Draw(GameTime gameTime)
        {
            if (_currentScreen != null)
                _currentScreen.Draw();

            Draw3D();

            if (_state == State.PlayingNetworked)
                Draw2D();
          
            base.Draw(gameTime);
        }

        public void Draw2D() 
        {
            switch (Network.CurrentGameState)
            {
                case GameState.SignIn:
                case GameState.FindSession:
                case GameState.CreateSession:
                    GraphicsDevice.Clear(Color.DarkGray);
                    break;
                case GameState.Start:
                    DrawReadyToStartScreen();
                    break;
            }
        }

        public void Draw3D()
        {
            if (Cameras.ActiveCamera != null)
                for (int i = 0; i < ActiveScene.Objects.Count; i++)
                    ActiveScene.Objects[i].Draw(Cameras.ActiveCamera);

            Debug.Draw(Cameras.ActiveCamera);
        }//End of Method

        public void LoadScene(Scene _scene)
        {
            if (_scene!= null)
            {
                ActiveScene = _scene;
                ActiveScene.Initialize();

                for (int i = 0; i < ActiveScene.Objects.Count; i++)
                {
                    ActiveScene.Objects[i].LoadContent(Game.Content);
                }
            }
        }//End of Method

        private void DrawReadyToStartScreen()
        {
            // Clear screen
            GraphicsDevice.Clear(Color.AliceBlue);

            // Draw text for intro splash screen
            batch.Begin();
            
            // Draw instructions
            string text = Network.networkSession.Host.Gamertag + " is the HOST";
            batch.DrawString(font, text, new Vector2((GraphicsDevice.Viewport.Width / 2) - (font.MeasureString(text).X /2), 50),
                Color.SaddleBrown);

            // If both gamers are there, tell gamers to press space bar or Start to begin
            if (Network.networkSession.AllGamers.Count == 2)
            {
                text = "(Game is ready. Press Spacebar or Start button to begin)";
                batch.DrawString(font, text,
                    new Vector2((GraphicsDevice.Viewport.Width / 2) - (font.MeasureString(text).X / 2), GraphicsDevice.Viewport.Height - 100),
                    Color.SaddleBrown);
            }
            // If only one player is there, tell gamer you're waiting for players
            else
            {
                text = "(Waiting for players)";
                batch.DrawString(font, text,
                    new Vector2((GraphicsDevice.Viewport.Width / 2) - (font.MeasureString(text).X / 2), GraphicsDevice.Viewport.Height - 100),
                    Color.SaddleBrown);
            }

            // Loop through all gamers and get their gamertags,
            // then draw list of all gamers currently in the game
            text = "\n\nCurrent Player(s):";
            float _txtWidth = font.MeasureString(text).X / 2;
            foreach (Gamer gamer in Network.networkSession.AllGamers)
            {
                text += "\n" + gamer.Gamertag;
            }
            batch.DrawString(font, text,
                new Vector2((GraphicsDevice.Viewport.Width / 2) - _txtWidth, 100),
                Color.SaddleBrown);

            batch.End();
        }

        private void ScreenStates(GameTime gameTime)
        {
            switch (_state)
            {
                case State.MainMenu:
                    break;
                case State.NetworkGame:
                    Network = new NetworkEngine(this.Game, this);
                    _state = State.PlayingNetworked;
                    break;
                case State.NonNetworkGame:
                    StateChange(State.NonNetworkGame);
                    LoadScene(new StandardLevel(this));
                    _state = State.PlayingNormal;
                    break;
                case State.PlayingNormal:
                    break;
                case State.PlayingNetworked:
                    Network.Update(gameTime);
                    //Draw2D();
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
