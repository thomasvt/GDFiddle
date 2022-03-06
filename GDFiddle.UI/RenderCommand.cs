using Microsoft.Xna.Framework;

namespace GDFiddle.UI
{
    public readonly struct RenderCommand
    {
        public readonly Vector2 Offset, Size;
        public readonly int VertexOffset;
        public readonly int TriangleCount;
        public readonly string? Texture;

        public RenderCommand(Vector2 offset, Vector2 size, int vertexOffset, int triangleCount, string? texture)
        {
            Offset = offset;
            Size = size;
            VertexOffset = vertexOffset;
            TriangleCount = triangleCount;
            Texture = texture;
        }
    }
}
