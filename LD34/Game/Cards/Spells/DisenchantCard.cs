using System.Collections.ObjectModel;
using Artemis;
using System;
using LD34.Systems;
using LD34.Components;

namespace LD34.Game.Cards.Spells
{
    internal sealed class DisenchantCard : SpellCard
    {
        private readonly string[] description;

        public DisenchantCard()
            : base(6, 0, "Disenchant", "")
        {
            description = new string[] { "Destroy", "all", "Enchanted." };
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);

        public override void Play(EntityWorld world, bool player)
        {
            BoardSystem boardSystem = world.SystemManager.GetSystem<BoardSystem>()[0];
            for (int i = 0; i < boardSystem.Minions.Length; i++)
                if (boardSystem.Minions[i] != null)
                {
                    MinionComponent minionComponent = boardSystem.Minions[i].GetComponent<MinionComponent>();
                    if (minionComponent.Minion.Tag == "Enchanted")
                        minionComponent.Minion.Health -= minionComponent.Minion.Health;
                }
        }
    }
}
