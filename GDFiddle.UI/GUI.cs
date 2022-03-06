using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.UI
{
    public class GUI
    {
        public readonly Font Font;
        private readonly GrowingArray<VertexPositionColorTexture> _vertices;
        private readonly GrowingArray<RenderCommand> _renderCommands;

        public GUI(Font font)
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

            var a = new VertexPositionColorTexture(new Vector3(min.X, max.Y, 0), color, uvMin);
            var b = new VertexPositionColorTexture(new Vector3(max.X, max.Y, 0), color, new Vector2(uvMax.X, uvMin.Y));
            var c = new VertexPositionColorTexture(new Vector3(max.X, min.Y, 0), color, uvMax);
            var d = new VertexPositionColorTexture(new Vector3(min.X, min.Y, 0), color, new Vector2(uvMin.X, uvMax.Y));

            _vertices.Add(a);
            _vertices.Add(b);
            _vertices.Add(c);

            _vertices.Add(a);
            _vertices.Add(c);
            _vertices.Add(d);
        }

        public void DrawText(int x, int y, string text, Color color, Font? font = null)
        {
            // how to render text:  http://www.angelcode.com/products/bmfont/doc/render_text.html

            // Note: rendertarget Y+ is up, while texture Y+ (and most 2D graphics) is down.

            font ??= Font;
            var startVertexIdx = _vertices.Index;
            var previousCharCode = (ushort)0;
            for (var i = 0; i < text.Length; i++)
            {
                var code = (ushort)char.ConvertToUtf32(text, i);
                if (!font.Glyphs.TryGetValue(code, out var glyph))
                {
                    if (!font.Glyphs.TryGetValue('?', out glyph))
                        continue;
                }
                var min = new Vector2(x + glyph.XOffset, y - font.Base);
                var max = min + new Vector2(glyph.Width, glyph.Heigth);
                AppendQuad(min, max, glyph.UVMin, glyph.UVMax, color);

                x += glyph.XAdvance + font.GetKerningDistance(previousCharCode, code);
                previousCharCode = code;
            }
            _renderCommands.Add(new RenderCommand(startVertexIdx, (_vertices.Index - startVertexIdx) / 3, font.TextureFilename));
        }

        public RenderData ProduceRenderData()
        {
            return new RenderData
            {
                Vertices = _vertices.Array,
                RenderCommands = _renderCommands.AsSpan()
            };
        }
    }
}