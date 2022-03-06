using System.Drawing;

namespace GDFiddle.UI
{
    public readonly struct RenderCommand
    {
        public readonly Rectangle ScissorRectangle;
        public readonly int VertexOffset;
        public readonly int TriangleCount;
        public readonly string? Texture;

        public RenderCommand(Rectangle scissorRectangle, int vertexOffset, int triangleCount, string? texture = null)
        {
            ScissorRectangle = scissorRectangle;
            VertexOffset = vertexOffset;
            TriangleCount = triangleCount;
            Texture = texture;
        }
    }
}
