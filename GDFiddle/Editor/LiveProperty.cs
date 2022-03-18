using System.Drawing;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Text;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.Editor
{
    /// <summary>
    /// Property control showing a label and a value that is fetched directly from its datasource through a delegate.
    /// </summary>
    internal class LiveProperty : Control
    {
        public readonly string Label;
        public readonly Func<object> ValueGetter;

        public LiveProperty(string label, Func<object?> valueGetter)
        {
            Label = label;
            ValueGetter = valueGetter;
        }

        protected override Size Arrange(Size size)
        {
            return new Size(size.Width, (Font ?? GUI!.DefaultFont).RowHeight);
        }

        public override void Render(GuiRenderer guiRenderer, Size size)
        {
            base.Render(guiRenderer, size);
            guiRenderer.DrawText(0,0, Label, LabelColor, GUI!.DefaultFont);
            guiRenderer.DrawText(size.Width * 4 / 10, 0, ValueGetter()?.ToString() ?? "<null>", LabelColor, Font ?? GUI!.DefaultFont);
        }

        public Color LabelColor { get; set; } = Color.White;
        public Font? Font { get; set; }
    }
}
