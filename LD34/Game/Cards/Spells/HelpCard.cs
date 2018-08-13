using System;
using System.Collections.ObjectModel;
using Artemis;

namespace LD34.Game.Cards.Spells
{
    internal sealed class HelpCard : SpellCard
    {
        private readonly string[] description;

        public HelpCard()
            : base(0, 0, "Help", "")
        {
            description = new[] { "Refresh", "your", "memory", "regarding", "the current", "situation." };
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);

        public override void Play(EntityWorld world, bool player)
        {
        }
    }
}
