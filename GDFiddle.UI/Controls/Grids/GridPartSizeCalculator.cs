namespace GDFiddle.UI.Controls.Grids
{
    /// <summary>
    /// Calculates actual offsets and sized of the parts, based on GridLengths using Star and Pixel length-systems.
    /// </summary>
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

            var totalStars = 0f;
            var totalPixels = 0f;
            foreach (var part in PartDefinitions)
            {
                switch (part.Type)
                {
                    case GridUnitType.Star: totalStars += part.Amount; break;
                    case GridUnitType.Pixel: totalPixels += part.Amount; break;
                }
            }

            var sizeForStars = totalSize - totalPixels;
            var starLength = totalStars == 0 ? sizeForStars : sizeForStars / totalStars;
            var offset = 0f;
            foreach (var part in PartDefinitions)
            {
                var length = 0f;
                switch (part.Type)
                {
                    case GridUnitType.Star: length = starLength * part.Amount; break;
                    case GridUnitType.Pixel: length = part.Amount; break;
                }

                if (length < part.MinLength)
                    length = part.MinLength;

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
