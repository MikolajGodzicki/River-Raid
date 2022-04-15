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
    }
}
