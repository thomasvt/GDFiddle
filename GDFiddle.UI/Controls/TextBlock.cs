using System.Drawing;
using GDFiddle.UI.Text;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.UI.Controls
{
    public class TextBlock : Control
    {
        public override void Render(GuiRenderer guiRenderer, Size size)
        {
            base.Render(guiRenderer, size);
            guiRenderer.DrawText(0,0, Text, Foreground, Font ?? GUI!.DefaultFont);
        }

        protected override Size Arrange(Size size)
        {
            return (Font ?? GUI!.DefaultFont).Measure(Text);
        }

        public string Text { get; set; } = "";
        public Color Foreground { get; set; } = Color.White;
        public Font? Font { get; set; }
    }
}
