using System.Drawing;
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

        public override Control? GetControlAt(Vector2 position)
        {
            if (Items.Any())
            {
                var hitItem = Items.First(item => new RectangleF(item.OffsetFromParent, item.ArrangedSize).Contains(position));
                var offset = hitItem.OffsetFromParent;
                return hitItem.GetControlAt(position - offset);
            }

            return this;
        }

        public override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            foreach (var item in Items)
            {
                using var scope = guiRenderer.PushSubArea(item.ArrangeArea);
                item.Render(guiRenderer);
            }
        }

        public ItemCollection Items { get; }
    }
}
