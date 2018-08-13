using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace LD34.Input
{
    internal sealed class MouseHandler
    {
        private readonly Dictionary<MouseButtons, MouseButton> buttons;

        public MouseState State { get; private set; }
        public IButton Left { get { return buttons[MouseButtons.Left]; } }
        public IButton Middle { get { return buttons[MouseButtons.Middle]; } }
        public IButton Right { get { return buttons[MouseButtons.Right]; } }

        public MouseHandler()
        {
            buttons = new Dictionary<MouseButtons, MouseButton>()
            {
                { MouseButtons.Left, new MouseButton(MouseButtons.Left) },
                { MouseButtons.Middle, new MouseButton(MouseButtons.Middle) },
                { MouseButtons.Right, new MouseButton(MouseButtons.Right) }
            };
        }

        public IButton this[MouseButtons button]
        {
            get { return GetButton(button); }
        }

        public void Update()
        {
            Update(Mouse.GetState());
        }

        public void Update(MouseState state)
        {
            State = state;
            for (int i = 0; i < buttons.Count; i++)
                buttons.ElementAt(i).Value.Update(state);
        }

        public IButton GetButton(MouseButtons button)
        {
            return buttons[button];
        }
    }
}
