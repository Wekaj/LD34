using Artemis;
using Artemis.Manager;
using Artemis.System;
using LD34.Components;
using LD34.Display;
using LD34.Game.Cards.Spells;
using LD34.Game.Modifiers;
using LD34.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LD34.Systems
{
    internal sealed class MenuSystem : ProcessingSystem
    {
        private enum Choice
        {
            Start,
            Help,
            Quit,
            None
        }

        private readonly LD34Game game;
        private readonly ContentManager content;
        private readonly MouseHandler mouse;
        private readonly WindowControl window;
        private readonly SpriteBatch spriteBatch;

        // Content.
        private readonly Texture2D title, growIcon, playIcon, help, help2, bigHelp;
        private readonly SoundEffect cardOpening, grow, cardSelection;

        private readonly List<Entity> cards;
        private Entity titleEntity, helpEntity, help2Entity, bigHelpEntity;
        private Entity growEntity, playEntity;
        private Entity target;
        private Choice choice;

        public MenuSystem(LD34Game game, SpriteBatch spriteBatch, MouseHandler mouse, WindowControl window, ContentManager content)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.content = content;
            this.mouse = mouse;
            this.window = window;
            title = content.Load<Texture2D>("Textures/title");
            help = content.Load<Texture2D>("Textures/help");
            help2 = content.Load<Texture2D>("Textures/help2");
            bigHelp = content.Load<Texture2D>("Textures/big_help");
            growIcon = content.Load<Texture2D>("Textures/grow_icon");
            playIcon = content.Load<Texture2D>("Textures/play_icon");
            cardOpening = content.Load<SoundEffect>("SoundEffects/card_opening");
            grow = content.Load<SoundEffect>("SoundEffects/grow");
            cardSelection = content.Load<SoundEffect>("SoundEffects/card_selection");
            choice = Choice.None;
            cards = new List<Entity>();
        }

        public void Initialize()
        {
            titleEntity = EntityWorld.CreateEntity();
            titleEntity.AddComponent(new SpriteComponent(title, title.Bounds, Color.White, 1f));
            titleEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - title.Width / 2f, (window.Height / LD34Game.Scale) / 4f - title.Height / 2f)));

            helpEntity = EntityWorld.CreateEntity();
            helpEntity.AddComponent(new SpriteComponent(help, help.Bounds, Color.White, 1f));
            helpEntity.AddComponent(new PositionComponent(new Vector2(16f, 16f)));

            help2Entity = EntityWorld.CreateEntity();
            help2Entity.AddComponent(new SpriteComponent(help2, help2.Bounds, Color.White, 1f));
            help2Entity.AddComponent(new PositionComponent(new Vector2(16f, 24f + help.Height)));

            Entity startCard = EntityWorld.CreateEntity();
            startCard.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f, -96f)));
            startCard.AddComponent(new CardComponent(new StartCard(), true, 0f, 1f, Vector2.Zero, false, false, 1f));
            cards.Add(startCard);

            Entity helpCard = EntityWorld.CreateEntity();
            helpCard.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f, -96f)));
            helpCard.AddComponent(new CardComponent(new HelpCard(), true, 0f, 1f, Vector2.Zero, false, false, 1f));
            cards.Add(helpCard);

            Entity quitCard = EntityWorld.CreateEntity();
            quitCard.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f, -96f)));
            quitCard.AddComponent(new CardComponent(new QuitCard(), true, 0f, 1f, Vector2.Zero, false, false, 1f));
            cards.Add(quitCard);
        }

        public override void ProcessSystem()
        {
            float delta = BlackBoard.GetEntry<float>("delta");

            switch (choice)
            {
                case Choice.None:
                    if (growEntity == null)
                    {
                        growEntity = EntityWorld.CreateEntity();
                        growEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f + 48f, (window.Height / LD34Game.Scale) / 2f + 256f)));
                        growEntity.AddComponent(new SpriteComponent(growIcon, growIcon.Bounds, Color.White, 0f));
                    }

                    if (playEntity == null)
                    {
                        playEntity = EntityWorld.CreateEntity();
                        playEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - 48f - playIcon.Width, (window.Height / LD34Game.Scale) / 2f + 256f)));
                        playEntity.AddComponent(new SpriteComponent(playIcon, playIcon.Bounds, Color.White, 0f));
                    }

                    SpriteComponent growSpriteComponent = growEntity.GetComponent<SpriteComponent>();
                    growSpriteComponent.Opacity += (1f - growSpriteComponent.Opacity) * 4f * delta;
                    SpriteComponent playSpriteComponent = playEntity.GetComponent<SpriteComponent>();
                    playSpriteComponent.Opacity += (1f - playSpriteComponent.Opacity) * 4f * delta;

                    PositionComponent growPositionComponent = growEntity.GetComponent<PositionComponent>();
                    growPositionComponent.Position += (new Vector2(growPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 2f + 32f) - growPositionComponent.Position) * 8f * delta;
                    PositionComponent playPositionComponent = playEntity.GetComponent<PositionComponent>();
                    playPositionComponent.Position += (new Vector2(playPositionComponent.Position.X, (window.Height / LD34Game.Scale) / 2f + 32f) - playPositionComponent.Position) * 8f * delta;

                    if (target == null)
                    {
                        target = cards[0];
                        cards.RemoveAt(0);
                        cardOpening.Play();
                    }

                    PositionComponent focusPositionComponent = target.GetComponent<PositionComponent>();
                    Vector2 focusTargetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f - 32f, (window.Height / LD34Game.Scale) / 2f);
                    focusPositionComponent.Position += (focusTargetPosition - focusPositionComponent.Position) * 4f * delta;

                    CardComponent focusCardComponent = target.GetComponent<CardComponent>();
                    focusCardComponent.Separation += (1f - focusCardComponent.Separation) * 12f * delta;

                    for (int i = 0; i < cards.Count; i++)
                    {
                        PositionComponent positionComponent = cards[i].GetComponent<PositionComponent>();
                        Vector2 targetPosition = new Vector2(((window.Width / LD34Game.Scale) / 2f) + (i - (cards.Count / 2f)) * 64f, (window.Height / LD34Game.Scale) / 1.1f - 64f);

                        CardComponent cardComponent = cards[i].GetComponent<CardComponent>();
                        cardComponent.Separation += (0f - cardComponent.Separation) * 8f * delta;

                        positionComponent.Position += (targetPosition - positionComponent.Position) * 4f * delta;
                    }

                    if (mouse.Left.ToHeld)
                    {
                        if (target.GetComponent<CardComponent>().Card is StartCard)
                            choice = Choice.Start;
                        else if (target.GetComponent<CardComponent>().Card is HelpCard)
                            choice = Choice.Help;
                        else
                            choice = Choice.Quit;

                        cards.Add(target);
                        target = null;
                        cardSelection.Play();

                        playEntity.GetComponent<PositionComponent>().Position += new Vector2(0f, -64f);
                    }
                    else if (mouse.Right.ToHeld)
                    {
                        cards.Add(target);
                        target = null;
                        grow.Play();

                        growEntity.GetComponent<PositionComponent>().Position += new Vector2(0f, 64f);
                    }
                    break;
                case Choice.Start:
                    if (cards.Count > 0 || titleEntity != null || growEntity != null || playEntity != null || helpEntity != null || help2Entity != null)
                        RunQuit(delta);
                    else
                    {
                        EntityWorld.SystemManager.SetSystem(new DraftingSystem(game, spriteBatch, mouse, window, content), GameLoopType.Update);
                        EntityWorld.SystemManager.GetSystem<DraftingSystem>()[0].Initialize();
                        Toggle();
                    }
                    break;
                case Choice.Quit:
                    game.Exit();
                    break;
                case Choice.Help:
                    if (cards.Count > 0 || titleEntity != null || growEntity != null || playEntity != null || helpEntity != null || help2Entity != null)
                        RunQuit(delta);
                    else
                    {
                        bigHelpEntity = EntityWorld.CreateEntity();
                        bigHelpEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - bigHelp.Width / 2f, (window.Height / LD34Game.Scale) / 2f - bigHelp.Height / 2f)));
                        bigHelpEntity.AddComponent(new SpriteComponent(bigHelp, bigHelp.Bounds, Color.White, 0f));

                        SpriteComponent spriteComponent = bigHelpEntity.GetComponent<SpriteComponent>();
                        spriteComponent.Opacity += (1f - spriteComponent.Opacity) * 8f * delta;

                        if (mouse.Left.ToHeld || mouse.Right.ToHeld)
                            Program.Restart();
                    }
                    break;
            }
        }

        private void RunQuit(float delta)
        {
            if (playEntity != null)
            {
                PositionComponent playPositionComponent2 = playEntity.GetComponent<PositionComponent>();
                playPositionComponent2.Position += (new Vector2(playPositionComponent2.Position.X, (window.Height / LD34Game.Scale) / 2f + 32f) - playPositionComponent2.Position) * 8f * delta;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                PositionComponent positionComponent = cards[i].GetComponent<PositionComponent>();
                Vector2 targetPosition = new Vector2((window.Width / LD34Game.Scale) / 2f, window.Height / LD34Game.Scale + 32f);
                positionComponent.Position += (targetPosition - positionComponent.Position) * 8f * delta;
                if (Vector2.Distance(targetPosition, positionComponent.Position) < 32f)
                {
                    cards[i].Delete();
                    cards.RemoveAt(i);
                    i--;
                }
            }

            if (titleEntity != null)
            {
                SpriteComponent spriteComponent = titleEntity.GetComponent<SpriteComponent>();
                spriteComponent.Opacity += (0f - spriteComponent.Opacity) * 8f * delta;
                if (spriteComponent.Opacity <= 0.025f)
                {
                    titleEntity.Delete();
                    titleEntity = null;
                }
            }

            if (growEntity != null)
            {
                SpriteComponent spriteComponent = growEntity.GetComponent<SpriteComponent>();
                spriteComponent.Opacity += (0f - spriteComponent.Opacity) * 8f * delta;
                if (spriteComponent.Opacity <= 0.025f)
                {
                    growEntity.Delete();
                    growEntity = null;
                }
            }

            if (playEntity != null)
            {
                SpriteComponent spriteComponent = playEntity.GetComponent<SpriteComponent>();
                spriteComponent.Opacity += (0f - spriteComponent.Opacity) * 8f * delta;
                if (spriteComponent.Opacity <= 0.025f)
                {
                    playEntity.Delete();
                    playEntity = null;
                }
            }

            if (helpEntity != null)
            {
                SpriteComponent spriteComponent = helpEntity.GetComponent<SpriteComponent>();
                spriteComponent.Opacity += (0f - spriteComponent.Opacity) * 8f * delta;
                if (spriteComponent.Opacity <= 0.025f)
                {
                    helpEntity.Delete();
                    helpEntity = null;
                }
            }

            if (help2Entity != null)
            {
                SpriteComponent spriteComponent = help2Entity.GetComponent<SpriteComponent>();
                spriteComponent.Opacity += (0f - spriteComponent.Opacity) * 8f * delta;
                if (spriteComponent.Opacity <= 0.025f)
                {
                    help2Entity.Delete();
                    help2Entity = null;
                }
            }
        }
    }
}
