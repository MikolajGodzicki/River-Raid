using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class EnemyPlane : ExplodeableGameObject {
        public EnemyPlane(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            this.position.X = new Random().Next(Main.MinimumObjectPos, Main.MaximumObjectPos);
            FrameCount = 4;
        }

        public void UpdateEnemy(GameTime gameTime) {
            if (!IsExploding)
                position.Y += Main.PlaneMovementSpeed;
            else
                position.Y += Main.BackgroundMovementSpeed;

            base.Update(gameTime);
        }
    }
}
