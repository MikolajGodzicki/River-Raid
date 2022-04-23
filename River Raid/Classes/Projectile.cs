using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Projectile {
        public Texture2D texture;
        public Vector2 position;
        public Projectile(Texture2D texture, Vector2 position) {
            this.texture = texture;
            this.position = position;
        }
        public void UpdateProjectile() {
            position.Y -= Config.ProjectileSpeed;
        }

        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCount = 4) {
            if (position.Y + texture.Height >= OtherPosition.Y + 20f &&
                position.Y <= OtherPosition.Y + OtherTexture.Height &&
                position.X <= OtherPosition.X + (OtherTexture.Width / FrameCount) &&
                position.X + texture.Width >= OtherPosition.X)
                return true;
            return false;
        }
    }
}
