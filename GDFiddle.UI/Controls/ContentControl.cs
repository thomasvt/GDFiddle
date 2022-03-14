using System.Drawing;
using System.Numerics;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// A control containing a single child, taking the entire size available to the ContentControl.
    /// </summary>
    public class ContentControl : Control
    {
        private Control? _content;

        public override Control? GetControlAt(Vector2 position)
        {
            return Content?.GetControlAt(position);
        }

        protected override Size Arrange(Size size)
        {
            return Content?.DoArrange(size) ?? new Size(0, 0);
        }

        public override void Render(GuiRenderer guiRenderer, Size size)
        {
            Content?.Render(guiRenderer, size);
        }

        public Control? Content
        {
            get => _content;
            set
            {
                if (Content != null)
                    Content.Parent = null;
                _content = value;
                if (value != null)
                    value.Parent = this;
            }
        }
    }
}
