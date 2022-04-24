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
        private SpriteFont LanaPixel_24, LanaPixel_48, bitArcadeOut_24, bitArcadeOut_48;
        private EventManager eventManager = new EventManager();
        #endregion

        #region Player
        Player Player;
        private int Score, Level, tempLevel;

        private List<Projectile> Projectiles = new List<Projectile>();
        private List<EnemyPlane> Enemies = new List<EnemyPlane>();
        private List<FuelBarrel> FuelBarrels = new List<FuelBarrel>();
        #endregion

        #region Textures
        private Texture2D ProjectileTexture;
        private Texture2D PlaneEnemy;
        private Texture2D FuelBarrel;
        #endregion

        #region Other
        private List<Background> Backgrounds = new List<Background>();
        private Texture2D Shadow;

        private Texture2D UI;
        private FuelPtr Fuel;
        private Vector2 FuelPosition = new Vector2(320f, 689f);

        private Texture2D ExplosionEffect;
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

            #region Initialize Content
            for (int i = 0; i < Config.BGCount; i++) {
                Backgrounds.Add(new Background(Content.Load<Texture2D>($"BG_{i+1}"), i));
            }
            Shadow = Content.Load<Texture2D>("Shadow");
            PlaneEnemy = Content.Load<Texture2D>("Plane_Enemy");
            ProjectileTexture = Content.Load<Texture2D>("Projectile");
            ExplosionEffect = Content.Load<Texture2D>("ExplosionEffect");
            UI = Content.Load<Texture2D>("UI");
            LanaPixel_24 = Content.Load<SpriteFont>("LanaPixel_24");
            LanaPixel_48 = Content.Load<SpriteFont>("LanaPixel_48");
            bitArcadeOut_24 = Content.Load<SpriteFont>("8bitArcadeOut_24");
            bitArcadeOut_48 = Content.Load<SpriteFont>("8bitArcadeOut_48");
            FuelBarrel = Content.Load<Texture2D>("Fuel_Barrel");
            Fuel = new FuelPtr(Content.Load<Texture2D>("Fuel_Level"), Content.Load<Texture2D>("Fuel_UI"), 64, 320, FuelPosition);

            Player = new Player(Content.Load<Texture2D>("Plane"), ExplosionEffect);

            Backgrounds[0].position = new Vector2(Backgrounds[0].position.X, -Backgrounds[0].texture.Height);
            #endregion

            #region Events
            Fuel.OnFuelEmpty += Player.ExplodePlane;
            Player.OnAnimationTick += AddScore;
            Player.OnFireButtonClick += InstantiateProjectile;
            eventManager.OnEnemySpawnTick += InstantiateEnemy;
            eventManager.OnFuelBarrelSpawnTick += InstantiateFuelBarrel;
            #endregion

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region Updating
            KeyboardState InputKey = Keyboard.GetState();
            eventManager.Update(gameTime, InputKey);

