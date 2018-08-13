using System;

namespace LD34.Game.Modifiers
{
    internal sealed class GrowthModifier : IModifier
    {
        private readonly int amount;

        public GrowthModifier(int amount)
        {
            this.amount = amount;
        }

        public event EventHandler Removed;

        public int Modify(int input)
        {
            return input += amount;
        }
    }
}
