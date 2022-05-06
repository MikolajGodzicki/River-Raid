using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class AmmoCase : GameObject {
        public AmmoCase(Texture2D texture) {
            this.texture = texture;
            this.position.X = new Random().Next(Main.MinimumObjectPos, Main.MaximumObjectPos);
        }

        public void Update(GameTime gameTime) {
            this.position.Y += Main.FallingObjectMovementSpeed;
            base.Update(gameTime, 1);
        }
    }
}
