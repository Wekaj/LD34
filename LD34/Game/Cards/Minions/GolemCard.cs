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
    internal sealed class GolemCard : MinionCard 
    {
        private readonly EntityWorld world;
        private readonly string[] description;

        public GolemCard(ContentManager content, EntityWorld world)
            : base(4, 4, 6, 1, "Golem", "Enchanted", content.Load<Texture2D>("Textures/grunt"), new Vector2(0f, -6f))
        {
            this.world = world;
            description = new[] { "Deals dmg", "to enemies", "adjacent to", "target." };
            Attacked += GolemCardAttacked;
        }

        public override void Initialize()
        {
        }

        private void GolemCardAttacked(object sender, AttackEventArgs e)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            int secondTarget = e.SecondAttacker + (e.Player ? 1 : -1);
            if (secondTarget >= 0 && secondTarget < boardSystem.Minions.Length)
                if (boardSystem.Minions[secondTarget] != null)
                    boardSystem.Minions[secondTarget].GetComponent<MinionComponent>().Minion.Health -= Attack;
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
