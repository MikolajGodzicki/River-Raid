using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class Player : ExplodeableGameObject {
        bool CanGoLeft = true, CanGoRight = true;
        public int AnimationFrame;
        public int Health;

        float ProjectileTime, ProjectileDelay = 800f;

        public event Action OnFireButtonClick;

        public Player(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.position = new Vector2(500f);
            this.ExplodeTexture = ExplodeTexture;
        }
        public void UpdatePlayer(KeyboardState InputKey, GameTime gameTime) {
            if (IsAlive) {
                if ((InputKey.IsKeyDown(Keys.A) || InputKey.IsKeyDown(Keys.Left)) && CanGoLeft) {
                    position.X -= Config.PlaneMovementSpeed;
                } else if ((InputKey.IsKeyDown(Keys.D) || InputKey.IsKeyDown(Keys.Right)) && CanGoRight) {
                    position.X += Config.PlaneMovementSpeed;
                }

                ProjectileTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ProjectileTime >= ProjectileDelay) {
                    if (InputKey.IsKeyDown(Keys.F)) {
                        OnFireButtonClick?.Invoke();
                        ProjectileTime = 0;
                    }
                }
            }

            base.Update(gameTime);
        }

        public void ExplodePlane() {
            AnimationFrame = 0;
            IsExploding = true;
            IsAlive = false;
            Config.BGMovementSpeed = 0f;
            Config.FuelBarrelSpeed = 0f;
        }
    }
}
