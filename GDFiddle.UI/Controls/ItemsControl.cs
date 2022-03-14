using System.Drawing;
using System.Numerics;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// A control containing a list of child controls.
    /// </summary>
    public class ItemsControl : Control
    {
        public ItemsControl()
        {
            Items = new ItemCollection(this);
        }

        protected override Size Arrange(Size size)
        {
            var neededSize = new Size(0, 0);

            foreach (var item in Items)
            {
                var itemSize = item.DoArrange(size);
                if (itemSize.Width > neededSize.Width)
                    neededSize.Width = itemSize.Width;
                neededSize.Height += itemSize.Height;
            }

            return neededSize;
        }

        public override Control GetControlAt(Vector2 position)
        {
            var y = 0;
            foreach (var item in Items)
            {
                if (new Rectangle(new Point(0, y), item.ActualSize).Contains(new Point((int) position.X, (int) position.Y)))
                    return item;
                y += item.ActualSize.Height;
            }

            return this;
        }

        public override void Render(GuiRenderer guiRenderer, Size size)
        {
            base.Render(guiRenderer, size);
            var y = 0;
            foreach (var item in Items)
            {
                var subArea = new Rectangle(0, y, size.Width, item.ActualSize.Height);
                using var scope = guiRenderer.PushSubArea(subArea);
                item.Render(guiRenderer, subArea.Size);
                y += item.ActualSize.Height;
            }
        }

        public ItemCollection Items { get; }
    }
}
