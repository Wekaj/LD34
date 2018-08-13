using Artemis;
using Artemis.System;
using LD34.Args;
using LD34.Components;
using LD34.Display;
using LD34.Game.Cards;
using LD34.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LD34.Systems
{
    internal sealed class BoardSystem : ProcessingSystem
    {
        private readonly MouseHandler mouse;
        private readonly WindowControl window;
        private readonly List<Entity> deadMinions;
        private readonly List<Entity> tiles;
        private State state;

        // Content.
        private readonly Texture2D tile0, tile1, spawnRune, won, lost, restart;

        private Entity wonEntity, lostEntity, restartEntity;

        public BoardSystem(MouseHandler mouse, ContentManager content, WindowControl window)
        {
            this.mouse = mouse;
            this.window = window;
            tiles = new List<Entity>();
            deadMinions = new List<Entity>();
            Minions = new Entity[16];

            // Load content.
            tile0 = content.Load<Texture2D>("Textures/tile_0");
            tile1 = content.Load<Texture2D>("Textures/tile_1");
            spawnRune = content.Load<Texture2D>("Textures/spawn_rune");
            won = content.Load<Texture2D>("Textures/won");
            lost = content.Load<Texture2D>("Textures/lost");
            restart = content.Load<Texture2D>("Textures/restart");

            state = State.None;
        }

        public Entity[] Minions { get; }

        public void Initialize()
        {
            for (int i = 0; i < Minions.Length; i++)
            {
                Entity tile = EntityWorld.CreateEntity();
                tile.AddComponent(new PositionComponent(GetPosition(i)));
                tile.AddComponent(new SpriteComponent(i % 2 == 0 ? tile0 : tile1, new Rectangle(0, 0, 32, 40), Color.White, 0f));
                tiles.Add(tile);

                if (i == 0 || i == Minions.Length - 1)
                {
                    Entity rune = EntityWorld.CreateEntity();
                    rune.AddComponent(new PositionComponent(GetPosition(i)));
                    rune.AddComponent(new SpriteComponent(spawnRune, new Rectangle(0, 0, 32, 40), Color.White, 1f));
                }
            }
        }

        public override void ProcessSystem()
        {
            float delta = BlackBoard.GetEntry<float>("delta");

            if (Minions[0] != null && state == State.None)
                if (!Minions[0].GetComponent<MinionComponent>().Player)
                    state = State.EnemyWon;
            if (Minions[Minions.Length - 1] != null && state == State.None)
                if (Minions[Minions.Length - 1].GetComponent<MinionComponent>().Player)
                    state = State.PlayerWon;

            switch (state)
            {
                case State.PlayerWon:
                    if (wonEntity == null)
                    {
                        wonEntity = EntityWorld.CreateEntity();
                        wonEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - won.Width / 2f, (window.Height / LD34Game.Scale) / 2f - 48f - won.Height / 2f)));
                        wonEntity.AddComponent(new SpriteComponent(won, won.Bounds, Color.White, 1f));
                    }
                    break;
                case State.EnemyWon:
                    if (lostEntity == null)
                    {
                        lostEntity = EntityWorld.CreateEntity();
                        lostEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - lost.Width / 2f, (window.Height / LD34Game.Scale) / 2f - 48f - lost.Height / 2f)));
                        lostEntity.AddComponent(new SpriteComponent(lost, lost.Bounds, Color.White, 1f));
                    }
                    break;
            }

            if (state != State.None)
            {
                if (restartEntity == null)
                {
                    restartEntity = EntityWorld.CreateEntity();
                    restartEntity.AddComponent(new PositionComponent(new Vector2((window.Width / LD34Game.Scale) / 2f - restart.Width / 2f, (window.Height / LD34Game.Scale) / 2f + 48f - restart.Height / 2f)));
                    restartEntity.AddComponent(new SpriteComponent(restart, restart.Bounds, Color.White, 1f));
                }

                if (mouse.Left.ToHeld || mouse.Right.ToHeld)
                    Program.Restart();
            }

            foreach (Entity tile in tiles)
            {
                SpriteComponent spriteComponent = tile.GetComponent<SpriteComponent>();
                spriteComponent.Opacity += (1f - spriteComponent.Opacity) * 8f * delta;
            }

            for (int i = 0; i < Minions.Length; i++)
            {
                Entity minion = Minions[i];
                if (minion != null)
                {
                    PositionComponent positionComponent = minion.GetComponent<PositionComponent>();
                    Vector2 targetPosition = GetPosition(i);
                    positionComponent.Position += (targetPosition - positionComponent.Position) * 8f * delta;
                    MinionComponent minionComponent = minion.GetComponent<MinionComponent>();
                    minionComponent.Opacity += (1f - minionComponent.Opacity) * 8f * delta;
                }
            }

            for (int i = 0; i < deadMinions.Count; i++)
            {
                MinionComponent minionComponent = deadMinions[i].GetComponent<MinionComponent>();
                minionComponent.Opacity += (0f - minionComponent.Opacity) * 8f * delta;
                if (minionComponent.Opacity <= 0.025f)
                {
                    deadMinions[i].Delete();
                    deadMinions.RemoveAt(i);
                    i--;
                }
            }
        }

        public bool Spawn(MinionCard minion, bool player)
        {
            int start = player ? 0 : Minions.Length - 1;
            if (Minions[start] != null)
            {
                MiniMarch(player);
                if (Minions[start] != null)
                    return false;
            }
            Entity entity = EntityWorld.CreateEntity();
            entity.AddComponent(new MinionComponent(minion, player, 0f));
            entity.AddComponent(new PositionComponent(GetPosition(start) + new Vector2(0f, -128f)));
            entity.GetComponent<MinionComponent>().Minion.HealthChanged += MinionHealthChanged(entity);
            Minions[start] = entity;
            return true;
        }

        private EventHandler<CardValueEventArgs> MinionHealthChanged(Entity minion)
        {
            return (object sender, CardValueEventArgs e) =>
            {
                for (int i = 0; i < Minions.Length; i++)
                    if (Minions[i] == minion)
                    {
                        Entity indicator = EntityWorld.CreateEntity();
                        indicator.AddComponent(new PositionComponent(GetPosition(i) + new Vector2(16f)));
                        indicator.AddComponent(new IndicatorComponent(e.NewValue - e.OldValue, 1f));

                        if (e.NewValue <= 0)
                        {
                            Minions[i].GetComponent<MinionComponent>().Minion.Destroy();
                            deadMinions.Add(Minions[i]);
                            Minions[i] = null;
                        }
                    }
            };
        }

        public void MiniMarch(bool player)
        {
            int start = player ? Minions.Length - 2 : 1;
            int end = player ? -1 : Minions.Length;
            int direction = player ? -1 : 1;

            for (int i = start; i != end; i += direction)
                if (Minions[i] != null)
                {
                    MinionComponent minionComponent = Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Player == player)
                        if (Minions[i + direction * -1] == null)
                        {
                            Minions[i + direction * -1] = Minions[i];
                            Minions[i] = null;
                        }
                }
        }

        public void March(bool player)
        {
            int start = player ? Minions.Length - 2 : 1;
            int end = player ? -1 : Minions.Length;
            int direction = player ? -1 : 1;

            for (int i = start; i != end; i += direction)
                if (Minions[i] != null)
                {
                    MinionComponent minionComponent = Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Player == player)
                        for (int j = 1; j <= minionComponent.Minion.Initiative && i + direction * -1 * j < Minions.Length && i + direction * -1 * j >= 0; j++)
                            if (Minions[i + direction * -1 * j] == null)
                            {
                                Minions[i + direction * -1 * j] = Minions[i + direction * -1 * (j - 1)];
                                Minions[i + direction * -1 * (j - 1)] = null;
                            }
                            else
                                break;
                }
        }

        public void Attack()
        {
            for (int i = 0; i < Minions.Length - 1; i++)
                if (Minions[i] != null && Minions[i + 1] != null)
                {
                    MinionComponent firstMinionComponent = Minions[i].GetComponent<MinionComponent>();
                    MinionComponent secondMinionComponent = Minions[i + 1].GetComponent<MinionComponent>();
                    if (firstMinionComponent.Player && !secondMinionComponent.Player)
                    {
                        firstMinionComponent.Minion.AttackOccured(firstMinionComponent.Player, i, i + 1);
                        secondMinionComponent.Minion.AttackOccured(firstMinionComponent.Player, i + 1, i);
                        firstMinionComponent.Minion.Health -= secondMinionComponent.Minion.Attack;
                        secondMinionComponent.Minion.Health -= firstMinionComponent.Minion.Attack;
                        i++;
                    }
                }
        }

        private Vector2 GetPosition(int tile)
        {
            return new Vector2(((window.Width / LD34Game.Scale) / 2f) - (Minions.Length / 2f) * 32f + tile * 32f, (window.Height / LD34Game.Scale) / 2f - 20f);
        }

        private enum State
        {
            PlayerWon,
            EnemyWon,
            None
        }
    }
}
