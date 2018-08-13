using Artemis.Interface;
using LD34.Game.Cards;

namespace LD34.Components
{
    internal sealed class MinionComponent : IComponent
    {
        public MinionComponent(MinionCard minion, bool player, float opacity)
        {
            Minion = minion;
            Player = player;
            Opacity = opacity;
        }

        public MinionCard Minion { get; set; }
        public bool Player { get; set; }
        public float Opacity { get; set; }
    }
}
