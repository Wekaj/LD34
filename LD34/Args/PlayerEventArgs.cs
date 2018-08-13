using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD34.Args
{
    internal sealed class PlayerEventArgs : EventArgs
    {
        public PlayerEventArgs(bool player)
        {
            Player = player;
        }

        public bool Player { get; }
    }
}
