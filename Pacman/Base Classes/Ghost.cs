using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.GameObjects;
using Pacman.Managers;
using Pacman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Pacman.Base_Classes
{
    public class Ghost
    {
        // Point
        protected Point CurrentTile;
        protected Point? DestinationTile;

        // Bool
        protected bool IsMoving;
        protected bool IsActive;

        // Other
        protected Texture2D Tex;
        protected Rectangle DestinationRec;
        protected Vector2 Vel;
        protected float DrawLayer;
        protected int? MoveDirection;
        protected Tile[,] TileMap;
        protected GhostAnimationManager AnimationManager;

        public void Update(float deltaTime, Player player)
        {
            if (IsActive)
            {
                EnemyLogic();

                AnimationManager.Update(deltaTime, MoveDirection.Value);

                if (DestinationRec.Intersects(player.DestinationRec))
                    player.GetHurt();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Tex, DestinationRec, AnimationManager.GetCurrentFrame(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, DrawLayer);
        }

        protected Rectangle[] CreateSpriteFrames(int spriteRow)
        {
            Rectangle[] spriteFrames = new Rectangle[8];

            for (int x = 0; x < 8; x++)
                spriteFrames[x] = new(4 + 16 * x, spriteRow, 14, 14);

            return spriteFrames;
        }

        protected void Move()
        {
            Tile desTile = TileMap[DestinationTile.Value.Y, DestinationTile.Value.X];
            switch (MoveDirection.Value)
            {
                case 0:
                    DestinationRec.Y -= (int)Vel.Y;
                    if (DestinationRec.Y <= desTile.DestinationRec.Y)
                    {
                        DestinationRec.Y = desTile.DestinationRec.Y;
                        CurrentTile = DestinationTile.Value;
                        StopMoving();
                    }
                    break;
                case 1:
                    DestinationRec.Y += (int)Vel.Y;
                    if (DestinationRec.Y >= desTile.DestinationRec.Y)
                    {
                        DestinationRec.Y = desTile.DestinationRec.Y;
                        CurrentTile = DestinationTile.Value;
                        StopMoving();
                    }
                    break;
                case 2:
                    DestinationRec.X -= (int)Vel.X;
                    if (DestinationRec.X <= desTile.DestinationRec.X)
                    {
                        DestinationRec.X = desTile.DestinationRec.X;
                        CurrentTile = DestinationTile.Value;
                        StopMoving();
                    }
                    break;
                case 3:
                    DestinationRec.X += (int)Vel.X;
                    if (DestinationRec.X >= desTile.DestinationRec.X)
                    {
                        DestinationRec.X = desTile.DestinationRec.X;
                        CurrentTile = DestinationTile.Value;
                        StopMoving();
                    }
                    break;
            }
        }

        public void SetTileMap(Tile[,] tileMap)
        {
            TileMap = tileMap;
        }

        protected void StopMoving()
        {
            IsMoving = false;
            DestinationTile = null;
        }

        protected int GetNewMoveDirection(Point point)
        {
            int moveDir = -1;
            Point temp = CurrentTile - point;

            if (temp.Y == 1)
                moveDir = 0;
            else if (temp.Y == -1)
                moveDir = 1;
            else if (temp.X == 1)
                moveDir = 2;
            else if (temp.X == -1)
                moveDir = 3;

            return moveDir;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Logic used to determin the next Destination Tile as well as it's move Direction
        /// </summary>
        public virtual void EnemyLogic()
        {
            
        }
    }
}
