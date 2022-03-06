namespace GDFiddle.UI.Controls.Grids
{
    public class GridLength
    {
        public float MinLength { get; set; }
        public float Amount { get; internal set; }
        public GridUnitType Type { get; }
        public ActualLayout ActualLayout { get; internal set; }

        public GridLength(float amount, GridUnitType type, float minLength)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            MinLength = minLength;
            Amount = amount;
            Type = type;
        }

        public static GridLength Star(float amount = 1f, float minSize = 8f) => new(amount, GridUnitType.Star, minSize);
        public static GridLength Pixels(float amount) => new(amount, GridUnitType.Pixel, amount);
    }
}
