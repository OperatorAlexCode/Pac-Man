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

        // Point
        public Point[] Exits;
        public Point? TeleporterExit;

        // Other
        Texture2D? Texture;
        float DrawLayer;
        ItemType? TileItem;
        bool ItemUsed;
        public TileType Type;
        Color TileColor = Color.White;

        public Tile(Texture2D? texture, Rectangle destinationRec, Rectangle sourceRec, float drawLayer, TileType type, ItemType? tileItem = null)
        {
            Texture = texture;
            DestinationRec = destinationRec;
            SourceRec = sourceRec;
            DrawLayer = drawLayer;
            Type = type;
            TileItem = tileItem;
            ItemUsed = false;

            if (Type == TileType.Teleporter)
            {
                TileColor = Color.Purple;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Type == TileType.Teleporter)
            {
                Texture2D background = new(spriteBatch.GraphicsDevice,1,1);
                background.SetData(new[] { Color.White });
                spriteBatch.Draw(background, DestinationRec, SourceRec, TileColor, 0f, new Vector2(), SpriteEffects.None, DrawLayer);
            }
                
            else 
            if (Texture != null && !ItemUsed)
                spriteBatch.Draw(Texture, DestinationRec, SourceRec, TileColor, 0f, new Vector2(), SpriteEffects.None, DrawLayer);
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

        public void SetTeleportDestination(Point newDestination)
        {
            TeleporterExit = newDestination;
        }
    }
}
