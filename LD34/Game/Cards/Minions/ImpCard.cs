using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.ObjectModel;

namespace LD34.Game.Cards.Minions
{
    internal sealed class ImpCard : MinionCard
    {
        private readonly string[] description;

        public ImpCard(ContentManager content)
            : base(1, 1, 1, 3, "Imp", "Demon", content.Load<Texture2D>("Textures/imp"), new Vector2(0f, -7f))
        {
            description = new string[0];
        }

        public override void Initialize()
        {
        }

        public override ReadOnlyCollection<string> Description => Array.AsReadOnly(description);
    }
}
