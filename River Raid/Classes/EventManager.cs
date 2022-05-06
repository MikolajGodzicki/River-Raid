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
            GameOver,
            EndGame
        }

        public GameState gameState;
        float VolumeChangeTime;
        float EnemySpawnTime, FuelBarrelSpawnTime, AmmoCaseSpawnTime;
        int EnemyRandom, FuelBarrelRandom, AmmoCaseRandom;

        public event Action OnEnemySpawnTick, 
            OnFuelBarrelSpawnTick, 
            OnAmmoCaseSpawnTick,
            OnRestartGame;

        public EventManager() {
            gameState = GameState.Menu;
        }
        public void Update(GameTime gameTime, KeyboardState InputKey) {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (gameState == GameState.Menu && InputKey.IsKeyDown(Keys.Enter)) {
                Main.audioManager.PlaySound("Select"); 
                gameState = GameState.Game;
            }
                

            if (gameState == GameState.EndGame && InputKey.IsKeyDown(Keys.Enter)) {
                Main.audioManager.PlaySound("Select");
                gameState = GameState.Game;
                OnRestartGame.Invoke();
            }
            VolumeChangeTime += time;
            if (VolumeChangeTime >= 100) {
                Main.audioManager.Update(InputKey);
                VolumeChangeTime = 0;
            }

            if (gameState == GameState.Game) {
                EnemySpawnTime += time;
                if (EnemySpawnTime >= EnemyRandom) {
                    EnemyRandom = new Random().Next(600, 2300);
                    OnEnemySpawnTick?.Invoke();
                    EnemySpawnTime = 0;
                }

                FuelBarrelSpawnTime += time;
                if (FuelBarrelSpawnTime >= FuelBarrelRandom) {
                    FuelBarrelRandom = new Random().Next(300, 700);
                    OnFuelBarrelSpawnTick?.Invoke();
                    FuelBarrelSpawnTime = 0;
                }

                AmmoCaseSpawnTime += time;
                if (AmmoCaseSpawnTime >= AmmoCaseRandom) {
                    AmmoCaseRandom = new Random().Next(3000, 3500);
                    OnAmmoCaseSpawnTick?.Invoke();
                    AmmoCaseSpawnTime = 0;
                }
            }
        }
    }
}
