using System.Drawing;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.UI.Controls
{
    public class TextBlock : IControl
    {
        public Vector2 Measure(Renderer renderer)
        {
            return renderer.MeasureText(Text);
        }

        public void Render(Renderer renderer, Size size)
        {
            renderer.FillRectangle(Vector2.Zero, new Vector2(size.Width, size.Height), Color.Gray);
            renderer.DrawText(0,0, Text, Foreground);
        }

        public string Text { get; set; } = "";
        public Color Foreground { get; set; } = Color.White;
    }
}
