using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Enums;
using Pacman.Managers;
using Pacman.Utility;
using SharpDX.Direct3D11;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using Timer = Pacman.Utility.Timer;

namespace Pacman.GameObjects
{
    public class Player
    {
        // Float
        //float CurrentRotation;
        float DrawLayer;
        float GraceDuration = 1.5f;
        float EatGhostTime = 8.0f;

        // Int
        public int PelletAmount = 0;
        public int Score = 0;
        public int Lives = 3;
        int? LastMoveIndex;
        int? NextMoveIndex;

        // Point
        Point CurrentTile;
        Point? DestinationTile;
        Point? NextDestinationTile;

        // bool
        bool IsHurt;
        bool IsActive = true;

        // Timer
        Timer GraceTimer;
        Timer PowerUpTimer;

        // Other
        Texture2D Tex;
        public Rectangle DestinationRec;
        PlayerState CurrentState;
        PlayerAnimationManager AnimationManager;
        Vector2 Vel;
        Tile[,] TileMap;
        Color CurrentColor;
        //public PowerUpType? CurrentPowerUp;


        public Player(Texture2D tex, Rectangle destinationRec, Vector2 vel, Point currentTile, float drawLayer)
        {
            Tex = tex;
            DestinationRec = destinationRec;

            Vel = vel;
            DrawLayer = drawLayer;
            //CurrentRotation = 0.0f;
            CurrentState = PlayerState.Idle;

            IsHurt = false;

            CurrentTile = currentTile;

            CurrentColor = Color.White;

            AnimationManager = new(CreateSpriteFrames(), 0.1f, CurrentState);

            GraceTimer = new();
            PowerUpTimer = new();
        }

        public void Update(float deltaTime, int? moveIndex, EnemyManager ghostManager)
        {
            UpdateTimers(deltaTime);

            if (GraceTimer.IsDone())
                IsHurt = false;

            //if (PowerUpTimer.IsDone() && CurrentPowerUp.HasValue)
            //    CurrentPowerUp = null;

            if (IsHurt)
                CurrentColor = Color.Red;
            else
                CurrentColor = Color.White;


            if (IsActive)
            {
                if (moveIndex.HasValue && LastMoveIndex != moveIndex.Value)
                    MoveTo(moveIndex);
                else
                    MoveTo(null);
            }

            AnimationManager.Update(deltaTime, LastMoveIndex, CurrentState);

            if (DestinationTile.HasValue)
                if (TileMap[DestinationTile.Value.Y, DestinationTile.Value.X].DestinationRec.Contains(DestinationRec.Center))
                    switch (TileMap[DestinationTile.Value.Y, DestinationTile.Value.X].UseItem())
                    {
                        case ItemType.Dot:
                            PelletAmount--;
                            Score += 10;
                            break;
                        case ItemType.Cherry:
                            Score += 100;
                            break;
                        case ItemType.EatGhostPill:
                            //CurrentPowerUp = PowerUpType.EatGhost;
                            Score += 50;
                            ghostManager.MakeGhostsVulnerable();
                            PowerUpTimer.StartTimer(EatGhostTime);
                            break;
                    }
        }

        void UpdateTimers(float deltaTime)
        {
            GraceTimer.Update(deltaTime);
            PowerUpTimer.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Rectangle currentFrame = AnimationManager.GetCurrentFrame();
            //Rectangle tempRec = new(DestinationRec.Width / 2+DestinationRec.X, DestinationRec.Height / 2 + DestinationRec.Y, DestinationRec.Width,DestinationRec.Height);
            //spriteBatch.Draw(Tex, tempRec, currentFrame, CurrentColor, CurrentRotation, new Vector2(currentFrame.Width/2+currentFrame.X, currentFrame.Height / 2 + currentFrame.Y), SpriteEffects.None, DrawLayer);
            spriteBatch.Draw(Tex, DestinationRec, AnimationManager.GetCurrentFrame(), CurrentColor, 0.0f, Vector2.Zero, SpriteEffects.None, DrawLayer);
        }

        Rectangle[] CreateSpriteFrames()
        {
            Rectangle[] spriteframes = new Rectangle[9];

            spriteframes[0] = new(35, 0, 16, 16);

            for (int x = 1; x < 9; x++)
            {
                if (x % 2 == 0)
                    spriteframes[x] = new(19, 15 * ((x - 1) / 2) + 1, 16, 16);
                else
                    spriteframes[x] = new(3, 15 * (x / 2) + 1, 16, 16);
            }

            //for (int x = 0; x < 11;x++)
            //    spriteframes[x] = new(3, 16 * (X/2) + 51, 16, 16);

            return spriteframes;
        }

