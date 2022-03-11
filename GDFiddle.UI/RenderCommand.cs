using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.UI
{
    public readonly struct RenderCommand
    {
        public readonly RectangleF ScissorRectangle;
        public readonly int VertexOffset;
        public readonly int TriangleCount;
        public readonly Texture2D? Texture;

        public RenderCommand(RectangleF scissorRectangle, int vertexOffset, int triangleCount, Texture2D? texture = null)
        {
            ScissorRectangle = scissorRectangle;
            VertexOffset = vertexOffset;
            TriangleCount = triangleCount;
            Texture = texture;
        }
    }
}
