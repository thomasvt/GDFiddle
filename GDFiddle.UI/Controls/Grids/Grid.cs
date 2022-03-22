﻿using System.Numerics;

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

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            _columnDistributor.CalculatePartSizes(parentAvailableSize.X);
            _rowDistributor.CalculatePartSizes(parentAvailableSize.Y);

            foreach (var child in Children)
            {
                var horizontalActual = _columnDistributor.GetActualLayout(child.GridProperties.Column);
                var verticalActual = _rowDistributor.GetActualLayout(child.GridProperties.Row);
                child.Control.DoArrange(new RectangleF(horizontalActual.Offset, verticalActual.Offset, horizontalActual.Size, verticalActual.Size));
            }

            return parentAvailableSize;
        }

        public override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            foreach (var child in Children)
            {
                var control = child.Control;
                using var scope = guiRenderer.PushSubArea(control.ArrangeArea);
                control.Render(guiRenderer);
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