        public void SetTileMap(Tile[,] tileMap)
        {
            TileMap = tileMap;
        }

        public void SetPelletAmount(int pelletAmount)
        {
            PelletAmount = pelletAmount;
        }

        void MoveTo(int? moveIndex)
        {
            int? moveDirection = moveIndex;

            if (!moveDirection.HasValue)
                moveDirection = LastMoveIndex;

            if (moveDirection.HasValue)
            {
                Point desIndex;
                switch (moveDirection.Value)
                {
                    case 0:
                        desIndex = new(CurrentTile.X, CurrentTile.Y - 1);
                        if (CurrentState == PlayerState.Idle && IsTileValid(desIndex))
                        {
                            CurrentState = PlayerState.Moving;
                            DestinationTile = desIndex;
                            LastMoveIndex = moveIndex.Value;
                        }
                        else if (moveDirection.Value != LastMoveIndex && LastMoveIndex.HasValue)
                        {
                            if (LastMoveIndex == 1)
                            {
                                Reverse();
                                LastMoveIndex = moveDirection;
                            }


                            else
                            {
                                Point newDes = new(DestinationTile.Value.X, DestinationTile.Value.Y - 1);
                                if (IsTileValid(newDes))
                                {
                                    NextMoveIndex = moveDirection.Value;
                                    NextDestinationTile = newDes;
                                    break;
                                }
                            }
                        }
                        break;
                    case 1:
                        desIndex = new(CurrentTile.X, CurrentTile.Y + 1);
                        if (CurrentState == PlayerState.Idle && IsTileValid(desIndex))
                        {
                            CurrentState = PlayerState.Moving;
                            DestinationTile = desIndex;
                            LastMoveIndex = moveIndex.Value;
                        }
                        else if (moveDirection.Value != LastMoveIndex && LastMoveIndex.HasValue)
                        {
                            if (LastMoveIndex == 0)
                            {
                                Reverse();
                                LastMoveIndex = moveDirection;
                            }

                            else
                            {
                                Point newDes = new(DestinationTile.Value.X, DestinationTile.Value.Y + 1);
                                if (IsTileValid(newDes))
                                {
                                    NextMoveIndex = moveDirection.Value;
                                    NextDestinationTile = newDes;
                                    break;
                                }
                            }

                        }
                        break;
                    case 2:
                        desIndex = new(CurrentTile.X - 1, CurrentTile.Y);
                        if (CurrentState == PlayerState.Idle && IsTileValid(desIndex))
                        {
                            CurrentState = PlayerState.Moving;
                            DestinationTile = desIndex;
                            LastMoveIndex = moveIndex.Value;
                        }
                        else if (moveDirection.Value != LastMoveIndex && LastMoveIndex.HasValue)
                        {
                            if (LastMoveIndex == 3)
                            {
                                Reverse();
                                LastMoveIndex = moveDirection;
                            }

                            else
                            {
                                Point newDes = new(DestinationTile.Value.X - 1, DestinationTile.Value.Y);
                                if (IsTileValid(newDes))
                                {
                                    NextMoveIndex = moveDirection.Value;
                                    NextDestinationTile = newDes;
                                    break;
                                }
                            }
                        }
                        break;
                    case 3:
                        desIndex = new(CurrentTile.X + 1, CurrentTile.Y);
                        if (CurrentState == PlayerState.Idle && IsTileValid(desIndex))
                        {
                            CurrentState = PlayerState.Moving;
                            DestinationTile = desIndex;
                            LastMoveIndex = moveIndex.Value;
                        }
                        else if (moveDirection.Value != LastMoveIndex && LastMoveIndex.HasValue)
                        {
                            if (LastMoveIndex == 2)
                            {
                                Reverse();
                                LastMoveIndex = moveDirection;
                            }

                            else
                            {
                                Point newDes = new(DestinationTile.Value.X + 1, DestinationTile.Value.Y);
                                if (IsTileValid(newDes))
                                {
                                    NextMoveIndex = moveDirection.Value;
                                    NextDestinationTile = newDes;
                                    break;
                                }
                            }
                        }
                        break;
                }

                if (LastMoveIndex.HasValue)
                    Move(LastMoveIndex.Value);
            }

        }

        bool IsTileValid(Point tileToCheck)
        {
            try
            {
                return TileMap[tileToCheck.Y, tileToCheck.X].Type != TileType.Wall;
            }
            catch
            {
                return false;
            }
        }

