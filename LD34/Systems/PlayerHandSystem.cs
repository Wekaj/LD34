using Artemis;
using LD34.Components;
using Microsoft.Xna.Framework;
using LD34.Display;
using LD34.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using LD34.Game.Cards;

namespace LD34.Systems
{
    internal sealed class PlayerHandSystem : HandSystem
    {
        private readonly LD34Game game;
        private readonly WindowControl window;
        private readonly MouseHandler mouse;

        // Content
        private readonly Texture2D growIcon, playIcon;

        private bool offsetFocus;
        private Entity growEntity, playEntity;

        public PlayerHandSystem(LD34Game game, WindowControl window, ContentManager content, MouseHandler mouse, List<Card> deck)
            : base(content, new Vector2(window.Width / LD34Game.Scale, window.Height / LD34Game.Scale - 64f), true, true, deck)
        {
            this.game = game;
            this.window = window;
            this.mouse = mouse;

            // Load content.
            growIcon = content.Load<Texture2D>("Textures/grow_icon");
            playIcon = content.Load<Texture2D>("Textures/play_icon");
        }

        protected override void WaitingForTurn(float delta)
        {
            UpdateCards(delta);

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
            UpdateCards(delta);

            PositionComponent growPositionComponent = growEntity.GetComponent<PositionComponent>();
            growPositionComponent.Position += (new Vector2(growPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 1.5f + 32f) - growPositionComponent.Position) * 16f * delta;
            PositionComponent playPositionComponent = playEntity.GetComponent<PositionComponent>();
            playPositionComponent.Position += (new Vector2(playPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 1.5f + 32f) - playPositionComponent.Position) * 16f * delta;

            PositionComponent focusPositionComponent = FocusCard.GetComponent<PositionComponent>();
            Vector2 targetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, (window.Height / LD34Game.Scale) / 2f - 48f);
            focusPositionComponent.Position += (targetPosition - focusPositionComponent.Position) * 4f * delta;

            if (Vector2.Distance(targetPosition, focusPositionComponent.Position) <= (window.Height / LD34Game.Scale) / 16f)
            {
                CardComponent focusCardComponent = FocusCard.GetComponent<CardComponent>();
                focusCardComponent.Opacity += (0f - focusCardComponent.Opacity) * 8f * delta;
                if (focusCardComponent.Opacity <= 0.025f)
                {
                    focusCardComponent.Card.Play(EntityWorld, true);
                    return true;
                }
            }
            return false;
        }

        protected override void ChoosingCard(float delta)
        {
            UpdateCards(delta);

            if (growEntity == null)
            {
                growEntity = EntityWorld.CreateEntity();
                growEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f + 48f, (window.Height / LD34Game.Scale) / 1.5f + 256f)));
                growEntity.AddComponent(new SpriteComponent(growIcon, growIcon.Bounds, Color.White, 0f));
            }

            if (playEntity == null)
            {
                playEntity = EntityWorld.CreateEntity();
                playEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - 48f - playIcon.Width, (window.Height / LD34Game.Scale) / 1.5f + 256f)));
                playEntity.AddComponent(new SpriteComponent(playIcon, playIcon.Bounds, Color.White, 0f));
            }

            SpriteComponent growSpriteComponent = growEntity.GetComponent<SpriteComponent>();
            growSpriteComponent.Opacity += (1f - growSpriteComponent.Opacity) * 4f * delta;
            SpriteComponent playSpriteComponent = playEntity.GetComponent<SpriteComponent>();
            playSpriteComponent.Opacity += (1f - playSpriteComponent.Opacity) * 4f * delta;

            PositionComponent growPositionComponent = growEntity.GetComponent<PositionComponent>();
            growPositionComponent.Position += (new Vector2(growPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 1.5f + 32f) - growPositionComponent.Position) * 8f * delta;
            PositionComponent playPositionComponent = playEntity.GetComponent<PositionComponent>();
            playPositionComponent.Position += (new Vector2(playPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 1.5f + 32f) - playPositionComponent.Position) * 8f * delta;

            PositionComponent focusPositionComponent = FocusCard.GetComponent<PositionComponent>();
            Vector2 focusTargetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, (window.Height / LD34Game.Scale) / 1.5f);
            focusTargetPosition.Y -= offsetFocus ? 48f : 0f;
            focusPositionComponent.Position += (focusTargetPosition - focusPositionComponent.Position) * 4f * delta;

            CardComponent focusCardComponent = FocusCard.GetComponent<CardComponent>();
            focusCardComponent.Separation += (1f - focusCardComponent.Separation) * 12f * delta;
        }

        protected override Action GetAction(float delta)
        {
            if (game.IsActive)
            {
                if (mouse.Right.ToHeld)
                {
                    growEntity.GetComponent<PositionComponent>().Position += new Vector2(0f, 32f);
                    return Action.Grow;
                }
                else if (mouse.Left.ToHeld)
                {
                    playEntity.GetComponent<PositionComponent>().Position += new Vector2(0f, -32f);
                    return Action.Play;
                }
            }
            return Action.Wait;
        }

        protected override void StartOpponentTurn()
        {
            EntityWorld.SystemManager.GetSystem<EnemyHandSystem>()[0].StartTurn();
        }

        private void UpdateCards(float delta)
        {
            offsetFocus = false;
            for (int i = 0; i < Cards.Count; i++)
            {
                PositionComponent positionComponent = Cards[i].GetComponent<PositionComponent>();
                Vector2 targetPosition = new Vector2(((window.Width / LD34Game.Scale) / 2f) + (i - (Cards.Count / 2f)) * 64f, (window.Height / LD34Game.Scale) - 64f);

                CardComponent cardComponent = Cards[i].GetComponent<CardComponent>();
                if (new Rectangle(targetPosition.ToPoint(), new Point(64, 96)).Intersects(new Rectangle((int)(mouse.State.X / LD34Game.Scale), (int)(mouse.State.Y / LD34Game.Scale), 0, 0)))
                {
                    cardComponent.Separation += (1f - cardComponent.Separation) * 12f * delta;
                    targetPosition.Y -= 48f;
                    if (i == Cards.Count / 2 || (Cards.Count % 2 == 0 && i == Cards.Count / 2 - 1))
                        offsetFocus = true;
                }
                else
                    cardComponent.Separation += (0f - cardComponent.Separation) * 8f * delta;

                positionComponent.Position += (targetPosition - positionComponent.Position) * 4f * delta;
            }
        }
    }
}
