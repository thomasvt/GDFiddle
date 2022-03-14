using System.Collections;

namespace GDFiddle.UI.Controls
{
    public class ItemCollection : IEnumerable<Control>
    {
        private readonly Control _owner;
        private readonly List<Control> _items;

        public ItemCollection(Control owner)
        {
            _owner = owner;
            _items = new List<Control>();
        }

        public void Add(Control control)
        {
            _items.Add(control);
            control.Parent = _owner;
        }

        public IEnumerator<Control> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
