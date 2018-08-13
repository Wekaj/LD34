using Artemis;
using LD34.Components;
using LD34.Display;
using LD34.Game.Cards;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LD34.Systems
{
    internal sealed class EnemyHandSystem : HandSystem
    {
        private readonly WindowControl window;
        private readonly Random random;

        // Content
        private readonly Texture2D growIconFlipped, playIconFlipped;

        private float changeTimer, selectionDelay;
        private int targetCard;
        private Entity growEntity, playEntity;

        public EnemyHandSystem(WindowControl window, ContentManager content, List<Card> deck)
            : base(content, new Vector2(window.Width / LD34Game.Scale, 0f), false, false, deck)
        {
            this.window = window;
            random = new Random();
            changeTimer = 0f;
            selectionDelay = 1f;
            targetCard = 0;

            // Load content.
            growIconFlipped = content.Load<Texture2D>("Textures/grow_icon_flipped");
            playIconFlipped = content.Load<Texture2D>("Textures/play_icon_flipped");
        }
            
        protected override void WaitingForTurn(float delta)
        {
            UpdateCards(delta, false);

            if (growEntity != null)
            {
                SpriteComponent growSpriteComponent = growEntity.GetComponent<SpriteComponent>();
                growSpriteComponent.Opacity += (0f - growSpriteComponent.Opacity) * 4f * delta;
                if (growSpriteComponent.Opacity <= 0.025f)
                {
                    growEntity.Delete();
                    growEntity = null;
                }
            }
            if (playEntity != null)
            {
                SpriteComponent playSpriteComponent = playEntity.GetComponent<SpriteComponent>();
                playSpriteComponent.Opacity += (0f - playSpriteComponent.Opacity) * 4f * delta;
                if (playSpriteComponent.Opacity <= 0.025f)
                {
                    playEntity.Delete();
                    playEntity = null;
                }
            }
        }

        protected override bool PlayingCard(float delta)
        {
            UpdateCards(delta, true);

            PositionComponent growPositionComponent = growEntity.GetComponent<PositionComponent>();
            growPositionComponent.Position += (new Vector2(growPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 4f + 32f) - growPositionComponent.Position) * 16f * delta;
            PositionComponent playPositionComponent = playEntity.GetComponent<PositionComponent>();
            playPositionComponent.Position += (new Vector2(playPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 4f + 32f) - playPositionComponent.Position) * 16f * delta;

            PositionComponent focusPositionComponent = FocusCard.GetComponent<PositionComponent>();
            Vector2 targetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, (window.Height / LD34Game.Scale) / 2f - 48f);
            focusPositionComponent.Position += (targetPosition - focusPositionComponent.Position) * 2f * delta;
            
            CardComponent focusCardComponent = FocusCard.GetComponent<CardComponent>();
            focusCardComponent.Separation = 1f;
            focusCardComponent.FaceUp = true;

            if (Vector2.Distance(targetPosition, focusPositionComponent.Position) <= (window.Height / LD34Game.Scale) / 16f)
            {
                focusCardComponent.Opacity += (0f - focusCardComponent.Opacity) * 8f * delta;
                if (focusCardComponent.Opacity <= 0.025f)
                {
                    focusCardComponent.Card.Play(EntityWorld, false);
                    return true;
                }
            }
            return false;
        }

        protected override void ChoosingCard(float delta)
        {
            UpdateCards(delta, true);

            if (growEntity == null)
            {
                growEntity = EntityWorld.CreateEntity();
                growEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f + 48f, (window.Height / LD34Game.Scale) / 4f - 256f)));
                growEntity.AddComponent(new SpriteComponent(growIconFlipped, growIconFlipped.Bounds, Color.White, 0f));
            }

            if (playEntity == null)
            {
                playEntity = EntityWorld.CreateEntity();
                playEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - 48f - playIconFlipped.Width, (window.Height / LD34Game.Scale) / 4f - 256f)));
                playEntity.AddComponent(new SpriteComponent(playIconFlipped, playIconFlipped.Bounds, Color.White, 0f));
            }

            SpriteComponent growSpriteComponent = growEntity.GetComponent<SpriteComponent>();
            growSpriteComponent.Opacity += (1f - growSpriteComponent.Opacity) * 4f * delta;
            SpriteComponent playSpriteComponent = playEntity.GetComponent<SpriteComponent>();
            playSpriteComponent.Opacity += (1f - playSpriteComponent.Opacity) * 4f * delta;

            PositionComponent growPositionComponent = growEntity.GetComponent<PositionComponent>();
            growPositionComponent.Position += (new Vector2(growPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 4f + 32f) - growPositionComponent.Position) * 8f * delta;
            PositionComponent playPositionComponent = playEntity.GetComponent<PositionComponent>();
            playPositionComponent.Position += (new Vector2(playPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 4f + 32f) - playPositionComponent.Position) * 8f * delta;

            PositionComponent focusPositionComponent = FocusCard.GetComponent<PositionComponent>();
            Vector2 focusTargetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, (window.Height / LD34Game.Scale) / 4f);
            focusPositionComponent.Position += (focusTargetPosition - focusPositionComponent.Position) * 4f * delta;
        }

        protected override Action GetAction(float delta)
        {
            if (selectionDelay <= 0f)
            {
                selectionDelay = 1f;
                if (random.Next(100) < 33)
                {
                    growEntity.GetComponent<PositionComponent>().Position += new Vector2(0f, -32f);
                    return Action.Grow;
                }
                else
                {
                    playEntity.GetComponent<PositionComponent>().Position += new Vector2(0f, 32f);
                    return Action.Play;
                }
            }
            selectionDelay -= delta;
            return Action.Wait;
        }

        protected override void StartOpponentTurn()
        {
            EntityWorld.SystemManager.GetSystem<PlayerHandSystem>()[0].StartTurn();
        }

        private void UpdateCards(float delta, bool choosing)
        {
            if (!choosing)
            {
                if (changeTimer <= 0f)
                {
                    if (targetCard == 0)
                        targetCard++;
                    else if (targetCard == Cards.Count - 1)
                        targetCard--;
                    else
                        targetCard += random.Next(2) == 0 ? 1 : -1;
                    changeTimer = random.Next(1, 10);
                }
                else
                    changeTimer -= delta;
                if (targetCard >= Cards.Count)
                    targetCard = Cards.Count - 1;
            }

            for (int i = 0; i < Cards.Count; i++)
            {
                PositionComponent positionComponent = Cards[i].GetComponent<PositionComponent>();
                Vector2 targetPosition = new Vector2(((window.Width / LD34Game.Scale) / 2f) + (i - (Cards.Count / 2f)) * 64f, 16f + (i == targetCard && !choosing ? 16f : 0f));
                positionComponent.Position += (targetPosition - positionComponent.Position) * 4f * delta;
            }
        }
    }
}
