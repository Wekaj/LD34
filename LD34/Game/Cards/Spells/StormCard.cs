using Artemis;
using LD34.Components;
using LD34.Systems;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Spells
{
    internal sealed class StormCard : SpellCard
    {
        public StormCard()
            : base(4, 0, "Storm", "")
        {
        }

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < boardSystem.Minions.Length; i++)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Player != player)
                        minionComponent.Minion.Health -= 2 + Power;
                }
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(new[] { "Deal " + (Power > 0 ? "*" : "") + (2 + Power).ToString() + (Power > 0 ? "*" : ""),
            "dmg to",
            "all enemy",
            "minions." });
    }
}
