using GDFiddle.UI.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.UI
{
    public class Renderer
    {
        public readonly Font Font;
        private readonly GrowingArray<VertexPositionColorTexture> _vertices;
        private readonly GrowingArray<RenderCommand> _renderCommands;
        private Vector2 _offset;
        private Vector2 _size;

        public Renderer(Font font)
        {
            Font = font;
            _vertices = new GrowingArray<VertexPositionColorTexture>(500);
            _renderCommands = new GrowingArray<RenderCommand>(50);
        }

        public void BeginFrame()
        {
            _vertices.Clear();
            _renderCommands.Clear();
        }

        public void FillRectangle(Vector2 topLeft, Vector2 size, Color color)
        {
            AppendQuad(topLeft, topLeft + size, Vector2.Zero, Vector2.Zero, color);
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
            
            _renderCommands.Add(new RenderCommand(_offset, _size, startVertexIdx, (_vertices.Index - startVertexIdx) / 3, font.TextureFilename));
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

        public void PushViewport(Vector2 offset, Vector2 size)
        {
            _offset = offset;
            _size = size;
        }
    }
}