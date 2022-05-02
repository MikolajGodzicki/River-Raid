using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class Player : ExplodeableGameObject {
        bool CanGoLeft = true, CanGoRight = true;
        public Texture2D NormalTexture, BlinkingTexture;
        public bool IsImmunity;
        public int Health = 3;

        float ProjectileTime, ProjectileDelay = 800f;
        float MachinegunTime, MachinegunDelay = 200f;
        public int MachinegunMagazine = 30;
        public float ImmunityTime, ImmunityDelay = 2400f;
        float tempAnimationDelay;
        public Texture2D ProjectileTexture, ProjectileMachineGunTexture;

        public event Action<Texture2D> OnFireButtonClick, OnFireMachineGunButtonClick;

        public Player(Texture2D NormalTexture, Texture2D ExplodeTexture, Texture2D BlinkingTexture) {
            this.NormalTexture = NormalTexture;
            this.texture = this.NormalTexture;
            this.position = new Vector2(500f);
            this.ExplodeTexture = ExplodeTexture;
            this.BlinkingTexture = BlinkingTexture;
            tempAnimationDelay = AnimationDelay;
            MovementSpeed = 5f;
            FrameCount = 4;
        }
        public void UpdatePlayer(KeyboardState InputKey, GameTime gameTime) {
            if (IsAlive) {
                if ((InputKey.IsKeyDown(Keys.A) || InputKey.IsKeyDown(Keys.Left)) && CanGoLeft) {
                    position.X -= MovementSpeed;
                } else if ((InputKey.IsKeyDown(Keys.D) || InputKey.IsKeyDown(Keys.Right)) && CanGoRight) {
                    position.X += MovementSpeed;
                }

                if (InputKey.IsKeyDown(Keys.W)) {
                    Main.BackgroundMovementSpeed = 6f;
                    Main.FuelBarrelMovementSpeed = 6f;
                    Main.PlaneMovementSpeed = 7f;
                } else if (InputKey.IsKeyDown(Keys.S)) {
                    Main.BackgroundMovementSpeed = 2f;
                    Main.FuelBarrelMovementSpeed = 2f;
                    Main.PlaneMovementSpeed = 3f;
                } else {
                    Main.BackgroundMovementSpeed = 4f;
                    Main.FuelBarrelMovementSpeed = 4f;
                    Main.PlaneMovementSpeed = 5f;
                }

                ProjectileTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ProjectileTime >= ProjectileDelay) {
                    if (InputKey.IsKeyDown(Keys.Space)) {
                        OnFireButtonClick?.Invoke(ProjectileTexture);
                        ProjectileTime = 0;
                    }
                }

                MachinegunTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (MachinegunTime >= MachinegunDelay) {
                    if (InputKey.IsKeyDown(Keys.V) && MachinegunMagazine > 0) {
                        OnFireMachineGunButtonClick?.Invoke(ProjectileMachineGunTexture);
                        MachinegunMagazine--;
                        MachinegunTime = 0;
                    }
                }

                if (Health <= 0) {
                    IsAlive = false;
                    ExplodePlane();
                }

                ImmunityTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (IsImmunity && ImmunityTime >= ImmunityDelay) {
                    IsImmunity = false;
                }
                if (IsImmunity) {
                    AnimationDelay = tempAnimationDelay * 3;
                } else {
                    AnimationDelay = tempAnimationDelay;
                }
            }

            base.Update(gameTime);
        }

        public void ExplodePlane() {
            AnimationFrame = 0;
            IsExploding = true;
            IsAlive = false;
            Main.BackgroundMovementSpeed = 0f;
            Main.FuelBarrelMovementSpeed = 0f;
        }

        public void DealDamage(int amount = 1) {
            Health -= amount;
            IsImmunity = true;
            ImmunityTime = 0;
        }
    }
}
