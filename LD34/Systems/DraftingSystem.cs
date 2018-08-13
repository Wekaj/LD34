using Artemis;
using Artemis.Manager;
using Artemis.System;
using LD34.Components;
using LD34.Display;
using LD34.Game.Cards;
using LD34.Game.Cards.Minions;
using LD34.Game.Cards.Spells;
using LD34.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LD34.Systems
{
    internal sealed class DraftingSystem : ProcessingSystem
    {
        private delegate Card CardCreator(ContentManager content, EntityWorld world);

        // Content.
        private readonly SoundEffect cardSelection;
        private readonly Texture2D help3;

        private readonly LD34Game game;
        private readonly SpriteBatch spriteBatch;
        private readonly MouseHandler mouse;
        private readonly WindowControl window;
        private readonly ContentManager content;
        private readonly Random random;
        private readonly CardCreator[] cards;
        private readonly List<Entity> deck;
        private readonly Entity[] choices;
        private readonly List<Entity> discarded;
        private List<Card> newDeck;
        private Entity help3Entity;

        public DraftingSystem(LD34Game game, SpriteBatch spriteBatch, MouseHandler mouse, WindowControl window, ContentManager content)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.mouse = mouse;
            this.window = window;
            this.content = content;
            random = new Random();
            cards = new CardCreator[] { (ContentManager contentManager, EntityWorld world) => new GolemCard(content, world),
                (ContentManager contentManager, EntityWorld world) => new ImpCard(content),
                (ContentManager contentManager, EntityWorld world) => new ImpLordCard(content, world),
                (ContentManager contentManager, EntityWorld world) => new ManaGolem(content, world),
                (ContentManager contentManager, EntityWorld world) => new DemonFuryCard(),
                (ContentManager contentManager, EntityWorld world) => new DisenchantCard(),
                (ContentManager contentManager, EntityWorld world) => new FireballCard(),
                (ContentManager contentManager, EntityWorld world) => new SanctuaryCard(),
                (ContentManager contentManager, EntityWorld world) => new StatueCard(content, world),
                (ContentManager contentManager, EntityWorld world) => new ChargeCard(),
                (ContentManager contentManager, EntityWorld world) => new StormCard(),
                (ContentManager contentManager, EntityWorld world) => new BlinkCard(),
                (ContentManager contentManager, EntityWorld world) => new HawkCard(content),
                (ContentManager contentManager, EntityWorld world) => new BearCard(content),
                (ContentManager contentManager, EntityWorld world) => new SwordsmanCard(content),
                (ContentManager contentManager, EntityWorld world) => new ShieldsmanCard(content),
                (ContentManager contentManager, EntityWorld world) => new ImpBallCard(content),
                (ContentManager contentManager, EntityWorld world) => new LionCard(content, world),
            };
            deck = new List<Entity>();
            choices = new Entity[2];
            discarded = new List<Entity>();

            // Load content.
            cardSelection = content.Load<SoundEffect>("SoundEffects/card_opening");
            help3 = content.Load<Texture2D>("Textures/help3");
        }

        public void Initialize()
        {
            CreateChoices();

            help3Entity = EntityWorld.CreateEntity();
            help3Entity.AddComponent(new SpriteComponent(help3, help3.Bounds, Color.White, 0f));
            help3Entity.AddComponent(new PositionComponent(new Vector2(16f, window.Height / LD34Game.Scale - 16f - help3.Height)));
        }

        public override void ProcessSystem()
        {
            float delta = BlackBoard.GetEntry<float>("delta");

            if (deck.Count >= 30 || newDeck != null)
            {
                if (help3Entity != null)
                {
                    SpriteComponent spriteComponent = help3Entity.GetComponent<SpriteComponent>();
                    spriteComponent.Opacity += (0f - spriteComponent.Opacity) * 8f * delta;
                    if (spriteComponent.Opacity <= 0.025f)
                    {
                        help3Entity.Delete();
                        help3Entity = null;
                    }
                }

                if (newDeck == null)
                {
                    newDeck = new List<Card>();
                    foreach (Entity card in deck)
                        newDeck.Add(card.GetComponent<CardComponent>().Card);
                }

                for (int i = 0; i < discarded.Count; i++)
                {
                    Entity card = discarded[i];

                    PositionComponent positionComponent = card.GetComponent<PositionComponent>();
                    Vector2 targetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, (window.Height / LD34Game.Scale) + 32f);
                    positionComponent.Position += (targetPosition - positionComponent.Position) * 8f * delta;

                    CardComponent cardComponent = card.GetComponent<CardComponent>();
                    cardComponent.Separation += (1f - cardComponent.Separation) * 4f * delta;

                    if (Vector2.Distance(positionComponent.Position, targetPosition) < 10f)
                    {
                        discarded[i].Delete();
                        discarded.RemoveAt(i);
                        i--;
                    }
                }

                for (int i = 0; i < deck.Count; i++)
                {
                    Entity card = deck[i];
                    
                    PositionComponent positionComponent = card.GetComponent<PositionComponent>();
                    Vector2 targetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, (window.Height / LD34Game.Scale) + 32f);
                    positionComponent.Position += (targetPosition - positionComponent.Position) * 8f * delta;

                    CardComponent cardComponent = card.GetComponent<CardComponent>();
                    cardComponent.Separation += (1f - cardComponent.Separation) * 4f * delta;

                    if (Vector2.Distance(positionComponent.Position, targetPosition) < 32f)
                    {
                        deck[i].Delete();
                        deck.RemoveAt(i);
                        i--;
                    }
                }

                if (discarded.Count == 0 && deck.Count == 0 && help3Entity == null)
                {
                    EntityWorld.SystemManager.SetSystem(new BoardSystem(mouse, content, window), GameLoopType.Update);
                    EntityWorld.SystemManager.SetSystem(new EnemyHandSystem(window, content, new List<Card>() { new ImpCard(content) }), GameLoopType.Update);
                    EntityWorld.SystemManager.SetSystem(new PlayerHandSystem(game, window, content, mouse, newDeck), GameLoopType.Update);
                    EntityWorld.SystemManager.SetSystem(new MinionHoverSystem(mouse), GameLoopType.Update);
                    EntityWorld.SystemManager.SetSystem(new MinionDrawSystem(spriteBatch), GameLoopType.Draw);
                    EntityWorld.SystemManager.SetSystem(new ResourceDrawSystem(spriteBatch, window, content), GameLoopType.Draw);
                    EntityWorld.SystemManager.SetSystem(new IndicatorDrawSystem(spriteBatch, content), GameLoopType.Draw);
                    EntityWorld.SystemManager.GetSystem<BoardSystem>()[0].Initialize();
                    for (int i = 0; i < 4; i++)
                        EntityWorld.SystemManager.GetSystem<PlayerHandSystem>()[0].DrawCard();
                    for (int i = 0; i < 4; i++)
                        EntityWorld.SystemManager.GetSystem<EnemyHandSystem>()[0].DrawCard();
                    EntityWorld.SystemManager.GetSystem<PlayerHandSystem>()[0].StartTurn();
                    Toggle();
                }
            }
            else
            {
                SpriteComponent spriteComponent = help3Entity.GetComponent<SpriteComponent>();
                spriteComponent.Opacity += (1f - spriteComponent.Opacity) * 8f * delta;

                for (int i = 0; i < choices.Length; i++)
                {
                    Entity card = choices[i];

                    PositionComponent positionComponent = card.GetComponent<PositionComponent>();
                    Vector2 targetPosition = new Vector2((window.Width / LD34Game.Scale) / 1.25f - 32f + (i * 2 - 1) * 40f, (window.Height / LD34Game.Scale) / 2f - 48f);
                    positionComponent.Position += (targetPosition - positionComponent.Position) * 4f * delta;

                    CardComponent cardComponent = card.GetComponent<CardComponent>();
                    cardComponent.Separation += (1f - cardComponent.Separation) * 4f * delta;
                }

                int width = Math.Min((int)((window.Width / LD34Game.Scale) / 1.25f - 72f) / 68, 5);
                float[] offsets = new float[width];
                for (int i = 0; i < deck.Count; i++)
                {
                    Entity card = deck[i];

                    int column = i % width;
                    int row = i / width;
                    PositionComponent positionComponent = card.GetComponent<PositionComponent>();
                    Vector2 targetPosition = new Vector2(4f + column * 68f, 4f + row * 32f + offsets[column]);
                    positionComponent.Position += (targetPosition - positionComponent.Position) * 16f * delta;

                    CardComponent cardComponent = card.GetComponent<CardComponent>();
                    if (new Rectangle(positionComponent.Position.ToPoint(), new Point(64, 26 + (int)(cardComponent.Separation * 70f))).Intersects(new Rectangle((int)(mouse.State.X / LD34Game.Scale), (int)(mouse.State.Y / LD34Game.Scale), 0, 0)))
                        cardComponent.Separation += (1f - cardComponent.Separation) * 8f * delta;
                    else
                        cardComponent.Separation += (0f - cardComponent.Separation) * 8f * delta;

                    offsets[column] += cardComponent.Separation * 70f;
                }

                for (int i = 0; i < discarded.Count; i++)
                {
                    Entity card = discarded[i];

                    PositionComponent positionComponent = card.GetComponent<PositionComponent>();
                    Vector2 targetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, window.Height / LD34Game.Scale + 32f);
                    positionComponent.Position += (targetPosition - positionComponent.Position) * 8f * delta;

                    CardComponent cardComponent = card.GetComponent<CardComponent>();
                    cardComponent.Separation += (1f - cardComponent.Separation) * 4f * delta;

                    if (Vector2.Distance(positionComponent.Position, targetPosition) < 10f)
                    {
                        discarded[i].Delete();
                        discarded.RemoveAt(i);
                        i--;
                    }
                }

                if (game.IsActive)
                {
                    if (mouse.Left.ToHeld)
                    {
                        deck.Add(choices[0]);
                        choices[0] = null;
                        CreateChoices();
                    }
                    else if (mouse.Right.ToHeld)
                    {
                        deck.Add(choices[1]);
                        choices[1] = null;
                        CreateChoices();
                    }

                    if (mouse.Left.ToHeld || mouse.Right.ToHeld)
                        cardSelection.Play();
                }
            }
        }

        private void CreateChoices()
        {
            for (int i = 0; i < choices.Length; i++)
                if (choices[i] != null)
                {
                    discarded.Add(choices[i]);
                    choices[i] = null;
                }

            List<int> values = new List<int>();
            for (int i = 0; i < cards.Length; i++)
                values.Add(i);

            for (int i = 0; i < choices.Length; i++)
            {
                int value = values[random.Next(values.Count)];
                values.Remove(value);

                Entity card = EntityWorld.CreateEntity();
                card.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, -96f)));
                card.AddComponent(new CardComponent(cards[value](content, EntityWorld), true, 0f, 1f, Vector2.Zero, false, false, 1f));
                choices[i] = card;
            }
        }
    }
}
