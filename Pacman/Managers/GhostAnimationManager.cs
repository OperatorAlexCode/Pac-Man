using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Base_Classes;
using Pacman.Enums;

namespace Pacman.Managers
{
    public class GhostAnimationManager : AnimationManager
    {
        int LastMovementDirection;
        GhostState LastState;
        public GhostAnimationManager(Rectangle[] spriteFrames, float refreshRate)
        {
            SpriteFrames = spriteFrames;
            RefreshRate = refreshRate;
            CurrentFrame = 0;
            NextFrame = new();
        }

        public void Update(float deltaTime, int movementDirection, GhostState state)
        {
            NextFrame.Update(deltaTime);

            if (state == GhostState.Vulnerable)
            {
                if (state != LastState)
                {
                    CurrentFrame = 8;
                    NextFrame.StartTimer(RefreshRate);
                }
                else if (NextFrame.IsDone())
                {
                    if (CurrentFrame == 8)
                        CurrentFrame = 9;
                    else if (CurrentFrame == 9)
                        CurrentFrame = 8;

                    NextFrame.StartTimer(RefreshRate);
                }
            }
            else
                switch (movementDirection)
                {
                    case 0:
                        if (LastMovementDirection != movementDirection || LastState != state)
                        {
                            CurrentFrame = 4;
                            NextFrame.StartTimer(RefreshRate);
                        }
                        else if (NextFrame.IsDone())
                        {
                            if (CurrentFrame == 4)
                                CurrentFrame = 5;
                            else if (CurrentFrame == 5)
                                CurrentFrame = 4;

                            NextFrame.StartTimer(RefreshRate);
                        }
                        break;
                    case 1:
                        if (LastMovementDirection != movementDirection || LastState != state)
                        {
                            CurrentFrame = 6;
                            NextFrame.StartTimer(RefreshRate);
                        }
                        else if (NextFrame.IsDone())
                        {
                            if (CurrentFrame == 6)
                                CurrentFrame = 7;
                            else if (CurrentFrame == 7)
                                CurrentFrame = 6;

                            NextFrame.StartTimer(RefreshRate);
                        }
                        break;
                    case 2:
                        if (LastMovementDirection != movementDirection || LastState != state)
                        {
                            CurrentFrame = 2;
                            NextFrame.StartTimer(RefreshRate);
                        }
                        else if (NextFrame.IsDone())
                        {
                            if (CurrentFrame == 2)
                                CurrentFrame = 3;
                            else if (CurrentFrame == 3)
                                CurrentFrame = 2;

                            NextFrame.StartTimer(RefreshRate);
                        }
                        break;
                    case 3:
                        if (LastMovementDirection != movementDirection || LastState != state)
                        {
                            CurrentFrame = 1;
                            NextFrame.StartTimer(RefreshRate);
                        }
                        else if (NextFrame.IsDone())
                        {
                            if (CurrentFrame == 1)
                                CurrentFrame = 0;
                            else if (CurrentFrame == 0)
                                CurrentFrame = 1;

                            NextFrame.StartTimer(RefreshRate);
                        }
                        break;
                    default:
                        goto case 3;
                }

            LastMovementDirection = movementDirection;
            LastState = state;
        }
    }
}
