using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Core;
using River_Ride___MG;
using System;

namespace River_Raid.GameObjects.Fuel {
    class FuelBarrel : ExplodeableGameObject {
        public FuelBarrel(Texture2D texture, Texture2D ExplodeTexture) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            position.X = new Random().Next(Main.MinimumObjectPos, Main.MaximumObjectPos);
        }

        public void Update(GameTime gameTime) {
            position.Y += Main.FallingObjectMovementSpeed;
            Update(gameTime, 1);
        }

        public int GetFuelAmount() => Main.Fuel[new Random().Next(Main.Fuel.Count)];
    }
}
