using System.Collections;

namespace GDFiddle.UI.Controls.Grids
{
    public class GridChildCollection : IEnumerable<GridItemContainer>
    {
        private readonly Grid _owner;
        private readonly Dictionary<Control, GridItemContainer> _children;

        public GridChildCollection(Grid owner)
        {
            _owner = owner;
            _children = new Dictionary<Control, GridItemContainer>();
        }

        public void Add(Control control)
        {
            Add(control, GridProperties.Default());
        }

        public void Add(Control control, GridProperties gridProperties)
        {
            _children.Add(control, new GridItemContainer { Control = control, GridProperties = gridProperties });
            control.Parent = _owner;
        }

        /// <summary>
        /// Returns the GridProperties instance associated with a child.
        /// </summary>
        public GridProperties GetGridProperties(Control child)
        {
            if (!_children.TryGetValue(child, out var result))
                throw new ArgumentOutOfRangeException(nameof(child), $"That is not a child of this Grid");
            return result.GridProperties;
        }

        public IEnumerator<GridItemContainer> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
