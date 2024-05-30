using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Core;
using River_Raid.GameObjects.Interactable;
using River_Ride___MG;
using System;

namespace River_Raid.GameObjects.Enemy {
    class Enemy : ExplodeableGameObject {
        private EnemyType enemyType;
        public EnemyType EnemyType { get => enemyType; set => enemyType = value; }

        public Enemy(Texture2D texture, Texture2D explodeTexture, EnemyType enemyType = EnemyType.Plane) {
            this.texture = texture;
            ExplodeTexture = explodeTexture;
            EnemyType = enemyType;

            InitPosition();

            FrameCount = 4;
        }

        private void InitPosition() {

            if (EnemyType == EnemyType.HelicopterLeftSide)
                position.X = -30f;
            else if (EnemyType == EnemyType.HelicopterRightSide)
                position.X = 1024f;
            else
                position.X = new Random().Next(Main.MinimumObjectPos, Main.MaximumObjectPos);
        }

        public void Update(GameTime gameTime, Player player) {
            switch (EnemyType) {
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
                        position.X += Main.EnemyHelicopterMovementSpeed - 2;
                    } else
                        position.Y += Main.BackgroundMovementSpeed;
                    break;
                case EnemyType.HelicopterRightSide:
                    if (!IsExploding) {
                        if (player.IsAlive)
                            position.Y += Main.EnemyMovementSpeed - 1;
                        position.X -= Main.EnemyHelicopterMovementSpeed - 2;
                    } else
                        position.Y += Main.BackgroundMovementSpeed;
                    break;
                default:
                    break;
            }
            Update(gameTime);
        }
    }
}
