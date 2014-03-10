using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BraveChess.Engines
{
    public class FrameRateCounter : DrawableGameComponent
    {
        readonly Vector2 _position;

        int _frameRate;
        int _frameCounter;
        TimeSpan _elapsedTime = TimeSpan.Zero;

        SpriteBatch _batch;
        SpriteFont _sfont;

        public int FrameRate { get { return _frameRate; } }

        public FrameRateCounter(Game game, Vector2 position)
            : base(game)
        {
            _position = position;
            game.Components.Add(this);
        }//End of Method

        public override void Initialize()
        {
            base.Initialize();
        }//End of Override Method

        protected override void LoadContent()
        {
            _batch = new SpriteBatch(GraphicsDevice);
            _sfont = Game.Content.Load<SpriteFont>("Fonts\\debug");

            base.LoadContent();
        }//End of Override Method

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }

            base.Update(gameTime);
        }//End of Override Method

        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            _batch.Begin();
            if (_frameRate < 25)
                _batch.DrawString(_sfont, "FPS: " + _frameRate, _position, Color.Red);
            else
                _batch.DrawString(_sfont, "FPS :" + _frameRate, _position, Color.LawnGreen);
           // _batch.DrawString(_sfont, "Press:\n1-To turn on Cam1\n2-To turn on Cam2\nN-Pause song\nM-Resume song", new Vector2(10,40), Color.AntiqueWhite);
            _batch.End();

            base.Draw(gameTime);
        }//End of Override Method

    }//end class
}
