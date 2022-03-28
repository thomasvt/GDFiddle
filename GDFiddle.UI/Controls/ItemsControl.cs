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

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            var neededSize = new Vector2(0, 0);
            var childOffset = new Vector2(0, 0);
            var remainingSize = parentAvailableSize;

            foreach (var item in Items)
            {
                var itemSize = item.DoArrange(new RectangleF(childOffset, parentAvailableSize));

                if (itemSize.X > neededSize.X)
                    neededSize.X = itemSize.X;

                childOffset.Y += itemSize.Y;
                remainingSize.Y -= itemSize.Y;
                neededSize.Y += itemSize.Y;
            }

            return neededSize;
        }
        
        protected override IEnumerable<Control> GetVisibleChildren()
        {
            return Items;
        }

        public ItemCollection Items { get; }
    }
}
