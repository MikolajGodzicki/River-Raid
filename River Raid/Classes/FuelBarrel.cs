using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class FuelBarrel {
        public Texture2D texture;
        public Texture2D ExplodeTexture;
        public Vector2 position = new Vector2(0f, -100f);
        public bool IsExploding, IsExploded;
        float AnimationTime, AnimationDelay = 50f;
        int AnimationFrame;
        public Rectangle ExplodeAnimation;

        public FuelBarrel(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            this.position.X = new Random().Next(Config.MinimumObjectPos, Config.MaximumObjectPos);
        }

        public void UpdateFuelBarrel(GameTime gameTime) {
            this.position.Y += Config.FuelBarrelSpeed;

            if (IsExploding) {
                AnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (AnimationTime >= AnimationDelay) {
                    if (AnimationFrame >= 3) {
                        IsExploded = true;
                        IsExploding = false;
                    } else {
                        AnimationFrame++;
                    }

                    AnimationTime = 0;
                }

                ExplodeAnimation = new Rectangle(ExplodeTexture.Width / 4 * AnimationFrame, 0, ExplodeTexture.Width / 4, ExplodeTexture.Height);
            }
        }

        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCount = 4) {
            if (position.Y + texture.Height >= OtherPosition.Y + 20f &&
                position.Y <= OtherPosition.Y + OtherTexture.Height &&
                position.X <= OtherPosition.X + (OtherTexture.Width / FrameCount) &&
                position.X + texture.Width >= OtherPosition.X)
                return true;
            return false;
        }
        public void Explode(SpriteBatch spriteBatch) {
            spriteBatch.Draw(ExplodeTexture, position - new Vector2(30f), ExplodeAnimation, Color.White, 0f, new Vector2(), 0.5f, SpriteEffects.None, 0f);
        }


        public int GetFuelAmount() => Config.Fuel[new Random().Next(Config.Fuel.Count)];
    }
}
