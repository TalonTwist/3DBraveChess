using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BraveChess.Engines
{
    public class FrameRateCounter : DrawableGameComponent
    {
        Vector2 _position;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        SpriteBatch _batch;
        SpriteFont _sfont;

        public int FrameRate { get { return frameRate; } }

        public FrameRateCounter(Game _game, Vector2 position)
            : base(_game)
        {
            _position = position;
            _game.Components.Add(this);
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
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            base.Update(gameTime);
        }//End of Override Method

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            _batch.Begin();
            if (frameRate < 25)
                _batch.DrawString(_sfont, "FPS: " + frameRate, _position, Color.Red);
            else
                _batch.DrawString(_sfont, "FPS :" + frameRate, _position, Color.LawnGreen);
           // _batch.DrawString(_sfont, "Press:\n1-To turn on Cam1\n2-To turn on Cam2\nN-Pause song\nM-Resume song", new Vector2(10,40), Color.AntiqueWhite);
            _batch.End();

            base.Draw(gameTime);
        }//End of Override Method

    }//end class
}
