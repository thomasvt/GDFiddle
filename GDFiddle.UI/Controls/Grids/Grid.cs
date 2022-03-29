using System.Numerics;

namespace GDFiddle.UI.Controls.Grids
{
    public class Grid : Control
    {
        private readonly GridChildDistributor _columnDistributor;
        private readonly GridChildDistributor _rowDistributor;

        public Grid()
        {
            Children = new ItemWithMetaCollection<GridProperties>(this);
            _columnDistributor = new GridChildDistributor();
            _rowDistributor = new GridChildDistributor();
        }

        protected override Vector2 Measure(Vector2 availableSize)
        {
            _columnDistributor.CalculatePartSizes(availableSize.X);
            _rowDistributor.CalculatePartSizes(availableSize.Y);

            foreach (var (control, gridProperties) in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(gridProperties.Column);
                var verticalActual = _rowDistributor.GetActualLayout(gridProperties.Row);
                control.DoMeasure(new Vector2(horizontalActual.Size, verticalActual.Size));
            }

            return availableSize;
        }

        protected override void Arrange(Vector2 assignedSize)
        {
            foreach (var (control, gridProperties) in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(gridProperties.Column);
                var verticalActual = _rowDistributor.GetActualLayout(gridProperties.Row);
                control.DoArrange(new RectangleF(horizontalActual.Offset, verticalActual.Offset, horizontalActual.Size, verticalActual.Size));
            }
        }

        protected override IEnumerable<Control> GetVisibleChildren()
        {
            return Children.Select(c => c.Control);
        }

        public ItemWithMetaCollection<GridProperties> Children { get; }

        public List<GridLength> ColumnDefinitions => _columnDistributor.PartDefinitions;
        public List<GridLength> RowDefinitions => _rowDistributor.PartDefinitions;
    }
}
