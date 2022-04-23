using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Background : GameObject {
        public int SpawnMinPosition, SpawnMaxPosition;
        public int id;

        public Background(Texture2D texture, int id) {
            this.texture = texture;
            this.position = new Vector2(-300, 0f);
            this.id = id;
        }

        public void UpdatePosition() {
            position.Y += Config.BGMovementSpeed;
            if (position.Y >= texture.Height) {
                position.Y = -texture.Height;
            }
        }
    }
}
