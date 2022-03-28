using System.Numerics;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// A control containing a single child, taking the entire size available to the ContentControl.
    /// </summary>
    public class ContentControl : Control
    {
        private Control? _content;

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            return Content?.DoArrange(new RectangleF(Vector2.Zero, parentAvailableSize)) ?? Vector2.Zero;
        }

        protected override IEnumerable<Control> GetVisibleChildren()
        {
            if (Content != null)
                yield return Content;
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
