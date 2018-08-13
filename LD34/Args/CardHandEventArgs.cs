using LD34.Game.Cards;
using System;

namespace LD34.Args
{
    internal sealed class CardHandEventArgs : EventArgs
    {
        public CardHandEventArgs(bool player, Card card)
        {
            Player = player;
            Card = card;
        }

        public bool Player { get; }
        public Card Card { get; }
    }
}
