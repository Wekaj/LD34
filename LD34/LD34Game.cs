using Artemis;
using Artemis.Manager;
using Artemis.System;
using LD34.Components;
using LD34.Display;
using LD34.Game.Cards.Minions;
using LD34.Game.Cards.Spells;
using LD34.Input;
using LD34.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LD34 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class LD34Game : Microsoft.Xna.Framework.Game {
        private readonly GraphicsDeviceManager graphics;
        private State state;

        public static float Scale { get; set; }

        public LD34Game() {
            graphics = new GraphicsDeviceManager(this);
            EntityWorld = new EntityWorld();
            WindowControl = new WindowControl(graphics, Window);
            KeyboardHandler = new KeyboardHandler();
            MouseHandler = new MouseHandler();
            state = State.Drafting;

            KeyboardHandler.AddButton("left", Keys.Left);
            KeyboardHandler.AddButton("right", Keys.Right);

            Content.RootDirectory = "Content";
        }

        public EntityWorld EntityWorld { get; }
        public WindowControl WindowControl { get; }
        public KeyboardHandler KeyboardHandler { get; }
        public MouseHandler MouseHandler { get; }
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            WindowControl.SetDisplayType(DisplayType.Borderless);
            WindowControl.SetSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            Scale = 2f;
            if (WindowControl.Height <= 1000)
                Scale = 1f;
            IsMouseVisible = true;

            Window.Title = "Void Grid";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Add entity systems.
            EntityWorld.SystemManager.SetSystem(new MenuSystem(this, SpriteBatch, MouseHandler, WindowControl, Content), GameLoopType.Update);
            EntityWorld.SystemManager.SetSystem(new StarSystem(SpriteBatch, Content, WindowControl), GameLoopType.Draw);
            EntityWorld.SystemManager.SetSystem(new SpriteDrawSystem(SpriteBatch), GameLoopType.Draw);
            EntityWorld.SystemManager.SetSystem(new CardDrawSystem(SpriteBatch, Content), GameLoopType.Draw);

            EntityWorld.SystemManager.GetSystem<MenuSystem>()[0].Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            KeyboardHandler.Update(Keyboard.GetState());
            MouseHandler.Update(Mouse.GetState());

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            EntitySystem.BlackBoard.SetEntry("delta", (float)gameTime.ElapsedGameTime.TotalSeconds);
            EntityWorld.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(Scale));

            EntitySystem.BlackBoard.SetEntry("delta", (float)gameTime.ElapsedGameTime.TotalSeconds);
            EntityWorld.Draw();

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        private enum State {
            Drafting,
            Playing
        }
    }
}