if (eventManager.gameState == EventManager.GameState.Game) { 
            Player.UpdatePlayer(InputKey, gameTime);
            Fuel.UpdateFuelSpend();

            foreach (Background item in Backgrounds) 
                item.UpdatePosition();

            foreach (Projectile item in Projectiles)
                item.UpdateProjectile();

            foreach (EnemyPlane item in Enemies)
                item.UpdateEnemy(gameTime);

            foreach (FuelBarrel item in FuelBarrels)
                item.UpdateFuelBarrel(gameTime);

            if (tempLevel >= Config.levelUpScore) {
                tempLevel = 0;
                Level++;
            }
            #endregion

            #region Collisions
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
                if (FuelBarrels[i].CheckCollision(Player.texture, Player.position) && !FuelBarrels[i].IsExploding && !FuelBarrels[i].IsExploded) {
                    Fuel.AddFuel(FuelBarrels[i].GetFuelAmount());
                    FuelBarrels.RemoveAt(i);
                }
            }

            for (int i = 0; i < Enemies.Count; i++) {
                if (Enemies[i].CheckCollision(Player.texture, Player.position)) {
                    Player.Health--;
                }
            }
            #endregion

            #region Destroying
            for (int i = 0; i < Projectiles.Count; i++) {
                if (Projectiles[i].position.Y < -80f) {
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
            #endregion
}
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            switch (eventManager.gameState) {
                case EventManager.GameState.Menu:
                    GraphicsDevice.Clear(Color.Black);
                    _spriteBatch.Begin();
                    _spriteBatch.DrawString(LanaPixel_24, "Kliknij [ENTER] aby ruszyć", new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString("Kliknij [ENTER] aby ruszyć").Length() / 2, GraphicsDevice.Viewport.Height / 2), Color.White);
                    _spriteBatch.End();
                    break;
                case EventManager.GameState.Game:
                    GraphicsDevice.Clear(Color.White);
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    #region Background
                    foreach (Background item in Backgrounds) {
                        _spriteBatch.Draw(item.texture, item.position, new Rectangle(0, 0, item.texture.Width, item.texture.Height), Color.White);
                    }
                    #endregion

                    #region Entities
                    foreach (FuelBarrel item in FuelBarrels) {
                        if (item.IsExploding)
                            item.Explode(_spriteBatch, new Vector2(60f), 0.5f);

                        if (!item.IsExploding && !item.IsExploded)
                            _spriteBatch.Draw(item.texture, item.position, Color.White);
                    }

                    foreach (Projectile item in Projectiles) {
                        _spriteBatch.Draw(item.texture, item.position, Color.White);
                    }

                    foreach (EnemyPlane item in Enemies) {
                        if (item.IsExploding)
                            item.Explode(_spriteBatch, new Vector2(30f), 0.5f);

                        if (!item.IsExploding && !item.IsExploded)
                            _spriteBatch.Draw(item.texture, item.position, item.ObjectAnimation, Color.White);
                    }
                    #endregion

                    #region Player
                    if (Player.IsExploding)
                        Player.Explode(_spriteBatch, new Vector2(95f));
                    else if (Player.IsAlive)
                        _spriteBatch.Draw(Player.texture, Player.position, Player.ObjectAnimation, Color.White);
                    #endregion

                    #region UI
                    _spriteBatch.Draw(Shadow, new Vector2(), Color.White);
                    _spriteBatch.Draw(UI, new Vector2(), Color.White);
                    _spriteBatch.Draw(Fuel.Fuel_Pointer, Fuel.position, Color.White);
                    _spriteBatch.Draw(Fuel.Fuel_UI, new Vector2(), Color.White);
                    _spriteBatch.DrawString(LanaPixel_24, $"Wynik: {Score}", new Vector2(422f, 661f), Color.White);
                    _spriteBatch.DrawString(LanaPixel_24, $"Poziom: {Level}", new Vector2(422f, 701f), Color.White);
                    if (!Player.IsAlive)
                        StartBlinkingGameOver(_spriteBatch, gameTime);
                    #endregion

                    _spriteBatch.End();
                    break;
                default:
                    break;
            }

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
            if (Player.IsAlive) {
                Score += amount;
                tempLevel += amount;
            }
        }

        protected void InstantiateProjectile() {
            Projectiles.Add(new Projectile(ProjectileTexture, Player.position + new Vector2(27f, 0f)));
        }

        protected void InstantiateEnemy() {
            Enemies.Add(new EnemyPlane(PlaneEnemy, ExplosionEffect));
        }

        protected void InstantiateFuelBarrel() {
            FuelBarrels.Add(new FuelBarrel(FuelBarrel, ExplosionEffect));
        }


        float OverallBlinkTime, BlinkTimes = 5, BlinkTime, BlinkDelay = 500f; 
        protected void StartBlinkingGameOver(SpriteBatch _spritebatch, GameTime gameTime) {
            if (OverallBlinkTime < BlinkTimes * BlinkDelay * 2) {
                BlinkTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                OverallBlinkTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (BlinkTime >= BlinkDelay) {
                    if (BlinkTime <= BlinkDelay * 2)
                        _spriteBatch.DrawString(bitArcadeOut_48, "Game Over", new Vector2(GraphicsDevice.Viewport.Width / 2 - bitArcadeOut_48.MeasureString("Game Over").Length() / 2, GraphicsDevice.Viewport.Height / 2), Color.Black);
                    else
                        BlinkTime = 0;
                }
            }
        }
    }
}
