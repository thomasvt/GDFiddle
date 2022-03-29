using System.Numerics;

namespace GDFiddle.UI.Controls.Tabs
{
    internal class TabHeaderButton : ContentControl
    {
        protected override Vector2 Measure(Vector2 availableSize)
        {
            var size = Content?.DoMeasure(availableSize) ?? Vector2.Zero; 
            return size + new Vector2(16, 8);
        }

        protected override void Arrange(Vector2 assignedSize)
        {
            if (Content == null)
                return;

            var offset = (assignedSize - Content.DesiredSize) * 0.5f;
            Content.DoArrange(new RectangleF(MathF.Floor(offset.X), MathF.Floor(offset.Y), Content.DesiredSize.X, Content.DesiredSize.Y));
        }
    }
}
