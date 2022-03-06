using System.Drawing;
using System.Numerics;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Text;
using Rectangle = System.Drawing.Rectangle;

namespace GDFiddle.UI
{
    public class GUI
    {
        private readonly Renderer _renderer;
        private Control? _mouseOverControl;
        public Font DefaultFont { get; }

        public GUI(Font defaultFont)
        {
            DefaultFont = defaultFont;
            _renderer = new Renderer(DefaultFont);
        }

        public void Update(Rectangle viewArea, Vector2 mousePosition)
        {
            RootControl?.Arrange(viewArea.Size);
            var mouseOverControl = viewArea.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y)) ? RootControl?.GetControlAt(mousePosition) : null;
            if (_mouseOverControl != null) _mouseOverControl.IsMouseOver = false;
            if (mouseOverControl != null) mouseOverControl.IsMouseOver = true;
            _mouseOverControl = mouseOverControl;
        }

        public RenderData Render(Rectangle viewArea)
        {
            _renderer.BeginFrame(viewArea);
            RootControl?.Render(_renderer, viewArea.Size);
            return _renderer.GetRenderData();
        }

        public Control? RootControl { get; set; }
    }
}
