using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD34.Components
{
    internal sealed class SpriteComponent : IComponent
    {
        public SpriteComponent(Texture2D texture, Rectangle bounds, Color color, float opacity)
        {
            Texture = texture;
            Bounds = bounds;
            Color = color;
            Opacity = opacity;
        }

        public Texture2D Texture { get; set; }
        public Rectangle Bounds { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
    }
}
