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
using System.Linq;

namespace River_Ride___MG
{
    public class Main : Game
    {
        #region Utils
        public static Main Instance { get; set; }
        private GraphicsDeviceManager _graphics;
        private Random Random = new Random();
        private SpriteBatch _spriteBatch;
        private SpriteFont LanaPixel_24, LanaPixel_48, bitArcadeOut_24, bitArcadeOut_96;
        private static EventManager eventManager = new EventManager();

        public static int PrefferedHeight = 768;
        public static int PrefferedWidth = 1024;

        public static float FuelSpeed;
        public static float EnemyMovementSpeed;
        public static float EnemyHelicopterMovementSpeed = 5f;
        public static float BackgroundMovementSpeed, FallingObjectMovementSpeed;
        public static List<int> Points = new List<int> { 50, 100, 150, 250 };
        public static List<int> Fuel = new List<int> { 5, 10, 15 };

        public static int MinimumObjectPos = 200;
        public static int MaximumObjectPos = 750;
        #endregion

        #region Player
        Player Player;
        public int Score, Level, tempLevel;

        private List<Projectile> Projectiles = new List<Projectile>();
        private List<Enemy> Enemies = new List<Enemy>();
        private List<FuelBarrel> FuelBarrels = new List<FuelBarrel>();
        private List<AmmoCase> AmmoCases = new List<AmmoCase>();
        private List<Entity> Entities = new List<Entity>();
        #endregion

        #region Textures
        private Texture2D FuelBarrel;
        private Texture2D AmmoCase;
        private Texture2D HealthUI;
        private List<BackgroundTexture> BackgroundTextures = new List<BackgroundTexture>();
        private List<Texture2D> HelicopterTextures = new List<Texture2D>();
        private List<Texture2D> PlaneEnemyTextures = new List<Texture2D>();
        private List<Texture2D> EntitiesTextuers = new List<Texture2D>();
        #endregion

        #region Audio
        public static AudioManager audioManager;
        #endregion

        #region Other
        private List<Background> Backgrounds = new List<Background>();
        private Texture2D Shadow;

        private Texture2D UI;
        private FuelPtr FuelPtr;
        private Vector2 FuelPosition = new Vector2(320f, 689f);

        private Texture2D ExplosionEffect;

