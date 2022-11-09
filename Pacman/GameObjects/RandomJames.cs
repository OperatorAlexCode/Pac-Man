using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Base_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.GameObjects
{
    public class RandomJames : Ghost
    {
        public RandomJames(Texture2D tex, Rectangle destinationRec, Vector2 vel, Point currentTile, float drawLayer)
        {
            Tex = tex;
            DestinationRec = destinationRec;
            Vel = vel;
            CurrentTile = currentTile;
            DrawLayer = drawLayer;
            IsMoving = false;
            IsActive = true;
            AnimationManager = new(CreateSpriteFrames(65), .2f);
        }

        public override void EnemyLogic()
        {
            if (!IsMoving)
            {
                Point[] exits = TileMap[CurrentTile.Y, CurrentTile.X].Exits;
                DestinationTile = exits[new Random().Next(exits.Length)];

                MoveDirection = GetNewMoveDirection(DestinationTile.Value);
                IsMoving = true;
            }

            else
                Move();
        }
    }
}
