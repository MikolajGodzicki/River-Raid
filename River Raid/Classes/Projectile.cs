using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Projectile {
        public Texture2D ProjectileTexture;
        public Vector2 ProjectilePosition;
        public Projectile(Texture2D ProjectileTexture, Vector2 ProjectilePosition) {
            this.ProjectileTexture = ProjectileTexture;
            this.ProjectilePosition = ProjectilePosition;
        }
        public void UpdateProjectile() {
            ProjectilePosition.Y -= Config.ProjectileSpeed;
        }

        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCount = 1) {
            if (ProjectilePosition.Y <= OtherPosition.Y &&
                ProjectilePosition.X >= OtherPosition.X &&
                ProjectilePosition.X + ProjectileTexture.Width <= OtherPosition.X + (OtherTexture.Width / FrameCount))
                return true;
            return false;
        }
    }
}
