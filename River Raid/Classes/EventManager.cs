using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace River_Raid.Classes {
    class EventManager {
        public enum GameState {
            Menu,
            Game
        }

        public GameState gameState;
        float EnemySpawnTime, EnemySpawnDelay = 2300f;
        float FuelBarrelSpawnTime, FuelBarrelSpawnDelay = 1400f;

        public event Action OnEnemySpawnTick, OnFuelBarrelSpawnTick;

        public EventManager() {
            gameState = GameState.Menu;
        }
        public void Update(GameTime gameTime, KeyboardState InputKey) {
            if (gameState == GameState.Menu && InputKey.IsKeyDown(Keys.Enter))
                gameState = GameState.Game;

            EnemySpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EnemySpawnTime >= EnemySpawnDelay) {
                OnEnemySpawnTick?.Invoke();
                EnemySpawnTime = 0;
            }

            FuelBarrelSpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (FuelBarrelSpawnTime >= FuelBarrelSpawnDelay) {
                OnFuelBarrelSpawnTick?.Invoke();
                FuelBarrelSpawnTime = 0;
            }
        }
    }
}
