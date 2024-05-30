using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Core;
using River_Ride___MG;
using System;

namespace River_Raid.GameObjects {
    class Entity : GameObject {
        public Entity(Texture2D texture) {
            int[,] spawnPoints = {
                { 0, Main.MinimumObjectPos - 100 },
                { Main.MaximumObjectPos + 100, Main.PrefferedWidth },
            };
            int random = new Random().Next(0, 2);
            position.X = new Random().Next(spawnPoints[random, 0], spawnPoints[random, 1]);
            this.texture = texture;
        }

        public void Update(GameTime gameTime) {
            position.Y += Main.FallingObjectMovementSpeed;
            Update(gameTime, 1);
        }
    }
}
