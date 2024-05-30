using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Raid.Core;
using River_Ride___MG;
using System;

namespace River_Raid.GameObjects.Interactable {
    public class Player : ExplodeableGameObject {
        bool CanGoLeft = true, CanGoRight = true;

        public Texture2D NormalTexture, BlinkingTexture;
        public bool IsImmunity;
        public int HealthAtStart, Health = 3;
        public int MaxFuel = 320;

        float ProjectileTime, ProjectileDelay = 800f;
        float MachinegunTime, MachinegunDelay = 150f;
        public int MachinegunMagazineAtStart, MachinegunMagazine = 30;
        public float ImmunityTime, ImmunityDelay = 2400f;
        float tempAnimationDelay;
        public Texture2D ProjectileTexture, ProjectileMachineGunTexture;

        public event Action<Texture2D> OnFireButtonClick, OnFireMachineGunButtonClick;

        public Player(Texture2D NormalTexture, Texture2D ExplodeTexture, Texture2D BlinkingTexture) {
            this.NormalTexture = NormalTexture;
            this.ExplodeTexture = ExplodeTexture;
            this.BlinkingTexture = BlinkingTexture;
            InitValues();
        }

        private void InitValues() {
            texture = this.NormalTexture;
            position = new Vector2(500f);
            MovementSpeed = 5;
            FrameCount = 4;
            HealthAtStart = Health;
            MachinegunMagazineAtStart = MachinegunMagazine;
            tempAnimationDelay = AnimationDelay;
        }

        public void UpdatePlayer(KeyboardState InputKey, GameTime gameTime) {
            if (IsAlive) {
                float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                HandleMovement(InputKey);

                HandleProjectileFire(InputKey, time);

                HandleMachinegunFire(InputKey, time);

                CheckPlayerHealth();

                HandleImmunity(time);
            }

            Update(gameTime);
        }

        private void CheckPlayerHealth() {
            if (Health <= 0) {
                IsAlive = false;
                ExplodePlane();
            }
        }

        private void HandleImmunity(float time) {
            ImmunityTime += time;
            if (IsImmunity && ImmunityTime >= ImmunityDelay) {
                IsImmunity = false;
            }
            if (IsImmunity) {
                AnimationDelay = tempAnimationDelay * 3;
            } else {
                AnimationDelay = tempAnimationDelay;
            }
        }

        private void HandleMachinegunFire(KeyboardState inputKey, float time) {
            MachinegunTime += time;
            if (MachinegunTime >= MachinegunDelay) {
                if (inputKey.IsKeyDown(Keys.V)) {
                    if (MachinegunMagazine > 0) {
                        OnFireMachineGunButtonClick?.Invoke(ProjectileMachineGunTexture);
                        MachinegunMagazine--;
                        Main.audioManager.PlaySound("MachineGun");
                    } else {
                        Main.audioManager.PlaySound("EmptyWeapon");
                    }
                    MachinegunTime = 0;
                }
            }
        }

        private void HandleProjectileFire(KeyboardState inputKey, float time) {
            ProjectileTime += time;
            if (ProjectileTime >= ProjectileDelay) {
                if (inputKey.IsKeyDown(Keys.Space)) {
                    OnFireButtonClick?.Invoke(ProjectileTexture);
                    Main.audioManager.PlaySound("Shoot");
                    ProjectileTime = 0;
                }
            }
        }

        private void HandleMovement(KeyboardState inputKey) {
            if ((inputKey.IsKeyDown(Keys.A) || inputKey.IsKeyDown(Keys.Left)) && CanGoLeft) {
                position.X -= MovementSpeed;
            } else if ((inputKey.IsKeyDown(Keys.D) || inputKey.IsKeyDown(Keys.Right)) && CanGoRight) {
                position.X += MovementSpeed;
            }

            if (inputKey.IsKeyDown(Keys.W) || inputKey.IsKeyDown(Keys.Up)) {
                Main.SetSpeed(2);
                ScoreAdding = 10;
            } else if (inputKey.IsKeyDown(Keys.S) || inputKey.IsKeyDown(Keys.Down)) {
                Main.SetSpeed(-2);
                ScoreAdding = 2;
            } else {
                Main.SetSpeed(0);
                ScoreAdding = 5;
            }
        }

        public void ExplodePlane() {
            AnimationFrame = 0;
            IsExploding = true;
            IsAlive = false;
            Main.BackgroundMovementSpeed = 0f;
            Main.FallingObjectMovementSpeed = 0f;
            Main.audioManager.PlaySound("Explosion");
        }

        public void DealDamage(int amount = 1) {
            Health -= amount;
            IsImmunity = true;
            ImmunityTime = 0;
        }

        public void AddAmmo(int amount) {
            int tempAmount = amount;
            if (MachinegunMagazine + amount > 90)
                tempAmount -= MachinegunMagazine + amount - 90;
            MachinegunMagazine += tempAmount;
        }
    }
}
