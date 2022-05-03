using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Ride___MG;
using System;

namespace River_Raid.Classes {
    public class EventManager {
        public enum GameState {
            Menu,
            Game,
            EndGame
        }

        public GameState gameState;
        float EnemySpawnTime;
        float FuelBarrelSpawnTime;

        public event Action OnEnemySpawnTick, OnFuelBarrelSpawnTick, OnRestartGame;

        public EventManager() {
            gameState = GameState.Menu;
        }
        public void Update(GameTime gameTime, KeyboardState InputKey) {
            if (gameState == GameState.Menu && InputKey.IsKeyDown(Keys.Enter))
                gameState = GameState.Game;

            if (gameState == GameState.EndGame && InputKey.IsKeyDown(Keys.Enter)) {
                gameState = GameState.Game;
                OnRestartGame.Invoke();
            }

            EnemySpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EnemySpawnTime >= new Random().Next(600, 2300)) {
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
