using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Raid;

namespace River_Ride___MG
{
    public class Main : Game
    {
        #region Utils
        private GraphicsDeviceManager _graphics;
        
        private SpriteBatch _spriteBatch;
        #endregion

        #region Player
        private Texture2D Plane;
        private Vector2 Plane_Position = new Vector2(500f, 500f);

        float time, delay = 100f;
        int frame;
        Rectangle PlaneAnimation;

        bool CanGoLeft = true, CanGoRight = true;
        #endregion

        #region Textures
        private List<Background> Backgrounds = new List<Background>();
        private Texture2D Shadow;

        private Texture2D UI;
        private FuelPtr Fuel;
        private Vector2 FuelPosition = new Vector2(320f, 689f);
        bool isExploding = false, isExploded = false;
        private Texture2D ExplosionEffect;
        private Rectangle ExplosionAnimation;
        #endregion

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = Config.ContentRootDirectory;
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
                Backgrounds.Add(new Background(Content.Load<Texture2D>($"BG_{i+1}"), i));
            }
            Shadow = Content.Load<Texture2D>("Shadow");
            Plane = Content.Load<Texture2D>("Plane");
            ExplosionEffect = Content.Load<Texture2D>("ExplosionEffect");
            UI = Content.Load<Texture2D>("UI");
            Fuel = new FuelPtr(Content.Load<Texture2D>("Fuel_Level"), Content.Load<Texture2D>("Fuel_UI"), 64, 320, FuelPosition);
            Fuel.OnFuelEmpty += ExplodePlane;

            Backgrounds[0].BG_position = new Vector2(Backgrounds[0].BG_position.X, -Backgrounds[0].BG_texture.Height);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Movement
            KeyboardState InputKey = Keyboard.GetState();
            if ((InputKey.IsKeyDown(Keys.A) || InputKey.IsKeyDown(Keys.Left)) && CanGoLeft) {
                Plane_Position.X -= Config.PlaneMovementSpeed;
            } else if ((InputKey.IsKeyDown(Keys.D) || InputKey.IsKeyDown(Keys.Right)) && CanGoRight) {
                Plane_Position.X += Config.PlaneMovementSpeed;
            }

            if (Plane_Position.X <= 100)
                CanGoLeft = false;
            else 
                CanGoLeft = true;

            if (Plane_Position.X >= 830)
                CanGoRight = false;
            else
                CanGoRight = true;
            // Movement

            // Animation

            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (time >= delay) {
                if (isExploding && frame >= 3)
                    isExploding = false;
                if (frame >= 3) {
                    frame = 0;
                } else {
                    frame++;
                }
                
                time = 0;
            }

            foreach (Background item in Backgrounds) {
                item.UpdatePosition();
            }

            Fuel.UpdateFuelSpend();

            if (InputKey.IsKeyDown(Keys.J))
                Fuel.AddFuel(20f);

            PlaneAnimation = new Rectangle(Plane.Width / 4 * frame, 0, Plane.Width/4, Plane.Height);
            ExplosionAnimation = new Rectangle(ExplosionEffect.Width / 4 * frame, 0, ExplosionEffect.Width/4, ExplosionEffect.Height);
            // Animation

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            foreach (Background item in Backgrounds) {
                _spriteBatch.Draw(item.BG_texture, item.BG_position, new Rectangle(0, 0, item.BG_texture.Width, item.BG_texture.Height), Color.White);
            }
            if (!isExploded)
                _spriteBatch.Draw(Plane, Plane_Position, PlaneAnimation, Color.White);
            if (isExploding)
                _spriteBatch.Draw(ExplosionEffect, Plane_Position - new Vector2(95f), ExplosionAnimation, Color.White);
            _spriteBatch.Draw(Shadow, new Vector2(), Color.White);
            _spriteBatch.Draw(UI, new Vector2(), Color.White);
            _spriteBatch.Draw(Fuel.Fuel_Pointer, Fuel.position, Color.White);
            _spriteBatch.Draw(Fuel.Fuel_UI, new Vector2(), Color.White);
            
            _spriteBatch.End();


            base.Draw(gameTime);
        }

        public void ExplodePlane() {
            frame = 0;
            isExploding = true;
            isExploded = true;
            Config.BG_speed = 0f;
        }
    }
}
