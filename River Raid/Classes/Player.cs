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

        public Player(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.position = new Vector2(500f);
            this.ExplodeTexture = ExplodeTexture;
        }
        public void UpdatePlayer(KeyboardState InputKey, GameTime gameTime) {
            
            if (!IsExploding) {
                if ((InputKey.IsKeyDown(Keys.A) || InputKey.IsKeyDown(Keys.Left)) && CanGoLeft) {
                    position.X -= Config.PlaneMovementSpeed;
                } else if ((InputKey.IsKeyDown(Keys.D) || InputKey.IsKeyDown(Keys.Right)) && CanGoRight) {
                    position.X += Config.PlaneMovementSpeed;
                }
            }

            /*
            if (position.X <= 100)
                CanGoLeft = false;
            else
                CanGoLeft = true;

            if (position.X >= 830)
                CanGoRight = false;
            else
                CanGoRight = true;
            */

            base.Update(gameTime);
        }

        public void ExplodePlane() {
            AnimationFrame = 0;
            IsExploding = true;
            Config.BGMovementSpeed = 0f;
            Config.FuelBarrelSpeed = 0f;
        }
    }
}
