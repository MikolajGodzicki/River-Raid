using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class FuelBarrel {
        public Texture2D texture;
        public Vector2 position = new Vector2(0f, -100f);

        public FuelBarrel(Texture2D texture) {
            this.texture = texture;
            this.position.X = new Random().Next(Config.MinimumObjectPos, Config.MaximumObjectPos);
        }

        public void UpdateFuelBarrel() {
            this.position.Y += Config.FuelBarrelSpeed;
        }

        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCount = 1) {
            if (position.Y <= OtherPosition.Y &&
                position.Y + texture.Height >= OtherPosition.Y + (OtherTexture.Height / FrameCount) &&
                position.X >= OtherPosition.X &&
                position.X + texture.Width <= OtherPosition.X + (OtherTexture.Width / FrameCount))
                return true;
            return false;
        }
    }
}
