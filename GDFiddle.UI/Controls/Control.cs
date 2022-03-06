using System.Drawing;
using System.Numerics;

namespace GDFiddle.UI.Controls
{
    public abstract class Control
    {
        public virtual void Render(Renderer renderer, Size size)
        {
        }

        public virtual Control? GetControlAt(Vector2 position)
        {
            return this;
        }

        public virtual void Arrange(Size size)
        {
        }

        public bool IsMouseOver { get; internal set; }
    }
}
