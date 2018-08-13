using Artemis.System;
using Artemis;
using LD34.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LD34.Systems
{
    internal sealed class IndicatorDrawSystem : EntityProcessingSystem
    {
        private readonly SpriteBatch spriteBatch;

        // Content.
        private readonly SpriteFont cardAttributeFont;

        private float delta;

        public IndicatorDrawSystem(SpriteBatch spriteBatch, ContentManager content)
            : base(Aspect.All(typeof(PositionComponent), typeof(IndicatorComponent)))
        {
            this.spriteBatch = spriteBatch;

            // Load content.
            cardAttributeFont = content.Load<SpriteFont>("Fonts/card_attribute_font");
        }

        protected override void Begin()
        {
            delta = BlackBoard.GetEntry<float>("delta");
        }

        public override void Process(Entity entity)
        {
            PositionComponent positionComponent = entity.GetComponent<PositionComponent>();
            IndicatorComponent indicatorComponent = entity.GetComponent<IndicatorComponent>();

            Vector2 size = cardAttributeFont.MeasureString((indicatorComponent.Change > 0 ? "+" : "") + indicatorComponent.Change.ToString());
            spriteBatch.DrawString(cardAttributeFont,
                (indicatorComponent.Change > 0 ? "+" : "") + indicatorComponent.Change.ToString(),
                positionComponent.Position - size / 2f,
                (indicatorComponent.Change > 0 ? Color.Green : (indicatorComponent.Change == 0 ? Color.Yellow : Color.Red)) * indicatorComponent.Opacity);

            positionComponent.Position += new Vector2(0, -delta * 4f);
            indicatorComponent.Opacity += (0f - indicatorComponent.Opacity) * delta;

            if (indicatorComponent.Opacity <= 0.025f)
                entity.Delete();
        }
    }
}
