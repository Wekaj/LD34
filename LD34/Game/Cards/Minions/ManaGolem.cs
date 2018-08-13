using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Artemis;
using LD34.Systems;
using LD34.Args;
using LD34.Game.Modifiers;
using LD34.Components;

namespace LD34.Game.Cards.Minions
{
    internal sealed class ManaGolem : MinionCard
    {
        private readonly EntityWorld world;
        private readonly string[] description;
        private bool played, player;

        public ManaGolem(ContentManager content, EntityWorld world)
            : base(4, 2, 5, 1, "Mana Golem", "Enchanted", content.Load<Texture2D>("Textures/mana_golem"), new Vector2(0f, -6f))
        {
            this.world = world;
            description = new[] { "+1 spell", "power." };
            Played += ManaGolemPlayed;
        }

        public override void Initialize()
        {
            world.SystemManager.GetSystem<PlayerHandSystem>()[0].CardAdded += CardAdded;
            world.SystemManager.GetSystem<EnemyHandSystem>()[0].CardAdded += CardAdded;
        }

        private void ManaGolemPlayed(object sender, PlayerEventArgs e)
        {
            played = true;
            player = e.Player;
            foreach (Entity card in (player ? world.SystemManager.GetSystem<PlayerHandSystem>()[0].Cards : world.SystemManager.GetSystem<EnemyHandSystem>()[0].Cards))
            {
                CardComponent cardComponent = card.GetComponent<CardComponent>();
                if (cardComponent.Card is SpellCard)
                    ((SpellCard)cardComponent.Card).AddPowerModifier(new DeathModifier(1, this));
            }
        }

        private void CardAdded(object sender, CardHandEventArgs e)
        {
            if (played)
                if (player == e.Player)
                    if (e.Card is SpellCard)
                        ((SpellCard)e.Card).AddPowerModifier(new DeathModifier(1, this));
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
