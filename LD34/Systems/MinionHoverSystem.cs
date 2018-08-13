using Artemis.System;
using Artemis;
using LD34.Components;
using LD34.Input;
using Microsoft.Xna.Framework;

namespace LD34.Systems
{
    internal sealed class MinionHoverSystem : EntityProcessingSystem
    {
        private readonly MouseHandler mouse;
        private float delta;

        public MinionHoverSystem(MouseHandler mouse)
            : base(Aspect.All(typeof(MinionComponent), typeof(PositionComponent)))
        {
            this.mouse = mouse;
        }

        protected override void Begin()
        {
            delta = BlackBoard.GetEntry<float>("delta");
        }

        public override void Process(Entity entity)
        {
            MinionComponent minionComponent = entity.GetComponent<MinionComponent>();
            PositionComponent positionComponent = entity.GetComponent<PositionComponent>();

            if (new Rectangle(positionComponent.Position.ToPoint(), new Point(32, 32)).Intersects(new Rectangle((int)(mouse.State.X / LD34Game.Scale), (int)(mouse.State.Y / LD34Game.Scale), 0, 0)))
            {
                if (!entity.HasComponent<CardComponent>())
                    entity.AddComponent(new CardComponent(minionComponent.Minion, true, 0f, 0f, new Vector2(-16f, -96f), false, false, 0f));
                CardComponent cardComponent = entity.GetComponent<CardComponent>();
                cardComponent.ModifierOpacity += (1f - cardComponent.ModifierOpacity) * 6f * delta;
                cardComponent.Separation += (1f - cardComponent.Separation) * 12f * delta;
                if (cardComponent.Separation > 1f)
                    cardComponent.Separation = 1f;
                cardComponent.Opacity += (1f - cardComponent.Opacity) * 12f * delta;
                cardComponent.Offset += (new Vector2(-16f, -128f) - cardComponent.Offset) * 12f * delta;
            }
            else if (entity.HasComponent<CardComponent>())
            {
                CardComponent cardComponent = entity.GetComponent<CardComponent>();
                cardComponent.Separation += (0f - cardComponent.Separation) * 12f * delta;
                cardComponent.Opacity += (0f - cardComponent.Opacity) * 12f * delta;
                cardComponent.Offset += (new Vector2(-16f, -96f) - cardComponent.Offset) * 12f * delta;
                if (cardComponent.Opacity < 0.025f)
                    entity.RemoveComponent<CardComponent>();
            }
        }
    }
}
