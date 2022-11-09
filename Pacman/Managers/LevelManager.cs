using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SharpDX.Direct3D9;
using System.IO;
using Pacman.GameObjects;
using Pacman.Enums;
using Point = Microsoft.Xna.Framework.Point;
using Pacman.Base_Classes;
using System.Text.Encodings;
using static System.Net.Mime.MediaTypeNames;

namespace Pacman.Managers
{
    public class LevelManager
    {
        // Float
        float EntityLayer = 0.6f;
        float ItemLayer = 0.3f;
        float BackgroundLayer = 0.0f;

        // Int
        int CurrentLevel = 0;
        int SpriteSheetTileHeight;
        int SpriteSheetTileWidth;
        int SpriteSheetTileMargin;
        int ScreenHeight;
        int ScreenWidth;
        int ScreenMarginX;
        int ScreenMarginY;
        int TileAmountX;
        int TileAmountY;
        int PelletAmount;

        // Texture2D
        Texture2D TileSpriteSheet;
        Texture2D ItemSpriteSheet;
        Texture2D PlayerEnemySpriteSheet;

        // Rectangle
        Rectangle TileSpecs;
        Rectangle DotSourceRec = new(49, 0, 8, 8);
        Rectangle CherrySourceRec = new(36, 49, 14, 14);

        // Vector2
        Vector2 PlayerVel = new(3, 3);
        Vector2 GhostVel = new(2, 2);

        // Other
        string[] Tiles = new string[] { "╋", "╞", "╨", "╚", "╡", "═", "╝", "╩", "╥", "╔", "║", "╠", "╗", "╦", "╣", "╬", "▵", "R", "C", "○", "🍒" };
        List<List<List<string>>> LevelsData;
        
        Tile[,]? TileMap;
        EnemyManager GhostManager;
        Encoding EncodingType = Encoding.UTF8;

        public LevelManager(Texture2D[] textures, int[] screenSpecs, int[] spriteSheetTileSpecs)
        {
            TileSpriteSheet = textures[0];
            ItemSpriteSheet = textures[1];
            PlayerEnemySpriteSheet = textures[2];

            SpriteSheetTileHeight = spriteSheetTileSpecs[0];
            SpriteSheetTileWidth = spriteSheetTileSpecs[1];
            SpriteSheetTileMargin = spriteSheetTileSpecs[2];

            ScreenHeight = screenSpecs[0];
            ScreenWidth = screenSpecs[1];
            ScreenMarginX = screenSpecs[2];
            ScreenMarginY = screenSpecs[3];

            LevelsData = new();

            ReadLevelData();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (TileMap != null)
            {
                for (int y = 0; y < TileAmountY; y++)
                    for (int x = 0; x < TileAmountX; x++)
                        TileMap[y, x].Draw(spriteBatch);
            }
        }

        void ReadLevelData()
        {
            List<StreamReader> levels = new() { new("Levels/Level1.txt", EncodingType), new("Levels/Level2.txt", EncodingType) };

            // Gets bytes from str, converts that to unicode bytes then decodes the bytes into a string
            Func<string,string> unicodeToString = str => EncodingType.GetString(Encoding.Convert(Encoding.Unicode, EncodingType, Encoding.Unicode.GetBytes(str)));

            for (int x = 0; x < levels.Count; x++)
            {
                int levelWidth = 0;
                int currentRow = 0;

                List<List<string>> levelData = new();
                string currentLine;
                List<char> tempCache = new();
                int temp;
                do
                {
                    temp = 0;
                    currentLine = levels[x].ReadLine();
                    levelData.Add(new List<string>());
                    if (currentLine.Length > levelWidth)
                        levelWidth = currentLine.Length;

                    for (int y = 0; y < currentLine.Length; y++)
                    {
                        char cha = currentLine[y];
                        string character = unicodeToString($"{cha}");

                        if (character == "�")
                        {
                            temp++;
                            tempCache.Add(cha);                 
                            if (unicodeToString(new string(tempCache.ToArray())) != "�")
                            {
                                character = unicodeToString(new string(tempCache.ToArray()));
                                tempCache.Clear();
                                levelData[currentRow].Add(character);
                            }
                        }  

                        else
                            levelData[currentRow].Add(character);
                    }

                    currentRow++;

                } while (!levels[x].EndOfStream);

                levels[x].Close();

                LevelsData.Add(levelData);
            }
        }

        void LoadLevel(out Player playerOut)
        {
            TileSpecs = new();

            PelletAmount = 0;

            int levelWidth = LevelsData[CurrentLevel][0].Count;

            TileSpecs.Width = (ScreenWidth - ScreenMarginX * 2) / levelWidth;
            TileSpecs.Height = (ScreenHeight - ScreenMarginY * 2) / LevelsData[CurrentLevel].Count;

            TileMap = new Tile[LevelsData[CurrentLevel].Count, levelWidth];

            TileAmountX = levelWidth;
            TileAmountY = LevelsData[CurrentLevel].Count;

            List<List<string>> currentLevel = LevelsData[CurrentLevel];

            Player player = null;

            GhostManager = new();

            for (int y = 0; y < TileAmountY; y++)
            {
                for (int x = 0; x < TileAmountX; x++)
                {
                    Rectangle destinationRec = new(TileSpecs.Width * x + ScreenMarginX, TileSpecs.Height * y + ScreenMarginY, TileSpecs.Width, TileSpecs.Height);

                    if (TileAmountX >= currentLevel[y].Count())
                        currentLevel[y].Add(" ");

                    Player tempPlayer;

                    TileMap[y, x] = TileMaker(currentLevel[y][x], destinationRec, out tempPlayer);

                    if (tempPlayer != null)
                        player = tempPlayer;
                }
            }

            CreateExits();

            player.SetTileMap(TileMap);
            player.SetPelletAmount(PelletAmount);

            GhostManager.SetTileMapForGhosts(TileMap);

            playerOut = player;
        }

