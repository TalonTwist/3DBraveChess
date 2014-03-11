using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BraveChess.Engines;
using BraveChess.Base;
using BraveChess.Helpers;

namespace BraveChess.Objects
{
    public class Button
    {
        readonly Texture2D _texture;
        Vector2 _position;
        Rectangle _rectangle;
        Color _color = Color.Gray;
        float _viewportX;
        public Vector2 Size;
        public bool IsClicked;
        public bool IsHover = false;
        bool _soundPlayed;
        

        public Button(Texture2D texture,float y)
        {
            _texture = texture;
            _position = new Vector2(((Helper.GraphicsDevice.Viewport.Width / 2)-(texture.Width/2)), y);
            Size = new Vector2(Helper.GraphicsDevice.Viewport.Width / 6, Helper.GraphicsDevice.Viewport.Height / 15);
        }

        public void Update(GameEngine e)
        {
            _rectangle = new Rectangle((int)_position.X,(int) _position.Y,(int) Size.X,(int) Size.Y);
            Rectangle mouseRec = new Rectangle(InputEngine.CurrentMouseState.X, InputEngine.CurrentMouseState.Y, 1, 1);

            if(mouseRec.Intersects(_rectangle))
            {
                IsHover = true;
                _color = new Color(255, 255, 255, 255);

                if(!_soundPlayed)
                {
                    e.Audio.PlayEffect("MenuHover");
                    _soundPlayed = true;
                }

                if (InputEngine.IsMouseLeftClick()) 
                    IsClicked = true;
            }
            else if (!mouseRec.Intersects(_rectangle))
            {
                _color = Color.Gray;
                IsClicked = false;
                _soundPlayed = false;
                IsHover = false;
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
