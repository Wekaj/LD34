using LD34.Game.Cards;
using System;

namespace LD34.Game.Modifiers
{
    internal sealed class DeathModifier : IModifier
    {
        private readonly int amount;
        
        public DeathModifier(int amount, MinionCard minion)
        {
            this.amount = amount;
            minion.Destroyed += MinionDestroyed;
        }

        public event EventHandler Removed;

        public int Modify(int input) => input + amount;

        private void MinionDestroyed(object sender, EventArgs e)
        {
            if (Removed != null)
                Removed(this, EventArgs.Empty);
        }
    }
}
