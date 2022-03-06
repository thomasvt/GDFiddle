using System.Drawing;

namespace GDFiddle.UI.Controls.Grids
{
    public class Grid : IControl
    {
        private readonly GridPartSizeCalculator _columnDistributor;
        private readonly GridPartSizeCalculator _rowDistributor;

        public Grid()
        {
            Children = new GridChildCollection();
            _columnDistributor = new GridPartSizeCalculator();
            _rowDistributor = new GridPartSizeCalculator();
        }

        public void Render(Renderer renderer, Size size)
        {
            _columnDistributor.CalculatePartSizes(size.Width);
            _rowDistributor.CalculatePartSizes(size.Height);

            foreach (var child in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(child.GridProperties.Column);
                var verticalActual = _rowDistributor.GetActualLayout(child.GridProperties.Row);
                var subArea = new Rectangle((int)horizontalActual.Offset, (int)verticalActual.Offset, (int)horizontalActual.Size, (int)verticalActual.Size);
                using var scope = renderer.CreateSubArea(subArea);
                child.Control.Render(renderer, subArea.Size);
            }
        }

        public GridChildCollection Children { get; }

        public List<GridLength> ColumnDefinitions => _columnDistributor.PartDefinitions;
        public List<GridLength> RowDefinitions => _rowDistributor.PartDefinitions;
    }
}
