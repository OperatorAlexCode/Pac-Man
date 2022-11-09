using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pacman.Base_Classes;
using Pacman.Enums;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Security.Cryptography.X509Certificates;

namespace Pacman.Managers
{
    public class PlayerAnimationManager : AnimationManager
    {
        // Int
        int LastMovementDirection;

        // Other
        PlayerState LastState;

        public PlayerAnimationManager(Rectangle[] spriteFrames, float refreshRate, PlayerState lastState)
        {
            SpriteFrames = spriteFrames;
            RefreshRate = refreshRate;
            LastState = lastState;
            CurrentFrame = 0;
            NextFrame = new();
        }

        public void Update(float deltaTime, int? moveIndex, PlayerState currentState)
        {
            UpdateTimers(deltaTime);

            int moveDirection = LastMovementDirection;

            if (moveIndex.HasValue)
                moveDirection = moveIndex.Value;

            switch (currentState)
            {
                case PlayerState.Moving:
                    switch (moveDirection)
                    {
                        case 0:
                            if (LastMovementDirection != moveDirection)
                            {
                                CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            else if (NextFrame.IsDone())
                            {
                                if (CurrentFrame == 0)
                                    CurrentFrame = 5;
                                else if (CurrentFrame == 5)
                                    CurrentFrame = 6;
                                else if (CurrentFrame == 6)
                                    CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            break;
                        case 1:
                            if (LastMovementDirection != moveDirection)
                            {
                                CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            else if (NextFrame.IsDone())
                            {
                                if (CurrentFrame == 0)
                                    CurrentFrame = 7;
                                else if (CurrentFrame == 7)
                                    CurrentFrame = 8;
                                else if (CurrentFrame == 8)
                                    CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            break;
                        case 2:
                            if (LastMovementDirection != moveDirection)
                            {
                                CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            else if (NextFrame.IsDone())
                            {
                                if (CurrentFrame == 0)
                                    CurrentFrame = 3;
                                else if (CurrentFrame == 3)
                                    CurrentFrame = 4;
                                else if (CurrentFrame == 4)
                                    CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            break;
                        case 3:
                            if (LastMovementDirection != moveDirection)
                            {
                                CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            else if (NextFrame.IsDone())
                            {
                                if (CurrentFrame == 0)
                                    CurrentFrame = 1;
                                else if (CurrentFrame == 1)
                                    CurrentFrame = 2;
                                else if (CurrentFrame == 2)
                                    CurrentFrame = 0;
                                NextFrame.StartTimer(RefreshRate);
                            }
                            break;
                    }
                    break;
                default:
                    CurrentFrame = 0;
                    break;
            }

            LastState = currentState;
            LastMovementDirection = moveDirection;
        }

        void UpdateTimers(float deltaTime)
        {
            NextFrame.Update(deltaTime);
        }

    }
}
