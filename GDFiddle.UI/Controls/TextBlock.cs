using System.Drawing;
using System.Numerics;
using GDFiddle.UI.Text;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.UI.Controls
{
    public class TextBlock : Control
    {
        public override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            guiRenderer.DrawText(0,0, Text, Foreground, Font ?? GUI!.DefaultFont);
        }

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            return (Font ?? GUI!.DefaultFont).Measure(Text);
        }

        public string Text { get; set; } = "";
        public Color Foreground { get; set; } = Color.White;
        public Font? Font { get; set; }
    }
}
