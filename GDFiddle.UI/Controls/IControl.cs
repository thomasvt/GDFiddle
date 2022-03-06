using System.Drawing;

namespace GDFiddle.UI.Controls
{
    public interface IControl
    {
        void Render(Renderer renderer, Size size);
    }
}
