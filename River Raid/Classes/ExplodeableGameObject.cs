using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class ExplodeableGameObject : GameObject {
        public Texture2D ExplodeTexture;
        public bool IsExploding, IsExploded;
        public bool IsAlive = true;

        float ExplosionAnimationTime, ExplosionAnimationDelay = 100f;
        int ExplosionAnimationFrame;
        public Rectangle ExplodeAnimation;

        public void Update(GameTime gameTime, int FrameCountX = 4, int ExplosionFrameCountX = 4) {
            if (IsExploding) {
                ExplosionAnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ExplosionAnimationTime >= ExplosionAnimationDelay) {
                    if (ExplosionAnimationFrame >= ExplosionFrameCountX - 1) {
                        IsExploded = true;
                        IsExploding = false;
                        ExplosionAnimationFrame = 0;
                    } else {
                        ExplosionAnimationFrame++;
                    }

                    ExplosionAnimationTime = 0;
                }

                ExplodeAnimation = new Rectangle(ExplodeTexture.Width / ExplosionFrameCountX * ExplosionAnimationFrame, 0, ExplodeTexture.Width / ExplosionFrameCountX, ExplodeTexture.Height);
            }

            base.Update(gameTime, FrameCountX);
        }

        public void Explode(SpriteBatch spriteBatch, Vector2 OffsetPosition = new Vector2(), float scale = 1f) {
            spriteBatch.Draw(ExplodeTexture, position - OffsetPosition, ExplodeAnimation, Color.White, 0f, new Vector2(), scale, SpriteEffects.None, 0f);
        }
    }
}
