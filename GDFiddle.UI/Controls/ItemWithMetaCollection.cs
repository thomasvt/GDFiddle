using System.Collections;
using GDFiddle.UI.Controls.Tabs;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// A child control collection that supports optional metadata per child. Used for annotating a child with data specific to the parent control. (like attached properties in WPF)
    /// </summary>
    public class ItemWithMetaCollection<TMetaData> : IEnumerable<ItemWithMetaData<TMetaData>> where TMetaData : class
    {
        private readonly Control _owner;
        private readonly Func<ContentControl>? _itemContainerFactory;
        private readonly Dictionary<Control, ItemWithMetaData<TMetaData>> _children;

        public ItemWithMetaCollection(Control owner, Func<ContentControl>? itemContainerFactory = null)
        {
            _owner = owner;
            _itemContainerFactory = itemContainerFactory;
            _children = new Dictionary<Control, ItemWithMetaData<TMetaData>>();
        }

        public void Add(Control control)
        {
            Add(control, default);
        }

        public void Add(Control control, TMetaData? gridProperties)
        {
            if (_itemContainerFactory != null)
            {
                var container = _itemContainerFactory.Invoke();
                container.Content = control;
                control = container;
            }
            _children.Add(control, new ItemWithMetaData<TMetaData>(control, gridProperties));
            control.Parent = _owner; // must be set after adding (OnParentChanged may want to walk the tree)
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

        public ItemWithMetaData<TMetaData> Get(Control control)
        {
            return _children[control];
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
