using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Base_Classes;

namespace Pacman.Managers
{
    public class GhostAnimationManager : AnimationManager
    {
        int LastMovementDirection;
        public GhostAnimationManager(Rectangle[] spriteFrames, float refreshRate)
        {
            SpriteFrames = spriteFrames;
            RefreshRate = refreshRate;
            CurrentFrame = 0;
            NextFrame = new();
        }

        public void Update(float deltaTime, int movementDirection)
        {
            NextFrame.Update(deltaTime);

             switch (movementDirection)
            {
                case 0:
                    if (LastMovementDirection != movementDirection)
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
                    if (LastMovementDirection != movementDirection)
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
                    if (LastMovementDirection != movementDirection)
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
                    if (LastMovementDirection != movementDirection)
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
        }
    }
}
