using System.Drawing;
using System.Numerics;
using Rectangle = System.Drawing.Rectangle;

namespace GDFiddle.UI.Controls.Grids
{
    public class Grid : Control
    {
        private readonly GridPartSizeCalculator _columnDistributor;
        private readonly GridPartSizeCalculator _rowDistributor;

        public Grid()
        {
            Children = new GridChildCollection(this);
            _columnDistributor = new GridPartSizeCalculator();
            _rowDistributor = new GridPartSizeCalculator();
        }

        protected override void Arrange(Size size)
        {
            _columnDistributor.CalculatePartSizes(size.Width);
            _rowDistributor.CalculatePartSizes(size.Height);

            foreach (var child in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(child.GridProperties.Column);
                var verticalActual = _rowDistributor.GetActualLayout(child.GridProperties.Row);
                child.Control.DoArrange(new Size((int) horizontalActual.Size, (int) verticalActual.Size));
            }
        }

        public override void Render(GuiRenderer guiRenderer, Size size)
        {
            base.Render(guiRenderer, size);
            foreach (var child in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(child.GridProperties.Column);
                var verticalActual = _rowDistributor.GetActualLayout(child.GridProperties.Row);
                var subArea = new Rectangle((int)horizontalActual.Offset, (int)verticalActual.Offset, (int)horizontalActual.Size, (int)verticalActual.Size);
                using var scope = guiRenderer.CreateSubClipArea(subArea);
                child.Control.Render(guiRenderer, subArea.Size);
            }
        }

        public override Control? GetControlAt(Vector2 position)
        {
            foreach (var child in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(child.GridProperties.Column);
                if (!horizontalActual.Contains(position.X))
                    continue;
                var verticalActual = _rowDistributor.GetActualLayout(child.GridProperties.Row);
                if (!verticalActual.Contains(position.Y))
                    continue;
                return child.Control.GetControlAt(new Vector2(position.X - horizontalActual.Offset, position.Y - verticalActual.Offset));
            }

            return this;
        }

        public GridChildCollection Children { get; }

        public List<GridLength> ColumnDefinitions => _columnDistributor.PartDefinitions;
        public List<GridLength> RowDefinitions => _rowDistributor.PartDefinitions;
    }
}
