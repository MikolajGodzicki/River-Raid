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
        public int AnimationFrame;
        public int Health = 3;

        float ProjectileTime, ProjectileDelay = 800f;

        public event Action OnFireButtonClick;

        public Player(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.position = new Vector2(500f);
            this.ExplodeTexture = ExplodeTexture;
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

                ProjectileTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ProjectileTime >= ProjectileDelay) {
                    if (InputKey.IsKeyDown(Keys.F)) {
                        OnFireButtonClick?.Invoke();
                        ProjectileTime = 0;
                    }
                }

                if (Health <= 0) {
                    IsAlive = false;
                    ExplodePlane();
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
    }
}
