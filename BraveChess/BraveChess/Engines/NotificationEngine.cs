using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BraveChess.Engines
{
    public class NotificationEngine : DrawableGameComponent
    {

        SpriteBatch _batch;
        SpriteFont _sfont;

        static List<Notification> _notifications = new List<Notification>();
        static Queue<Notification> _queuedNotification = new Queue<Notification>();

        private static int _maxNotifications;

        public NotificationEngine(Game game, int maxNotifications)
            :base(game)
        {
            _maxNotifications = maxNotifications;
           
            Game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            _batch = new SpriteBatch(GraphicsDevice);
            _sfont = Game.Content.Load<SpriteFont>("Fonts\\debug");
            base.LoadContent();
        }

        public static void AddNotification(Notification _newNotification)
        {
            if (_newNotification != null)
            {
                if(_notifications.Count >= _maxNotifications)
                _queuedNotification.Enqueue(_newNotification);
                else
                _notifications.Add(_newNotification);
            }
        }

        private void UpdateNotifications(float elapsedTime)
        {
            for (int i = 0; i < _notifications.Count; i++)
            {
                if (_notifications[i].LifeTime != -9)
                {
                    _notifications[i].LifeTime -= elapsedTime;

                    if (_notifications[i].LifeTime <= 0)
                    {
                        _notifications.RemoveAt(i);

                        if (_queuedNotification.Count > 0 && _notifications.Count < _maxNotifications)
                        {
                            AddNotification(_queuedNotification.Dequeue());
                        }
                    }
                }
            }
        }//end updateNotifications method

        public override void Update(GameTime gameTime)
        {
            UpdateNotifications(gameTime.ElapsedGameTime.Milliseconds);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawNotifications();
            base.Draw(gameTime);
        }

        public void DrawNotifications()
        {
            _batch.Begin();

            for (int i = 0; i < _notifications.Count; i++)
            {
                _batch.DrawString(_sfont, _notifications[i].Message,
                    new Vector2((GraphicsDevice.Viewport.Width / 2 - MeasureWidth(_notifications[i].Message) / 2),
                        22 * i), _notifications[i].Color);
            }

            _batch.End();
        }

        private float MeasureWidth(string text)
        {
            return _sfont.MeasureString(text).X;
        }
    }
}
