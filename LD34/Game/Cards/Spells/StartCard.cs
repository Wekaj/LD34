using System;
using System.Collections.ObjectModel;
using Artemis;

namespace LD34.Game.Cards.Spells
{
    internal sealed class StartCard : SpellCard
    {
        private readonly string[] description;

        public StartCard()
            : base(0, 0, "Start", "")
        {
            description = new[] { "Teleport", "to the", "Gate,", "letting", "you start." };
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);

        public override void Play(EntityWorld world, bool player)
        {
        }
    }
}
