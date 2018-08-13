using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Minions
{
    internal sealed class ShieldsmanCard : MinionCard
    {
        private readonly string[] description;

        public ShieldsmanCard(ContentManager content)
            : base(2, 1, 4, 2, "Shieldsman", "Human", content.Load<Texture2D>("Textures/shieldsman"), new Vector2(0f, -4f))
        {
            description = new string[0];
        }

        public override void Initialize()
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
