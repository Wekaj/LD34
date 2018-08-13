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
    internal sealed class LionCard : MinionCard
    {
        private readonly EntityWorld world;
        private readonly string[] description;
        private bool played, player;

        public LionCard(ContentManager content, EntityWorld world)
            : base(7, 5, 6, 2, "Lion", "Animal", content.Load<Texture2D>("Textures/lion"), new Vector2(0f, -3f))
        {
            this.world = world;
            description = new[] { "Gives all", "allied", "Animals", "+3 init." };
            Played += LionCardPlayed;
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
                    if (e.Card is MinionCard && e.Card != this && e.Card.Tag == "Animal")
                        ((MinionCard)e.Card).AddInitiativeModifier(new DeathModifier(3, this));
        }

        private void LionCardPlayed(object sender, PlayerEventArgs e)
        {
            played = true;
            player = e.Player;

            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < boardSystem.Minions.Length; i++)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Minion.Tag == "Animal" && minionComponent.Player == player && minionComponent.Minion != this)
                        minionComponent.Minion.AddInitiativeModifier(new DeathModifier(3, this));
                }
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
