using Artemis.System;
using LD34.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD34.Systems
{
    internal sealed class ResourceDrawSystem : ProcessingSystem
    {
        private readonly SpriteBatch spriteBatch;
        private readonly WindowControl window;

        // Content.
        private readonly Texture2D resourceAvailable, resourceSpent, resourceAvailableFlipped, resourceSpentFlipped;

        public ResourceDrawSystem(SpriteBatch spriteBatch, WindowControl window, ContentManager content)
        {
            this.spriteBatch = spriteBatch;
            this.window = window;

            // Load content.
            resourceAvailable = content.Load<Texture2D>("Textures/resource_available");
            resourceSpent = content.Load<Texture2D>("Textures/resource_spent");
            resourceAvailableFlipped = content.Load<Texture2D>("Textures/resource_available_flipped");
            resourceSpentFlipped = content.Load<Texture2D>("Textures/resource_spent_flipped");
        }
        
        public override void ProcessSystem()
        {
            float delta = BlackBoard.GetEntry<float>("delta");

            int total = EntityWorld.SystemManager.GetSystem<PlayerHandSystem>()[0].TotalResources;
            int spent = EntityWorld.SystemManager.GetSystem<PlayerHandSystem>()[0].SpentResources;
            for (int i = 0; i < total; i++)
                spriteBatch.Draw(i < total - spent ? resourceAvailable : resourceSpent,
                    new Vector2((window.Width / LD34Game.Scale) / 2f - total * resourceAvailable.Width / 2 + i * resourceAvailable.Width, window.Height / LD34Game.Scale - resourceAvailable.Height),
                    Color.White);

            total = EntityWorld.SystemManager.GetSystem<EnemyHandSystem>()[0].TotalResources;
            spent = EntityWorld.SystemManager.GetSystem<EnemyHandSystem>()[0].SpentResources;
            for (int i = 0; i < total; i++)
                spriteBatch.Draw(i < total - spent ? resourceAvailableFlipped : resourceSpentFlipped,
                    new Vector2((window.Width / LD34Game.Scale) / 2f - total * resourceAvailable.Width / 2 + i * resourceAvailable.Width, 0f),
                    Color.White);
        }
    }
}
