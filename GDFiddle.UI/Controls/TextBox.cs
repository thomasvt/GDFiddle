using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls
{
    public class TextBox : Control
    {
        public TextBox()
        {
            IsFocusable = true;
        }

        public override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            guiRenderer.DrawRectangle(new Vector2(0,0), ArrangedSize, null, IsFocused ? Color.Yellow : Color.White);
            guiRenderer.DrawText(2,2, Text, Color.White, Font);
        }

        public string Text { get; set; } = string.Empty;
    }
}
