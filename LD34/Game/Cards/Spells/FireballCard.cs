using System;
using System.Collections.ObjectModel;
using Artemis;
using LD34.Systems;
using LD34.Components;

namespace LD34.Game.Cards.Spells
{
    internal sealed class FireballCard : SpellCard
    {
        public FireballCard()
            : base(2, 0, "Fireball", "")
        {
        }

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            int start = player ? 0 : boardSystem.Minions.Length - 1;
            int direction = player ? 1 : -1;
            int end = player ? boardSystem.Minions.Length : -1;
            for (int i = start; i != end; i += direction)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Player != player)
                    {
                        minionComponent.Minion.Health -= 2 + Power;
                        break;
                    }
                }
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(new[] { "Deal " + (Power > 0 ? "*" : "") + (2 + Power).ToString() + (Power > 0 ? "*" : ""),
            "dmg to",
            "the left",
            "most enemy",
            "minion." });
    }
}