        void CreateExits()
        {
            for (int y = 0; y < TileAmountY; y++)
            {
                for (int x = 0; x < TileAmountX; x++)
                {
                    Tile tile = TileMap[y, x];
                    if (tile.Type == TileType.Normal)
                    {
                        List<Point> exits = new();

                        if (x - 1 >= 0)
                            if (TileMap[y, x - 1].Type != TileType.Wall)
                                exits.Add(new Point(x - 1, y));

                        if (x + 1 < TileAmountX)
                            if (TileMap[y, x + 1].Type != TileType.Wall)
                                exits.Add(new Point(x + 1, y));

                        if (y - 1 >= 0)
                            if (TileMap[y - 1, x].Type != TileType.Wall)
                                exits.Add(new Point(x, y - 1));

                        if (y + 1 < TileAmountY)
                            if (TileMap[y + 1, x].Type != TileType.Wall)
                                exits.Add(new Point(x, y + 1));

                        TileMap[y, x].Exits = exits.ToArray();
                    }
                }
            }
        }

        public void LoadRandomLevel(out Player playerOut, out EnemyManager ghostManager)
        {
            Player? plyr = null;

            CurrentLevel = new Random().Next(LevelsData.Count);

            LoadLevel(out plyr);

            playerOut = plyr;
            ghostManager = GhostManager;
        }

        public void LoadNextLevel(out Player? playerOut, out EnemyManager ghostManager, out GameState newState)
        {
            Player? plyr = null;
            if (CurrentLevel == LevelsData.Count-1)
                newState = GameState.GameEnd;

            else
            {
                CurrentLevel++;
                newState = GameState.InGame;
                LoadLevel(out plyr);
            }

            playerOut = plyr;
            ghostManager = GhostManager;
        }

        public void LoadCurrentLevel(out Player playerOut, out EnemyManager ghostManager)
        {
            Player? plyr = null;

            LoadLevel(out plyr);

            playerOut = plyr;
            ghostManager = GhostManager;
        }

        public void LoadSpecificLevel(int levelIndex,out Player playerOut, out EnemyManager ghostManager)
        {
            Player? plyr = null;

            CurrentLevel = levelIndex;

            LoadLevel(out plyr);

            playerOut = plyr;
            ghostManager = GhostManager;
        }

        public void LoadPreviousLevel(out Player playerOut)
        {
            if (CurrentLevel > 0)
                CurrentLevel--;
            LoadLevel(out playerOut);
        }

        Tile TileMaker(string str, Rectangle destinationRec, out Player? player)
        {
            Texture2D? texture = null;
            Rectangle sourceRec;
            TileType type;
            float layer;
            ItemType? itemType = null;
            int stringIndex = Array.IndexOf(Tiles, str);
            Player? playerOut = null;
            Point entityTile = new((destinationRec.X - ScreenMarginX) / TileSpecs.Width, (destinationRec.Y - ScreenMarginY) / TileSpecs.Height);

            if (stringIndex >= 0 && stringIndex < 16)
            {
                sourceRec = CreateWallSourceRec((int)MathF.Floor(stringIndex % 4), (int)MathF.Floor(stringIndex / 4));
                texture = TileSpriteSheet;
                type = TileType.Wall;
                layer = BackgroundLayer;
            }

            else
                switch (stringIndex)
                {
                    // Player spawn Tile
                    case 16:
                        sourceRec = new();
                        type = TileType.Normal;
                        layer = BackgroundLayer;
                        playerOut = new Player(PlayerEnemySpriteSheet, destinationRec, PlayerVel, entityTile, EntityLayer);
                        break;
                    // RandomJames spawn Tile
                    case 17:
                        sourceRec = new();
                        type = TileType.Normal;
                        layer = BackgroundLayer;
                        GhostManager.SetJames(new(PlayerEnemySpriteSheet, destinationRec, GhostVel, entityTile, EntityLayer));
                        break;
                    // CommitmentJones spawn Tile
                    case 18:
                        sourceRec = new();
                        type = TileType.Normal;
                        layer = BackgroundLayer;
                        GhostManager.SetJones(new(PlayerEnemySpriteSheet, destinationRec, GhostVel, entityTile, EntityLayer));
                        break;
                    // Dot tile
                    case 19:
                        sourceRec = DotSourceRec;
                        texture = ItemSpriteSheet;
                        type = TileType.Normal;
                        layer = ItemLayer;
                        itemType = ItemType.Dot;
                        PelletAmount++;
                        break;
                    // Cherry Tile
                    case 20:
                        sourceRec = CherrySourceRec;
                        texture = PlayerEnemySpriteSheet;
                        type = TileType.Normal;
                        layer = ItemLayer;
                        itemType = ItemType.Cherry;
                        break;
                    default:
                        sourceRec = new();
                        layer = BackgroundLayer;
                        type = TileType.Normal;
                        break;
                }

            Tile newTile = new(texture, destinationRec, sourceRec, layer, type, itemType);
            player = playerOut;
            return newTile;
        }

        Rectangle CreateWallSourceRec(int spriteSheetColumn, int spriteSheetRow)
        {
            Rectangle sourceRec = new((SpriteSheetTileWidth + SpriteSheetTileMargin) * spriteSheetColumn, (SpriteSheetTileHeight + SpriteSheetTileMargin) * spriteSheetRow, SpriteSheetTileHeight, SpriteSheetTileWidth);

            return sourceRec;
        }
    }
}
