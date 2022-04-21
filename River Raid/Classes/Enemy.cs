using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class Enemy {
        public Texture2D EnemyTexture;
        public Vector2 EnemyPosition = new Vector2(0f, -80f);
        public Enemy(Texture2D EnemyTexture) {
            this.EnemyTexture = EnemyTexture;
            this.EnemyPosition.X = new Random().Next(Config.MinimumEnemyPos, Config.MaximumEnemyPos);
        }

        public void UpdateEnemy() {
            EnemyPosition.Y += Config.EnemySpeed;
        }
    }
}
