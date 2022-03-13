namespace GDFiddle.Framework.Graphics;

/// <summary>
/// A System service that is being called once per frame to render the game world.
/// </summary>
public interface IRenderSystem : IService
{
    void Render();
}