using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class FuelBarrel : ExplodeableGameObject {
        

        public FuelBarrel(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            this.position.X = new Random().Next(Config.MinimumObjectPos, Config.MaximumObjectPos);
        }

        public void UpdateFuelBarrel(GameTime gameTime) {
            this.position.Y += Config.FuelBarrelSpeed;
            base.Update(gameTime);
        }

        public int GetFuelAmount() => Config.Fuel[new Random().Next(Config.Fuel.Count)];
    }
}
