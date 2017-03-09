using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using straat.Control;
using straat.View.Screen;
using System.Collections.Generic;
using straat.View.Drawing;
using straat.Model.Entities;

namespace straat
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
		public SpriteBatch spriteBatch { get; private set;}

        Input InputHandler;

        ScreenManager screenManager;

		SpriteFont font;

		double simulationTime = 0.0;
		double simulationSpeed = 1.0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			this.IsMouseVisible = true;

			graphics.PreferredBackBufferWidth = 1920;
			graphics.PreferredBackBufferHeight = 1080;
			graphics.IsFullScreen = false;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InputHandler = new Input();
			screenManager = new ScreenManager(this, graphics.GraphicsDevice.Viewport.Bounds);
			screenManager.activateScreen(new MainMenuScreen(screenManager,screenManager.maxBounds));


			this.Window.AllowUserResizing = true;
			this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("testfont");

			// Load content in all singletons

			GeometryDrawer.init(this);
			EntityFactory.Instance.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			// step forward in simulation
			simulationTime += gameTime.ElapsedGameTime.TotalMilliseconds * simulationTime;

            InputHandler.Update();

			// play/pause
			if(InputHandler.pop(InputCommand.SPACE))
				simulationSpeed = simulationSpeed == 0.0 ? 1.0 : 0.0;

			screenManager.Update(gameTime.ElapsedGameTime.TotalSeconds, InputHandler);


            if (InputHandler.pop(InputCommand.EXIT))    // no active screen intercepted ESC-key, terminate program
				Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

			screenManager.Draw(gameTime.ElapsedGameTime.TotalSeconds);

            base.Draw(gameTime);
        }

		void Window_ClientSizeChanged( object sender,EventArgs e )
		{
			screenManager.changeBounds( GraphicsDevice.PresentationParameters.Bounds );
			Console.WriteLine("Client size change");
		}
    }
}
