using System.Numerics;

namespace GDFiddle.Framework.Graphics
{
    /// <summary>
    /// Service providing all methods to render game graphics.
    /// </summary>
    public interface IRenderer
    {
        void BeginFrame(Matrix3x2 viewTransform);
        void EndFrame();
        void Draw(Sprite sprite, Vector2 position);
        /// <summary>
        /// The size of the latest frame that was rendered, in device pixels. Frame size can change when the viewport is resized in the editor or the player changes the game settings.
        /// </summary>
        Vector2 LastFrameSize { get; }
    }
}
