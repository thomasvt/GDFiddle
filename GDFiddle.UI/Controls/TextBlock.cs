using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.UI.Controls
{
    public class TextBlock : Control
    {
        public override void Render(Renderer renderer, Size size)
        {
            renderer.DrawText(0,0, Text, Foreground);
        }

        public string Text { get; set; } = "";
        public Color Foreground { get; set; } = Color.White;
    }
}
