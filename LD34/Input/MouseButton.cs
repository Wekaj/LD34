using Microsoft.Xna.Framework.Input;

namespace LD34.Input
{
    internal enum MouseButtons
    {
        Left,
        Middle,
        Right
    }

    internal sealed class MouseButton : IButton
    {
        private MouseButtons mouseButton;
        private MouseState currentState,
            pastState;

        public bool Held { get { return IsHeld(currentState); } }
        public bool ToHeld { get { return Held && !IsHeld(pastState); } }
        public bool ToReleased { get { return !Held && IsHeld(pastState); } }

        public MouseButton(MouseButtons mouseButton)
        {
            this.mouseButton = mouseButton;
        }

        public void Update(MouseState mouseState)
        {
            pastState = currentState;
            currentState = mouseState;
        }

        public bool IsHeld(MouseState mouseState)
        {
            switch (mouseButton)
            {
                case MouseButtons.Left:
                    return mouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.Middle:
                    return mouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.Right:
                    return mouseState.RightButton == ButtonState.Pressed;
            }
            return false;
        }
    }
}
