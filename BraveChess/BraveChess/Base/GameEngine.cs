using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BraveChess.Engines;
using BraveChess.Scenes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace BraveChess.Base
{
    public class GameEngine : DrawableGameComponent
    {
        SpriteBatch batch;
        SpriteFont font;
        GameWindow Window;

        public InputEngine Input { get; set; }
        public CameraEngine Cameras { get; set; }
        public AudioEngine Audio { get; set; }
        public DebugEngine Debug { get; set; }
        public FrameRateCounter FPSCounter { get; set; }
        public Networking GameNetwork { get; set; }

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
            GameNetwork = new Networking(_game, this);
        }//End of Constructor

        public override void Initialize()
        {
            Debug.Initialize();
            
            base.Initialize();
        }//End of Override

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("Fonts\\debug");
            Debug.LoadContent(Game.Content);
            base.LoadContent();
        }//End of Override

        public override void Update(GameTime gameTime)
        {
            if (ActiveScene != null)
                ActiveScene.Update(gameTime);
            GameNetwork.Update(gameTime);
            base.Update(gameTime);
        }//End of Override

        public override void Draw(GameTime gameTime)
        {
            Draw3D();
            Draw2D();

            base.Draw(gameTime);
        }

        public void Draw2D() 
        {
            switch(GameNetwork.CurrentGameState)
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
            string text = GameNetwork.networkSession.Host.Gamertag + " is the HOST";
            batch.DrawString(font, text, new Vector2((GraphicsDevice.Viewport.Width / 2) - (font.MeasureString(text).X /2), 50),
                Color.SaddleBrown);

            // If both gamers are there, tell gamers to press space bar or Start to begin
            if (GameNetwork.networkSession.AllGamers.Count == 2)
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
            foreach (Gamer gamer in GameNetwork.networkSession.AllGamers)
            {
                text += "\n" + gamer.Gamertag;
            }
            batch.DrawString(font, text,
                new Vector2((GraphicsDevice.Viewport.Width / 2) - _txtWidth, 100),
                Color.SaddleBrown);

            batch.End();
        }

        
    }//End of Class
}
