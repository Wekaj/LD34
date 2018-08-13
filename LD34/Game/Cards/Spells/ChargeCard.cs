using Artemis;
using LD34.Components;
using LD34.Game.Modifiers;
using LD34.Systems;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Spells
{
    internal sealed class ChargeCard : SpellCard
    {
        public ChargeCard()
            : base(5, 0, "Charge", "")
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(new[] { "Give all",
            "friendly",
            "minions " + (Power > 0 ? "*" : "") + (3 + Power).ToString() + (Power > 0 ? "*" : ""),
            "init." });

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < boardSystem.Minions.Length; i++)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Player == player)
                        minionComponent.Minion.AddInitiativeModifier(new GrowthModifier(3 + Power));
                }
        }
    }
}
