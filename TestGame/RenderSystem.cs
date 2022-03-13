using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.Ecs.Querying;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;

namespace TestGame
{
    public class RenderSystem : IRenderSystem
    {
        private readonly IRenderer _renderer;
        private readonly EntityQuery<PositionComponent, SpriteComponent> _renderQuery;

        public RenderSystem(IRenderer renderer, IScene scene)
        {
            _renderer = renderer;
            _renderQuery = scene.Querying.DefineQuery<PositionComponent, SpriteComponent>();
        }

        public void Render()
        {
            _renderer.BeginFrame();
            _renderQuery.VisitAll((ids, positions, sprites) =>
            {
                for (var i = 0; i < ids.Length; i++)
                {
                    var sprite = sprites[i].Sprite;
                    if (sprite != null)
                        _renderer.Draw(sprite, positions[i].Position);
                }
            });
            _renderer.EndFrame();
        }

        /// <summary>
        /// Finds the entity at a certain screenposition (by its sprite). Return null if no entity is there.
        /// </summary>
        public EntityId? GetEntityAt(Vector2 screenPosition)
        {
            EntityId? entityId = null;
            _renderQuery.VisitAll((ids, positions, sprites) =>
            {
                for (var i = 0; i < ids.Length; i++)
                {
                    var sprite = sprites[i].Sprite;
                    if (sprite == null)
                        continue;

                    var origin = positions[i].Position;
                    var aabb = sprite.Aabb.Translate(origin);
                    if (aabb.Contains(screenPosition))
                    {
                        entityId = ids[i];
                        return;
                    }
                }
            });
            return entityId;
        }
    }
}