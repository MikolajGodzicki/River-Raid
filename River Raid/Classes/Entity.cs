using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class Entity : GameObject {
        public Entity(Texture2D texture) {
            int[,] spawnPoints = {
                {0, Main.MinimumObjectPos - 100 },
                {Main.MaximumObjectPos + 100, Main.PrefferedWidth },
            };
            int random = new Random().Next(0, 2);
            this.position.X = new Random().Next(spawnPoints[random, 0], spawnPoints[random, 1]);
            this.texture = texture;
        }

        public void Update(GameTime gameTime) {
            this.position.Y += Main.FallingObjectMovementSpeed;
            base.Update(gameTime, 1);
        }
    }
}
