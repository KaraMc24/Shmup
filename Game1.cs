﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Shmup
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D saucerTxr, missileTxr, backgroundTxr;
        Point screenSize = new Point(800, 450);
        float spawnCooldown = 2;

        Sprite backgroundSprite;
        PlayerSprite playerSprite;
        List<MissileSprite> missileList = new List<MissileSprite>();
        SpriteFont UiFont;

        public SpriteFont BigFont { get; private set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            saucerTxr = Content.Load<Texture2D>("saucer");
            missileTxr = Content.Load<Texture2D>("missile");
            backgroundTxr = Content.Load<Texture2D>("background");
            UiFont = Content.Load<SpriteFont>("UIFont");
            BigFont = Content.Load<SpriteFont>("BigFont");

            backgroundSprite = new Sprite(backgroundTxr, new Vector2());
            playerSprite = new PlayerSprite(saucerTxr, new Vector2(screenSize.X/6, screenSize.Y/2));
            
        }

        protected override void Update(GameTime gameTime)
        {
           Random rng = new Random();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (spawnCooldown > 0)
            {
                spawnCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (missileList.Count < 5)
            {

                missileList.Add(new MissileSprite(
                missileTxr,
                new Vector2(screenSize.X, rng.Next(0, screenSize.Y - missileTxr.Height))
                ));
                spawnCooldown = (float)(rng.NextDouble() + 0.5);
            }

            playerSprite.Update(gameTime, screenSize);
            foreach (MissileSprite missile in missileList)
            {
                missile.Update(gameTime, screenSize);
                if (playerSprite.IsColliding(missile))
                {
                    missile.dead = true;
                    playerSprite.playerLives--;
                }
            }

           
            
            missileList.RemoveAll(missile => missile.dead);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            backgroundSprite.Draw(_spriteBatch);
            playerSprite.Draw(_spriteBatch);


            foreach(MissileSprite missile in missileList) missile.Draw(_spriteBatch);

            _spriteBatch.DrawString(
                UiFont,
                "Lives: " + playerSprite.playerLives,
                new Vector2(10,10),
                Color.White
                );

            if (playerSprite.playerLives <= 0)
            {
                Vector2 textSize = BigFont.MeasureString("GAME OVER");
                _spriteBatch.DrawString(
                    BigFont,
                    "GAME OVER",
                    new Vector2((screenSize.X / 2) - (textSize.X) / 2, (screenSize.Y / 2) - (textSize.Y / 2)),
                    Color.White);


            }


            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
