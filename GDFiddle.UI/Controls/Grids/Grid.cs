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

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            _columnDistributor.CalculatePartSizes(parentAvailableSize.X);
            _rowDistributor.CalculatePartSizes(parentAvailableSize.Y);

            foreach (var child in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(child.MetaData.Column);
                var verticalActual = _rowDistributor.GetActualLayout(child.MetaData.Row);
                child.Control.DoArrange(new RectangleF(horizontalActual.Offset, verticalActual.Offset, horizontalActual.Size, verticalActual.Size));
            }

            return parentAvailableSize;
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
