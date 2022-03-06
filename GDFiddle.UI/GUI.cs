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
            Controls = new List<IControl>();
            DefaultFont = defaultFont;
            _renderer = new Renderer(DefaultFont);
        }

        public RenderData ProduceRenderData()
        {
            _renderer.BeginFrame();
            foreach (var control in Controls)
            {
                control.Render(_renderer);
            }
            return _renderer.ProduceRenderData();
        }

        public List<IControl> Controls { get; set; }
    }
}
