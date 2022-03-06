namespace GDFiddle.UI.Text
{
    internal readonly struct Kerning
    {
        public readonly ushort First;
        public readonly ushort Second;
        public readonly int Amount;

        public Kerning(ushort first, ushort second, int amount)
        {
            First = first;
            Second = second;
            Amount = amount;
        }
    }
}
