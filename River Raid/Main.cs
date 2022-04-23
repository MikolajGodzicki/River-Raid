using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Raid;
using River_Raid.Classes;
using System.Text;

namespace River_Ride___MG
{
    public class Main : Game
    {
        #region Utils
        private GraphicsDeviceManager _graphics;
        private Random Random = new Random();
        private SpriteBatch _spriteBatch;
        private SpriteFont UI_Font;
        #endregion

        #region Player
        private Texture2D Plane;
        private Vector2 PlanePosition = new Vector2(500f, 500f);
        private int Score, Level, tempLevel;

        float AnimationTime, AnimationDelay = 100f;
        int AnimationFrame;
        Rectangle PlaneAnimation;

        bool CanGoLeft = true, CanGoRight = true;

        private Texture2D ProjectileTexture;
        private List<Projectile> Projectiles = new List<Projectile>();
        float ProjectileTime, ProjectileDelay = 800f;

        private Texture2D PlaneEnemy;
        private List<Enemy> Enemies = new List<Enemy>();
        float EnemySpawnTime, EnemySpawnDelay = 2300f;

        private Texture2D FuelBarrel;
        private List<FuelBarrel> FuelBarrels = new List<FuelBarrel>();
        float FuelBarrelSpawnTime, FuelBarrelSpawnDelay = 1400f;
        #endregion

        #region Other
        private List<Background> Backgrounds = new List<Background>();
        private Texture2D Shadow;

        private Texture2D UI;
        private FuelPtr Fuel;
        private Vector2 FuelPosition = new Vector2(320f, 689f);

        bool isExploding = false, isExploded = false;
        private Texture2D ExplosionEffect;
        private Rectangle ExplosionAnimation;

        private Texture2D GameOverText;
        private Rectangle GameOverTextAnimation;
        #endregion

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            LoadScore();
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
            for (int i = 0; i < Config.BGCount; i++) {
                Backgrounds.Add(new Background(Content.Load<Texture2D>($"BG_{i+1}"), i));
            }
            Shadow = Content.Load<Texture2D>("Shadow");
            Plane = Content.Load<Texture2D>("Plane");
            PlaneEnemy = Content.Load<Texture2D>("Plane_Enemy");
            ProjectileTexture = Content.Load<Texture2D>("Projectile");
            ExplosionEffect = Content.Load<Texture2D>("ExplosionEffect");
            UI = Content.Load<Texture2D>("UI");
            GameOverText = Content.Load<Texture2D>("GameOver");
            UI_Font = Content.Load<SpriteFont>("UI_Font");
            FuelBarrel = Content.Load<Texture2D>("Fuel_Barrel");
            Fuel = new FuelPtr(Content.Load<Texture2D>("Fuel_Level"), Content.Load<Texture2D>("Fuel_UI"), 64, 320, FuelPosition);
            Fuel.OnFuelEmpty += ExplodePlane;

            Backgrounds[0].position = new Vector2(Backgrounds[0].position.X, -Backgrounds[0].texture.Height);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (tempLevel >= Config.levelUpScore) {
                tempLevel = 0;
                Level++;
            }
                

            // Movement
            KeyboardState InputKey = Keyboard.GetState();
            if (!isExploding) {
                if ((InputKey.IsKeyDown(Keys.A) || InputKey.IsKeyDown(Keys.Left)) && CanGoLeft) {
                    PlanePosition.X -= Config.PlaneMovementSpeed;
                } else if ((InputKey.IsKeyDown(Keys.D) || InputKey.IsKeyDown(Keys.Right)) && CanGoRight) {
                    PlanePosition.X += Config.PlaneMovementSpeed;
                }
            }

            if (PlanePosition.X <= 100)
                CanGoLeft = false;
            else 
                CanGoLeft = true;

            if (PlanePosition.X >= 830)
                CanGoRight = false;
            else
                CanGoRight = true;
            // Movement

            // Animation
            AnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (AnimationTime >= AnimationDelay) {
                if (isExploding && AnimationFrame >= 3) {
                    isExploding = false;
                    isExploded = true;
                }
                if (isExploded) {
                    AnimationDelay = 500f;
                }
                   
                if (AnimationFrame >= 3) {
                    AnimationFrame = 0;
                } else {
                    AnimationFrame++;
                    AddScore(5);
                }
                
                AnimationTime = 0;
            }

            Fuel.UpdateFuelSpend();

            foreach (Background item in Backgrounds) 
                item.UpdatePosition();

            foreach (Projectile item in Projectiles)
                item.UpdateProjectile();

            foreach (Enemy item in Enemies)
                item.UpdateEnemy(gameTime);

            foreach (FuelBarrel item in FuelBarrels)
                item.UpdateFuelBarrel(gameTime);

            for (int y = 0; y < Projectiles.Count; y++) {
                for (int i = 0; i < Enemies.Count; i++) {
                    if (Projectiles[y].CheckCollision(Enemies[i].texture, Enemies[i].position)) {
                        Enemies[i].IsExploding = true;
                        if (Enemies[i].IsExploding && !Enemies[i].IsExploded) {
                            AddScore(Config.Points[Random.Next(Config.Points.Count)]);
                            Projectiles.RemoveAt(y);
                            break;
                        }
                            
                        if (Enemies[i].IsExploded) 
                            Enemies.RemoveAt(i);
                    }
                }
            }

