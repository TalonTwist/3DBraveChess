using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BraveChess.Engines;
using BraveChess.Base;
using BraveChess.Helpers;

namespace BraveChess.Objects
{
    public enum Buttontype
    {
        Menu,
        Ui,
        Promotion
    }

    public class Button
    {
        public Buttontype Type { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size{ get; set; }

        Rectangle _rectangle;
        Color _color = Color.Gray;
        public bool IsClicked;
        public bool IsHover = false;
        bool _soundPlayed;


        public Button(Buttontype type, Texture2D texture, float f)
        {
            Type = type;
            Texture = texture;
            SetSize();
            Position = Type == Buttontype.Promotion ? new Vector2(f, (Helper.GraphicsDevice.Viewport.Height / 2) - (Texture.Height/2)) : new Vector2(((Helper.GraphicsDevice.Viewport.Width / 2) - (Texture.Width / 2)), f);
        }

        public Button(Buttontype type, Texture2D texture, Vector2 pos)
        {
            Type = Type;
            Texture = texture;
            Position = pos;
            
            SetSize();
        }

        public void SetSize()
        {
            switch (Type)
            {
                    case Buttontype.Menu:
                    Size = new Vector2(Helper.GraphicsDevice.Viewport.Width / 6, Helper.GraphicsDevice.Viewport.Height / 15);
                    break;
                    case Buttontype.Promotion:
                    Size = new Vector2(Helper.GraphicsDevice.Viewport.Width / 14, Helper.GraphicsDevice.Viewport.Height / 10);
                    break;
                    case Buttontype.Ui:
                    Size = new Vector2(Helper.GraphicsDevice.Viewport.Width / 8, Helper.GraphicsDevice.Viewport.Height / 18);
                    break;
            }
        }

        public void Update(GameEngine e)
        {
            _rectangle = new Rectangle((int)Position.X,(int) Position.Y,(int) Size.X,(int) Size.Y);
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
            batch.Draw(Texture, _rectangle, _color);
            batch.End();
        }

    }
}
