using Artemis.System;
using LD34.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD34.Systems
{
    internal sealed class StarSystem : ProcessingSystem
    {
        private readonly SpriteBatch spriteBatch;
        private readonly WindowControl window;

        // Content.
        private readonly Texture2D stars0, stars1, stars2;

        private float offset0, offset1, offset2;

        public StarSystem(SpriteBatch spriteBatch, ContentManager content, WindowControl window)
        {
            this.spriteBatch = spriteBatch;
            this.window = window;

            // Load content.
            stars0 = content.Load<Texture2D>("Textures/stars_0");
            stars1 = content.Load<Texture2D>("Textures/stars_1");
            stars2 = content.Load<Texture2D>("Textures/stars_2");

            offset0 = 0f;
            offset1 = 0f;
            offset2 = 0f;
        }

        public override void ProcessSystem()
        {
            float delta = BlackBoard.GetEntry<float>("delta");

            float y = 0f;
            while (y < window.Height / LD34Game.Scale)
            {
                float x = offset0;
                while (x < window.Width / LD34Game.Scale)
                {
                    spriteBatch.Draw(stars0, new Vector2(x, y), Color.White);
                    x += stars0.Width;
                }

                x = offset1;
                while (x < window.Width / LD34Game.Scale)
                {
                    spriteBatch.Draw(stars1, new Vector2(x, y), Color.White);
                    x += stars1.Width;
                }

                x = offset2;
                while (x < window.Width / LD34Game.Scale)
                {
                    spriteBatch.Draw(stars2, new Vector2(x, y), Color.White);
                    x += stars2.Width;
                }

                y += stars0.Height;
            }

            offset0 -= delta;
            if (offset0 <= -stars0.Width)
                offset0 += stars0.Width;

            offset1 -= delta * 2f;
            if (offset1 <= -stars1.Width)
                offset1 += stars1.Width;

            offset2 -= delta * 4f;
            if (offset2 <= -stars2.Width)
                offset2 += stars2.Width;
        }
    }
}
