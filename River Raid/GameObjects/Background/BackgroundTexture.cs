using Microsoft.Xna.Framework.Graphics;

namespace River_Raid.GameObjects.Background {

    public class BackgroundTexture {
        public Texture2D texture;
        public int[] CollisionPoints;

        public BackgroundTexture(Texture2D texture, int[] CollisionPoints) {
            this.CollisionPoints = CollisionPoints;
            this.texture = texture;
        }
    }
}
