using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Raid.Core;
using River_Raid.GameObjects;
using River_Raid.GameObjects.Background;
using River_Raid.GameObjects.Enemy;
using River_Raid.GameObjects.Fuel;
using River_Raid.GameObjects.Interactable;
using River_Raid.StateSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace River_Ride___MG {
    public class Main : Game {
        #region Utils
        public static Main Instance { get; set; }

        private GraphicsDeviceManager _graphics;
        private Random Random = new Random();
        private SpriteBatch _spriteBatch;
        private SpriteFont LanaPixel_24, bitArcadeOut_96;
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

        float OverallBlinkTime, BlinkTimes = 2, BlinkTime, BlinkDelay = 500f;
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

        List<int> scores = new List<int>();
        #endregion

        public Main() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            _graphics.PreferredBackBufferHeight = PrefferedHeight;
            _graphics.PreferredBackBufferWidth = PrefferedWidth;
            _graphics.ApplyChanges();
            Window.Title = "River Ride - Mikołaj Godzicki";
            audioManager = new AudioManager(Content);
            SetSpeed(0);
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadEffects();
            LoadGameObjects();
            LoadUI();
            LoadFonts();
            LoadBackgrounds();

            Content.RootDirectory = "Content/Environment";
            LoadEntities();
            Content.RootDirectory = "Content";

            AssignEvents();
        }

        private void LoadEffects() {

            Shadow = LoadContent<Texture2D>("Shadow");
            ExplosionEffect = LoadContent<Texture2D>("ExplosionEffect");
        }

        private void LoadGameObjects() {
            FuelBarrel = LoadContent<Texture2D>("Fuel_Barrel");
            AmmoCase = LoadContent<Texture2D>("AmmoCase");
            Player = new Player(LoadContent<Texture2D>("Plane"), ExplosionEffect, LoadContent<Texture2D>("Plane_Blinking"));

            Player.ProjectileTexture = LoadContent<Texture2D>("Projectile");
            Player.ProjectileMachineGunTexture = LoadContent<Texture2D>("ProjectileMachinegun");


            for (int i = 1; i <= 3; i++) {
                HelicopterTextures.Add(LoadContent<Texture2D>($"Helicopter_{i}_LeftSide"));
                HelicopterTextures.Add(LoadContent<Texture2D>($"Helicopter_{i}_RightSide"));
            }

            for (int i = 1; i <= 3; i++) {
                PlaneEnemyTextures.Add(LoadContent<Texture2D>($"Plane_Enemy_{i}"));
            }
        }

        private void LoadUI() {
            HealthUI = LoadContent<Texture2D>("Heart_UI");
            UI = LoadContent<Texture2D>("UI");
            FuelPtr = new FuelPtr(LoadContent<Texture2D>("Fuel_Level"), LoadContent<Texture2D>("Fuel_UI"), 64, Player.MaxFuel, FuelPosition);
        }

        private void LoadFonts() {
            LanaPixel_24 = Content.Load<SpriteFont>("LanaPixel_24");
            bitArcadeOut_96 = Content.Load<SpriteFont>("8bitArcadeOut_96");
        }

        private void LoadBackgrounds() {
            BackgroundTextures.Add(new BackgroundTexture(LoadContent<Texture2D>($"BG_1"), new int[] { 352, 672 }));
            BackgroundTextures.Add(new BackgroundTexture(LoadContent<Texture2D>($"BG_2"), new int[] { 256, 768 }));

            Backgrounds.Add(new Background(BackgroundTextures[0]));
            Backgrounds.Add(new Background(BackgroundTextures[1]));

            Backgrounds[0].position = new Vector2(0f, -662);
            Backgrounds[1].position = new Vector2(0f, 0);


            for (int i = 0; i < PlaneEnemyTextures.Count; i++)
                EnemyDictionary.Add(i, new Enemy(PlaneEnemyTextures[i], ExplosionEffect, EnemyType.Plane));
            for (int i = 0; i < HelicopterTextures.Count; i++) {
                EnemyDictionary.Add(i + PlaneEnemyTextures.Count, new Enemy(HelicopterTextures[i], ExplosionEffect, i % 2 == 0 ? EnemyType.HelicopterLeftSide : EnemyType.HelicopterRightSide));
            }

        }

        private void LoadEntities() {
            EntitiesTextuers.Add(LoadContent<Texture2D>("Seeds"));
            for (int i = 0; i < 2; i++)
                EntitiesTextuers.Add(LoadContent<Texture2D>($"Rock_{i}"));
            for (int i = 0; i < 3; i++)
                EntitiesTextuers.Add(LoadContent<Texture2D>($"Tree_{i}"));
        }

        private T LoadContent<T>(string name) => Content.Load<T>(name);

        private void AssignEvents() {
            FuelPtr.OnFuelEmpty += Player.ExplodePlane;
            Player.OnAnimationTick += AddScore;
            Player.OnFireButtonClick += InstantiateProjectile;
            Player.OnFireMachineGunButtonClick += InstantiateProjectile;
            eventManager.OnEnemySpawnTick += InstantiateEnemy;
            eventManager.OnFuelBarrelSpawnTick += InstantiateFuelBarrel;
            eventManager.OnAmmoCaseSpawnTick += InstantiateAmmoCase;
            eventManager.OnEntitySpawnTick += InstantiateEntity;
            eventManager.OnRestartGame += RestartGame;
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState inputKey = Keyboard.GetState();

            UpdateManagers(gameTime, inputKey);

            if (eventManager.gameState == GameState.Game) {
                UpdateObjects(inputKey, gameTime);

                UpdateAudio();

                UpdateCollisions();

                UpdateDestroys();
            }
            base.Update(gameTime);
        }

        private void UpdateManagers(GameTime gameTime, KeyboardState inputKey) {
            eventManager.Update(gameTime, inputKey);
            audioManager.UpdateTheme(gameTime);
        }

        private void UpdateObjects(KeyboardState inputKey, GameTime gameTime) {
            Player.UpdatePlayer(inputKey, gameTime);
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
        }

        private void UpdateAudio() {
            if (Player.IsAlive)
                audioManager.FlyInstance.Play();
            else
                audioManager.FlyInstance.Stop();
        }

        private void UpdateCollisions() {
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
        }

        private void UpdateDestroys() {
            DestroyUnavailableObjects(Projectiles, -80f, true);

            DestroyUnavailableObjects(Enemies, PrefferedHeight + 20f);

            DestroyUnavailableObjects(FuelBarrels, PrefferedHeight + 20f);

            DestroyUnavailableObjects(AmmoCases, PrefferedHeight + 20f);

            DestroyUnavailableObjects(Entities, PrefferedHeight + 50);

            MoveBackgrounds();

            DestroyExplodedObjects(Enemies);

            DestroyExplodedObjects(FuelBarrels);
        }

        private void DestroyUnavailableObjects<T>(List<T> objects, float height, bool lowerThan = false) where T : GameObject {
            for (int i = 0; i < objects.Count; i++) {
                bool condition = objects[i].position.Y > height;

                if (lowerThan)
                    condition = objects[i].position.Y < height;

                if (condition)
                    objects.RemoveAt(i);
            }
        }

        private void DestroyExplodedObjects<T>(List<T> objects) where T : ExplodeableGameObject {
            for (int i = 0; i < objects.Count; i++) {
                if (objects[i].IsExploded)
                    objects.RemoveAt(i);
            }
        }

        private void MoveBackgrounds() {
            for (int i = 0; i < Backgrounds.Count; i++) {
                if (Backgrounds[i].position.Y >= 662) {
                    Backgrounds[i].position.Y = -662 + (Backgrounds[i].position.Y - 662);
                    Backgrounds[i].backgroundTexture = BackgroundTextures[Random.Next(BackgroundTextures.Count)];
                    MinimumObjectPos = Backgrounds[i].backgroundTexture.CollisionPoints[0];
                    MaximumObjectPos = Backgrounds[i].backgroundTexture.CollisionPoints[1];
                }
            }
        }

        protected override void Draw(GameTime gameTime) {
            switch (eventManager.gameState) {
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Game:
                    DrawMainGame(gameTime);
                    break;
                case GameState.EndGame:
                    DrawEndGame();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawMenu() {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            string[] MenuText = { "Kliknij [ENTER] aby ruszyć", "Spacja - Strzał", "V - Strzał z Machine Gun'a" };
            float offset = -100;
            foreach (string item in MenuText) {
                _spriteBatch.DrawString(LanaPixel_24, item, new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString(item).Length() / 2, GraphicsDevice.Viewport.Height / 2 + offset), Color.White);
                offset += 40f;
            }
            _spriteBatch.End();
        }

        private void DrawMainGame(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawBackgrounds();

            DrawObjects(AmmoCases);

            DrawObjects(Entities);

            DrawObjects(Projectiles);

            DrawExplodeableObjects(FuelBarrels, 60f);

            DrawExplodeableObjects(Enemies, 30f);

            DrawPlayer();

            DrawUI(gameTime);

            _spriteBatch.End();
        }

        private void DrawObjects<T>(List<T> objects) where T : GameObject {
            foreach (T item in objects)
                item.Draw(_spriteBatch);
        }

        private void DrawExplodeableObjects<T>(List<T> objects, float offset) where T : ExplodeableGameObject {
            foreach (T item in objects) {
                if (item.IsExploding)
                    item.Explode(_spriteBatch, new Vector2(offset), 0.5f);

                if (!item.IsExploding && !item.IsExploded)
                    item.Draw(_spriteBatch);
            }
        }

        private void DrawPlayer() {
            if (Player.IsExploding)
                Player.Explode(_spriteBatch, new Vector2(95f));
            else if (Player.IsAlive) {
                if (Player.IsImmunity)
                    _spriteBatch.Draw(Player.BlinkingTexture, Player.position, Player.ObjectAnimation, Color.White);
                else
                    _spriteBatch.Draw(Player.NormalTexture, Player.position, Player.ObjectAnimation, Color.White);
            }
        }

        private void DrawBackgrounds() {
            foreach (Background item in Backgrounds) {
                _spriteBatch.Draw(item.backgroundTexture.texture,
                    item.position,
                    new Rectangle(0,
                                    0,
                                    item.backgroundTexture.texture.Width,
                                    item.backgroundTexture.texture.Height),
                    Color.White);
            }
        }

        private void DrawUI(GameTime gameTime) {
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
        }

        private void DrawEndGame() {
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
        }

        protected void ShowLeaderboard() {
            for (int i = 0; i < 10; i++) {
                _spriteBatch.DrawString(LanaPixel_24, $"{i + 1}: {scores[i]}", new Vector2(GraphicsDevice.Viewport.Width / 2 - LanaPixel_24.MeasureString($"{i + 1}: {scores[i]}").Length() / 2, GraphicsDevice.Viewport.Height / 4 + 160f + (40f * i)), Color.White);
            }
        }

        protected void AddScore(int amount) {
            if (Player.IsAlive) {
                Score += amount;
                tempLevel += amount;
            }
        }

        protected void InstantiateProjectile(Texture2D texture) {
            if (IsGameAndPlayerIsAlive())
                Projectiles.Add(new Projectile(texture, Player.position + new Vector2(27f, 0f)));
        }

        protected void InstantiateEnemy() {
            if (IsGameAndPlayerIsAlive()) {
                int randomEnemy = Random.Next(EnemyDictionary.Count);
                Enemies.Add(new Enemy(EnemyDictionary[randomEnemy].texture, EnemyDictionary[randomEnemy].ExplodeTexture, EnemyDictionary[randomEnemy].EnemyType));
            }
        }

        protected void InstantiateFuelBarrel() {
            if (IsGameAndPlayerIsAlive())
                FuelBarrels.Add(new FuelBarrel(FuelBarrel, ExplosionEffect));
        }

        protected void InstantiateAmmoCase() {
            if (IsGameAndPlayerIsAlive())
                AmmoCases.Add(new AmmoCase(AmmoCase));
        }

        protected void InstantiateEntity() {
            if (IsGameAndPlayerIsAlive())
                Entities.Add(new Entity(EntitiesTextuers[Random.Next(EntitiesTextuers.Count)]));
        }

        private bool IsGameAndPlayerIsAlive() => eventManager.gameState == GameState.Game && Player.IsAlive;

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
                ChangeState(out eventManager.gameState, GameState.EndGame);
            }
        }

        public void ChangeState(out GameState gameState, GameState gameStateToChange) {
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

        protected void SaveAndLoadScores() {
            SaveScores();
            LoadScores();
        }

        private void SaveScores() {
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
        }

        private void LoadScores() {
            StreamReader SR = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RR_scores.txt");
            string item;
            while ((item = SR.ReadLine()) != null) {
                int value;
                if (int.TryParse(item, out value))
                    scores.Add(value);
            }
            SR.Close();

            scores.Sort();
            scores.Reverse();
        }

        public static void SetSpeed(int speed) {
            BackgroundMovementSpeed = 4f + speed;
            FallingObjectMovementSpeed = 4f + speed;
            EnemyMovementSpeed = 5f + speed;
            FuelSpeed = 0.25f + ((float)speed / 10);

            int[,] RandomTime = {
                { 600, 2300, } ,
                { 200, 1000, } ,
                { 7000, 8500, }
            };

            if (speed < 0) {
                SetRandomArray(
                    new int[] { RandomTime[0, 0] * Math.Abs(speed), RandomTime[0, 1] * Math.Abs(speed) },
                    new int[] { RandomTime[1, 0] * Math.Abs(speed), RandomTime[1, 1] * Math.Abs(speed) },
                    new int[] { RandomTime[2, 0] * Math.Abs(speed), RandomTime[2, 1] * Math.Abs(speed) });
            } else if (speed > 0) {
                SetRandomArray(
                    new int[] { RandomTime[0, 0] / Math.Abs(speed), RandomTime[0, 1] / Math.Abs(speed) },
                    new int[] { RandomTime[1, 0] / Math.Abs(speed), RandomTime[1, 1] / Math.Abs(speed) },
                    new int[] { RandomTime[2, 0] / Math.Abs(speed), RandomTime[2, 1] / Math.Abs(speed) });
            } else {
                SetRandomArray(
                    new int[] { RandomTime[0, 0], RandomTime[0, 1] },
                    new int[] { RandomTime[1, 0], RandomTime[1, 1] },
                    new int[] { RandomTime[2, 0], RandomTime[2, 1] });
            }
        }

        private static void SetRandomArray(int[] enemyRandomArray,
                                    int[] fuelRandomArray,
                                    int[] ammoRandomArray) {
            eventManager.EnemyRandomArray = enemyRandomArray;
            eventManager.FuelRandomArray = fuelRandomArray;
            eventManager.AmmoRandomArray = ammoRandomArray;
        }
    }
}
