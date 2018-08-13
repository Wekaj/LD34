using Artemis;
using LD34.Args;
using LD34.Components;
using LD34.Game.Modifiers;
using LD34.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Minions
{
    internal sealed class ImpLordCard : MinionCard
    {
        private readonly EntityWorld world;
        private readonly string[] description;
        private bool played, player;

        public ImpLordCard(ContentManager content, EntityWorld world)
            : base(3, 1, 3, 1, "Imp Lord", "Demon", content.Load<Texture2D>("Textures/imp_lord"), new Vector2(0f, -7f))
        {
            this.world = world;
            description = new[] { "Gives all", "allied", "Demons +2", "atk." };
            Played += ImpLordCardPlayed;
        }

        public override void Initialize()
        {
            world.SystemManager.GetSystem<PlayerHandSystem>()[0].CardPlayed += CardPlayed;
            world.SystemManager.GetSystem<EnemyHandSystem>()[0].CardPlayed += CardPlayed;
        }

        private void CardPlayed(object sender, CardHandEventArgs e)
        {
            if (played)
                if (e.Player == player)
                    if (e.Card is MinionCard && e.Card != this && e.Card.Tag == "Demon")
                        ((MinionCard)e.Card).AddAttackModifier(new DeathModifier(2, this));
        }

        private void ImpLordCardPlayed(object sender, PlayerEventArgs e)
        {
            played = true;
            player = e.Player;

            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < boardSystem.Minions.Length; i++)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Minion.Tag == "Demon" && minionComponent.Player == player && minionComponent.Minion != this)
                        minionComponent.Minion.AddAttackModifier(new DeathModifier(2, this));
                }
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
