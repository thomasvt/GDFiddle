namespace GDFiddle.UI
{
    public readonly struct RenderCommand
    {
        public readonly int VertexOffset;
        public readonly int TriangleCount;
        public readonly string? Texture;

        public RenderCommand(int vertexOffset, int triangleCount, string? texture)
        {
            VertexOffset = vertexOffset;
            TriangleCount = triangleCount;
            Texture = texture;
        }
    }
}
