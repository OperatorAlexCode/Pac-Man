using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Base_Classes
{
    public class AnimationManager
    {
        protected Rectangle[] SpriteFrames;
        protected int CurrentFrame;
        protected float RefreshRate;
        protected Timer NextFrame;

        public Rectangle GetCurrentFrame()
        {
            return SpriteFrames[CurrentFrame];
        }

        public void ChangeRefreshRate(float newRefreshRate)
        {
            RefreshRate = newRefreshRate;
        }
    }
}
