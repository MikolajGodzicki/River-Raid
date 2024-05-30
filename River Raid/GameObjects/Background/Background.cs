using Microsoft.Xna.Framework;
using River_Raid.Core;
using River_Ride___MG;

namespace River_Raid.GameObjects.Background {
    class Background {
        public int id;
        public BackgroundTexture backgroundTexture;
        public Vector2 position = new Vector2();

        public Background(BackgroundTexture backgroundTexture, int id = 0) {
            this.backgroundTexture = backgroundTexture;
            position = new Vector2(0f);
            this.id = id;
        }

        public void Update() {
            position.Y += Main.BackgroundMovementSpeed;
        }

        public bool CheckCollision(GameObject gameObject, int FrameCountX = 4) {
            if ((gameObject.position.X <= backgroundTexture.CollisionPoints[0] ||
                gameObject.position.X + gameObject.texture.Width / FrameCountX >= backgroundTexture.CollisionPoints[1]) &&
                gameObject.position.Y <= position.Y + backgroundTexture.texture.Height &&
                gameObject.position.Y + gameObject.texture.Height >= position.Y)
                return true;
            return false;
        }
    }
}