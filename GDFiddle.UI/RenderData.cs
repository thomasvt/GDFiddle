using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.UI
{
    public ref struct RenderData
    {
        
        public VertexPositionColorTexture[] Vertices;
        public Span<RenderCommand> RenderCommands;
    }
}
