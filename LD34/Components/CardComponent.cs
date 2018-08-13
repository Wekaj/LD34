using Artemis.Interface;
using LD34.Game.Cards;
using Microsoft.Xna.Framework;

namespace LD34.Components
{
    internal sealed class CardComponent : IComponent
    {
        public CardComponent(Card card, bool faceUp, float separation, float opacity, Vector2 offset, bool held, bool growing, float modifierOpacity)
        {
            Card = card;
            FaceUp = faceUp;
            Separation = separation;
            Opacity = opacity;
            Offset = offset;
            Held = held;
            Growing = growing;
            ModifierOpacity = modifierOpacity;
        }

        public Card Card { get; set; }
        public bool FaceUp { get; set; }
        public float Separation { get; set; }
        public float Opacity { get; set; }
        public Vector2 Offset { get; set; }
        public bool Held { get; set; }
        public bool Growing { get; set; }
        public float ModifierOpacity { get; set; }
    }
}
