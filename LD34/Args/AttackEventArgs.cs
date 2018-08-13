using System;

namespace LD34.Args
{
    internal sealed class AttackEventArgs : EventArgs
    {
        public AttackEventArgs(bool player, int firstAttacker, int secondAttacker)
        {
            Player = player;
            FirstAttacker = firstAttacker;
            SecondAttacker = secondAttacker;
        }

        public bool Player { get; }
        public int FirstAttacker { get; }
        public int SecondAttacker { get; }
    }
}
