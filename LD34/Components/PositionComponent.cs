using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace LD34.Components
{
    internal sealed class PositionComponent : IComponent
    {
        public PositionComponent(Vector2 position)
        {
            Position = position;
        }

        public Vector2 Position { get; set; }
    }
}
