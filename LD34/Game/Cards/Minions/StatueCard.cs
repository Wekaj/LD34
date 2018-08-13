using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LD34.Args;
using Artemis;
using LD34.Systems;
using LD34.Components;

namespace LD34.Game.Cards.Minions
{
    internal sealed class StatueCard : MinionCard 
    {
        private readonly EntityWorld world;
        private readonly string[] description;
        private bool played, player;

        public StatueCard(ContentManager content, EntityWorld world)
            : base(4, 0, 10, 1, "Statue", "Enchanted", content.Load<Texture2D>("Textures/statue"), new Vector2(0f, -6f))
        {
            this.world = world;
            description = new[] { "Heals by", "2 at the", "end of", "each turn." };
            Played += StatuePlayed;
        }

        public override void Initialize()
        {
            world.SystemManager.GetSystem<PlayerHandSystem>()[0].TurnEnded += TurnEnded;
            world.SystemManager.GetSystem<EnemyHandSystem>()[0].TurnEnded += TurnEnded;
        }

        private void StatuePlayed(object sender, PlayerEventArgs e)
        {
            played = true;
            player = e.Player;
        }

        private void TurnEnded(object sender, PlayerEventArgs e)
        {
            if (played)
                if (e.Player == player)
                    Health += 2;
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
