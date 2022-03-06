using System.Drawing;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Text;

namespace GDFiddle.UI
{
    public class GUI
    {
        private readonly Renderer _renderer;
        public Font DefaultFont { get; }

        public GUI(Font defaultFont)
        {
            DefaultFont = defaultFont;
            _renderer = new Renderer(DefaultFont);
        }

        public RenderData Render(Rectangle viewArea)
        {
            _renderer.BeginFrame(viewArea);
            RootControl?.Render(_renderer, viewArea.Size);
            return _renderer.ProduceRenderData();
        }

        public IControl? RootControl { get; set; }
    }
}
