using Artemis;
using LD34.Args;
using LD34.Game.Modifiers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards
{
    internal abstract class Card
    {
        private readonly int cost;
        private readonly List<IModifier> costModifiers;

        protected Card(int cost, string name, string tag)
        {
            this.cost = cost;
            costModifiers = new List<IModifier>();
            Name = name;
            Tag = tag;
        }

        public event EventHandler<CardValueEventArgs> CostChanged;

        public int Cost
        {
            get
            {
                int currentCost = cost;
                foreach (IModifier modifier in costModifiers)
                    currentCost = modifier.Modify(currentCost);
                return Math.Max(currentCost, 0);
            }
        }

        public int CostModifier => Cost - cost;

        public string Name { get; }
        public string Tag { get; }
        public abstract ReadOnlyCollection<string> Description { get; }

        public abstract void Play(EntityWorld world, bool player);

        public void AddCostModifier(IModifier modifier)
        {
            int oldCost = Cost;

            costModifiers.Add(modifier);
            modifier.Removed += CostModifierRemoved;

            int newCost = Cost;
            if (oldCost != newCost && CostChanged != null)
                CostChanged(this, new CardValueEventArgs(oldCost, newCost));
        }

        public void ClearCostModifiers()
        {
            int oldCost = Cost;

            foreach (IModifier modifier in costModifiers)
                modifier.Removed -= CostModifierRemoved;
            costModifiers.Clear();

            int newCost = Cost;
            if (oldCost != newCost && CostChanged != null)
                CostChanged(this, new CardValueEventArgs(oldCost, newCost));
        }

        private void CostModifierRemoved(object sender, EventArgs e)
        {
            int oldCost = Cost;

            costModifiers.Remove(sender as IModifier);

            int newCost = Cost;
            if (oldCost != newCost && CostChanged != null)
                CostChanged(this, new CardValueEventArgs(oldCost, newCost));
        }
    }
}
