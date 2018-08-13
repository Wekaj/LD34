using Artemis.Interface;

namespace LD34.Components
{
    internal sealed class IndicatorComponent : IComponent
    {
        public IndicatorComponent(int change, float opacity)
        {
            Change = change;
            Opacity = opacity;
        }

        public int Change { get; set; }
        public float Opacity { get; set; }
    }
}
