using Microsoft.Xna.Framework.Input;

namespace LD34.Input
{
    internal sealed class KeyboardButton : IButton
    {
        private Keys[] keys;
        private KeyboardState currentState,
            pastState;

        public bool Held { get { return IsHeld(currentState); } }
        public bool ToHeld { get { return Held && !IsHeld(pastState); } }
        public bool ToReleased { get { return !Held && IsHeld(pastState); } }

        public KeyboardButton(Keys[] keys)
        {
            this.keys = keys;
        }

        public void Update(KeyboardState keyboardState)
        {
            pastState = currentState;
            currentState = keyboardState;
        }

        public bool IsHeld(KeyboardState keyboardState)
        {
            for (int i = 0; i < keys.Length; i++)
                if (keyboardState.IsKeyDown(keys[i]))
                    return true;
            return false;
        }
    }
}
