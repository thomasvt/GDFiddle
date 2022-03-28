using System.Collections;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// A child control collection that supports optional metadata per child. Used for annotating a child with data specific to the parent control. (like attached properties in WPF)
    /// </summary>
    public class ItemWithMetaCollection<TMetaData> : IEnumerable<ItemWithMetaData<TMetaData>> where TMetaData : class
    {
        private readonly Control _owner;
        private readonly Dictionary<Control, ItemWithMetaData<TMetaData>> _children;

        public ItemWithMetaCollection(Control owner)
        {
            _owner = owner;
            _children = new Dictionary<Control, ItemWithMetaData<TMetaData>>();
        }

        public void Add(Control control)
        {
            Add(control, default(TMetaData));
        }

        public void Add(Control control, TMetaData? gridProperties)
        {
            _children.Add(control, new ItemWithMetaData<TMetaData>(control, gridProperties));
            control.Parent = _owner;
        }

        public void Clear()
        {
            foreach (var container in _children)
            {
                container.Key.Detach();
            }
            _children.Clear();
        }

        /// <summary>
        /// Returns the MetaData instance associated with a child.
        /// </summary>
        public TMetaData? GetMetaData(Control child)
        {
            if (!_children.TryGetValue(child, out var result))
                throw new ArgumentOutOfRangeException(nameof(child), $"That is not a child of this Grid");
            return result.MetaData;
        }

        public int Count => _children.Count;

        public IEnumerator<ItemWithMetaData<TMetaData>> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
