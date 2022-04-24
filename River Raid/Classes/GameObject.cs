using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class GameObject {
        public Texture2D texture;
        public Vector2 position = new Vector2(0f, -100f);

        float AnimationTime, AnimationDelay = 100f;
        int AnimationFrame;
        public Rectangle ObjectAnimation;
        public event Action<int> OnAnimationTick;

        public void Update(GameTime gameTime, int FrameCountX = 4) {
            AnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (AnimationTime >= AnimationDelay) {
                if (AnimationFrame >= FrameCountX - 1) {
                    AnimationFrame = 0;
                } else {
                    AnimationFrame++;
                    OnAnimationTick?.Invoke(5);
                }

                AnimationTime = 0;
            }

            ObjectAnimation = new Rectangle(texture.Width / FrameCountX * AnimationFrame, 0, texture.Width / FrameCountX, texture.Height);
        }
        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCountX = 4) {
            if (position.Y + texture.Height >= OtherPosition.Y &&
                position.Y <= OtherPosition.Y + OtherTexture.Height &&
                position.X <= OtherPosition.X + (OtherTexture.Width / FrameCountX) &&
                position.X + texture.Width >= OtherPosition.X)
                return true;
            return false;
        }
    }
}
