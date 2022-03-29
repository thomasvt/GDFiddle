using System.Numerics;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// A control containing a list of child controls that are rendered in a vertical stack.
    /// </summary>
    public class ItemsControl : Control
    {
        public ItemsControl()
        {
            Items = new ItemCollection(this);
        }

        protected override Vector2 Measure(Vector2 availableSize)
        {
            var totalSize = Vector2.Zero;
            var defaultSize = new Vector2(availableSize.X, DefaultItemHeight);
            foreach (var item in Items)
            {
                var itemSize = item.DoMeasure(defaultSize);
                if (itemSize.X > totalSize.X)
                    totalSize.X = itemSize.X;
                totalSize.Y += itemSize.Y;
            }
            return totalSize;
        }

        protected override void Arrange(Vector2 assignedSize)
        {
            var childOffset = new Vector2(0, 0);

            foreach (var item in Items)
            {
                item.DoArrange(new RectangleF(childOffset, new Vector2(assignedSize.X, item.DesiredSize.Y)));

                childOffset.Y += item.DesiredSize.Y;
            }
        }
        
        protected override IEnumerable<Control> GetVisibleChildren()
        {
            return Items;
        }

        public ItemCollection Items { get; }
        public float DefaultItemHeight { get; set; } = 24f;
    }
}
