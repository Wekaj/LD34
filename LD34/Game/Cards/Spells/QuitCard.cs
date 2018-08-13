using System;
using System.Collections.ObjectModel;
using Artemis;

namespace LD34.Game.Cards.Spells
{
    internal sealed class QuitCard : SpellCard
    {
        private readonly string[] description;

        public QuitCard()
            : base(0, 0, "Quit", "")
        {
            description = new[] { "Accept", "your odds", "and take", "the easy", "way out." };
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);

        public override void Play(EntityWorld world, bool player)
        {
        }
    }
}
