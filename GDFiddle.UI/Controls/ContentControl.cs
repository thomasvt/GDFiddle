using System.Numerics;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// A control containing a single child, taking the entire size available to the ContentControl.
    /// </summary>
    public class ContentControl : Control
    {
        private Control? _content;

        protected override Vector2 Measure(Vector2 availableSize)
        {
            if (Content == null)
                return Vector2.Zero;

            return Content.DoMeasure(availableSize - Padding * 2);
        }

        protected override void Arrange(Vector2 assignedSize)
        {
            Content?.DoArrange(new RectangleF(Padding, assignedSize - Padding * 2));
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

        public Vector2 Padding { get; set; }
    }
}
