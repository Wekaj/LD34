using System.Collections.ObjectModel;
using Artemis;
using System;
using LD34.Systems;
using LD34.Components;

namespace LD34.Game.Cards.Spells
{
    internal sealed class BlinkCard : SpellCard
    {
        public BlinkCard()
            : base(4, 0, "Blink", "")
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(new[] { "Teleport",
            "your right",
            "most minion",
            (Power > 0 ? "*" : "") + (2 + Power).ToString() + (Power > 0 ? "*" : "") + " tiles." });

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            int start = !player ? 0 : boardSystem.Minions.Length - 1;
            int direction = !player ? 1 : -1;
            int end = !player ? boardSystem.Minions.Length : -1;
            for (int i = start; i != end; i += direction)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Player == player)
                    {
                        int target = i + direction * -1 * (2 + Power);
                        if (target < boardSystem.Minions.Length && target >= 0 && boardSystem.Minions[target] == null)
                        {
                            boardSystem.Minions[target] = boardSystem.Minions[i];
                            boardSystem.Minions[i] = null; 
                        }
                        break;
                    }
                }
        }
    }
}
