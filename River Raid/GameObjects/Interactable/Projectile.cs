using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Core;

namespace River_Raid.GameObjects.Interactable {
    class Projectile : GameObject {
        public Projectile(Texture2D texture, Vector2 position) {
            this.texture = texture;
            this.position = position;
            MovementSpeed = 6;
        }
        public void Update(GameTime gameTime) {
            position.Y -= MovementSpeed;
            Update(gameTime, 1);
        }
    }
}
