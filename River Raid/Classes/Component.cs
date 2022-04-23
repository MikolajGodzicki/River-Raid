using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class GameObject {
        public Texture2D texture;
        public Vector2 position = new Vector2(0f, -100f);

        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCount = 4) {
            if (position.Y + texture.Height >= OtherPosition.Y &&
                position.Y <= OtherPosition.Y + OtherTexture.Height - 20f &&
                position.X <= OtherPosition.X + (OtherTexture.Width / FrameCount) &&
                position.X + texture.Width >= OtherPosition.X)
                return true;
            return false;
        }
    }
}
