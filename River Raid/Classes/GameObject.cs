using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class GameObject {
        public Texture2D texture;
        public Vector2 position = new Vector2(0f, -100f);
        public int MovementSpeed = 4;
        public int ScoreAdding = 5;

        protected float AnimationTime, AnimationDelay = 100f;
        public int AnimationFrame;
        public int FrameCount = 1;
        public Rectangle ObjectAnimation;
        public event Action<int> OnAnimationTick;

        public void Update(GameTime gameTime, int FrameCountX = 4) {
            if (FrameCount > 1) {
                AnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (AnimationTime >= AnimationDelay) {
                    if (AnimationFrame >= FrameCountX - 1) {
                        AnimationFrame = 0;
                    } else {
                        AnimationFrame++;
                        OnAnimationTick?.Invoke(ScoreAdding);
                    }

                    AnimationTime = 0;
                }
            }

            ObjectAnimation = new Rectangle(texture.Width / FrameCountX * AnimationFrame, 0, texture.Width / FrameCountX, texture.Height);
        }
        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCountX = 4) {
            if (position.Y + texture.Height >= OtherPosition.Y &&
                position.Y <= OtherPosition.Y + OtherTexture.Height &&
                position.X <= OtherPosition.X + (OtherTexture.Width / FrameCountX) &&
                position.X + texture.Width / FrameCount >= OtherPosition.X)
                return true;
            return false;
        }

        public bool CheckCollision(GameObject gameObject) {
            if (position.Y + texture.Height >= gameObject.position.Y &&
                position.Y <= gameObject.position.Y + gameObject.texture.Height &&
                position.X <= gameObject.position.X + (gameObject.texture.Width / gameObject.FrameCount) &&
                position.X + texture.Width / FrameCount >= gameObject.position.X)
                return true;
            return false;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, position, ObjectAnimation, Color.White);
        }
    }
}