            for (int y = 0; y < Projectiles.Count; y++) { 
                for (int i = 0; i < FuelBarrels.Count; i++) {
                    if (Projectiles[y].CheckCollision(FuelBarrels[i].texture, FuelBarrels[i].position, 1)) {
                        FuelBarrels[i].IsExploding = true;
                        if (FuelBarrels[i].IsExploding && !FuelBarrels[i].IsExploded) {
                            Projectiles.RemoveAt(y);
                            break;
                        }

                        if (FuelBarrels[i].IsExploded)
                            FuelBarrels.RemoveAt(i);
                    }

                }
            }

            for (int i = 0; i < FuelBarrels.Count; i++) {
                if (FuelBarrels[i].CheckCollision(Plane, PlanePosition) && !FuelBarrels[i].IsExploding) {
                    Fuel.AddFuel(FuelBarrels[i].GetFuelAmount());
                    FuelBarrels.RemoveAt(i);
                }
            }
            

            if (InputKey.IsKeyDown(Keys.J))
                Fuel.AddFuel(20f);

            ProjectileTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (ProjectileTime >= ProjectileDelay) {
                if (InputKey.IsKeyDown(Keys.F)) { 
                    Projectiles.Add(new Projectile(ProjectileTexture, PlanePosition + new Vector2(40f, 0f)));
                    ProjectileTime = 0;
                }
            }

            EnemySpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EnemySpawnTime >= EnemySpawnDelay) {
                Enemies.Add(new Enemy(PlaneEnemy, ExplosionEffect));
                EnemySpawnTime = 0;
            }

            FuelBarrelSpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (FuelBarrelSpawnTime >= FuelBarrelSpawnDelay) {
                FuelBarrels.Add(new FuelBarrel(FuelBarrel, ExplosionEffect));
                FuelBarrelSpawnTime = 0;
            }

            for (int i = 0; i < Projectiles.Count; i++) {
                if (Projectiles[i].position.Y < -20f) {
                    Projectiles.RemoveAt(i);
                } 
            }

            for (int i = 0; i < Enemies.Count; i++) {
                if (Enemies[i].position.Y > Config.PrefferedHeight + 20f) {
                    Enemies.RemoveAt(i);
                }
            }

            for (int i = 0; i < FuelBarrels.Count; i++) {
                if (FuelBarrels[i].position.Y > 700f) {
                    FuelBarrels.RemoveAt(i);
                }
            }

            PlaneAnimation = new Rectangle(Plane.Width / 4 * AnimationFrame, 0, Plane.Width/4, Plane.Height);
            ExplosionAnimation = new Rectangle(ExplosionEffect.Width / 4 * AnimationFrame, 0, ExplosionEffect.Width/4, ExplosionEffect.Height);
            GameOverTextAnimation = new Rectangle(GameOverText.Width / 4 * AnimationFrame, 0, GameOverText.Width/4, GameOverText.Height);
            // Animation

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (Background item in Backgrounds) {
                _spriteBatch.Draw(item.texture, item.position, new Rectangle(0, 0, item.texture.Width, item.texture.Height), Color.White);
            }

            foreach (FuelBarrel item in FuelBarrels) {
                if (item.IsExploding)
                    item.Explode(_spriteBatch);

                if (!item.IsExploding && !item.IsExploded)
                    _spriteBatch.Draw(item.texture, item.position, Color.White);
            }

            foreach (Projectile item in Projectiles) {
                _spriteBatch.Draw(item.texture, item.position, Color.White);
            }

            foreach (Enemy item in Enemies) {
                if (item.IsExploding)
                    item.Explode(_spriteBatch);

                if (!item.IsExploding && !item.IsExploded)
                    _spriteBatch.Draw(item.texture, item.position, PlaneAnimation, Color.White);
            }
            
            if (isExploded)
                _spriteBatch.Draw(GameOverText, new Vector2(), GameOverTextAnimation, Color.White);
            if (!isExploding && !isExploded)
                _spriteBatch.Draw(Plane, PlanePosition, PlaneAnimation, Color.White);
            if (isExploding)
                _spriteBatch.Draw(ExplosionEffect, PlanePosition - new Vector2(95f), ExplosionAnimation, Color.White);
            _spriteBatch.Draw(Shadow, new Vector2(), Color.White);
            
            _spriteBatch.Draw(UI, new Vector2(), Color.White);
            _spriteBatch.Draw(Fuel.Fuel_Pointer, Fuel.position, Color.White);
            _spriteBatch.Draw(Fuel.Fuel_UI, new Vector2(), Color.White);
            _spriteBatch.DrawString(UI_Font, $"Wynik: {Score}", new Vector2(422f, 651f), Color.White);
            _spriteBatch.DrawString(UI_Font, $"Poziom: {Level}", new Vector2(422f, 701f), Color.White);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        public void ExplodePlane() {
            AnimationFrame = 0;
            isExploding = true;
            Config.BGMovementSpeed = 0f;
            Config.FuelBarrelSpeed = 0f;
        }

        protected override void OnExiting(object sender, EventArgs args) {
            StreamWriter streamWriter = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/MGRiverRideScore.txt");
            streamWriter.WriteLine(Score.ToString());
            streamWriter.WriteLine(Level.ToString());
            streamWriter.WriteLine(tempLevel.ToString());
            streamWriter.Close();
            base.OnExiting(sender, args);
        }

        protected void LoadScore() {
            try {
                StreamReader streamReader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/MGRiverRideScore.txt");
                int.TryParse(streamReader.ReadLine(), out Score);
                int.TryParse(streamReader.ReadLine(), out Level);
                int.TryParse(streamReader.ReadLine(), out tempLevel);
                streamReader.Close();
            } catch (FileNotFoundException ex) {
                return;
            }
        }

        protected void AddScore(int amount) {
            Score += amount;
            tempLevel += amount;
        }
    }
}
