using System.Drawing;
using System.Numerics;
using GDFiddle.UI.Text;
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
        private readonly Stack<RectangleF> _areaStack;

        public Renderer(Font font)
        {
            Font = font;
            _vertices = new GrowingArray<VertexPositionColorTexture>(500);
            _renderCommands = new GrowingArray<RenderCommand>(50);
            _areaStack = new Stack<RectangleF>();
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

        private void AppendQuad(Vector2 min, Vector2 max, Vector2 uvMin, Vector2 uvMax, Color color)
        {
            _vertices.EnsureCapacity(6);

            var a = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(min.X, min.Y, 0), color, new Microsoft.Xna.Framework.Vector2(uvMin.X, uvMin.Y));
            var b = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(max.X, min.Y, 0), color, new Microsoft.Xna.Framework.Vector2(uvMax.X, uvMin.Y));
            var c = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(max.X, max.Y, 0), color, new Microsoft.Xna.Framework.Vector2(uvMax.X, uvMax.Y));
            var d = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(min.X, max.Y, 0), color, new Microsoft.Xna.Framework.Vector2(uvMin.X, uvMax.Y));

            _vertices.Add(a);
            _vertices.Add(b);
            _vertices.Add(c);

            _vertices.Add(a);
            _vertices.Add(c);
            _vertices.Add(d);
        }

        public RenderData GetRenderData()
        {
            return new RenderData
            {
                Vertices = _vertices.Array,
                RenderCommands = _renderCommands.AsSpan()
            };
        }

        public IDisposable CreateSubArea(RectangleF subArea)
        {
            var parentArea = _areaStack.Peek();
            _areaStack.Push(new RectangleF(parentArea.X + subArea.X, parentArea.Y + subArea.Y, subArea.Width, subArea.Height));
            return new AreaScope(_areaStack);
        }
    }
}