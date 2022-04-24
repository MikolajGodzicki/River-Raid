using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class EnemyPlane : ExplodeableGameObject {
        public EnemyPlane(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            this.position.X = new Random().Next(Config.MinimumObjectPos, Config.MaximumObjectPos);
        }

        public void UpdateEnemy(GameTime gameTime) {
            if (!IsExploding)
                position.Y += Config.EnemyMovementSpeed;
            else
                position.Y += Config.BGMovementSpeed;
            base.Update(gameTime);
        }
    }
}
