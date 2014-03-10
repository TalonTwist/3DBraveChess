using System;
using BraveChess.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BraveChess.Engines;
using BraveChess.Base;

namespace BraveChess
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        SpriteBatch _spriteBatch;
        readonly GameEngine _gameEngine;

        public Game1()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 980,
                PreferredBackBufferHeight = 520,
                PreferMultiSampling = true
            };
            graphics.ApplyChanges();


            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            IsMouseVisible = true;

            _gameEngine = new GameEngine(this);

            Content.RootDirectory = "Content";  
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (_gameEngine.CurrentScreen != null)
                _gameEngine.CurrentScreen.OnResize();
        }

        

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Helper.GraphicsDevice = GraphicsDevice;
            Components.Add(new GamerServicesComponent(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

           // gameEngine.LoadScene(new StartMenu(spriteBatch)); //use ruminate.dll for menu
           // gameEngine.LoadScene(new Level0(gameEngine));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            if (InputEngine.IsKeyPressed(Keys.Escape) || InputEngine.IsButtonPressed(Buttons.Back))
                Exit();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
