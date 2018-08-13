using Artemis;
using LD34.Components;
using LD34.Game.Cards.Minions;
using LD34.Game.Modifiers;
using LD34.Systems;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Spells
{
    internal sealed class ImpBallCard : SpellCard
    {
        private readonly ContentManager content;

        public ImpBallCard(ContentManager content)
            : base(4, 0, "Imp Ball", "")
        {
            this.content = content;
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(new[] { "Spawn " + (Power > 0 ? "*" : "") + (4 + Power).ToString() + (Power > 0 ? "*" : ""),
            "imps." });

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < 4 + Power; i++)
                boardSystem.Spawn(new ImpCard(content), player);
        }
    }
}
