using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Background {
        public Texture2D BG_texture;
        public Vector2 BG_position = new Vector2(-300f, 0f);
        public int id;

        public Background(Texture2D BG_texture, int id) {
            this.BG_texture = BG_texture;
            this.id = id;
        }

        public void UpdatePosition() {
            BG_position.Y += Config.BG_speed;
            if (BG_position.Y >= BG_texture.Height) {
                BG_position.Y = -BG_texture.Height;
            }
        }
    }
}
