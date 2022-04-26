using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Classes;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Background : GameObject {
        //public int SpawnMinPosition, SpawnMaxPosition;
        public int[] CollisionPositions;
        public int id;

        public Background(Texture2D texture, int id, int[] CollisionPositions) {
            this.texture = texture;
            this.position = new Vector2(0f, texture.Height);
            this.id = id;
            this.CollisionPositions = CollisionPositions;
        }

        public void UpdatePosition(int Count) {
            position.Y += Main.BackgroundMovementSpeed;
            if (position.Y >= texture.Height * Count) {
                position.Y = -texture.Height * Count;
            }
        }

        public new bool CheckCollision(GameObject gameObject) {
            return false;
        }
    }
}
