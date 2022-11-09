using Microsoft.Xna.Framework.Graphics;
using Pacman.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Managers
{
    public class EnemyManager
    {
        RandomJames? James;
        CommitmentJones? Jones;
        public EnemyManager()
        {
        }

        public void Update(float deltaTime, Player player)
        {
            if (James != null)
                James.Update(deltaTime, player);

            if (Jones != null)
                Jones.Update(deltaTime, player);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (James != null)
                James.Draw(spriteBatch);

            if (Jones != null)
                Jones.Draw(spriteBatch);
        }


        public void SetJames(RandomJames newJames)
        {
            James = newJames;
        }

        public void SetJones(CommitmentJones newJones)
        {
            Jones = newJones;
        }

        public void SetTileMapForGhosts(Tile[,] tileMap)
        {
            if (James != null)
                James.SetTileMap(tileMap);
            if (Jones != null)
                Jones.SetTileMap(tileMap);
        }

        /// <summary>
        /// Deactivates ghost based upon the index
        /// </summary>
        /// <param name="ghostIndex">index of ghost to be deactivated, else if not specified deactivates all ghosts</param>
        public void DeactivateGhost(int? ghostIndex = null)
        {
            switch (ghostIndex)
            {
                case 0:
                    if (James != null)
                        James.Deactivate();
                    break;
                case 1:
                    if (Jones != null)
                        Jones.Deactivate();
                    break;
                default:
                    if (James != null)
                        James.Deactivate();
                    if (Jones != null)
                        Jones.Deactivate();
                    break;
            }
        }
    }
}
