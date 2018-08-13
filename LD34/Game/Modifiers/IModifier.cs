using System;

namespace LD34.Game.Modifiers
{
    internal interface IModifier
    {
        event EventHandler Removed;
        int Modify(int input);
    }
}
