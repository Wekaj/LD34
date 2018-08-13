using Artemis.System;
using Artemis;
using LD34.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD34.Systems
{
    internal sealed class MinionDrawSystem : EntityProcessingSystem
    {
        private readonly SpriteBatch spriteBatch;

        public MinionDrawSystem(SpriteBatch spriteBatch)
            : base(Aspect.All(typeof(MinionComponent), typeof(PositionComponent)))
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Process(Entity entity)
        {
            MinionComponent minionComponent = entity.GetComponent<MinionComponent>();
            PositionComponent positionComponent = entity.GetComponent<PositionComponent>();

            Vector2 position = positionComponent.Position + minionComponent.Minion.PieceOffset;
            spriteBatch.Draw(minionComponent.Minion.Piece, 
                new Rectangle((int)position.X,
                    (int)position.Y,
                    minionComponent.Minion.Piece.Width,
                    minionComponent.Minion.Piece.Height),
                null,
                (minionComponent.Player ? new Color(200, 175, 125) : new Color(150, 150, 200)) * minionComponent.Opacity,
                0f,
                Vector2.Zero,
                !minionComponent.Player ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f);
        }
    }
}
