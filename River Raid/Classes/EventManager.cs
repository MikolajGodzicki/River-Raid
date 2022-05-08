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
        float EnemySpawnTime, FuelBarrelSpawnTime, AmmoCaseSpawnTime, EntitySpawnTime;
        int EnemyRandom, FuelBarrelRandom, AmmoCaseRandom, EntityRandom;
        public int[] EnemyRandomArray;
        public int[] FuelRandomArray;
        public int[] AmmoRandomArray;

        public event Action OnEnemySpawnTick, 
            OnFuelBarrelSpawnTick, 
            OnAmmoCaseSpawnTick,
            OnEntitySpawnTick,
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
                    EnemyRandom = new Random().Next(EnemyRandomArray[0], EnemyRandomArray[1]);
                    OnEnemySpawnTick?.Invoke();
                    EnemySpawnTime = 0;
                }

                FuelBarrelSpawnTime += time;
                if (FuelBarrelSpawnTime >= FuelBarrelRandom) {
                    FuelBarrelRandom = new Random().Next(FuelRandomArray[0], FuelRandomArray[1]);
                    OnFuelBarrelSpawnTick?.Invoke();
                    FuelBarrelSpawnTime = 0;
                }

                AmmoCaseSpawnTime += time;
                if (AmmoCaseSpawnTime >= AmmoCaseRandom) {
                    AmmoCaseRandom = new Random().Next(AmmoRandomArray[0], AmmoRandomArray[1]);
                    OnAmmoCaseSpawnTick?.Invoke();
                    AmmoCaseSpawnTime = 0;
                }

                EntitySpawnTime += time;
                if (EntitySpawnTime >= EntityRandom) {
                    EntityRandom = new Random().Next(100, 400);
                    OnEntitySpawnTick?.Invoke();
                    EntitySpawnTime = 0;
                }
            }
        }
    }
}
