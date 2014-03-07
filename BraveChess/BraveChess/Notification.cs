using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BraveChess
{
    public class Notification 
    {
        public float LifeTime { get; set; }
        public string Message { get; set; }
        public Color Color { get; set; }


        public Notification(string message, float lifetime)
        {
            Message = message;
            LifeTime = lifetime;
            Color = Color.Black;
        }

        public Notification(string message, float lifetime, Color color)
        {
            Message = message;
            LifeTime = lifetime;
            Color = color;
        }
    }
}
