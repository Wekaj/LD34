using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Minions
{
    internal sealed class BearCard : MinionCard
    {
        private readonly string[] description;

        public BearCard(ContentManager content)
            : base(1, 1, 2, 2, "Bear", "Animal", content.Load<Texture2D>("Textures/bear"), new Vector2(0f, -3f))
        {
            description = new string[0];
        }

        public override void Initialize()
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