        private Dictionary<int, Enemy> EnemyDictionary = new Dictionary<int, Enemy>();
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
            audioManager = new AudioManager(Content);
            SetSpeed(0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Initialize Content
            BackgroundTextures.Add(new BackgroundTexture(Content.Load<Texture2D>($"BG_1"), new int[] { 352, 672 }));
            BackgroundTextures.Add(new BackgroundTexture(Content.Load<Texture2D>($"BG_2"), new int[] { 256, 768 }));
            
            Shadow = Content.Load<Texture2D>("Shadow");
            ExplosionEffect = Content.Load<Texture2D>("ExplosionEffect");
            UI = Content.Load<Texture2D>("UI");
            HealthUI = Content.Load<Texture2D>("Heart_UI");
            LanaPixel_24 = Content.Load<SpriteFont>("LanaPixel_24");
            LanaPixel_48 = Content.Load<SpriteFont>("LanaPixel_48");
            bitArcadeOut_24 = Content.Load<SpriteFont>("8bitArcadeOut_24");
            bitArcadeOut_96 = Content.Load<SpriteFont>("8bitArcadeOut_96");
            FuelBarrel = Content.Load<Texture2D>("Fuel_Barrel");
            AmmoCase = Content.Load<Texture2D>("AmmoCase");
            Player = new Player(Content.Load<Texture2D>("Plane"), ExplosionEffect, Content.Load<Texture2D>("Plane_Blinking"));
            Player.ProjectileTexture = Content.Load<Texture2D>("Projectile");
            Player.ProjectileMachineGunTexture = Content.Load<Texture2D>("ProjectileMachinegun");

            FuelPtr = new FuelPtr(Content.Load<Texture2D>("Fuel_Level"), Content.Load<Texture2D>("Fuel_UI"), 64, Player.MaxFuel, FuelPosition);

            for (int i = 1; i <= 3; i++) {
                HelicopterTextures.Add(Content.Load<Texture2D>($"Helicopter_{i}_LeftSide"));
                HelicopterTextures.Add(Content.Load<Texture2D>($"Helicopter_{i}_RightSide"));
            }

            for (int i = 1; i <= 3; i++) {
                PlaneEnemyTextures.Add(Content.Load<Texture2D>($"Plane_Enemy_{i}"));
            }

            Backgrounds.Add(new Background(BackgroundTextures[0]));
            Backgrounds.Add(new Background(BackgroundTextures[1]));

            Backgrounds[0].position = new Vector2(0f, -662);
            Backgrounds[1].position = new Vector2(0f, 0);

            
            for (int i = 0; i < PlaneEnemyTextures.Count; i++)
                EnemyDictionary.Add(i, new Enemy(PlaneEnemyTextures[i], ExplosionEffect, Enemy.EnemyType.Plane));
            for (int i = 0; i < HelicopterTextures.Count; i++) {
                EnemyDictionary.Add(i + PlaneEnemyTextures.Count, new Enemy(HelicopterTextures[i], ExplosionEffect, i % 2 == 0 ? Enemy.EnemyType.HelicopterLeftSide : Enemy.EnemyType.HelicopterRightSide));
            }

            Content.RootDirectory = "Content/Environment";
            EntitiesTextuers.Add(Content.Load<Texture2D>("Seeds"));
            for (int i = 0; i < 2; i++)
                EntitiesTextuers.Add(Content.Load<Texture2D>($"Rock_{i}"));
            for (int i = 0; i < 3; i++)
                EntitiesTextuers.Add(Content.Load<Texture2D>($"Tree_{i}"));
            Content.RootDirectory = "Content";
            #endregion

            #region Events
            FuelPtr.OnFuelEmpty += Player.ExplodePlane;
            Player.OnAnimationTick += AddScore;
            Player.OnFireButtonClick += InstantiateProjectile;
            Player.OnFireMachineGunButtonClick += InstantiateProjectile;
            eventManager.OnEnemySpawnTick += InstantiateEnemy;
            eventManager.OnFuelBarrelSpawnTick += InstantiateFuelBarrel;
            eventManager.OnAmmoCaseSpawnTick += InstantiateAmmoCase;
            eventManager.OnEntitySpawnTick += InstantiateEntity;
            eventManager.OnRestartGame += RestartGame;
            #endregion

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region Updating
            KeyboardState InputKey = Keyboard.GetState();
            eventManager.Update(gameTime, InputKey);
            audioManager.UpdateTheme(gameTime);
            #endregion

            if (eventManager.gameState == EventManager.GameState.Game) {
                #region Update Game States
                Player.UpdatePlayer(InputKey, gameTime);
                FuelPtr.UpdateFuelSpend(Player);
                FuelPtr.IsAlive = Player.IsAlive;

                foreach (Background item in Backgrounds)
                    item.Update();

                foreach (Projectile item in Projectiles)
                    item.Update(gameTime);

                foreach (Enemy item in Enemies)
                    item.Update(gameTime, Player);

                foreach (FuelBarrel item in FuelBarrels)
                    item.Update(gameTime);

                foreach (Entity item in Entities)
                    item.Update(gameTime);

                foreach (AmmoCase item in AmmoCases)
                    item.Update(gameTime);

                if (tempLevel >= 2500) {
                    tempLevel -= 2500;
                    Level++;
                }
                #endregion

                #region Audio Update
                if (Player.IsAlive)
                    audioManager.FlyInstance.Play();
                else
                    audioManager.FlyInstance.Stop();
                #endregion

                #region Collisions
                for (int y = 0; y < Projectiles.Count; y++) {
                    for (int i = 0; i < Enemies.Count; i++) {
                        if (Projectiles[y].CheckCollision(Enemies[i]) && !Enemies[i].IsExploding) {
                            Enemies[i].IsExploding = true;
                            audioManager.PlaySound("Explosion");
                            if (!Enemies[i].IsExploded) {
                                AddScore(Points[Random.Next(Points.Count)]);
                                Projectiles.RemoveAt(y);
                                break;
                            }
                        }
                    }
                }

                for (int y = 0; y < Projectiles.Count; y++) { 
                    for (int i = 0; i < FuelBarrels.Count; i++) {
                        if (Projectiles[y].CheckCollision(FuelBarrels[i]) && !FuelBarrels[i].IsExploding) {
                            FuelBarrels[i].IsExploding = true;
                            audioManager.PlaySound("Explosion"); 
                            if (!FuelBarrels[i].IsExploded) {
                                Projectiles.RemoveAt(y);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < FuelBarrels.Count; i++) {
                    if (FuelBarrels[i].CheckCollision(Player) && !FuelBarrels[i].IsExploding && !FuelBarrels[i].IsExploded) {
                        FuelPtr.AddFuel(FuelBarrels[i].GetFuelAmount());
                        FuelBarrels.RemoveAt(i);
                        audioManager.PlaySound("Pickup");
                    }
                }

                for (int i = 0; i < AmmoCases.Count; i++) {
                    if (AmmoCases[i].CheckCollision(Player)) {
                        Player.AddAmmo(30);
                        AmmoCases.RemoveAt(i);
                        audioManager.PlaySound("Pickup");
                    }
                }

                for (int i = 0; i < Enemies.Count; i++) {
                    if (Enemies[i].CheckCollision(Player) && !Enemies[i].IsExploding && !Player.IsImmunity) {
                        if (!Enemies[i].IsExploded) {
                            Player.DealDamage();
                            audioManager.PlaySound("Explosion");
                        }

                        if (Player.IsAlive) {
                            Enemies[i].IsExploding = true;
                        }
                    }
                }

                foreach (Background item in Backgrounds) {
                    if (item.CheckCollision(Player)) {
                        Player.DealDamage();
                        if (Player.Health >= 1) {
                            Player.position = new Vector2(500f);
                        }
                        if (Player.IsAlive)
                            audioManager.PlaySound("Hit");
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
                    if (Enemies[i].position.Y > PrefferedHeight + 20f) {
                        Enemies.RemoveAt(i);
                    }
                }

                for (int i = 0; i < FuelBarrels.Count; i++) {
                    if (FuelBarrels[i].position.Y > PrefferedHeight + 20f) {
                        FuelBarrels.RemoveAt(i);
                    }
                }

                for (int i = 0; i < AmmoCases.Count; i++) {
                    if (AmmoCases[i].position.Y > PrefferedHeight + 20f) {
                        AmmoCases.RemoveAt(i);
                    }
                }

                for (int i = 0; i < Backgrounds.Count; i++) {
                    if (Backgrounds[i].position.Y >= 662) {
                        Backgrounds[i].position.Y = -662 + (Backgrounds[i].position.Y - 662);
                        Backgrounds[i].backgroundTexture = BackgroundTextures[Random.Next(BackgroundTextures.Count)];
                        MinimumObjectPos = Backgrounds[i].backgroundTexture.CollisionPoints[0];
                        MaximumObjectPos = Backgrounds[i].backgroundTexture.CollisionPoints[1];
                    }
                }

                for (int i = 0; i < Enemies.Count; i++) {
                    if (Enemies[i].IsExploded)
                        Enemies.RemoveAt(i);
                }

                for (int i = 0; i < FuelBarrels.Count; i++) {
                    if (FuelBarrels[i].IsExploded)
                        FuelBarrels.RemoveAt(i);
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
                    string[] MenuText = { "Kliknij [ENTER] aby ruszyć", "Spacja - Strzał", "V - Strzał z Machine Gun'a" };
                    float offset = 0;
                    foreach (string item in MenuText) {
                        _spriteBatch.DrawString(LanaPixel_24, item, new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString(item).Length() / 2, GraphicsDevice.Viewport.Height / 2 + offset), Color.White);
                        offset += 40f;
                    }
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

                    foreach (AmmoCase item in AmmoCases) 
                        item.Draw(_spriteBatch);

                    foreach (Entity item in Entities)
                        item.Draw(_spriteBatch);

                    foreach (Projectile item in Projectiles) {
                        item.Draw(_spriteBatch);
                    }

                    foreach (Enemy item in Enemies) {
                        if (item.IsExploding)
                            item.Explode(_spriteBatch, new Vector2(30f), 0.5f);

                        if (!item.IsExploding && !item.IsExploded)
                            item.Draw(_spriteBatch);
                    }
                    #endregion

                    #region Player
                    if (Player.IsExploding)
                        Player.Explode(_spriteBatch, new Vector2(95f));
                    else if (Player.IsAlive) {
                        if (Player.IsImmunity)
                            _spriteBatch.Draw(Player.BlinkingTexture, Player.position, Player.ObjectAnimation, Color.White);
                        else
                            _spriteBatch.Draw(Player.NormalTexture, Player.position, Player.ObjectAnimation, Color.White);
                    }
                    #endregion

                    #region UI
                    _spriteBatch.Draw(Shadow, new Vector2(), Color.White);
                    _spriteBatch.Draw(UI, new Vector2(), Color.White);
                    _spriteBatch.Draw(FuelPtr.Fuel_Pointer, FuelPtr.position, Color.White);
                    _spriteBatch.Draw(FuelPtr.Fuel_UI, new Vector2(), Color.White);
                    _spriteBatch.DrawString(LanaPixel_24, $"Naboje: {Player.MachinegunMagazine}", new Vector2(702f, 701f), Color.White);
                    _spriteBatch.DrawString(LanaPixel_24, $"Życia:", new Vector2(702f, 661f), Color.White);
                    for (int i = 0; i < Player.Health; i++) {
                        _spriteBatch.Draw(HealthUI, new Vector2(697f + LanaPixel_24.MeasureString("Życia:").Length() + (20 * i), 676f), Color.White);
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
                    _spriteBatch.DrawString(LanaPixel_24, $"{EndGameName} - Wynik: {Score}", new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString($"{EndGameName} - Wynik: {Score}").Length() / 2, GraphicsDevice.Viewport.Height / 4), Color.White);
                    
                    string EndGameToStartName = "Aby rozpocząć od nowa, naciśnij [ENTER]";
                    _spriteBatch.DrawString(LanaPixel_24, $"{EndGameToStartName}", new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString($"{EndGameToStartName}").Length() / 2, GraphicsDevice.Viewport.Height / 4 + 40f), Color.White);
                    
                    string Leaderboard = "Tabela Najlepszych Wyników:";
                    _spriteBatch.DrawString(LanaPixel_24, $"{Leaderboard}", new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString($"{Leaderboard}").Length() / 2, GraphicsDevice.Viewport.Height / 4 + 120f), Color.White);

                    ShowLeaderboard();

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

        protected void InstantiateProjectile(Texture2D texture) {
            if (eventManager.gameState == EventManager.GameState.Game && Player.IsAlive)
                Projectiles.Add(new Projectile(texture, Player.position + new Vector2(27f, 0f)));
        }

        protected void InstantiateEnemy() {
            if (eventManager.gameState == EventManager.GameState.Game && Player.IsAlive) {
                int i = Random.Next(EnemyDictionary.Count);
                Enemies.Add(new Enemy(EnemyDictionary[i].texture, EnemyDictionary[i].ExplodeTexture, EnemyDictionary[i].enemyType));
            }
        }

        protected void InstantiateFuelBarrel() {
            if (eventManager.gameState == EventManager.GameState.Game && Player.IsAlive)
                FuelBarrels.Add(new FuelBarrel(FuelBarrel, ExplosionEffect));
        }

        protected void InstantiateAmmoCase() {
            if (eventManager.gameState == EventManager.GameState.Game && Player.IsAlive)
                AmmoCases.Add(new AmmoCase(AmmoCase));
        }
        
        protected void InstantiateEntity() {
            if (eventManager.gameState == EventManager.GameState.Game && Player.IsAlive)
                Entities.Add(new Entity(EntitiesTextuers[Random.Next(EntitiesTextuers.Count)]));
        }

        float OverallBlinkTime, BlinkTimes = 2, BlinkTime, BlinkDelay = 500f; 
        protected void StartBlinkingGameOver(GameTime gameTime) {
            if (OverallBlinkTime < BlinkTimes * BlinkDelay * 2) {
                BlinkTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                OverallBlinkTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                string GameOverText = "Game Over";
                if (BlinkTime >= BlinkDelay) {
                    if (BlinkTime <= BlinkDelay * 2)
                        _spriteBatch.DrawString(bitArcadeOut_96, GameOverText, new Vector2(GraphicsDevice.Viewport.Width / 2 - bitArcadeOut_96.MeasureString(GameOverText).Length() / 2, GraphicsDevice.Viewport.Height / 2 - 100), Color.Black);
                    else
                        BlinkTime = 0;
                }
            } else {
                SaveAndLoadScores();
                ChangeState(out eventManager.gameState, EventManager.GameState.EndGame);
            }
        }

        public void ChangeState(out EventManager.GameState gameState, EventManager.GameState gameStateToChange){
            gameState = gameStateToChange;
        }

        public void RestartGame() {
            Score = 0;
            Level = 0;
            tempLevel = 0;
            Player.position = new Vector2(500f);
            Projectiles.Clear();
            Enemies.Clear();
            Player.Health = Player.HealthAtStart;
            Player.IsAlive = true;
            Player.IsImmunity = false;
            Player.MachinegunMagazine = Player.MachinegunMagazineAtStart;
            FuelPtr.position.X = Player.MaxFuel;
            OverallBlinkTime = 0;
            BlinkTime = 0;
        }

        List<int> scores = new List<int>();
        protected void SaveAndLoadScores() {
            StreamWriter SW = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RR_scores.txt", true);
            SW.WriteLine(Score.ToString());
            SW.Close();

            scores.Clear();
            int length = File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RR_scores.txt").Count();
            if (length < 10) {
                SW = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RR_scores.txt", true);
                for (int i = 0; i < 11 - length; i++) {
                    SW.WriteLine(0.ToString());
                }
                SW.Close();
            }
            StreamReader SR = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RR_scores.txt");
            string item;
            while ((item = SR.ReadLine()) != null) {
                int value;
                if (Int32.TryParse(item, out value))
                    scores.Add(value);
            }
            SR.Close();
            
            scores.Sort();
            scores.Reverse();
        }

        protected void ShowLeaderboard() {
            for (int i = 0; i < 10; i++) {
                _spriteBatch.DrawString(LanaPixel_24, $"{i+1}: {scores[i]}", new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString($"{i + 1}: {scores[i]}").Length() / 2, GraphicsDevice.Viewport.Height / 4 + 160f + (40f * i)), Color.White);
            }
        }

        public static void SetSpeed(int speed) {
            BackgroundMovementSpeed = 4f + speed;
            FallingObjectMovementSpeed = 4f + speed;
            EnemyMovementSpeed = 5f + speed;
            FuelSpeed = 0.25f + ((float) speed / 10);

            int[,] Array = {
                { 600, 2300, } ,
                { 400, 1000, } ,
                { 7000, 8500, }
            };

            if (speed < 0) {
                eventManager.EnemyRandomArray = new int[] { Array[0, 0] * Math.Abs(speed), Array[0, 1] * Math.Abs(speed) };
                eventManager.FuelRandomArray = new int[] { Array[1, 0] * Math.Abs(speed), Array[1, 1] * Math.Abs(speed) };
                eventManager.AmmoRandomArray = new int[] { Array[2, 0] * Math.Abs(speed), Array[2, 1] * Math.Abs(speed) };
            } else if (speed > 0) {
                eventManager.EnemyRandomArray = new int[] { Array[0, 0] / Math.Abs(speed), Array[0, 1] / Math.Abs(speed) };
                eventManager.FuelRandomArray = new int[] { Array[1, 0] / Math.Abs(speed), Array[1, 1] / Math.Abs(speed) };
                eventManager.AmmoRandomArray = new int[] { Array[2, 0] / Math.Abs(speed), Array[2, 1] / Math.Abs(speed) };
            } else {
                eventManager.EnemyRandomArray = new int[] { Array[0, 0], Array[0, 1] };
                eventManager.FuelRandomArray = new int[] { Array[1, 0], Array[1, 1] };
                eventManager.AmmoRandomArray = new int[] { Array[2, 0], Array[2, 1] };
            }

        }
    }
}
