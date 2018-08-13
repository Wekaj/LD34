using LD34.Args;
using LD34.Game.Modifiers;
using System;
using System.Collections.Generic;

namespace LD34.Game.Cards
{
    internal abstract class SpellCard : Card
    {
        private readonly int power;
        private readonly List<IModifier> powerModifiers;

        public SpellCard(int cost, int power, string name, string tag)
            : base(cost, name, tag)
        {
            this.power = power;
            powerModifiers = new List<IModifier>();
        }
        
        public event EventHandler<CardValueEventArgs> PowerChanged;

        public int Power
        {
            get
            {
                int currentPower = power;
                foreach (IModifier modifier in powerModifiers)
                    currentPower = modifier.Modify(currentPower);
                return Math.Max(currentPower, 0);
            }
        }

        public void AddPowerModifier(IModifier modifier)
        {
            int oldPower = Power;

            powerModifiers.Add(modifier);
            modifier.Removed += PowerModifierRemoved;

            int newPower = Power;
            if (oldPower != newPower && PowerChanged != null)
                PowerChanged(this, new CardValueEventArgs(oldPower, newPower));
        }

        public void ClearPowerModifiers()
        {
            int oldPower = Power;

            foreach (IModifier modifier in powerModifiers)
                modifier.Removed -= PowerModifierRemoved;
            powerModifiers.Clear();

            int newPower = Power;
            if (oldPower != newPower && PowerChanged != null)
                PowerChanged(this, new CardValueEventArgs(oldPower, newPower));
        }

        private void PowerModifierRemoved(object sender, EventArgs e)
        {
            int oldPower = Power;

            powerModifiers.Remove(sender as IModifier);

            int newPower = Power;
            if (oldPower != newPower && PowerChanged != null)
                PowerChanged(this, new CardValueEventArgs(oldPower, newPower));
        }
    }
}
