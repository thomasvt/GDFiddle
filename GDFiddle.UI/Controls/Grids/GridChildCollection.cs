using System.Collections;

namespace GDFiddle.UI.Controls
{
    public class GridChildCollection : IEnumerable<GridItemContainer>
    {
        private readonly List<GridItemContainer> _children;

        public GridChildCollection()
        {
            _children = new List<GridItemContainer>();
        }

        public void Add(IControl control)
        {
            Add(control, GridProperties.Default());
        }

        public void Add(IControl control, GridProperties gridProperties)
        {
            _children.Add(new GridItemContainer { Control = control, GridProperties = gridProperties });
        }

        public IEnumerator<GridItemContainer> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
