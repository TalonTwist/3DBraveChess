using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BraveChess.Engines;

namespace BraveChess.Base
{
    public class GameEngine : DrawableGameComponent
    {
        public InputEngine Input { get; set; }
        public CameraEngine Cameras { get; set; }
        public AudioEngine Audio { get; set; }
        public DebugEngine Debug { get; set; }
        public FrameRateCounter FPSCounter { get; set; }

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
        }//End of Constructor

        public override void Initialize()
        {
            Debug.Initialize();

            base.Initialize();
        }//End of Override

        protected override void LoadContent()
        {
            Debug.LoadContent(Game.Content);

            base.LoadContent();
        }//End of Override

        public override void Update(GameTime gameTime)
        {
            if (ActiveScene != null)
                ActiveScene.Update(gameTime);

            base.Update(gameTime);
        }//End of Override

        public override void Draw(GameTime gameTime)
        {
            Draw3D();
            Draw2D();

            base.Draw(gameTime);
        }

        public void Draw2D() { }

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

    }//End of Class
}
