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

        private int PrefferedHeight = 768;
        private int PrefferedWidth = 1024;

        public static float BackgroundMovementSpeed = 4f, FuelBarrelMovementSpeed = 4f;
        public static List<int> Points = new List<int> { 50, 100, 150, 250 };
        public static List<int> Fuel = new List<int> { 10, 15, 20, 25 };

        public static int MinimumObjectPos = 200;
        public static int MaximumObjectPos = 750;
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
        private Texture2D HealthUI;
        private List<BackgroundTexture> BackgroundTextures = new List<BackgroundTexture>();
        #endregion

        #region Other
        private List<Background> Backgrounds = new List<Background>();
        private Texture2D Shadow;

        private Texture2D UI;
        private FuelPtr FuelPtr;
        private Vector2 FuelPosition = new Vector2(320f, 689f);

        private Texture2D ExplosionEffect;
        #endregion

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = PrefferedHeight;
            _graphics.PreferredBackBufferWidth = PrefferedWidth;
            _graphics.ApplyChanges();
            Window.Title = "River Ride - Mikołaj Godzicki";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Initialize Content
            BackgroundTextures.Add(new BackgroundTexture(Content.Load<Texture2D>($"BG_1"), new int[] { 176, 336 }));
            BackgroundTextures.Add(new BackgroundTexture(Content.Load<Texture2D>($"BG_2"), new int[] { 128, 384 }));
            
            Shadow = Content.Load<Texture2D>("Shadow");
            PlaneEnemy = Content.Load<Texture2D>("Plane_Enemy");
            ProjectileTexture = Content.Load<Texture2D>("Projectile");
            ExplosionEffect = Content.Load<Texture2D>("ExplosionEffect");
            UI = Content.Load<Texture2D>("UI");
            HealthUI = Content.Load<Texture2D>("Heart_UI");
            LanaPixel_24 = Content.Load<SpriteFont>("LanaPixel_24");
            LanaPixel_48 = Content.Load<SpriteFont>("LanaPixel_48");
            bitArcadeOut_24 = Content.Load<SpriteFont>("8bitArcadeOut_24");
            bitArcadeOut_48 = Content.Load<SpriteFont>("8bitArcadeOut_48");
            FuelBarrel = Content.Load<Texture2D>("Fuel_Barrel");
            FuelPtr = new FuelPtr(Content.Load<Texture2D>("Fuel_Level"), Content.Load<Texture2D>("Fuel_UI"), 64, 320, FuelPosition);
            Player = new Player(Content.Load<Texture2D>("Plane"), ExplosionEffect);

            Backgrounds.Add(new Background(BackgroundTextures[0], new Vector2()));
            Backgrounds.Add(new Background(BackgroundTextures[1], new Vector2()));

            Backgrounds[0].position = new Vector2(0f, -662);
            Backgrounds[1].position = new Vector2(0f, 0);
            #endregion

            #region Events
            FuelPtr.OnFuelEmpty += Player.ExplodePlane;
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
            #endregion

            if (eventManager.gameState == EventManager.GameState.Game) {
                #region Update Game States
                Player.UpdatePlayer(InputKey, gameTime);
                FuelPtr.UpdateFuelSpend();
                FuelPtr.IsAlive = Player.IsAlive;

                // TODO: Randomizing Background
                //foreach (Background item in Backgrounds) 
                //    item.UpdatePosition(Backgrounds.Count - 2);
                foreach (Background item in Backgrounds)
                    item.UpdatePosition();

                foreach (Projectile item in Projectiles)
                    item.UpdateProjectile();

                foreach (EnemyPlane item in Enemies)
                    item.UpdateEnemy(gameTime);

                foreach (FuelBarrel item in FuelBarrels)
                    item.UpdateFuelBarrel(gameTime);

                if (tempLevel >= 2500) {
                    tempLevel = 0;
                    Level++;
                }
                #endregion

                #region Collisions
                for (int y = 0; y < Projectiles.Count; y++) {
                    for (int i = 0; i < Enemies.Count; i++) {
                        if (Projectiles[y].CheckCollision(Enemies[i])) {
                            Enemies[i].IsExploding = true;
                            if (Enemies[i].IsExploding && !Enemies[i].IsExploded) {
                                AddScore(Points[Random.Next(Points.Count)]);
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
                        if (Projectiles[y].CheckCollision(FuelBarrels[i])) {
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
                    if (FuelBarrels[i].CheckCollision(Player) && !FuelBarrels[i].IsExploding && !FuelBarrels[i].IsExploded) {
                        FuelPtr.AddFuel(FuelBarrels[i].GetFuelAmount());
                        FuelBarrels.RemoveAt(i);
                    }
                }

                for (int i = 0; i < Enemies.Count; i++) {
                    if (Enemies[i].CheckCollision(Player)) {
                        if (!Enemies[i].IsExploding && !Enemies[i].IsExploded)
                            Player.Health--;

                        if (Player.IsAlive)
                            Enemies[i].IsExploding = true;
                    }
                }

                foreach (Background item in Backgrounds) {
                    if (item.CheckCollision(Player))
                        Player.ExplodePlane();
                }
                #endregion

                #region Destroying
                for (int i = 0; i < Projectiles.Count; i++) {
                    if (Projectiles[i].position.Y < -80f) {
                        Projectiles.RemoveAt(i);
                    } 
                }

                for (int i = 0; i < Enemies.Count; i++) {
                    if (Enemies[i].position.Y > PrefferedHeight + 20f) {
                        Enemies.RemoveAt(i);
                    }
                }

                for (int i = 0; i < FuelBarrels.Count; i++) {
                    if (FuelBarrels[i].position.Y > PrefferedHeight + 20f) {
                        FuelBarrels.RemoveAt(i);
                    }
                }

                for (int i = 0; i < Backgrounds.Count; i++) {
                    if (Backgrounds[i].position.Y >= 662) {
                        Backgrounds[i].position.Y = -662 + (Backgrounds[i].position.Y - 662);
                        Backgrounds[i].backgroundTexture = BackgroundTextures[Random.Next(BackgroundTextures.Count)];
                        return;
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
                    string MenuName = "Kliknij [ENTER] aby ruszyć";
                    _spriteBatch.DrawString(LanaPixel_24, MenuName, new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString(MenuName).Length() / 2, GraphicsDevice.Viewport.Height / 2), Color.White);
                    _spriteBatch.End();
                    break;
                case EventManager.GameState.Game:
                    GraphicsDevice.Clear(Color.White);
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    #region Background
                    foreach (Background item in Backgrounds) {
                        _spriteBatch.Draw(item.backgroundTexture.texture, item.position, new Rectangle(0, 0, item.backgroundTexture.texture.Width, item.backgroundTexture.texture.Height), Color.White);
                    }
                    #endregion

                    #region Entities
                    foreach (FuelBarrel item in FuelBarrels) {
                        if (item.IsExploding)
                            item.Explode(_spriteBatch, new Vector2(60f), 0.5f);

                        if (!item.IsExploding && !item.IsExploded)
                            item.Draw(_spriteBatch);

                    }

                    foreach (Projectile item in Projectiles) {
                        _spriteBatch.Draw(item.texture, item.position, Color.White);
                    }

                    foreach (EnemyPlane item in Enemies) {
                        if (item.IsExploding)
                            item.Explode(_spriteBatch, new Vector2(30f), 0.5f);

                        if (!item.IsExploding && !item.IsExploded)
                            item.Draw(_spriteBatch);
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
                    _spriteBatch.Draw(FuelPtr.Fuel_Pointer, FuelPtr.position, Color.White);
                    _spriteBatch.Draw(FuelPtr.Fuel_UI, new Vector2(), Color.White);
                    for (int i = 1; i < Player.Health + 1; i++) {
                        _spriteBatch.Draw(HealthUI, new Vector2(722f + (30 * i), 671f), Color.White);
                    }
                    _spriteBatch.DrawString(LanaPixel_24, $"Wynik: {Score}", new Vector2(422f, 661f), Color.White);
                    _spriteBatch.DrawString(LanaPixel_24, $"Poziom: {Level}", new Vector2(422f, 701f), Color.White);
                    if (!Player.IsAlive)
                        StartBlinkingGameOver(gameTime);
                    #endregion

                    _spriteBatch.End();
                    break;
                case EventManager.GameState.EndGame:
                    GraphicsDevice.Clear(Color.Black);
                    _spriteBatch.Begin();
                    string EndGameName = "Rozbiłeś się";
                    _spriteBatch.DrawString(LanaPixel_24, $"{EndGameName} - Wynik: {Score}", new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString($"{EndGameName} - Wynik: {Score}").Length() / 2, GraphicsDevice.Viewport.Height / 2), Color.White);
                    _spriteBatch.End();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }

        protected void AddScore(int amount) {
            if (Player.IsAlive) {
                Score += amount;
                tempLevel += amount;
            }
        }

        protected void InstantiateProjectile() {
            if (eventManager.gameState == EventManager.GameState.Game)
                Projectiles.Add(new Projectile(ProjectileTexture, Player.position + new Vector2(27f, 0f)));
        }

        protected void InstantiateEnemy() {
            if (eventManager.gameState == EventManager.GameState.Game)
                Enemies.Add(new EnemyPlane(PlaneEnemy, ExplosionEffect));
        }

        protected void InstantiateFuelBarrel() {
            if (eventManager.gameState == EventManager.GameState.Game)
                FuelBarrels.Add(new FuelBarrel(FuelBarrel, ExplosionEffect));
        }

        float OverallBlinkTime, BlinkTimes = 5, BlinkTime, BlinkDelay = 500f; 
        protected void StartBlinkingGameOver(GameTime gameTime) {
            if (OverallBlinkTime < BlinkTimes * BlinkDelay * 2) {
                BlinkTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                OverallBlinkTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                string GameOverText = "Game Over";
                if (BlinkTime >= BlinkDelay) {
                    if (BlinkTime <= BlinkDelay * 2)
                        _spriteBatch.DrawString(bitArcadeOut_48, GameOverText, new Vector2(GraphicsDevice.Viewport.Width / 2 - bitArcadeOut_48.MeasureString(GameOverText).Length() / 2, GraphicsDevice.Viewport.Height / 2), Color.Black);
                    else
                        BlinkTime = 0;
                }
            } else {
                ChangeState(out eventManager.gameState, EventManager.GameState.EndGame);
            }
        }

        public void ChangeState(out EventManager.GameState gameState, EventManager.GameState gameStateToChange){
            gameState = gameStateToChange;
        }
    }
}
