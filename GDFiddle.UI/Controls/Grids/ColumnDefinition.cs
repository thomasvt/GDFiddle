namespace GDFiddle.UI.Controls.Grids
{
    public class GridLength
    {
        public float Amount { get; }
        public GridUnitType Type { get; }
        public ActualLayout ActualLayout { get; internal set; }

        public GridLength(float amount, GridUnitType type)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            Amount = amount;
            Type = type;
        }

        public static GridLength Star(float amount = 1f)
        {
            return new GridLength(amount, GridUnitType.Star);
        }
    }
}
