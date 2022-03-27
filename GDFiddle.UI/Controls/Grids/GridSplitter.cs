using System.Numerics;

namespace GDFiddle.UI.Controls.Grids
{
    public class GridSplitter: Control
    {
        private GridLength? _left;
        private GridLength? _right;
        private float _startDragPosition;
        private float _startDragLeftSize;
        private Grid? _grid;

        public GridSplitter()
        {
            MouseCursor = Microsoft.Xna.Framework.Input.MouseCursor.SizeWE;
            ParentChanged += parent =>
            {
                if (parent is not Grid grid)
                {
                    throw new Exception(
                        $"Cannot add a {nameof(GridSplitter)} as a child of a {parent.GetType().FullName}.");
                }

                ConfigureGridSplitting(grid);
            };
        }

        private void ConfigureGridSplitting(Grid grid)
        {
            _grid = grid;
            var gridProperties = grid.Children.GetGridProperties(this);
            var splitterColumn = gridProperties.Column;
            if (splitterColumn <= 0 || splitterColumn >= grid.ColumnDefinitions.Count - 1)
                throw new Exception($"A {nameof(GridSplitter)} must have at least 1 column on either of its sides.");
            _left = grid.ColumnDefinitions[splitterColumn - 1];
            _right = grid.ColumnDefinitions[splitterColumn + 1];
        }

        public override void OnMouseDown(Vector2 mousePosition)
        {
            if (_left == null || _right == null)
                return;

            GUI!.CaptureMouse(this);
            _startDragPosition = mousePosition.X;
            _startDragLeftSize = _left.ActualLayout.Size;
        }

        public override void OnMouseMove(Vector2 oldPosition, Vector2 newPosition)
        {
            if (GUI!.HasMouseCapture(this))
            {
                var delta = newPosition.X - _startDragPosition;
                MoveSplitter(_startDragLeftSize + delta);
            }
        }

        public override void OnMouseUp(Vector2 mousePosition)
        {
            GUI!.ReleaseMouse();
        }

        private void MoveSplitter(float newLeftSize)
        {
            if (_left == null || _right == null)
                return;

            if (newLeftSize == _left.ActualLayout.Size)
                return;

            var delta = newLeftSize - _left.ActualLayout.Size;
            // if these are Stars, this is ok: we just use as many starts as there are pixels. This way we get pixel-perfect control over their sizes without removing the dynamic proportioning when the grid gets resized etc. (idea comes from WPF)
            _left.Amount = _left.ActualLayout.Size + delta;
            if (_left.Amount < _left.MinLength)
                _left.Amount = _left.MinLength;
            _right.Amount = _right.ActualLayout.Size - delta; 
            if (_right.Amount < _right.MinLength)
                _right.Amount = _right.MinLength;

            _grid!.DoArrange(_grid.ArrangedArea);
        }
    }
}
