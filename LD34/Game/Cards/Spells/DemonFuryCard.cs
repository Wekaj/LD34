using System;
using System.Collections.ObjectModel;
using Artemis;
using LD34.Systems;
using LD34.Components;
using LD34.Game.Modifiers;

namespace LD34.Game.Cards.Spells
{
    internal sealed class DemonFuryCard : SpellCard
    {
        public DemonFuryCard()
            : base(2, 0, "Demon Fury", "")
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(new[] { "Deal " + (Power > 0 ? "*" : "") + (1 + Power).ToString() + (Power > 0 ? "*" : ""),
            "dmg to all",
            "Demons and",
            "give them",
            (Power > 0 ? "*" : "") + (2 + Power).ToString() + (Power > 0 ? "*" : "") + " init." });

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < boardSystem.Minions.Length; i++)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Minion.Tag == "Demon")
                    {
                        minionComponent.Minion.AddInitiativeModifier(new GrowthModifier(2 + Power));
                        minionComponent.Minion.Health -= 1 + Power;
                    }
                }
        }
    }
}
