using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pacman.Base_Classes;
using Pacman.Enums;
using Pacman.GameObjects;
using Pacman.Managers;
using Pacman.Utility;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Pacman
{
    public class Game1 : Game
    {
        // Texture2D
        Texture2D TileSet;
        Texture2D PlayerEnemySpriteSheet;
        Texture2D ItemSpriteSheet;
        Texture2D PacManMenuLogoTex;

        // Int
        int Height = 800;
        int Width = 800;
        int MarginX = 100;
        int MarginY = 100;
        int PelletsLeft = 0;
        int CurrentPlayerScore = 0;
        int LastSavedPlayerScore = 0;
        int PlayerLives = 3;
        int CurrentPlayerLives = 3;

        // Keys
        List<Keys[]> ValidKeyboardInputs = new() { new Keys[] { Keys.W, Keys.S, Keys.A, Keys.D }, new Keys[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right } };
        List<Keys[]> ValidMenuInputs = new() { new Keys[] { Keys.Enter, Keys.R } };
        List<Keys> SecretInputCode = new() { Keys.Up, Keys.Up, Keys.Down, Keys.Down, Keys.Left, Keys.Right, Keys.Left, Keys.Right,Keys.B, Keys.A,Keys.Space };
        List<Keys> SecretCodeChache = new();

        // Bool
        bool DoRandomLevels = false;
        bool IsGamePadConnected = false;
        bool KeyIsPressed = false;

        // Other
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        float TextLayer = 1.0f;
        LevelManager levelManager;
        Player? PlayerChar;
        GameState CurrentState;
        EnemyManager GhostManager;
        //List<GamePadButtons[]> ValidControllerInputs = new() { new GamePadButtons[] { GamePadButtons} };
        SpriteFont GameFont;
        Timer LevelTransitionTimer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.ApplyChanges();

            InitializeTexture();
            GameFont = Content.Load<SpriteFont>("GameFont");
            LevelTransitionTimer = new();

            CurrentState = GameState.MainMenu;

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateTimers(deltaTime);

            IsGamePadConnected = GamePad.GetState(PlayerIndex.One).IsConnected;

            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();

            if (Keyboard.GetState().GetPressedKeyCount() == 0)
                KeyIsPressed = false;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    levelManager = new(new Texture2D[] { TileSet, ItemSpriteSheet, PlayerEnemySpriteSheet }, new int[] { Height, Width, MarginX, MarginY }, new int[] { 32, 32, 1 });
                    if (IsGamePadConnected)
                    {
                        if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                        {
                            levelManager.LoadCurrentLevel(out PlayerChar, out GhostManager);
                            DoRandomLevels = false;
                            PlayerChar.SetResetLives(PlayerLives);
                            CurrentState = GameState.InGame;
                        }
                        else if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
                        {
                            levelManager.LoadRandomLevel(out PlayerChar, out GhostManager);
                            DoRandomLevels = true;
                            PlayerChar.SetResetLives(PlayerLives);
                            CurrentState = GameState.InGame;
                        }
                    }

                    if (pressedKeys.Length == 1 && !KeyIsPressed)
                    {
                        KeyIsPressed = true;
                        
                        if (pressedKeys[0] == SecretInputCode[SecretCodeChache.Count])
                         {
                            SecretCodeChache.Add(pressedKeys[0]);
                            if (SecretCodeChache.Count == SecretInputCode.Count)
                            {
                                levelManager.LoadSecretLevel(out PlayerChar, out GhostManager);
                                DoRandomLevels = false;
                                PlayerChar.SetResetLives(PlayerLives);
                                CurrentState = GameState.InGame;
                            }   
                        }
                        else
                            SecretCodeChache.Clear();
                    }

                    if (pressedKeys.Any(k => ValidMenuInputs[0].Contains(k)) && pressedKeys.Length == 1)
                    {
                        switch (pressedKeys[0])
                        {
                            case Keys.Enter:
                                levelManager.LoadCurrentLevel(out PlayerChar, out GhostManager);
                                DoRandomLevels = false;
                                break;
                            case Keys.R:
                                levelManager.LoadRandomLevel(out PlayerChar, out GhostManager);
                                DoRandomLevels = true;
                                break;
                        }

                        PlayerChar.SetResetLives(PlayerLives);
                        CurrentState = GameState.InGame;
                    }
                    break;
                case GameState.InGame:
                    UpdateEntities(deltaTime);

                    if (CurrentPlayerLives <= 0)
                        CurrentState = GameState.LevelLoss;

                    else if (PelletsLeft <= 0)
                    {
                        CurrentState = GameState.LevelWin;
                        PlayerChar.Deactivate();
                        LevelTransitionTimer.StartTimer(1);
                    }
                    break;
                case GameState.LevelWin:
                    UpdateEntities(deltaTime);

                    if (LevelTransitionTimer.IsDone())
                    {
                        SaveStats();

                        if (DoRandomLevels)
                            levelManager.LoadRandomLevel(out PlayerChar, out GhostManager);
                        else
                            levelManager.LoadNextLevel(out PlayerChar, out GhostManager, out CurrentState);

                        if (CurrentState != GameState.GameEnd)
                        {
                            PlayerChar.Score = CurrentPlayerScore;
                            PlayerChar.Lives = CurrentPlayerLives;
                        }
                    }
                    break;
                case GameState.LevelLoss:
                    ContinueCheck(false);
                    break;
                case GameState.GameEnd:
                    ContinueCheck(true);
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointWrap);

            levelManager.Draw(spriteBatch);

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.InGame:
                    DrawHud();
                    DrawEntities();
                    break;
                case GameState.LevelLoss:
                    DrawLoseScreen();
                    break;
                case GameState.LevelWin:
                    DrawEntities();
                    break;
                case GameState.GameEnd:
                    DrawEndScreen();
                    break;
            }

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        void InitializeTexture()
        {
            TileSet = Content.Load<Texture2D>("Tileset");
            PlayerEnemySpriteSheet = Content.Load<Texture2D>("PacManSpriteSheet");
            ItemSpriteSheet = Content.Load<Texture2D>("SpriteSheet");
            PacManMenuLogoTex = Content.Load<Texture2D>("PacManLogo");
        }

        void UpdateEntities(float deltaTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            Keys[] input = keyboard.GetPressedKeys();
            int? inputIndex = null;
            GamePadDPad dpad = GamePad.GetState(PlayerIndex.One).DPad;

            if (IsGamePadConnected)
            {
                ButtonState[] dpadInputs = new ButtonState[4];
                dpadInputs[0] = dpad.Up;
                dpadInputs[1] = dpad.Down;
                dpadInputs[2] = dpad.Left;
                dpadInputs[3] = dpad.Right;

                if (Array.FindAll(dpadInputs, i => i == ButtonState.Pressed).Length == 1)
                    inputIndex = Array.IndexOf(dpadInputs, ButtonState.Pressed);
            }

            else if (input.Length == 1 && CurrentState == GameState.InGame)
                foreach (Keys[] inputScheme in ValidKeyboardInputs)
                {
                    if (inputScheme.Contains(input[0]))
                    {
                        inputIndex = Array.IndexOf(inputScheme, input[0]);
                        break;
                    }
                }

            PlayerChar.Update(deltaTime, inputIndex, GhostManager);

            UpdateStats();

            if (CurrentState == GameState.InGame)
                GhostManager.Update(deltaTime, PlayerChar);
        }

        void UpdateTimers(float deltatime)
        {
            LevelTransitionTimer.Update(deltatime);
        }

        void UpdateStats()
        {
            PelletsLeft = PlayerChar.PelletAmount;
            CurrentPlayerScore = PlayerChar.Score;
            CurrentPlayerLives = PlayerChar.Lives;
        }

        void ClearStats()
        {
            CurrentPlayerScore = 0;
            LastSavedPlayerScore = 0;
        }

        void ResetStats()
        {
            CurrentPlayerScore = LastSavedPlayerScore;
            PlayerChar.Score = CurrentPlayerScore;
            PelletsLeft = PlayerChar.PelletAmount;
            PlayerChar.Lives = PlayerLives;
            CurrentPlayerLives = PlayerLives;
        }

        void SaveStats()
        {
            LastSavedPlayerScore = CurrentPlayerScore;
        }

        void DrawEntities()
        {
            PlayerChar.Draw(spriteBatch);
            if (CurrentState == GameState.InGame)
                GhostManager.Draw(spriteBatch);
        }

        void DrawHud()
        {
            string livesStr = "Lives: ";
            string scoreStr = "Score:";
            string scoreAmountStr = $"{CurrentPlayerScore}";

            Vector2 livesSize = MeasureString(livesStr) * 1.5f;
            Vector2 scoreSize = MeasureString(scoreStr) * 1.2f;
            Vector2 scoreAmountSize = MeasureString(scoreAmountStr) * 1.2f;

            Vector2 livesPos = Vector2.Zero;
            Vector2 scorePos = new(Width - scoreSize.X, 0);
            Vector2 scoreAmountPos = new(Width - scoreAmountSize.X, scoreSize.Y);

            if (scoreSize.X > scoreAmountSize.X)
                scoreAmountPos.X = scorePos.X + scoreSize.X / 2 - scoreAmountSize.X / 2;

            else
                scorePos.X = scoreAmountPos.X + scoreAmountSize.X / 2 - scoreSize.X / 2;

            DrawStringOnScreen(livesStr, livesPos, 1.5f);

            for (int x = 0; x < CurrentPlayerLives; x++)
            {
                Rectangle destinationRec = new((int)(livesPos.X + livesSize.X + (livesSize.Y * x)), (int)livesPos.Y, (int)livesSize.Y, (int)livesSize.Y);
                spriteBatch.Draw(PlayerEnemySpriteSheet, destinationRec, new Rectangle(4, 1, 14, 14), Color.White, 0f, new Vector2(), SpriteEffects.None, TextLayer);
            }

            DrawStringOnScreen(scoreStr, scorePos, 1.2f);
            DrawStringOnScreen(scoreAmountStr, scoreAmountPos, 1.2f);
        }

        void DrawMainMenu()
        {
            //int spriteWidth = (int)(Width * .075f);
            //int spriteHeight = (int)(Height * .075f);

            //Rectangle destRec = new(Width - spriteWidth, Height - spriteHeight, spriteWidth, spriteHeight);

            float scale = 0.4f;

            Vector2 pos = new(Width / 2 - (PacManMenuLogoTex.Width * scale) / 2, Height / 2 - (PacManMenuLogoTex.Height * scale) / 2);

            spriteBatch.Draw(PacManMenuLogoTex, pos, null, Color.White, 0f, new Vector2(), scale, SpriteEffects.None, TextLayer);
        }

        void DrawLoseScreen()
        {
            float scale = 2.0f;

            string loseStr = "You Lose!";
            string replayStr = "Play Again? (Y/N)";

            Vector2 loseSize = MeasureString(replayStr) * scale;
            Vector2 replaySize = MeasureString(replayStr) * scale;

            Vector2 losePos = new(Width / 2 - (loseSize.X / 2), Height / 3 - (loseSize.Y / 2));
            Vector2 replayPos = new(Width / 2 - (replaySize.X / 2), Height / 2 - (replaySize.Y / 2));

            DrawStringOnScreen(loseStr, losePos, scale);
            DrawStringOnScreen(replayStr, replayPos, scale);
        }

        void DrawEndScreen()
        {
            float gameWonScale = 3.0f;
            float scale = 2.0f;

            string gameWonStr = "You Won!";
            string finalScoreStr = "Final Score:";
            string scoreStr = $"{CurrentPlayerScore}";
            string replayStr = "Play Again? (Y/N)";

            Vector2 gameWonSize = MeasureString(gameWonStr) * gameWonScale;
            Vector2 finalScoreSize = MeasureString(finalScoreStr) * scale;
            Vector2 scoreSize = MeasureString(scoreStr) * scale;
            Vector2 replaySize = MeasureString(replayStr) * scale;

            Vector2 gameWonPos = new(Width / 2 - (gameWonSize.X / 2), Height / 3 - (gameWonSize.Y / 2));
            Vector2 finalScorePos = new(Width / 2 - (finalScoreSize.X / 2), (gameWonPos.Y + finalScoreSize.Y) * 1.1f);
            Vector2 scorePos = new(Width / 2 - (scoreSize.X / 2), finalScorePos.Y + scoreSize.Y);
            Vector2 replayPos = new(Width / 2 - (replaySize.X / 2), Height / 2 - (replaySize.Y / 2));

            DrawStringOnScreen(gameWonStr, gameWonPos, gameWonScale);
            DrawStringOnScreen(finalScoreStr, finalScorePos, scale);
            DrawStringOnScreen(scoreStr, scorePos, scale);
            DrawStringOnScreen(replayStr, replayPos, scale);
        }

        /// <summary>
        /// Takes a string and returns it's height and width
        /// </summary>
        /// <param name="stringToMeasure">The string that is to be measured</param>
        /// <returns>Vector2 containing the length and width of the string</returns>
        Vector2 MeasureString(string stringToMeasure)
        {
            return GameFont.MeasureString(stringToMeasure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringToDraw">String that is to be drawn</param>
        /// <param name="drawPos">Position to draw</param>
        /// <param name="scale">Scale of text</param>
        void DrawStringOnScreen(string stringToDraw, Vector2 drawPos, float scale)
        {
            spriteBatch.DrawString(GameFont, stringToDraw, drawPos, Color.White, 0f, new Vector2(), scale, new SpriteEffects(), TextLayer);
        }

        void ContinueCheck(bool gameIsOver)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                if (DoRandomLevels)
                {
                    levelManager.LoadRandomLevel(out PlayerChar, out GhostManager);
                    UpdateStats();
                }

                else if (gameIsOver)
                {
                    levelManager.LoadSpecificLevel(0, out PlayerChar, out GhostManager);
                    ClearStats();
                }

                else
                {
                    levelManager.LoadCurrentLevel(out PlayerChar, out GhostManager);
                    ResetStats();
                }

                CurrentState = GameState.InGame;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.N))
                Exit();
        }
    }
}