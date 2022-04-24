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
        float EnemySpawnTime;
        float FuelBarrelSpawnTime;

        public event Action OnEnemySpawnTick, OnFuelBarrelSpawnTick;

        public EventManager() {
            gameState = GameState.Menu;
        }
        public void Update(GameTime gameTime, KeyboardState InputKey) {
            if (gameState == GameState.Menu && InputKey.IsKeyDown(Keys.Enter))
                gameState = GameState.Game;

            EnemySpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EnemySpawnTime >= new Random().Next(1500, 2300)) {
                OnEnemySpawnTick?.Invoke();
                EnemySpawnTime = 0;
            }

            FuelBarrelSpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (FuelBarrelSpawnTime >= new Random().Next(500, 1400)) {
                OnFuelBarrelSpawnTick?.Invoke();
                FuelBarrelSpawnTime = 0;
            }
        }
    }
}
