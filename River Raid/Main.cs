using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace River_Ride___MG
{
    public class Main : Game
    {
        #region Utils
        private GraphicsDeviceManager _graphics;
        
        private SpriteBatch _spriteBatch;
        private Random random = new Random();
        #endregion

        #region Textures
        private List<Texture2D> BG_Textures = new List<Texture2D>();
        private Vector2 BG_Texture_Position = new Vector2(0f, 0f);
        private Texture2D Shadow;
        private Texture2D Plane;
        private Vector2 Plane_Position = new Vector2(500f, 600f);
        #endregion

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = Config.PrefferedHeight;
            _graphics.PreferredBackBufferWidth = Config.PrefferedWidth;
            _graphics.ApplyChanges();
            Window.Title = Config.TitleGame;
            base.Initialize();
        }

        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int i = 0; i < Config.BG_count; i++) {
                BG_Textures.Add(Content.Load<Texture2D>($"BG_{i + 1}"));
            }
            Shadow = Content.Load<Texture2D>("Shadow");
            Plane = Content.Load<Texture2D>("Plane");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Dodać ruszanie samolotem

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            _spriteBatch.Draw(BG_Textures[2], BG_Texture_Position, Color.White);
            _spriteBatch.Draw(Shadow, BG_Texture_Position, Color.White);
            _spriteBatch.Draw(Plane, Plane_Position, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
