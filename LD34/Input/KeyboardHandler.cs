using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace LD34.Input
{
    internal sealed class KeyboardHandler
    {
        private readonly Dictionary<string, KeyboardButton> buttons;

        public KeyboardState State { get; private set; }

        public KeyboardHandler()
        {
            buttons = new Dictionary<string, KeyboardButton>();
        }

        public IButton this[string key]
        {
            get { return GetButton(key); }
        }

        public void Update()
        {
            Update(Keyboard.GetState());
        }

        public void Update(KeyboardState state)
        {
            State = state;
            for (int i = 0; i < buttons.Count; i++)
                buttons.ElementAt(i).Value.Update(state);
        }

        public IButton AddButton(string key, params Keys[] keys)
        {
            KeyboardButton button = new KeyboardButton(keys);
            if (!buttons.ContainsKey(key))
                buttons.Add(key, button);
            else
                buttons[key] = button;
            return button;
        }

        public IButton GetButton(string key)
        {
            if (buttons.ContainsKey(key))
                return buttons[key];
            return null;
        }
    }
}
