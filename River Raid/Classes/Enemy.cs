using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class Enemy {
        public Texture2D texture;
        public Texture2D ExplodeTexture;
        public bool IsExploding, IsExploded;
        float AnimationTime, AnimationDelay = 50f;
        int AnimationFrame;
        public Rectangle ExplodeAnimation;
        public Vector2 position = new Vector2(0f, -80f);
        public Enemy(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            this.position.X = new Random().Next(Config.MinimumObjectPos, Config.MaximumObjectPos);
        }

        public void UpdateEnemy(GameTime gameTime) {
            position.Y += Config.EnemyMovementSpeed;

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

        public void Explode(SpriteBatch spriteBatch) {
            spriteBatch.Draw(ExplodeTexture, position - new Vector2(30f), ExplodeAnimation, Color.White, 0f, new Vector2(), 0.5f, SpriteEffects.None, 0f);
        }
    }
}
