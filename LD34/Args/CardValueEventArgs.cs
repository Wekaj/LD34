using System;

namespace LD34.Args
{
    internal sealed class CardValueEventArgs : EventArgs
    {
        public CardValueEventArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public int OldValue { get; }
        public int NewValue { get; }
    }
}
