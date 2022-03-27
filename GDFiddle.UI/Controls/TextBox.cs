using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// Like a common Textbox, but behaves a little different by ignoring the Text propery while in edit mode (keyboard focus), even though the app may be writing to Text each game frame.
    /// </summary>
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
            if (IsFocused)
            {
                guiRenderer.DrawText(2, 2, InputText, Color.White, Font);
            }
            else
            {
                guiRenderer.DrawText(2, 2, Text, Color.White, Font);
            }
        }

        
        public override void OnTextInput(Keys pressedKey, char typedCharacter)
        {
            
        }

        protected override void OnFocus()
        {
            InputText = Text;
        }

        public string Text { get; set; } = string.Empty;

        public string InputText { get; private set; }

        public event Action<string> InputCompleted;
    }
}
