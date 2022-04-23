using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Background {
        public Texture2D texture;
        public Vector2 position = new Vector2(-300f, 0f);
        public int SpawnMinPosition, SpawnMaxPosition;
        public int id;

        public Background(Texture2D texture, int id) {
            this.texture = texture;
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
