using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Core;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.GameObjects.Interactable
{
    class AmmoCase : GameObject
    {
        public AmmoCase(Texture2D texture)
        {
            this.texture = texture;
            position.X = new Random().Next(Main.MinimumObjectPos, Main.MaximumObjectPos);
        }

        public void Update(GameTime gameTime)
        {
            position.Y += Main.FallingObjectMovementSpeed;
            Update(gameTime, 1);
        }
    }
}
