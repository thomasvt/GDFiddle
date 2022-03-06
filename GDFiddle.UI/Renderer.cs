using GDFiddle.UI.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = System.Drawing.Rectangle;

namespace GDFiddle.UI
{
    public class Renderer
    {
        public readonly Font Font;
        private readonly GrowingArray<VertexPositionColorTexture> _vertices;
        private readonly GrowingArray<RenderCommand> _renderCommands;
        private readonly Stack<Rectangle> _areaStack;

        public Renderer(Font font)
        {
            Font = font;
            _vertices = new GrowingArray<VertexPositionColorTexture>(500);
            _renderCommands = new GrowingArray<RenderCommand>(50);
            _areaStack = new Stack<Rectangle>();
        }

        public void BeginFrame(Rectangle viewArea)
        {
            _vertices.Clear();
            _renderCommands.Clear();
            _areaStack.Clear();
            _areaStack.Push(viewArea);
        }

        public void FillRectangle(Vector2 topLeft, Vector2 size, Color color)
        {
            var startVertexIdx = _vertices.Index;
            AppendQuad(topLeft, topLeft + size, Vector2.Zero, Vector2.Zero, color);
            _renderCommands.Add(new RenderCommand(_areaStack.Peek(), startVertexIdx, (_vertices.Index - startVertexIdx) / 3));
        }

        private void AppendQuad(Vector2 min, Vector2 max, Vector2 uvMin, Vector2 uvMax, Color color)
        {
            _vertices.EnsureCapacity(6);

            var a = new VertexPositionColorTexture(new Vector3(min.X, min.Y, 0), color, uvMin);
            var b = new VertexPositionColorTexture(new Vector3(max.X, min.Y, 0), color, new Vector2(uvMax.X, uvMin.Y));
            var c = new VertexPositionColorTexture(new Vector3(max.X, max.Y, 0), color, uvMax);
            var d = new VertexPositionColorTexture(new Vector3(min.X, max.Y, 0), color, new Vector2(uvMin.X, uvMax.Y));

            _vertices.Add(a);
            _vertices.Add(b);
            _vertices.Add(c);

            _vertices.Add(a);
            _vertices.Add(c);
            _vertices.Add(d);
        }

        public void DrawText(int x, int y, string text, Color color, Font? font = null)
        {
            font ??= Font;
            var startVertexIdx = _vertices.Index;
            foreach (var glyphInfo in font.GetTextGlyphs(x, y, text))
            {
                AppendQuad(glyphInfo.QuadMin, glyphInfo.QuadMax, glyphInfo.UVMin, glyphInfo.UVMax, color);
            }
            
            _renderCommands.Add(new RenderCommand(_areaStack.Peek(), startVertexIdx, (_vertices.Index - startVertexIdx) / 3, font.TextureFilename));
        }

        public Vector2 MeasureText(string text, Font? font = null)
        {
            font ??= Font;
            return font.Measure(text);
        }

        public RenderData ProduceRenderData()
        {
            return new RenderData
            {
                Vertices = _vertices.Array,
                RenderCommands = _renderCommands.AsSpan()
            };
        }

        public IDisposable CreateSubArea(Rectangle subArea)
        {
            var parentArea = _areaStack.Peek();
            if (parentArea.Width < subArea.X + subArea.Width || parentArea.Height < subArea.Y + subArea.Height)
                throw new ArgumentOutOfRangeException(nameof(subArea), $"New sub area ({subArea}) does not fit in parent area ({parentArea}).");
            _areaStack.Push(new Rectangle(parentArea.X + subArea.X, parentArea.Y + subArea.Y, subArea.Width, subArea.Height));
            return new AreaScope(_areaStack);
        }
    }
}