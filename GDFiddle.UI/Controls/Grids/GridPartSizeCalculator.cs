namespace GDFiddle.UI.Controls.Grids
{
    internal class GridPartSizeCalculator
    {
        private float _totalSize;

        public GridPartSizeCalculator()
        {
            PartDefinitions = new List<GridLength>();
        }

        public void CalculatePartSizes(float totalSize)
        {
            _totalSize = totalSize;
            if (!PartDefinitions.Any())
                return;

            var totalStars = PartDefinitions.Where(part => part.Type == GridUnitType.Star).Sum(part => part.Amount);
            var starLength = totalStars == 0 ? totalSize : totalSize / totalStars;
            var offset = 0f;
            foreach (var part in PartDefinitions)
            {
                var length = starLength * part.Amount;
                part.ActualLayout = new ActualLayout(offset, length);
                offset += length;
            }
        }

        public ActualLayout GetActualLayout(int partIdx)
        {
            if (!PartDefinitions.Any())
                return new ActualLayout(0, _totalSize);

            if (partIdx >= PartDefinitions.Count || partIdx < 0)
                throw new ArgumentOutOfRangeException(nameof(partIdx));

            return PartDefinitions[partIdx].ActualLayout;
        }

        public List<GridLength> PartDefinitions { get; }
    }
}
