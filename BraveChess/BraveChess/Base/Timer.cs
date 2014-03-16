using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BraveChess.Base
{
    public class Timer
    {
        public int StartTimer { get; set; }
        public string DisplayTimer { get; set; }
        public Boolean IsComplete { get; set; }

        public Timer(int startTime)
        {
            StartTimer =startTime ;
            DisplayTimer = StartTimer.ToString();
            IsComplete = false;
        }

        public void CheckTimer(GameTime gameTime)
        {
            if (!IsComplete)
            {
                StartTimer -= gameTime.ElapsedGameTime.Seconds;
                DisplayTimer = StartTimer.ToString();

                if (StartTimer <= 0)
                {
                    IsComplete = true;
                }
            }

        }

        public void DrawTimer(SpriteBatch batch,SpriteFont font)
        {
            batch.Begin();
            batch.DrawString(font,DisplayTimer,new Vector2(300,20),Color.Black);
            batch.End();
        }

    }
}
