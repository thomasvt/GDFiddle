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
            var hitItem = Items.FirstOrDefault(item => item.ArrangedArea.Contains(position));
            if (hitItem == null)
                return this;

            var offset = hitItem.OffsetFromParent;
            return hitItem.GetControlAt(position - offset);
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            foreach (var item in Items)
            {
                item.DoRender(guiRenderer);
            }
        }

        public ItemCollection Items { get; }
    }
}
