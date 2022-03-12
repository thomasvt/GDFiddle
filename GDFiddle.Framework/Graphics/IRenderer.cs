using System.Numerics;

namespace GDFiddle.Framework.Graphics
{
    /// <summary>
    /// Service providing all methods to render game graphics.
    /// </summary>
    public interface IRenderer
    {
        void BeginFrame();
        void EndFrame();
        void Draw(Sprite sprite, Vector2 position);
    }
}
