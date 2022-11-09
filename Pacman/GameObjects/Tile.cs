using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Base_Classes;
using Pacman.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.GameObjects
{
    public class Tile
    {
        // Rectanlge
        public Rectangle DestinationRec;
        Rectangle SourceRec;

        // Other
        public Point[] Exits;
        Texture2D? Texture;
        float DrawLayer;
        ItemType? TileItem;
        bool ItemUsed;
        public TileType Type;

        public Tile(Texture2D? texture, Rectangle destinationRec, Rectangle sourceRec, float drawLayer, TileType type, ItemType? tileItem = null)
        {
            Texture = texture;
            DestinationRec = destinationRec;
            SourceRec = sourceRec;
            DrawLayer = drawLayer;
            Type = type;
            TileItem = tileItem;
            ItemUsed = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null && !ItemUsed)
            {
                spriteBatch.Draw(Texture, DestinationRec, SourceRec, Color.White, 0f, new Vector2(), SpriteEffects.None, DrawLayer);
            }
        }

        public ItemType? UseItem()
        {
            if (!ItemUsed)
            {
                ItemUsed = true;
                return TileItem;
            }
            return null;
        }

        public bool HasItem()
        {
            return !ItemUsed;
        }
    }
}
