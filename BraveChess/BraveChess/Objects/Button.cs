using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BraveChess.Engines;

namespace BraveChess.Objects
{
    public class Button
    {
        Texture2D _texture;
        Vector2 _position;
        Rectangle _rectangle;
        Color _color = Color.Gray;
        public Vector2 size;
        public bool isClicked;

        public Button(Texture2D texture,Vector2 newPos,GraphicsDevice graphics)
        {
            _texture = texture;
            _position = newPos;
            size = new Vector2(graphics.Viewport.Width / 8, graphics.Viewport.Height / 15);
        }

        public void Update()
        {
            _rectangle = new Rectangle((int)_position.X,(int) _position.Y,(int) size.X,(int) size.Y);
            Rectangle mouseRec = new Rectangle(InputEngine.CurrentMouseState.X, InputEngine.CurrentMouseState.Y, 1, 1);

            if(mouseRec.Intersects(_rectangle))
            {
                _color = new Color(255, 255, 255, 255);
                
                if (InputEngine.IsMouseLeftClick()) isClicked = true;
            }
            else if (!mouseRec.Intersects(_rectangle))
            {
                _color = Color.Gray;
                isClicked = false;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(_texture, _rectangle, _color);
            batch.End();
        }

    }
}
