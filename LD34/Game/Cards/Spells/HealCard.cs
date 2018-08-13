using System.Collections.ObjectModel;
using Artemis;
using System;
using LD34.Systems;
using LD34.Components;

namespace LD34.Game.Cards.Spells
{
    internal sealed class HealCard : SpellCard
    {
        public HealCard()
            : base(3, 0, "Heal", "")
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(new[] { "Heal all",
            "of your",
            "minions by",
            (Power > 0 ? "*" : "") + (3 + Power).ToString() + (Power > 0 ? "*" : "") + "." });

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < boardSystem.Minions.Length; i++)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Player == player)
                        minionComponent.Minion.Health += 3 + Power;
                }
        }
    }
}
