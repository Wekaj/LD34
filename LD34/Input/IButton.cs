namespace LD34.Input
{
    internal interface IButton
    {
        bool Held { get; }
        bool ToHeld { get; }
        bool ToReleased { get; }
    }
}
