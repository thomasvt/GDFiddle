using System.Numerics;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.UI.Controls
{
    public class TextBlock : Control
    {
        public override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            guiRenderer.DrawText(0,0, Text, Foreground, Font);
        }

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            return (Font).Measure(Text);
        }

        public string Text { get; set; } = "";
        public Color Foreground { get; set; } = Color.White;
    }
}
