using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Minions
{
    internal sealed class HawkCard : MinionCard
    {
        private readonly string[] description;

        public HawkCard(ContentManager content)
            : base(1, 2, 1, 2, "Hawk", "Animal", content.Load<Texture2D>("Textures/hawk"), new Vector2(0f, -4f))
        {
            description = new string[0];
        }

        public override void Initialize()
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