        void Move(int moveDirection)
        {
            Tile desTile = TileMap[DestinationTile.Value.Y, DestinationTile.Value.X];
            Point? nextDes = null;
            switch (moveDirection)
            {
                case 0:
                    DestinationRec.Y -= (int)Vel.Y;
                    //CurrentRotation = 3 * MathF.PI / 2;
                    if (DestinationRec.Y <= desTile.DestinationRec.Y)
                    {
                        DestinationRec.Y = desTile.DestinationRec.Y;
                        nextDes = new(CurrentTile.X, CurrentTile.Y - 1);
                    }
                    break;
                case 1:
                    DestinationRec.Y += (int)Vel.Y;
                    //CurrentRotation = MathF.PI / 2;
                    if (DestinationRec.Y >= desTile.DestinationRec.Y)
                    {
                        DestinationRec.Y = desTile.DestinationRec.Y;
                        nextDes = new(CurrentTile.X, CurrentTile.Y + 1);
                    }
                    break;
                case 2:
                    DestinationRec.X -= (int)Vel.X;
                    //CurrentRotation = MathF.PI;
                    if (DestinationRec.X <= desTile.DestinationRec.X)
                    {
                        DestinationRec.X = desTile.DestinationRec.X;
                        nextDes = new(CurrentTile.X - 1, CurrentTile.Y);
                    }
                    break;
                case 3:
                    DestinationRec.X += (int)Vel.X;
                    //CurrentRotation = 0;
                    if (DestinationRec.X >= desTile.DestinationRec.X)
                    {
                        DestinationRec.X = desTile.DestinationRec.X;
                        nextDes = new(CurrentTile.X + 1, CurrentTile.Y);
                    }
                    break;
            }

            if (nextDes.HasValue)
            {
                if (desTile.Type == TileType.Teleporter && desTile.TeleporterExit.HasValue && TileMap[CurrentTile.Y, CurrentTile.X].Type != TileType.Teleporter)
                {
                    Point teleporterExit = TileMap[DestinationTile.Value.Y, DestinationTile.Value.X].TeleporterExit.Value;
                    Rectangle teleporterDestRec = TileMap[teleporterExit.Y, teleporterExit.X].DestinationRec;
                    if (IsTileValid(new(teleporterExit.X, teleporterExit.Y)))
                    {
                        DestinationRec.X = teleporterDestRec.X;
                        DestinationRec.Y = teleporterDestRec.Y;
                        CurrentTile = teleporterExit;
                        if (NextMoveIndex.HasValue)
                        {
                            LastMoveIndex = NextMoveIndex.Value;
                            NextMoveIndex = null;
                        }
                        
                        switch(moveDirection)
                        {
                            case 0:
                                nextDes = new(CurrentTile.X, CurrentTile.Y - 1);
                                break;
                            case 1:
                                nextDes = new(CurrentTile.X, CurrentTile.Y + 1);
                                break;
                            case 2:
                                nextDes = new(CurrentTile.X - 1, CurrentTile.Y);
                                break;
                            case 3:
                                nextDes = new(CurrentTile.X + 1, CurrentTile.Y);
                                break;
                        }

                        if (IsTileValid(nextDes.Value))
                            DestinationTile = nextDes;

                        else
                            StopMoving();
                    }
                }

                else
                {
                    if (NextMoveIndex.HasValue)
                    {
                        LastMoveIndex = NextMoveIndex.Value;
                        NextMoveIndex = null;
                    }

                    CurrentTile = DestinationTile.Value;

                    if (NextDestinationTile.HasValue)
                    {
                        DestinationTile = NextDestinationTile.Value;
                        NextDestinationTile = null;
                    }
                    else if (IsTileValid(nextDes.Value))
                        DestinationTile = nextDes;

                    else
                        StopMoving();
                }
            }

        }

        void Reverse()
        {
            Point tempCache = DestinationTile.Value;
            DestinationTile = CurrentTile;
            CurrentTile = tempCache;
        }

        void StopMoving()
        {
            DestinationTile = null;
            LastMoveIndex = null;
            CurrentState = PlayerState.Idle;
        }

        public void GetHurt()
        {
            if (GraceTimer.IsDone())
            {
                IsHurt = true;
                Lives--;

                if (Lives <= 0)
                {
                    Die();
                }

                GraceTimer.StartTimer(GraceDuration);
            }
        }

        public void SetResetLives(int? newAmount = null)
        {
            if (newAmount.HasValue)
                Lives = newAmount.Value;
            else
                Lives = 3;
        }

        public void Deactivate()
        {
            IsActive = false;
            CurrentState = PlayerState.Idle;
        }

        void Die()
        {
            CurrentState = PlayerState.Dead;
        }

        public void EatGhost()
        {
            Score += 100;
        }
    }
}
