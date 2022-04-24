using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class FuelBarrel : ExplodeableGameObject {
        public FuelBarrel(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            this.position.X = new Random().Next(Main.MinimumObjectPos, Main.MaximumObjectPos);
        }

        public void UpdateFuelBarrel(GameTime gameTime) {
            this.position.Y += Main.FuelBarrelMovementSpeed;
            base.Update(gameTime);
        }

        public int GetFuelAmount() => Main.Fuel[new Random().Next(Main.Fuel.Count)];
    }
}
