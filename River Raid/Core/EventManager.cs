using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using River_Raid.StateSystem;
using River_Ride___MG;
using System;

namespace River_Raid.Core {
    public class EventManager {
        public GameState gameState;

        float VolumeChangeTime;

        float EnemySpawnTime,
            FuelBarrelSpawnTime,
            AmmoCaseSpawnTime,
            EntitySpawnTime;

        int EnemyRandom,
            FuelBarrelRandom,
            AmmoCaseRandom,
            EntityRandom;

        public int[] EnemyRandomArray,
            FuelRandomArray,
            AmmoRandomArray;

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

            if (InputKey.IsKeyDown(Keys.Enter)) {
                ChangeGameState(gameState == GameState.Menu, GameState.Game);
                ChangeGameState(gameState == GameState.EndGame, GameState.Game, OnRestartGame);
            }

            HandleAudioManagement(time, InputKey);

            if (gameState == GameState.Game) {
                HandleAllSpawnTimes(time);
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

        private void HandleAllSpawnTimes(float time) {
            HandleSpawnTime(ref EnemySpawnTime, time, ref EnemyRandom, EnemyRandomArray, OnEnemySpawnTick);

            HandleSpawnTime(ref FuelBarrelSpawnTime, time, ref FuelBarrelRandom, FuelRandomArray, OnFuelBarrelSpawnTick);

            HandleSpawnTime(ref AmmoCaseSpawnTime, time, ref AmmoCaseRandom, AmmoRandomArray, OnAmmoCaseSpawnTick);

            HandleSpawnTime(ref EntitySpawnTime, time, ref EntityRandom, new int[] { 100, 400 }, OnEntitySpawnTick);

        }

        private void HandleSpawnTime(ref float spawnTime, float time, ref int randomTime, int[] spawnTimeArray, Action callback) {
            spawnTime += time;
            if (spawnTime >= randomTime) {
                randomTime = GetRandomIntFromArray(spawnTimeArray);
                callback?.Invoke();
                spawnTime = 0;
            }
        }
        private int GetRandomIntFromArray(int[] randomArray) {
            return new Random().Next(randomArray[0], randomArray[1]);
        }
    }
}
