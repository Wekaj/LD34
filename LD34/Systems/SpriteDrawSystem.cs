using Artemis.System;
using Artemis;
using LD34.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD34.Systems
{
    internal sealed class SpriteDrawSystem : EntityProcessingSystem
    {
        private readonly SpriteBatch spriteBatch;

        public SpriteDrawSystem(SpriteBatch spriteBatch)
            : base(Aspect.All(typeof(PositionComponent), typeof(SpriteComponent)))
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Process(Entity entity)
        {
            PositionComponent positionComponent = entity.GetComponent<PositionComponent>();
            SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();

            spriteBatch.Draw(spriteComponent.Texture, 
                new Rectangle((int)positionComponent.Position.X + spriteComponent.Bounds.X,
                    (int)positionComponent.Position.Y + spriteComponent.Bounds.Y,  
                    spriteComponent.Bounds.Width, 
                    spriteComponent.Bounds.Height), 
                spriteComponent.Color * spriteComponent.Opacity);
        }
    }
}
