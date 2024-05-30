using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Raid.StateSystem;
using River_Ride___MG;
using System;

namespace River_Raid.Core
{
    public class EventManager
    {
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

        public EventManager()
        {
            gameState = GameState.Menu;
        }
        public void Update(GameTime gameTime, KeyboardState InputKey)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (InputKey.IsKeyDown(Keys.Enter))
            {
                ChangeGameState(gameState == GameState.Menu, GameState.Game);
                ChangeGameState(gameState == GameState.EndGame, GameState.Game, OnRestartGame);
            }

            HandleAudioManagement(time, InputKey);

            if (gameState == GameState.Game)
            {
                EnemySpawnTime += time;
                if (EnemySpawnTime >= EnemyRandom)
                {
                    EnemyRandom = new Random().Next(EnemyRandomArray[0], EnemyRandomArray[1]);
                    OnEnemySpawnTick?.Invoke();
                    EnemySpawnTime = 0;
                }

                FuelBarrelSpawnTime += time;
                if (FuelBarrelSpawnTime >= FuelBarrelRandom)
                {
                    FuelBarrelRandom = new Random().Next(FuelRandomArray[0], FuelRandomArray[1]);
                    OnFuelBarrelSpawnTick?.Invoke();
                    FuelBarrelSpawnTime = 0;
                }

                AmmoCaseSpawnTime += time;
                if (AmmoCaseSpawnTime >= AmmoCaseRandom)
                {
                    AmmoCaseRandom = new Random().Next(AmmoRandomArray[0], AmmoRandomArray[1]);
                    OnAmmoCaseSpawnTick?.Invoke();
                    AmmoCaseSpawnTime = 0;
                }

                EntitySpawnTime += time;
                if (EntitySpawnTime >= EntityRandom)
                {
                    EntityRandom = new Random().Next(100, 400);
                    OnEntitySpawnTick?.Invoke();
                    EntitySpawnTime = 0;
                }
            }
        }

        private void ChangeGameState(bool condition, GameState targetState, Action optionalCallback = null) {
            if (condition) {
                Main.audioManager.PlaySound("Select");
                gameState = targetState;

                optionalCallback?.Invoke();
            }
        }

        private void HandleAudioManagement(float time, KeyboardState InputKey) {
            VolumeChangeTime += time;
            if (VolumeChangeTime >= 100) {
                Main.audioManager.Update(InputKey);
                VolumeChangeTime = 0;
            }
        }
    }
}
