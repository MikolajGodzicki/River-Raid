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
        Player Player;
        private int Score, Level, tempLevel;

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

        private Texture2D ExplosionEffect;

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
            PlaneEnemy = Content.Load<Texture2D>("Plane_Enemy");
            ProjectileTexture = Content.Load<Texture2D>("Projectile");
            ExplosionEffect = Content.Load<Texture2D>("ExplosionEffect");
            UI = Content.Load<Texture2D>("UI");
            GameOverText = Content.Load<Texture2D>("GameOver");
            UI_Font = Content.Load<SpriteFont>("UI_Font");
            FuelBarrel = Content.Load<Texture2D>("Fuel_Barrel");
            Fuel = new FuelPtr(Content.Load<Texture2D>("Fuel_Level"), Content.Load<Texture2D>("Fuel_UI"), 64, 320, FuelPosition);

            Player = new Player(Content.Load<Texture2D>("Plane"), ExplosionEffect);

            // Events
            Fuel.OnFuelEmpty += Player.ExplodePlane;
            Player.OnAnimationTick += AddScore;
            // Events

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


            // Player Movement
            KeyboardState InputKey = Keyboard.GetState();
            Player.UpdatePlayer(InputKey, gameTime);
            // Player Movement

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
                if (FuelBarrels[i].CheckCollision(Player.texture, Player.position) && !FuelBarrels[i].IsExploding) {
                    Fuel.AddFuel(FuelBarrels[i].GetFuelAmount());
                    FuelBarrels.RemoveAt(i);
                }
            }

            for (int i = 0; i < Enemies.Count; i++) {
                if (Enemies[i].CheckCollision(Player.texture, Player.position)) {
                    Player.Health--;
                }
            }

            if (InputKey.IsKeyDown(Keys.J))
                Fuel.AddFuel(20f);

            ProjectileTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (ProjectileTime >= ProjectileDelay) {
                if (InputKey.IsKeyDown(Keys.F)) { 
                    Projectiles.Add(new Projectile(ProjectileTexture, Player.position + new Vector2(40f, 0f)));
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
                if (FuelBarrels[i].position.Y > Config.PrefferedHeight + 20f) {
                    FuelBarrels.RemoveAt(i);
                }
            }

            GameOverTextAnimation = new Rectangle(GameOverText.Width / 4 * Player.AnimationFrame, 0, GameOverText.Width/4, GameOverText.Height);

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
                    item.Explode(_spriteBatch, new Vector2(60f), 0.5f);

                if (!item.IsExploding && !item.IsExploded)
                    _spriteBatch.Draw(item.texture, item.position, Color.White);
            }

            foreach (Projectile item in Projectiles) {
                _spriteBatch.Draw(item.texture, item.position, Color.White);
            }

            foreach (Enemy item in Enemies) {
                if (item.IsExploding)
                    item.Explode(_spriteBatch, new Vector2(30f), 0.5f);

                if (!item.IsExploding && !item.IsExploded)
                    _spriteBatch.Draw(item.texture, item.position, item.ObjectAnimation, Color.White);
            }

            if (Player.IsExploding)
                Player.Explode(_spriteBatch, new Vector2(95f));
            else if (Player.IsExploded)
                _spriteBatch.Draw(GameOverText, new Vector2(), GameOverTextAnimation, Color.White);
            else
                _spriteBatch.Draw(Player.texture, Player.position, Player.ObjectAnimation, Color.White);

            _spriteBatch.Draw(Shadow, new Vector2(), Color.White);
            
            _spriteBatch.Draw(UI, new Vector2(), Color.White);
            _spriteBatch.Draw(Fuel.Fuel_Pointer, Fuel.position, Color.White);
            _spriteBatch.Draw(Fuel.Fuel_UI, new Vector2(), Color.White);
            _spriteBatch.DrawString(UI_Font, $"Wynik: {Score}", new Vector2(422f, 651f), Color.White);
            _spriteBatch.DrawString(UI_Font, $"Poziom: {Level}", new Vector2(422f, 701f), Color.White);
            
            _spriteBatch.End();

            base.Draw(gameTime);
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
