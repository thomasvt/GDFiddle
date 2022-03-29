using System.Numerics;

namespace GDFiddle.UI.Controls
{
    public class Button : ContentControl
    {
        public Button()
        {
            IsMouseInteractive = true;
        }

        public override void OnMouseUp(Vector2 mousePosition)
        {
            Click?.Invoke();
        }

        public event Action? Click;
    }
}
