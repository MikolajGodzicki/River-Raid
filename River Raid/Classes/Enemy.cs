using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Ride___MG;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid.Classes {
    class Enemy : ExplodeableGameObject {
        public EnemyType enemyType;
        public enum EnemyType {
            Plane,
            HelicopterLeftSide,
            HelicopterRightSide
        }
        public Enemy(Texture2D texture, Texture2D ExplodeTexture, EnemyType enemyType = EnemyType.Plane) {
            this.texture = texture;
            this.ExplodeTexture = ExplodeTexture;
            this.enemyType = enemyType;

            if (enemyType == EnemyType.HelicopterLeftSide)
                this.position.X = -30f;
            else if (enemyType == EnemyType.HelicopterRightSide)
                this.position.X = 1024f;
            else
                this.position.X = new Random().Next(Main.MinimumObjectPos, Main.MaximumObjectPos);
            FrameCount = 4;
        }

        public void UpdateEnemy(GameTime gameTime, Player player) {
            switch (enemyType) {
                case EnemyType.Plane:
                    if (!IsExploding)
                        position.Y += Main.EnemyMovementSpeed;
                    else
                        position.Y += Main.BackgroundMovementSpeed;
                    break;
                case EnemyType.HelicopterLeftSide:
                    if (!IsExploding) {
                        if (player.IsAlive)
                            position.Y += Main.EnemyMovementSpeed - 1;
                        position.X += Main.EnemyMovementSpeed - 2;
                    }
                    else
                        position.Y += Main.BackgroundMovementSpeed;
                    break;
                case EnemyType.HelicopterRightSide:
                    if (!IsExploding) {
                        if (player.IsAlive)
                            position.Y += Main.EnemyMovementSpeed - 1;
                        position.X -= Main.EnemyMovementSpeed - 2;
                    } else
                        position.Y += Main.BackgroundMovementSpeed;
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }
    }
}
