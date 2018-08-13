using Artemis;
using Artemis.System;
using LD34.Args;
using LD34.Components;
using LD34.Game.Cards;
using LD34.Game.Cards.Minions;
using LD34.Game.Cards.Spells;
using LD34.Game.Modifiers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LD34.Systems
{
    internal enum Action
    {
        Wait,
        Grow,
        Play
    }

    internal abstract class HandSystem : ProcessingSystem
    {
        private const int maxCards = 15;
        private readonly ContentManager content;
        private readonly Vector2 cardStart;
        private readonly bool faceUp;
        private readonly List<Entity> cards;
        private readonly bool player;
        private readonly List<Card> deck;
        private State state;
        private float focusCardTimer;
        private float attackTimer;
        private readonly CardCreator[] enemyCards;

        // Content.
        private readonly SoundEffect cardSelection;
        private readonly SoundEffect cardOpening;
        private readonly SoundEffect grow;
        private readonly SoundEffect noMana;
        private readonly SoundEffect turn;

        protected HandSystem(ContentManager content, Vector2 cardStart, bool faceUp, bool player, List<Card> deck)
        {
            enemyCards = new CardCreator[] { (ContentManager contentManager, EntityWorld world) => new GolemCard(content, world),
                (ContentManager contentManager, EntityWorld world) => new ImpCard(content),
                (ContentManager contentManager, EntityWorld world) => new ImpLordCard(content, world),
                (ContentManager contentManager, EntityWorld world) => new ManaGolem(content, world),
                (ContentManager contentManager, EntityWorld world) => new FireballCard(),
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

            this.content = content;
            this.cardStart = cardStart;
            this.faceUp = faceUp;
            this.player = player;
            this.deck = deck;
            cards = new List<Entity>();
            state = State.WaitingForTurn;
            focusCardTimer = 0f;
            attackTimer = 0f;

            FocusCard = null;
            TotalResources = !player ? 3 : 0;
            SpentResources = 0;
            Turn = false;

            // Load content.
            cardSelection = content.Load<SoundEffect>("SoundEffects/card_selection");
            cardOpening = content.Load<SoundEffect>("SoundEffects/card_opening");
            grow = content.Load<SoundEffect>("SoundEffects/grow");
            noMana = content.Load<SoundEffect>("SoundEffects/no_mana");
            turn = content.Load<SoundEffect>("SoundEffects/turn");
        }

        public int CardCount => FocusCard != null ? cards.Count + 1 : cards.Count;
        public int TotalResources { get; private set; }
        public int SpentResources { get; private set; }
        public bool Turn { get; private set; }
        public Entity FocusCard { get; private set; }
        public ReadOnlyCollection<Entity> Cards => cards.AsReadOnly();

        public event EventHandler<PlayerEventArgs> TurnStarted, TurnEnded;
        public event EventHandler<CardHandEventArgs> CardAdded, CardPlayed;

        public override void ProcessSystem()
        {
            float delta = BlackBoard.GetEntry<float>("delta");

            switch (state)
            {
                case State.WaitingForTurn:
                    WaitingForTurn(delta);
                    break;
                case State.ChoosingCard:
                    if (FocusCard == null)
                    {
                        if (cards.Count == 0 || cards[0].GetComponent<CardComponent>().Held)
                        {
                            state = State.Walking;
                            attackTimer = 1f;
                            break;
                        }
                        FocusCard = cards[0];
                        cards.RemoveAt(0);
                        focusCardTimer = 1f;
                        cardOpening.Play();
                    }

                    CardComponent focusCardComponent = FocusCard.GetComponent<CardComponent>();
                    if (focusCardComponent.Card.Cost > TotalResources - SpentResources)
                    {
                        if (focusCardTimer <= 0f)
                        {
                            focusCardComponent.Held = true;
                            cards.Add(FocusCard);
                            FocusCard = null;
                            noMana.Play();
                            break;
                        }
                        else
                            focusCardTimer -= delta;
                    }
                    else
                    {
                        switch (GetAction(delta))
                        {
                            case Action.Grow:
                                SpentResources += Math.Min(focusCardComponent.Card.Cost, 1);
                                focusCardComponent.Held = true;
                                focusCardComponent.Growing = true;
                                focusCardComponent.Card.AddCostModifier(new GrowthModifier(-1));
                                cards.Add(FocusCard);
                                FocusCard = null;
                                grow.Play();
                                return;
                            case Action.Play:
                                SpentResources += focusCardComponent.Card.Cost;
                                state = State.PlayingCard;
                                if (CardPlayed != null)
                                    CardPlayed(this, new CardHandEventArgs(player, focusCardComponent.Card));
                                cardSelection.Play();
                                return;
                        }
                    }

                    ChoosingCard(delta);
                    break;
                case State.PlayingCard:
                    if (PlayingCard(delta))
                    {
                        state = State.ChoosingCard;
                        FocusCard.Delete();
                        FocusCard = null;
                    }
                    break;
                case State.Walking:
                    if (attackTimer <= 0f)
                    {
                        EntityWorld.SystemManager.GetSystem<BoardSystem>()[0].March(player);
                        state = State.Attacking;
                        attackTimer = 1f;
                        break;
                    }
                    else
                        attackTimer -= delta;
                    WaitingForTurn(delta);
                    break;
                case State.Attacking:
                    if (attackTimer <= 0f)
                    {
                        EntityWorld.SystemManager.GetSystem<BoardSystem>()[0].Attack();
                        EndTurn();
                        break;
                    }
                    else
                        attackTimer -= delta;
                    WaitingForTurn(delta);
                    break;
            }
        }

        public bool AddCard(Card card)
        {
            if (CardCount >= maxCards)
                return false;

            Entity cardEntity = EntityWorld.CreateEntity();
            cardEntity.AddComponent(new PositionComponent(cardStart));
            cardEntity.AddComponent(new CardComponent(card, faceUp, 0f, 1f, Vector2.Zero, false, false, 0f));
            cards.Add(cardEntity);
            if (CardAdded != null)
                CardAdded(this, new CardHandEventArgs(player, card));
            return true;
        }

        public void StartTurn()
        {
            turn.Play();

            Turn = true;

            TotalResources++;
            SpentResources = 0;

            foreach (Entity card in cards)
            {
                CardComponent cardComponent = card.GetComponent<CardComponent>();
                cardComponent.Held = false;
                cardComponent.Growing = false;
            }

            DrawCard();

            state = State.ChoosingCard;

            if (TurnStarted != null)
                TurnStarted(this, new PlayerEventArgs(player));
        }

        public void EndTurn()
        {
            Turn = false;
            state = State.WaitingForTurn;

            StartOpponentTurn();

            if (TurnEnded != null)
                TurnEnded(this, new PlayerEventArgs(player));
        }

        public void DrawCard()
        {
            if (player)
            {
                if (deck.Count > 0)
                {
                    Random random = new Random();
                    int card = random.Next(deck.Count);
                    if (deck[card] is MinionCard)
                        ((MinionCard)deck[card]).Initialize();
                    AddCard(deck[card]);
                    deck.RemoveAt(card);
                    cardOpening.Play();
                }
            }
            else
            {
                Random random = new Random();
                AddCard(enemyCards[random.Next(enemyCards.Length)](content, EntityWorld));
                cardOpening.Play();
            }
        }

        private delegate Card CardCreator(ContentManager content, EntityWorld world);

        protected abstract void WaitingForTurn(float delta);
        protected abstract bool PlayingCard(float delta);
        protected abstract void ChoosingCard(float delta);
        protected abstract Action GetAction(float delta);
        protected abstract void StartOpponentTurn();

        private enum State
        {
            WaitingForTurn,
            PlayingCard,
            ChoosingCard,
            Attacking,
            Walking
        }
    }
}
