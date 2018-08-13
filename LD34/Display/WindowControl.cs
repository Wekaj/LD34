using Microsoft.Xna.Framework;
using System;

namespace LD34.Display
{
    internal enum DisplayType
    {
        Borderless,
        Windowed,
        Fullscreen
    }

    internal sealed class WindowControl
    {
        private GraphicsDeviceManager graphics;
        private GameWindow window;

        public WindowControl(GraphicsDeviceManager graphics, GameWindow window)
        {
            this.graphics = graphics;
            this.window = window;
            Width = graphics.PreferredBackBufferWidth;
            Height = graphics.PreferredBackBufferHeight;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public event EventHandler DimensionsChanged;

        public void SetDisplayType(DisplayType display)
        {
            // Apply the necessary adjustments based on the DisplayType supplied.
            switch (display)
            {
                case DisplayType.Windowed:
                    graphics.IsFullScreen = false;
                    window.IsBorderless = false;
                    break;
                case DisplayType.Fullscreen:
                    graphics.IsFullScreen = true;
                    window.IsBorderless = false;
                    break;
                case DisplayType.Borderless:
                    graphics.IsFullScreen = false;
                    window.IsBorderless = true;
                    window.Position = new Point(0, 0);
                    break;
            }
            graphics.ApplyChanges();
        }

        public void SetSize(int width, int height)
        {
            // Store the new window dimensions for external reference.
            Width = width;
            Height = height;

            // Update the GraphicsDeviceManager with the new dimensions and apply the changes.
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            if (DimensionsChanged != null)
                DimensionsChanged(this, EventArgs.Empty);
        }
    }
}
