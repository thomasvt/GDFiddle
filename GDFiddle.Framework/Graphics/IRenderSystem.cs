using System.Numerics;
using GDFiddle.Ecs;

namespace GDFiddle.Framework.Graphics;

/// <summary>
/// A System service that is being called once per frame to render the game world.
/// </summary>
public interface IRenderSystem : IService
{
    void Render();
    EntityId? GetEntityAt(Vector2 screenPosition);
}