using System.Drawing;
using System.Numerics;
using GDFiddle.UI.Text;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = System.Drawing.Rectangle;

namespace GDFiddle.UI
{
    public class GuiRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        public readonly Font Font;
        private readonly GrowingArray<VertexPositionColorTexture> _vertices;
        private readonly GrowingArray<RenderCommand> _renderCommands;
        /// <summary>
        /// Tracks the clipping rectangles of the controls, as the rendering process visits the control tree depth first.
        /// </summary>
        private readonly Stack<RectangleF> _clipAreaStack;

        private readonly Texture2D _fontTexture;

        public GuiRenderer(GraphicsDevice graphicsDevice, Font font)
        {
            _graphicsDevice = graphicsDevice;
            Font = font;
            _fontTexture = ConvertGrayValueToTransparency(Texture2D.FromFile(graphicsDevice, font.TextureFilename));
            _vertices = new GrowingArray<VertexPositionColorTexture>(500);
            _renderCommands = new GrowingArray<RenderCommand>(50);
            _clipAreaStack = new Stack<RectangleF>();
        }

        private static Texture2D ConvertGrayValueToTransparency(Texture2D texture)
        {
            var pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            for (var i = 0; i < pixels.Length; i++)
            {
                ref var c = ref pixels[i];
                c.A = c.R;
                c.R = 255;
                c.G = 255;
                c.B = 255;
            }

            texture.SetData(pixels);
            return texture;
        }

        public void BeginFrame(Rectangle viewArea)
        {
            _vertices.Clear();
            _renderCommands.Clear();
            _clipAreaStack.Clear();
            _clipAreaStack.Push(viewArea);
        }

        public void DrawRectangle(Vector2 topLeft, Vector2 size, Color? fillColor, Color? borderColor)
        {
            var startVertexIdx = _vertices.Index;
            if (fillColor.HasValue)
            {
                _vertices.EnsureCapacity(6);
                AppendQuad(topLeft, topLeft + size, Vector2.Zero, Vector2.Zero, fillColor.Value);
            }

            if (borderColor.HasValue)
            {
                _vertices.EnsureCapacity(6*4);
                var left = topLeft.X + 0.5f;
                var top = topLeft.Y + 0.5f;
                var right = topLeft.X + size.X - 0.5f;
                var bottom = topLeft.Y + size.Y - 0.5f;

                var a = new Vector2(left, top);
                var b = new Vector2(right, top);
                var c = new Vector2(right, bottom);
                var d = new Vector2(left, bottom);

                AppendLine(a, b, 1f, borderColor.Value);
                AppendLine(b, c, 1f, borderColor.Value);
                AppendLine(c, d, 1f, borderColor.Value);
                AppendLine(d, a, 1f, borderColor.Value);
            }
            _renderCommands.Add(new RenderCommand(_clipAreaStack.Peek(), startVertexIdx, (_vertices.Index - startVertexIdx) / 3));
        }

        public void DrawText(int x, int y, string text, Color color, Font? font = null)
        {
            font ??= Font;
            var startVertexIdx = _vertices.Index;
            foreach (var glyphInfo in font.GetTextGlyphs(x, y, text))
            {
                AppendQuad(glyphInfo.QuadMin, glyphInfo.QuadMax, glyphInfo.UVMin, glyphInfo.UVMax, color);
            }
            
            _renderCommands.Add(new RenderCommand(_clipAreaStack.Peek(), startVertexIdx, (_vertices.Index - startVertexIdx) / 3, _fontTexture));
        }

        public void Draw(Texture2D texture)
        {
            var startVertexIdx = _vertices.Index;
            AppendQuad(Vector2.Zero, new Vector2(texture.Width, texture.Height), Vector2.Zero, Vector2.One, Color.White);
            _renderCommands.Add(new RenderCommand(_clipAreaStack.Peek(), startVertexIdx, 2, texture));
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

        private void AppendLine(Vector2 v0, Vector2 v1, float thickness, Color color)
        {
            var longitudinalOffset = Vector2.Normalize(v1 - v0) * thickness * 0.5f;
            var lateralOffset = new Vector2(-longitudinalOffset.Y, longitudinalOffset.X);

            var vA = v0 - longitudinalOffset + lateralOffset;
            var vB = v1 + longitudinalOffset + lateralOffset;
            var vC = v1 + longitudinalOffset - lateralOffset;
            var vD = v0 - longitudinalOffset - lateralOffset;

            var a = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(vA.X, vA.Y, 0f), color, Microsoft.Xna.Framework.Vector2.Zero);
            var b = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(vB.X, vB.Y, 0f), color, Microsoft.Xna.Framework.Vector2.Zero);
            var c = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(vC.X, vC.Y, 0f), color, Microsoft.Xna.Framework.Vector2.Zero);
            var d = new VertexPositionColorTexture(new Microsoft.Xna.Framework.Vector3(vD.X, vD.Y, 0f), color, Microsoft.Xna.Framework.Vector2.Zero);

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

        public IDisposable CreateSubClipArea(RectangleF subArea)
        {
            var parentArea = _clipAreaStack.Peek();
            _clipAreaStack.Push(new RectangleF(parentArea.X + subArea.X, parentArea.Y + subArea.Y, subArea.Width, subArea.Height));
            return new AreaScope(_clipAreaStack);
        }

        /// <summary>
        /// Gets the visual area of the screen that is the canvas of the current <see cref="Control"/>.
        /// </summary>
        public RectangleF GetClipArea()
        {
            return _clipAreaStack.Peek();
        }
    }
}