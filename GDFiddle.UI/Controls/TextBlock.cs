using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.UI.Controls
{
    public class TextBlock : IControl
    {
        private Vector2 _viewport;

        public Vector2 Measure(Renderer renderer)
        {
            return renderer.MeasureText(Text);
        }

        public void Render(Renderer renderer)
        {
            renderer.PushViewport(new Vector2(0, 0), new Vector2(70, 50));
            renderer.DrawText(0, 0, Text, Foreground);
        }

        public string Text { get; set; }
        public Color Foreground { get; set; } = Color.White;
    }
}
