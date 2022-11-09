using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Base_Classes;
using Pacman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.GameObjects
{
    public class CommitmentJones : Ghost
    {
        public CommitmentJones(Texture2D tex, Rectangle destinationRec, Vector2 vel, Point currentTile, float drawLayer)
        {
            Tex = tex;
            DestinationRec = destinationRec;
            Vel = vel;
            CurrentTile = currentTile;
            DrawLayer = drawLayer;
            IsMoving = false;
            IsActive = true;
            AnimationManager = new(CreateSpriteFrames(81), .2f);
        }

        public override void EnemyLogic()
        {
            Point[] exits = TileMap[CurrentTile.Y, CurrentTile.X].Exits;
            Random randomizer = new();

            if (!IsMoving && !DestinationTile.HasValue)
            {
                if (!MoveDirection.HasValue)
                {
                    DestinationTile = GetRandomExit(exits);
                    goto BreakOut;
                }

                Func<int, int, bool> oppositeCheck = (i1, i2) => (i1 == 0 && i2 == 1) || (i1 == 1 && i2 == 0) || (i1 == 2 && i2 == 3) || (i1 == 3 && i2 == 2);

                Point[] availableExits = Array.FindAll(exits, e => !oppositeCheck(MoveDirection.Value, GetNewMoveDirection(e)));

                if (availableExits.Length >= 1)
                    DestinationTile = GetRandomExit(availableExits);
                else if (availableExits.Length == 0)
                    DestinationTile = GetRandomExit(exits);

                BreakOut:
                MoveDirection = GetNewMoveDirection(DestinationTile.Value);
                IsMoving = true;
            }

            else
                Move();
        }

        Point GetRandomExit(Point[] exits)
        {
            return exits[new Random().Next(exits.Length)];
        }
    }
}
