using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Enums;
using Pacman.GameObjects;
using Pacman.Managers;
using Pacman.Utility;
using SharpDX.Direct3D11;
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
        protected Point SpawnPos;

        // Bool
        protected bool IsMoving;
        protected bool IsActive;

        // Float
        protected float DrawLayer;
        protected float RespawnTime = 4.0f;
        protected float VulnerablityTime = 8.0f;

        // Timer
        protected Timer RespawnTimer;
        protected Timer VulnerablityTimer;

        // Other
        protected Microsoft.Xna.Framework.Graphics.Texture2D Tex;
        protected Rectangle DestinationRec;
        protected Vector2 Vel;
        protected int? MoveDirection;
        protected Tile[,] TileMap;
        protected GhostAnimationManager AnimationManager;
        protected GhostState CurrentState;

        public void Update(float deltaTime, Player player)
        {
            if (IsActive)
            {
                UpdateTimers(deltaTime);

                if (VulnerablityTimer.IsDone())
                    TurnNormal();

                if (CurrentState == GhostState.Eaten && RespawnTimer.IsDone())
                    Respawn();
                
                if (CurrentState != GhostState.Eaten)
                    EnemyLogic();

                AnimationManager.Update(deltaTime, MoveDirection.Value, CurrentState);

                if (DestinationRec.Intersects(player.DestinationRec))
                    switch (CurrentState)
                    {
                        case GhostState.Normal:
                            player.GetHurt();
                            break;
                        case GhostState.Vulnerable:
                            player.EatGhost();
                            CurrentState = GhostState.Eaten;
                            CurrentTile = SpawnPos;
                            RespawnTimer.StartTimer(RespawnTime);
                            break;
                    }
            }
        }

        public void UpdateTimers(float deltaTime)
        {
            RespawnTimer.Update(deltaTime);
            VulnerablityTimer.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentState != GhostState.Eaten)
            spriteBatch.Draw(Tex, DestinationRec, AnimationManager.GetCurrentFrame(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, DrawLayer);
        }

        protected Rectangle[] CreateSpriteFrames(int spriteRow)
        {
            Rectangle[] spriteFrames = new Rectangle[10];

            for (int x = 0; x < 8; x++)
                spriteFrames[x] = new(4 + 16 * x, spriteRow, 14, 14);

            spriteFrames[8] = new(132,65,14,14);
            spriteFrames[9] = new(148,65,14,14);

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

        public void MakeVunerable()
        {
            if (CurrentState != GhostState.Eaten)
            {
                CurrentState = GhostState.Vulnerable;
                VulnerablityTimer.StartTimer(VulnerablityTime);
            }     
        }

        public void TurnNormal()
        {
            CurrentState = GhostState.Normal;
        }

        protected void Respawn()
        {
            
            CurrentState = GhostState.Normal;
        }
    }
}
